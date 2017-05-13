using BaseStarShot.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using Base1902;

namespace BaseStarShot.Services
{
    public class XamarinFormsNavigation : INavigation
    {
        readonly Func<Application> getApplication;
        readonly XamarinFormsNavigation parentNavigation;

        Type mainViewModel;
        Xamarin.Forms.Page currentPage, currentModalPage;
        BaseViewModel viewModelForMainPage;
        Xamarin.Forms.MasterDetailPage masterPage;
        BaseViewModel viewModelForMasterPage;

        NavigationPage navigationPage;
        List<NavigationPage> navigationPages = new List<NavigationPage>();
        Dictionary<Page, NavigationPage> navigationMappings = new Dictionary<Page, NavigationPage>();

        public NavigationPage NavigationPage
        {
            get
            {
                if (navigationPage == null)
                {
                    InitNavigationPage();
                }
                return navigationPage;
            }
        }

        private List<BaseViewModel> modalStack = new List<BaseViewModel>();
        public IReadOnlyList<BaseViewModel> ModalStack
        {
            get { return modalStack; }
        }

        private List<BaseViewModel> navigationStack = new List<BaseViewModel>();
        public IReadOnlyList<BaseViewModel> NavigationStack
        {
            get { return navigationStack; }
        }

        public BaseViewModel CurrentViewModel
        {
            get { return viewModelForMasterPage != null ? navigationChildren[0].CurrentViewModel : modalStack.LastOrDefault() ?? navigationStack.LastOrDefault(); }
        }

        public BaseViewModel CurrentDetailViewModel { get; private set; }

        private Dictionary<Type, Type> registrations = new Dictionary<Type, Type>();
        internal Dictionary<Type, Type> registrationViews = new Dictionary<Type, Type>();
        private Dictionary<BaseViewModel, Page> mapping = new Dictionary<BaseViewModel, Page>();

        private List<XamarinFormsNavigation> navigationChildren = new List<XamarinFormsNavigation>();
        public Dictionary<Type, DetailCacheItem> DetailCache = new Dictionary<Type, DetailCacheItem>();

        protected UnityDependencyResolver Resolver { get { return Base1902.Resolver.Current as UnityDependencyResolver; } }

        public event EventHandler<NavigationChangedEventArgs> Pushed = delegate { };
        public event EventHandler<NavigationChangedEventArgs> Popped = delegate { };
        public event EventHandler<NavigationChangedEventArgs> Removed = delegate { };
        /// <summary>
        /// Event raised when a view is created after ResolveView is called.
        /// </summary>
        public event EventHandler<ViewCreatedEventArgs> ViewCreated = delegate { };
        public View CurrentView { get; private set; }

        #region Constructor
        public XamarinFormsNavigation(Func<Application> getApplication)
        {
            this.getApplication = getApplication;
            if (!(Base1902.Resolver.Current is UnityDependencyResolver))
                throw new InvalidOperationException("XamarinFormsNavigation can only work with a UnityDependencyResolver set as Resolver.");
        }

        internal XamarinFormsNavigation(XamarinFormsNavigation parentNavigation, Type mainViewModel, Dictionary<Type, Type> registrations)
        {
            this.parentNavigation = parentNavigation;
            this.mainViewModel = mainViewModel;
            this.registrations = registrations;
        }
        #endregion

        protected void InitNavigationPage(params object[] dependencies)
        {
            InitNavigationPage(null, null, dependencies);
        }

        protected void InitNavigationPage(Page startPage, object parameters, params object[] dependencies)
        {
            var viewModel = Resolve(mainViewModel, parameters, true, dependencies);
            navigationStack.Add(viewModel);
            currentPage = mapping.First().Value;
            currentModalPage = null;
            if (currentPage is NavigationPage)
            {
                navigationPage = (NavigationPage)currentPage;
				navigationPage.BarTextColor = Color.White;
                currentPage = navigationPage.CurrentPage;
                mapping[mapping.First().Key] = currentPage;
            }
            else
            {
                navigationPage = new NavigationPage(startPage ?? currentPage);
            }
            viewModel.Init();
            navigationPage.Popped += navigationPage_Popped;
        }

        protected TViewModel Resolve<TViewModel>(object parameters, params object[] dependencies)
            where TViewModel : BaseViewModel
        {
            var viewModelType = typeof(TViewModel);
            return (TViewModel)Resolve(viewModelType, parameters, dependencies);
        }

        protected BaseViewModel Resolve(Type viewModelType, object parameters, params object[] dependencies)
        {
            return Resolve(viewModelType, parameters, false, dependencies);
        }

        protected BaseViewModel Resolve(Type viewModelType, object parameters, bool skipInit, params object[] dependencies)
        {
            if (!registrations.ContainsKey(viewModelType))
                throw new InvalidOperationException(string.Format("Page for {0} is not yet registered for navigation.", viewModelType.Name));

            List<ResolverOverride> pageOverrides = new List<ResolverOverride>();

            foreach (var dependency in dependencies.Where(d => d is Page))
            {
                pageOverrides.Add(new DependencyOverride(dependency.GetType(), dependency));
            }

            var page = (Page)Resolver.Get(registrations[viewModelType], pageOverrides.ToArray());

            //var page = (Page)Activator.CreateInstance(registrations[viewModelType]);

            List<ResolverOverride> overrides = new List<ResolverOverride>
            {
                new DependencyOverride(typeof(IPage), new XamarinPage(page is NavigationPage ? ((NavigationPage)page).CurrentPage : page)),
                new DependencyOverride(typeof(INavigation), this)
            };
            if (parameters != null)
            {
                foreach (var item in ReflectionHelper.ToFlatDictionary(parameters, oneLevelOnly: true))
                {
                    overrides.Add(new ParameterOverride(item.Key, item.Value));
                }
            }
            if (dependencies != null)
            {
                foreach (var dependency in dependencies.Where(d => !(d is Page)))
                {
                    overrides.Add(new DependencyOverride(dependency.GetType(), dependency));
                }
            }
            BaseViewModel viewModel;
            try
            {
                viewModel = (BaseViewModel)Resolver.Get(viewModelType, overrides.ToArray());
                if (page is NavigationPage)
                {
                    var navPage = (NavigationPage)page;
                    navPage.Popped += navigationPage_Popped;
                    navPage.CurrentPage.BindingContext = viewModel;
                    navigationPages.Add(navPage);
                    navigationMappings.Add(navPage.CurrentPage, navPage);
                }
                else
                    page.BindingContext = viewModel;
                mapping.Add(viewModel, page);
                if (!skipInit)
                    viewModel.Init();
            }
            catch (ResolutionFailedException ex)
            {
                throw new InvalidOperationException(
                    string.Format("The type {0} cannot be resolved because not all parameters have been supplied.", viewModelType.FullName), ex);
            }

            return viewModel;
        }

        public TViewModel ResolveView<TViewModel>()
            where TViewModel : CommonViewModel
        {
            var viewModelType = typeof(TViewModel);
            return (TViewModel)ResolveView(viewModelType, null);
        }

        public TViewModel ResolveView<TViewModel>(object parameters)
            where TViewModel : CommonViewModel
        {
            var viewModelType = typeof(TViewModel);
            return (TViewModel)ResolveView(viewModelType, parameters);
        }

        public CommonViewModel ResolveView(Type viewModelType, object parameters)
        {
            var navigation = parentNavigation != null ? parentNavigation : this;
            if (!navigation.registrationViews.ContainsKey(viewModelType))
                throw new InvalidOperationException(string.Format("View for {0} is not yet registered.", viewModelType.Name));

            var view = (View)Resolver.Get(navigation.registrationViews[viewModelType]);

            List<ResolverOverride> overrides = new List<ResolverOverride>();

            if (parameters != null)
            {
                foreach (var item in ReflectionHelper.ToFlatDictionary(parameters, oneLevelOnly: true))
                {
                    overrides.Add(new ParameterOverride(item.Key, item.Value));
                }
            }

            CommonViewModel viewModel;
            try
            {
                viewModel = (CommonViewModel)Resolver.Get(viewModelType, overrides.ToArray());
                view.BindingContext = viewModel;
                CurrentView = view;
				ViewCreated(this, new ViewCreatedEventArgs(view, viewModel));
                viewModel.Init();
            }
            catch (ResolutionFailedException ex)
            {
                throw new InvalidOperationException(
                    string.Format("The type {0} cannot be resolved because not all parameters have been supplied.", viewModelType.FullName), ex);
            }

            return viewModel;
        }

        /// <summary>
        /// Registers a type mapping between a view model and a page.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>
        public XamarinFormsNavigation Register<TViewModel, TPage>()
            where TViewModel : BaseViewModel
            where TPage : Xamarin.Forms.Page
        {
            var viewModelType = typeof(TViewModel);
            if (mainViewModel == null)
                mainViewModel = viewModelType;
            registrations.Add(viewModelType, typeof(TPage));
            return this;
        }

        /// <summary>
        /// Registers a type mapping between a view model and a view.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public XamarinFormsNavigation RegisterView<TViewModel, TView>()
            where TViewModel : CommonViewModel
            where TView : Xamarin.Forms.View
        {
            var viewModelType = typeof(TViewModel);
            registrationViews.Add(viewModelType, typeof(TView));
            return this;
        }

        private void Register(Type viewModelType, Type pageType)
        {
            registrations.Add(viewModelType, pageType);
        }

        public Task<TViewModel> SetMainPageAsync<TViewModel, T1>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
        {
            return SetMainPageAsync<TViewModel>(typeof(T1));
        }

        public Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
        {
            return SetMainPageAsync<TViewModel>(typeof(T1), typeof(T2));
        }

        public Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel
        {
            return SetMainPageAsync<TViewModel>(typeof(T1), typeof(T2), typeof(T3));
        }

        public Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3, T4>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel
            where T4 : BaseViewModel
        {
            return SetMainPageAsync<TViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3, T4, T5>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel
            where T4 : BaseViewModel
            where T5 : BaseViewModel
        {
            return SetMainPageAsync<TViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        public async Task<TViewModel> SetMainPageAsync<TViewModel>(params Type[] types)
            where TViewModel : BaseViewModel
        {
            if (parentNavigation != null)
            {
                return await parentNavigation.SetMainPageAsync<TViewModel>(types);
            }
            var isChangingMainPage = viewModelForMainPage != null;
            if (!isChangingMainPage && Device.OS == TargetPlatform.Android)
            {
                if (navigationPage != null)
                {
                    navigationPage.Popped -= navigationPage_Popped;
                    navigationPage = null;
                }
                foreach (var page in navigationPages)
                {
                    page.Popped -= navigationPage_Popped;
                }
                navigationPages.Clear();
            }
            var oldModalStack = modalStack.ToList();
            modalStack.Clear();
            var oldNavigationStack = navigationStack.ToList();
            navigationStack.Clear();
            var oldMapping = mapping.ToDictionary(m => m.Key, m => m.Value);
            mapping.Clear();
            var oldNavigationChildren = navigationChildren.ToList();
            navigationChildren.Clear();
            var oldNavigation = currentPage.Navigation;
            currentPage = null;
            currentModalPage = null;

            var weakFactory = new WeakFactoryMethod<TViewModel>();

            Func<TViewModel> func = weakFactory.Get;

            List<BaseViewModel> children = new List<BaseViewModel>();

            foreach (var type in types)
            {
                var childNavigation = new XamarinFormsNavigation(this, type, this.registrations);
                childNavigation.InitNavigationPage(func);
                children.Add(childNavigation.navigationStack.Last());
                navigationChildren.Add(childNavigation);
            }

            var viewModel = Resolve<TViewModel>(null, navigationChildren.Select(c => c.NavigationPage).Cast<object>()
                .Union(children).ToArray());
            weakFactory.Set(viewModel);

            var mainPage = mapping[viewModel];
            navigationStack.Add(viewModel);

            // Android does not support multiple levels of navigation page.
            if (!isChangingMainPage && Device.OS == TargetPlatform.Android)
            {
                var animationPage = GetAnimationPage(mainPage);

                await oldNavigation.PushAsync(animationPage);

                // Android needs a little bit of delay before replacing MainPage or it will throw a null reference exception.
                await Task.Delay(50);
            }

            currentPage = mainPage;

            if (Device.OS == TargetPlatform.Android)
                getApplication().MainPage = mainPage;
            else
            {
                var push = oldNavigation.PushAsync(mainPage);
                var stackCount = oldNavigation.NavigationStack.Count;
                for (int i = 0; i < stackCount - 1; i++)
                {
                    oldNavigation.RemovePage(oldNavigation.NavigationStack[0]);
                }
                await push;
            }

            foreach (var vm in oldModalStack)
            {
                vm.OnRemove(true);
                CleanUpPage(oldMapping[vm]);
            }

            foreach (var vm in oldNavigationStack)
            {
                vm.OnRemove(false);
                CleanUpPage(oldMapping[vm]);
            }
            foreach (var navChild in oldNavigationChildren)
            {
                foreach (var vm in navChild.ModalStack)
                {
                    vm.OnRemove(true);
                    navChild.CleanUpPage(navChild.mapping[vm]);
                }
                foreach (var vm in navChild.NavigationStack)
                {
                    vm.OnRemove(false);
                    navChild.CleanUpPage(navChild.mapping[vm]);
                }
            }

            if (viewModelForMasterPage != null)
            {
                viewModelForMasterPage.OnRemove(false);
                CleanUpPage(oldMapping[viewModelForMasterPage]);
                Removed(this, new NavigationChangedEventArgs(viewModelForMasterPage, false));
                viewModelForMasterPage = null;
                masterPage = null;
            }

            viewModelForMainPage = viewModel;
            viewModel.OnPush(false);
            Pushed(this, new NavigationChangedEventArgs(viewModel, false));

            return viewModel;
        }

        protected Page GetAnimationPage(Page page)
        {
            Page animationPage;
            //if (page is TabbedPage)
            //{
            //    var tabbedPage = (TabbedPage)page;
            //    var tabbedPageCopy = tabbedPage is BaseStarShot.Controls.TabbedPage ? new BaseStarShot.Controls.TabbedPage() : new TabbedPage();
            //    tabbedPageCopy.Title = tabbedPage.Title;
            //    tabbedPageCopy.BackgroundImage = tabbedPage.BackgroundImage;
            //    foreach (var tabPage in tabbedPage.Children)
            //    {
            //        var subPage = tabPage;
            //        if (tabPage is NavigationPage)
            //            subPage = ((NavigationPage)tabPage).CurrentPage;
            //        var subPageCopy = new Page
            //        {
            //            Title = subPage.Title,
            //            BackgroundImage = subPage.BackgroundImage,
            //            Icon = subPage.Icon
            //        };
            //        tabbedPageCopy.Children.Add(subPageCopy);
            //        if (tabbedPage.CurrentPage == tabPage)
            //            tabbedPageCopy.CurrentPage = subPageCopy;
            //    }
            //    if (tabbedPage is BaseStarShot.Controls.TabbedPage)
            //    {
            //        var baseTabbedPage = (BaseStarShot.Controls.TabbedPage)tabbedPage;
            //        var baseTabbedPageCopy = (BaseStarShot.Controls.TabbedPage)tabbedPageCopy;
            //        foreach (var icon in baseTabbedPage.ActiveIcons)
            //            baseTabbedPageCopy.ActiveIcons.Add(icon);
            //    }
            //    animationPage = tabbedPageCopy;
            //}
            //else
            {
                animationPage = new Page
                {
                    Title = page.Title,
                    BackgroundImage = page.BackgroundImage
                };
                NavigationPage.SetHasNavigationBar(animationPage, false);
            }
            return animationPage;
        }

        public async Task<bool> ClearMainPageAsync<TViewModel>()
            where TViewModel : BaseViewModel
        {
            return await ClearMainPageAsync(typeof(TViewModel), null);
        }

        public async Task<bool> ClearMainPageAsync(Type mainViewModel)
        {
            return await ClearMainPageAsync(mainViewModel, null);
        }

        public async Task<bool> ClearMainPageAsync<TViewModel>(IEnumerable<Type> insertBefore)
            where TViewModel : BaseViewModel
        {
            return await ClearMainPageAsync(typeof(TViewModel), insertBefore);
        }

        public async Task<bool> ClearMainPageAsync(Type mainViewModel, IEnumerable<Type> insertBefore)
        {
            if (viewModelForMainPage != null && navigationStack.Count == 1)
            {
                var viewModel = viewModelForMainPage;
                viewModelForMainPage = null;
                var oldMapping = mapping.ToDictionary(m => m.Key, m => m.Value);
                mapping.Clear();
                var oldNavigationChildren = navigationChildren.ToList();
                navigationChildren.Clear();
                this.mainViewModel = mainViewModel;
                navigationStack.Clear();
                if (Device.OS == TargetPlatform.Android)
                {
                    var animationPage = GetAnimationPage(currentPage);
                    InitNavigationPage(animationPage, null);
                    getApplication().MainPage = NavigationPage;
                    // Wait to fully draw page.
                    if (Device.OS == TargetPlatform.WinPhone)
                        await Task.Delay(10);

                    animationPage.Navigation.InsertPageBefore(currentPage, animationPage);
                    if (insertBefore != null)
                        foreach (var vm in insertBefore)
                            InsertBefore(vm, navigationStack.First());
                    //animationPage.Navigation.InsertPageBefore()

                    if (animationPage.Navigation.NavigationStack.Count > 0)
                        await animationPage.Navigation.PopAsync();
                }
                else
                {
                    navigationStack.Add(Resolve(mainViewModel, null));
                    var newPage = mapping.First().Value;
                    currentPage.Navigation.InsertPageBefore(newPage, currentPage);
                    if (insertBefore != null)
                        foreach (var vm in insertBefore)
                            InsertBefore(vm, navigationStack.First());
                    isPopping = true;
                    await currentPage.Navigation.PopAsync();
                    isPopping = false;
                    currentPage = newPage;
                }
                foreach (var navChild in oldNavigationChildren)
                {
                    foreach (var vm in navChild.ModalStack)
                    {
                        vm.OnRemove(true);
                        navChild.CleanUpPage(navChild.mapping[vm]);
                    }
                    foreach (var vm in navChild.NavigationStack)
                    {
                        vm.OnRemove(false);
                        navChild.CleanUpPage(navChild.mapping[vm]);
                    }
                }
                viewModel.OnPop(false);
                CleanUpPage(oldMapping[viewModel]);
                Popped(this, new NavigationChangedEventArgs(viewModel, false));

                //animationPage.Navigation.RemovePage(animationPage);
                return true;
            }
            if (viewModelForMasterPage != null)
            {
                var viewModel = viewModelForMasterPage;
                viewModelForMasterPage = null;
                var oldMapping = mapping.ToDictionary(m => m.Key, m => m.Value);
                mapping.Clear();
                var oldNavigationChildren = navigationChildren.ToList();
                navigationChildren.Clear();
                this.mainViewModel = mainViewModel;
                navigationStack.Clear();
                masterPage = null;

                InitNavigationPage((Page)null, null);
                getApplication().MainPage = NavigationPage;
                // Wait to fully draw page.
                if (Device.OS == TargetPlatform.WinPhone)
                    await Task.Delay(10);

                if (insertBefore != null)
                    foreach (var vm in insertBefore)
                        InsertBefore(vm, navigationStack.First());

                foreach (var navChild in oldNavigationChildren)
                {
                    if (!DetailCache.Any(c => c.Value.Navigation == navChild))
                    {
                        ClearNavigationStack(navChild);
                    }
                }

                foreach (var nav in DetailCache)
                {
                    ClearNavigationStack(nav.Value.Navigation);
                }
                DetailCache.Clear();

                viewModel.OnRemove(false);
                CleanUpPage(oldMapping[viewModel]);
                Removed(this, new NavigationChangedEventArgs(viewModel, false));

                return true;
            }
            return false;
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>()
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TViewModel>(typeof(TDetailViewModel), null);
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(bool cached)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TViewModel>(typeof(TDetailViewModel), null, cached);
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(object parameters)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TViewModel>(typeof(TDetailViewModel), parameters);
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(object parameters, bool cached)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TViewModel>(typeof(TDetailViewModel), parameters, cached);
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel>(Type detailType, object parameters)
            where TViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TViewModel>(detailType, parameters, false);
        }

        public async Task<TViewModel> SetMasterPageAsync<TViewModel>(Type detailType, object parameters, bool cached)
            where TViewModel : BaseViewModel
        {
            if (navigationPage != null)
            {
                navigationPage.Popped -= navigationPage_Popped;
                navigationPage = null;
            }
            foreach (var page in navigationPages)
            {
                page.Popped -= navigationPage_Popped;
            }
            navigationPages.Clear();

            var oldModalStack = modalStack.ToList();
            modalStack.Clear();
            var oldNavigationStack = navigationStack.ToList();
            navigationStack.Clear();
            var oldMapping = mapping.ToDictionary(m => m.Key, m => m.Value);
            mapping.Clear();
            var oldNavigation = currentPage.Navigation;
            currentPage = null;
            currentModalPage = null;

            var viewModel = Resolve<TViewModel>(null);

            masterPage = (MasterDetailPage)mapping[viewModel];
            navigationStack.Add(viewModel);

            await SetDetailPageAsync(detailType, parameters, cached);

            currentPage = masterPage;

            getApplication().MainPage = masterPage;

            foreach (var vm in oldModalStack)
            {
                vm.OnRemove(true);
                CleanUpPage(oldMapping[vm]);
            }

            foreach (var vm in oldNavigationStack)
            {
                vm.OnRemove(false);
                CleanUpPage(oldMapping[vm]);
            }

            if (viewModelForMainPage != null)
            {
                viewModelForMainPage.OnRemove(false);
                CleanUpPage(oldMapping[viewModelForMainPage]);
                Removed(this, new NavigationChangedEventArgs(viewModelForMainPage, false));
                viewModelForMainPage = null;
            }

            if (viewModelForMasterPage != null)
            {
                viewModelForMasterPage.OnRemove(false);
                CleanUpPage(oldMapping[viewModelForMasterPage]);
                Removed(this, new NavigationChangedEventArgs(viewModelForMasterPage, false));
            }

            viewModelForMasterPage = viewModel;
            viewModel.OnPush(false);
            Pushed(this, new NavigationChangedEventArgs(viewModel, false));

            return viewModel;
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7, T8>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8));
        }

        public async Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7, T8, T9>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            return await SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9));
        }

        public async Task<TMasterViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(params Type[] types)
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel
        {
            if (navigationPage != null)
            {
                navigationPage.Popped -= navigationPage_Popped;
                navigationPage = null;
            }

            var oldModalStack = modalStack.ToList();
            modalStack.Clear();
            var oldNavigationStack = navigationStack.ToList();
            navigationStack.Clear();
            var oldMapping = mapping.ToDictionary(m => m.Key, m => m.Value);
            mapping.Clear();
            var oldNavigation = currentPage.Navigation;
            currentPage = null;

            var viewModel = Resolve<TMasterViewModel>(null);

            masterPage = (MasterDetailPage)mapping[viewModel];
            navigationStack.Add(viewModel);

            await SetDetailPageAsync<TTabbedViewModel>(types);

            currentPage = masterPage;

            getApplication().MainPage = masterPage;

            foreach (var vm in oldModalStack)
            {
                vm.OnRemove(true);
                CleanUpPage(oldMapping[vm]);
            }

            foreach (var vm in oldNavigationStack)
            {
                vm.OnRemove(false);
                CleanUpPage(oldMapping[vm]);
            }

            if (viewModelForMainPage != null)
            {
                viewModelForMainPage.OnRemove(false);
                CleanUpPage(oldMapping[viewModelForMainPage]);
                Removed(this, new NavigationChangedEventArgs(viewModelForMainPage, false));
                viewModelForMainPage = null;
            }

            if (viewModelForMasterPage != null)
            {
                viewModelForMasterPage.OnRemove(false);
                CleanUpPage(oldMapping[viewModelForMasterPage]);
                Removed(this, new NavigationChangedEventArgs(viewModelForMasterPage, false));
            }

            viewModelForMasterPage = viewModel;
            viewModel.OnPush(false);
            Pushed(this, new NavigationChangedEventArgs(viewModel, false));

            return viewModel;
        }

        public void SetSelectedPage(int selectedIndex)
        {
            if (parentNavigation != null)
            {
                parentNavigation.SetSelectedPage(selectedIndex);
            }
            else
            {
                if (masterPage != null)
                {
                    var tabbedPage = masterPage.Detail as TabbedPage;
                    Page currentPage;
                    if (Device.OS == TargetPlatform.iOS)
                    {
                        var parentTabIndex = (int)Math.Floor((float)(selectedIndex / 5));
                        var childTabbedPage = tabbedPage.Children[parentTabIndex] as TabbedPage;
                        if (childTabbedPage != null)
                        {
                            tabbedPage.SelectedItem = tabbedPage.Children[parentTabIndex];
                            childTabbedPage.SelectedItem = currentPage = childTabbedPage.Children[selectedIndex % 5];
                        }
                        else
                        {
                            tabbedPage.SelectedItem = currentPage = tabbedPage.Children[selectedIndex];
                        }
                    }
                    else
                    {
                        tabbedPage.SelectedItem = currentPage = tabbedPage.Children[selectedIndex];
                    }
                    CurrentDetailViewModel = navigationChildren[selectedIndex].CurrentViewModel;
                    CurrentDetailViewModel.OnRevisit(false, false);
                }
            }
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync(typeof(TViewModel), null);
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel>(bool cached)
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync(typeof(TViewModel), null, cached);
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel>(object parameters)
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync(typeof(TViewModel), parameters);
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel>(object parameters, bool cached)
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync(typeof(TViewModel), parameters, cached);
        }

        public async Task<BaseViewModel> SetDetailPageAsync(Type viewModelType)
        {
            return await SetDetailPageAsync(viewModelType, null);
        }

        public async Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, bool cached)
        {
            return await SetDetailPageAsync(viewModelType, null, cached);
        }

        public async Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, object parameters)
        {
            return await SetDetailPageAsync(viewModelType, parameters, false);
        }

        public async Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, object parameters, bool cached)
        {
            if (masterPage != null)
            {
                var oldNavigationChildren = navigationChildren.ToList();
                navigationChildren.Clear();

                BaseViewModel child = null;
                XamarinFormsNavigation childNavigation = null;
                bool isRevisited = false;

                if (cached)
                {
                    if (DetailCache.ContainsKey(viewModelType))
                    {
                        var cache = DetailCache[viewModelType];
                        bool getCache = true;
                        if (parameters != null)
                        {
                            getCache = cache.ParameterKey == parameters.ToString();
                        }
                        if (getCache)
                        {
                            childNavigation = cache.Navigation;
                            child = childNavigation.navigationStack.First();
                            isRevisited = true;
                        }
                    }
                }

                if (childNavigation == null)
                {
                    childNavigation = new XamarinFormsNavigation(this, viewModelType, this.registrations);
                    childNavigation.InitNavigationPage((Page)null, parameters);
                    child = childNavigation.navigationStack.Last();

                    if (cached)
                    {
                        DetailCache.Add(viewModelType, new DetailCacheItem(viewModelType, parameters, childNavigation));
                    }
                }
                navigationChildren.Add(childNavigation);

                if (masterPage.Detail != null && oldNavigationChildren.Count == 1)
                {
                    var navChild = oldNavigationChildren[0];
                    // Check if old navigation is not cached.
                    if (!DetailCache.Any(c => c.Value.Navigation == navChild))
                    {
                        while (navChild.NavigationStack.Count > 1)
                            await navChild.PopAsync(false);
                        //navChild.Remove(navChild.NavigationStack[0]);
                    }
                }

                masterPage.Detail = childNavigation.NavigationPage;

                foreach (var navChild in oldNavigationChildren)
                {
                    // Check if old navigation is not cached.
                    if (!DetailCache.Any(c => c.Value.Navigation == navChild))
                    {
                        ClearNavigationStack(navChild);
                    }
                }

                if (isRevisited)
                    child.OnRevisit(false, false);

                return child;
            }
            return null;
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync<TViewModel>(typeof(TViewModel), typeof(T1));
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync<TViewModel>(typeof(TViewModel), typeof(T1), typeof(T2));
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync<TViewModel>(typeof(TViewModel), typeof(T1), typeof(T2), typeof(T3));
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3, T4>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync<TViewModel>(typeof(TViewModel), typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3, T4, T5>()
            where TViewModel : BaseViewModel
        {
            return await SetDetailPageAsync<TViewModel>(typeof(TViewModel), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        public async Task<BaseViewModel> SetDetailPageAsync<TViewModel>(params Type[] types)
            where TViewModel : BaseViewModel
        {
            if (masterPage != null)
            {
                var oldNavigationChildren = navigationChildren.ToList();
                navigationChildren.Clear();

                BaseViewModel child = null;
                bool isRevisited = false;

                var weakFactory = new WeakFactoryMethod<TViewModel>();

                Func<TViewModel> func = weakFactory.Get;

                List<BaseViewModel> children = new List<BaseViewModel>();

                foreach (var type in types)
                {
                    var childNavigation = new XamarinFormsNavigation(this, type, this.registrations);
                    childNavigation.InitNavigationPage((Page)null, null);
                    children.Add(childNavigation.navigationStack.Last());
                    navigationChildren.Add(childNavigation);
                }

                var viewModel = Resolve<TViewModel>(null, navigationChildren.Select(c => c.NavigationPage).Cast<object>()
                    .Union(children).ToArray());
                weakFactory.Set(viewModel);

                var mainPage = mapping[viewModel];
                navigationStack.Add(viewModel);

                if (masterPage.Detail != null && oldNavigationChildren.Count == 1)
                {
                    var navChild = oldNavigationChildren[0];
                    // Check if old navigation is not cached.
                    if (!DetailCache.Any(c => c.Value.Navigation == navChild))
                    {
                        while (navChild.NavigationStack.Count > 1)
                            await navChild.PopAsync(false);
                        //navChild.Remove(navChild.NavigationStack[0]);
                    }
                }

                masterPage.Detail = mainPage;

                foreach (var navChild in oldNavigationChildren)
                {
                    // Check if old navigation is not cached.
                    if (!DetailCache.Any(c => c.Value.Navigation == navChild))
                    {
                        ClearNavigationStack(navChild);
                    }
                }

                if (isRevisited)
                    child.OnRevisit(false, false);

                return child;
            }
            return null;
        }

        private void ClearNavigationStack(XamarinFormsNavigation nav)
        {
            foreach (var vm in nav.ModalStack)
            {
                vm.OnRemove(true);
                nav.CleanUpPage(nav.mapping[vm]);
            }
            foreach (var vm in nav.NavigationStack)
            {
                vm.OnRemove(false);
                nav.CleanUpPage(nav.mapping[vm]);
            }
        }

        public bool IsOnStack(BaseViewModel viewModel)
        {
            return navigationStack.Contains(viewModel) || modalStack.Contains(viewModel);
        }

        public T InsertBefore<T>(BaseViewModel before)
            where T : BaseViewModel
        {
            return (T)InsertBefore(typeof(T), before, null);
        }

        public T InsertBefore<T>(BaseViewModel before, object parameters)
            where T : BaseViewModel
        {
            return (T)InsertBefore(typeof(T), before, parameters);
        }

        public BaseViewModel InsertBefore(Type viewModelType, BaseViewModel before)
        {
            return InsertBefore(viewModelType, before, null);
        }

        public BaseViewModel InsertBefore(Type viewModelType, BaseViewModel before, object parameters)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (before == null) throw new ArgumentNullException("before");

            var viewModel = Resolve(viewModelType, parameters);
            InsertBefore(viewModel, before);
            return viewModel;
        }

        public void InsertBefore(BaseViewModel viewModel, BaseViewModel before)
        {
            if (viewModel == null) throw new ArgumentNullException("viewModel");
            if (before == null) throw new ArgumentNullException("before");
            if (!mapping.ContainsKey(viewModel))
                throw new InvalidOperationException("Only view models that have been resolved by INavigation could be inserted in the navigation stack.");

            int idx = navigationStack.IndexOf(before);
            if (idx >= 0)
            {
                navigationStack.Insert(idx, viewModel);
                currentPage.Navigation.InsertPageBefore(mapping[viewModel], mapping[before]);
            }
            else
                throw new InvalidOperationException("Before view model does not exist in the navigation stack.");
        }

        public async Task<BaseViewModel> PopAsync()
        {
            return await PopAsync(true);
        }

        protected void ThrowOnEmptyStack()
        {
            if (navigationStack.Count == 0 || currentPage.Navigation == null)
                throw new InvalidOperationException("The navigation stack is empty. A call to push a new view model is needed.");
        }

        public async Task<BaseViewModel> PopAsync(bool animated)
        {
            ThrowOnEmptyStack();
            isPopping = true;
            var page = currentModalPage ?? currentPage;
            var task = page.Navigation.PopAsync(animated);
            isPopping = false;
            await task;
            return PopViewModel(task.Result, false);
        }

        void CleanUpPage(Page page)
        {
            if (page == null) return;

            if (Device.OS == TargetPlatform.Windows || Device.OS == TargetPlatform.Android)
                return;

            try
            {
                BaseStarShot.Logger.Write("XamarinFormsNavigation", "Cleanup: " + page.GetType().Name);
                page.BindingContext = null;
                BaseStarShot.Logger.Write("XamarinFormsNavigation", "BindingCOntext null");

                if (navigationMappings.ContainsKey(page))
                {
                    var navPage = navigationMappings[page];
                    navPage.Popped -= navigationPage_Popped;
                    navigationPages.Remove(navPage);
                    navigationMappings.Remove(page);
                }
                if (page is ContentPage)
                {
                    ((ContentPage)page).Content = null;
                }
                else if (page is MultiPage<Page>)
                {
                    var multiPage = (MultiPage<Page>)page;
                    foreach (var child in multiPage.Children)
                    {
                        CleanUpPage(child);
                    }
                }
            }
            catch
            {

            }
        }

        BaseViewModel PopViewModel(Page page, bool modal)
        {
            bool hasPopped = false;
            var viewModel = navigationStack.Last();
            if (mapping[viewModel] == page)
            {
                navigationStack.Remove(viewModel);
                currentPage = mapping[navigationStack.Last()];
                hasPopped = true;
            }
            else if (modalStack.Count > 0)
            {
                viewModel = modalStack.Last();
                if (mapping[viewModel] == page)
                {
                    modalStack.Remove(viewModel);
                    currentModalPage = modalStack.Count > 0 ? mapping[modalStack.Last()] : null;
                    hasPopped = true;
                }
                else
                {
                    viewModel = modalStack.First();
                    if (mapping[viewModel] == page)
                    {
                        //modalStack.Remove(viewModel);
                        //currentPage = mapping[modalStack.Count > 0 ? modalStack.Last() : navigationStack.Last()];
                        //currentModalPage = modalStack.Count > 0 ? mapping[modalStack.Last()] : null;
                        modalStack.Clear();
                        currentModalPage = null;
                        hasPopped = true;
                    }
                }
            }
            if (hasPopped)
            {
                mapping.Remove(viewModel);
                viewModel.OnPop(modal);
                CleanUpPage(page);
                Popped(this, new NavigationChangedEventArgs(viewModel, modal));
                if (modalStack.Count > 0)
                    modalStack.Last().OnRevisit(true, modal);
                else
                    navigationStack.Last().OnRevisit(false, modal);
                return viewModel;
            }
            return null;
        }

        bool isPopping;
        void navigationPage_Popped(object sender, NavigationEventArgs e)
        {
            if (!isPopping)
                PopViewModel(e.Page, false);
        }

        public async Task<BaseViewModel> PopModalAsync()
        {
            return await PopModalAsync(true);
        }

        public async Task<BaseViewModel> PopModalAsync(bool animated)
        {
            ThrowOnEmptyStack();
            isPopping = true;
            var task = currentPage.Navigation.PopModalAsync(animated);
            isPopping = false;
            await task;
            return PopViewModel(task.Result, true);
        }

        public async Task PopToRootAsync()
        {
            await PopToRootAsync(true);
        }

        public async Task PopToRootAsync(bool animated)
        {
            ThrowOnEmptyStack();
            var rootPage = currentPage.Navigation.NavigationStack[0];
            await currentPage.Navigation.PopToRootAsync(animated);
            var rootViewModel = navigationStack[0];
            var poppedViewModels = navigationStack.Skip(1).ToList();
            navigationStack.Clear();
            navigationStack.Add(rootViewModel);
            var poppedModalViewModels = modalStack.ToList();
            modalStack.Clear();
            currentPage = rootPage;

            if (poppedModalViewModels.Count > 0)
            {
                poppedModalViewModels.Reverse();
                foreach (var viewModel in poppedModalViewModels)
                {
                    viewModel.OnPop(true);
                    CleanUpPage(mapping[viewModel]);
                    Popped(this, new NavigationChangedEventArgs(viewModel, true));
                }
            }
            if (poppedViewModels.Count > 0)
            {
                poppedViewModels.Reverse();
                foreach (var viewModel in poppedViewModels)
                {
                    viewModel.OnPop(false);
                    CleanUpPage(mapping[viewModel]);
                    Popped(this, new NavigationChangedEventArgs(viewModel, false));
                }
            }
            mapping.Clear();
            mapping.Add(rootViewModel, rootPage);
            rootViewModel.OnRevisit(false, poppedViewModels.Count == 0);
        }

        public async Task<T> PushAsync<T>()
            where T : BaseViewModel
        {
            return (T)await PushAsync(typeof(T), null, true);
        }

        public async Task<T> PushAsync<T>(object parameters)
            where T : BaseViewModel
        {
            return (T)await PushAsync(typeof(T), parameters, true);
        }

        public async Task<T> PushAsync<T>(object parameters, bool animated)
            where T : BaseViewModel
        {
            return (T)await PushAsync(typeof(T), parameters, animated);
        }

        public async Task<BaseViewModel> PushAsync(Type viewModelType)
        {
            return await PushAsync(viewModelType, null, true);
        }

        public async Task<BaseViewModel> PushAsync(Type viewModelType, object parameters)
        {
            return await PushAsync(viewModelType, parameters, true);
        }

        public async Task<BaseViewModel> PushAsync(Type viewModelType, object parameters, bool animated)
        {
            BaseViewModel viewModel;
            if (!await ClearMainPageAsync(viewModelType))
            {
                viewModel = Resolve(viewModelType, parameters);
                if (currentModalPage == null)
                {
                    var oldPage = currentPage;
                    currentPage = mapping[viewModel];
                    navigationStack.Add(viewModel);
                    await oldPage.Navigation.PushAsync(currentPage, animated);
                }
                else
                {
                    var oldPage = currentModalPage;
                    currentModalPage = mapping[viewModel];
                    modalStack.Add(viewModel);
                    await oldPage.Navigation.PushAsync(currentModalPage, animated);
                }
                viewModel.OnPush(false);
                Pushed(this, new NavigationChangedEventArgs(viewModel, false));
            }
            else
                viewModel = navigationStack.First();
            return viewModel;
        }

        public async Task<T> PushTopAsync<T>()
            where T : BaseViewModel
        {
            return (T)await PushTopAsync(typeof(T), null, true);
        }

        public async Task<T> PushTopAsync<T>(object parameters)
            where T : BaseViewModel
        {
            return (T)await PushTopAsync(typeof(T), parameters, true);
        }

        public async Task<T> PushTopAsync<T>(object parameters, bool animated)
            where T : BaseViewModel
        {
            return (T)await PushTopAsync(typeof(T), parameters, animated);
        }

        public async Task<BaseViewModel> PushTopAsync(Type viewModelType)
        {
            return await PushTopAsync(viewModelType, null, true);
        }

        public async Task<BaseViewModel> PushTopAsync(Type viewModelType, object parameters)
        {
            return await PushTopAsync(viewModelType, parameters, true);
        }

        public async Task<BaseViewModel> PushTopAsync(Type viewModelType, object parameters, bool animated)
        {
            if (Device.OS != TargetPlatform.Android)
            {
                if (parentNavigation != null)
                {
                    return await parentNavigation.PushTopAsync(viewModelType, parameters, animated);
                }
                else
                {
                    var viewModel = Resolve(viewModelType, parameters);
                    var oldPage = currentPage;
                    currentPage = mapping[viewModel];
                    navigationStack.Add(viewModel);
                    await oldPage.Navigation.PushAsync(currentPage, animated);
                    viewModel.OnPush(false);
                    Pushed(this, new NavigationChangedEventArgs(viewModel, false));
                    return viewModel;
                }
            }

            return await PushAsync(viewModelType, parameters, animated);
        }

        public async Task<T> PushModalAsync<T>()
            where T : BaseViewModel
        {
            return (T)await PushModalAsync(typeof(T), null, true);
        }

        public async Task<T> PushModalAsync<T>(object parameters)
            where T : BaseViewModel
        {
            return (T)await PushModalAsync(typeof(T), parameters, true);
        }

        public async Task<T> PushModalAsync<T>(object parameters, bool animated)
            where T : BaseViewModel
        {
            return (T)await PushModalAsync(typeof(T), parameters, animated);
        }

        public async Task<BaseViewModel> PushModalAsync(Type viewModelType)
        {
            return await PushModalAsync(viewModelType, null, true);
        }

        public async Task<BaseViewModel> PushModalAsync(Type viewModelType, object parameters)
        {
            return await PushModalAsync(viewModelType, parameters, true);
        }

        public async Task<BaseViewModel> PushModalAsync(Type viewModelType, object parameters, bool animated)
        {
            ThrowOnEmptyStack();
            var viewModel = Resolve(viewModelType, parameters);
            //var oldPage = currentPage;
            //currentPage = mapping[viewModel];
            currentModalPage = mapping[viewModel];
            modalStack.Add(viewModel);
            await currentPage.Navigation.PushModalAsync(currentModalPage, animated);
            viewModel.OnPush(true);
            Pushed(this, new NavigationChangedEventArgs(viewModel, true));
            return viewModel;
        }

        public void Remove(BaseViewModel viewModel)
        {
            if (navigationStack.Contains(viewModel))
            {
                bool isOnTopOfStack = false;
                var cPage = currentPage;
                if (currentPage == mapping[viewModel])
                {
                    currentPage = currentPage.Navigation.NavigationStack[currentPage.Navigation.NavigationStack.Count - 2];
                    isOnTopOfStack = true;
                }
                isPopping = true;
                cPage.Navigation.RemovePage(mapping[viewModel]);
                isPopping = false;
                navigationStack.Remove(viewModel);
                viewModel.OnRemove(false);
                CleanUpPage(mapping[viewModel]);
                mapping.Remove(viewModel);
                Removed(this, new NavigationChangedEventArgs(viewModel, false));
                if (isOnTopOfStack)
                {
                    if (navigationStack.Count > 0)
                        navigationStack.Last().OnRevisit(false, false);
                }
            }
            else if (modalStack.Contains(viewModel))
            {
                bool isOnTopOfStack = false;
                bool isModal = true;
                var cPage = currentPage;
                if (currentModalPage == mapping[viewModel])
                {
                    if (currentPage.Navigation.ModalStack.Count > 1)
                    {
                        //currentPage = currentPage.Navigation.ModalStack[currentPage.Navigation.ModalStack.Count - 2];
                        currentModalPage = currentPage.Navigation.ModalStack[currentPage.Navigation.ModalStack.Count - 2];
                    }
                    else
                    {
                        //currentPage = currentPage.Navigation.NavigationStack[currentPage.Navigation.NavigationStack.Count - 1];
                        currentModalPage = null;
                        isModal = false;
                    }
                    isOnTopOfStack = true;
                }
                cPage.Navigation.RemovePage(mapping[viewModel]);
                modalStack.Remove(viewModel);
                viewModel.OnRemove(true);
                CleanUpPage(mapping[viewModel]);
                mapping.Remove(viewModel);
                Removed(this, new NavigationChangedEventArgs(viewModel, true));
                if (isOnTopOfStack)
                {
                    if (isModal)
                    {
                        if (modalStack.Count > 0)
                            modalStack.Last().OnRevisit(true, true);
                    }
                    else if (navigationStack.Count > 0)
                        navigationStack.Last().OnRevisit(false, true);
                }
            }
        }

        class XamarinPage : IPage
        {
            private readonly WeakReference<Xamarin.Forms.Page> _page;
            protected Xamarin.Forms.Page Page
            {
                get
                {
                    Xamarin.Forms.Page page;
                    _page.TryGetTarget(out page);
                    return page;
                }
            }

            public XamarinPage(Page page)
            {
                this._page = new WeakReference<Xamarin.Forms.Page>(page);
            }

            public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
            {
                var page = Page;
                if (page != null)
                    return await Page.DisplayActionSheet(title, cancel, destruction, buttons);
                return null;
            }

            public async Task DisplayAlert(string title, string message, string cancel)
            {
                var page = Page;
                if (page != null)
                    await page.DisplayAlert(title, message, cancel);
            }

            public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
            {
                var page = Page;
                if (page != null)
                    return await page.DisplayAlert(title, message, accept, cancel);
                return false;
            }
        }

        public class DetailCacheItem
        {
            public Type ViewModel { get; private set; }
            public object Parameters { get; private set; }
            public string ParameterKey { get; private set; }
            public XamarinFormsNavigation Navigation { get; private set; }

            public DetailCacheItem(Type viewModel, object parameters, XamarinFormsNavigation navigation)
            {
                this.ViewModel = viewModel;
                this.Parameters = parameters;
                this.Navigation = navigation;
                if (parameters != null)
                    ParameterKey = parameters.ToString();
            }
        }
    }
}
