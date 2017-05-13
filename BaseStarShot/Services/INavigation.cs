using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    /// <summary>
    /// Interface abstracting platform-specific navigation.
    /// </summary>
    public interface INavigation
    {
        /// <summary>
        /// Event raised when a view model is pushed to any stack.
        /// </summary>
        event EventHandler<NavigationChangedEventArgs> Pushed;
        /// <summary>
        /// Event raised when a view model is popped from any stack.
        /// </summary>
        event EventHandler<NavigationChangedEventArgs> Popped;
        /// <summary>
        /// Event raised when a view model is removed from any stack.
        /// </summary>
        event EventHandler<NavigationChangedEventArgs> Removed;

        /// <summary>
        /// Gets the modal navigation stack.
        /// </summary>
        IReadOnlyList<BaseViewModel> ModalStack { get; }

        /// <summary>
        /// Gets the stack of view models in the navigation.
        /// </summary>
        IReadOnlyList<BaseViewModel> NavigationStack { get; }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        BaseViewModel CurrentViewModel { get; }

        /// <summary>
        /// Gets the current detail view model.
        /// </summary>
        BaseViewModel CurrentDetailViewModel { get; }

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <typeparam name="T1">A child view model that can be mapped to a navigation page.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel, T1>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel;

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <typeparam name="T1">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T2">A child view model that can be mapped to a navigation page.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel;

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <typeparam name="T1">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T2">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T3">A child view model that can be mapped to a navigation page.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel;

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <typeparam name="T1">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T2">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T3">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T4">A child view model that can be mapped to a navigation page.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3, T4>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel
            where T4 : BaseViewModel;

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <typeparam name="T1">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T2">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T3">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T4">A child view model that can be mapped to a navigation page.</typeparam>
        /// <typeparam name="T5">A child view model that can be mapped to a navigation page.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel, T1, T2, T3, T4, T5>()
            where TViewModel : BaseViewModel
            where T1 : BaseViewModel
            where T2 : BaseViewModel
            where T3 : BaseViewModel
            where T4 : BaseViewModel
            where T5 : BaseViewModel;

        /// <summary>
        /// Sets a new main page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <param name="types">Children view models that can be mapped to a navigation page.</param>
        /// <returns></returns>
        Task<TViewModel> SetMainPageAsync<TViewModel>(params Type[] types) where TViewModel : BaseViewModel;

        /// <summary>
        /// Checks if the view model is still on the navigations stacks.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        bool IsOnStack(BaseViewModel viewModel);

        /// <summary>
        /// Inserts a view model in the navigation stack before an existing view model in the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="before"></param>
        /// <returns></returns>
        T InsertBefore<T>(BaseViewModel before) where T : BaseViewModel;

        /// <summary>
        /// Inserts a view model in the navigation stack before an existing view model in the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="before"></param>
        /// <param name="parameters"></param>
        T InsertBefore<T>(BaseViewModel before, object parameters) where T : BaseViewModel;

        /// <summary>
        /// Inserts a view model in the navigation stack before an existing view model in the stack.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        BaseViewModel InsertBefore(Type viewModelType, BaseViewModel before);

        /// <summary>
        /// Inserts a view model in the navigation stack before an existing view model in the stack.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="before"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        BaseViewModel InsertBefore(Type viewModelType, BaseViewModel before, object parameters);

        /// <summary>
        /// Inserts a view model in the navigation stack before an existing view model in the stack.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="before"></param>
        void InsertBefore(BaseViewModel viewModel, BaseViewModel before);

        /// <summary>
        /// Asynchronously removes the most recent view model from the navigation stack.
        /// </summary>
        /// <returns></returns>
        Task<BaseViewModel> PopAsync();

        /// <summary>
        /// Asynchronously removes the most recent view model from the navigation stack.
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<BaseViewModel> PopAsync(bool animated);

        /// <summary>
        /// Asynchronously dismisses the most recent modally presented view model.
        /// </summary>
        /// <returns></returns>
        Task<BaseViewModel> PopModalAsync();

        /// <summary>
        /// Asynchronously dismisses the most recent modally presented view model.
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<BaseViewModel> PopModalAsync(bool animated);

        /// <summary>
        /// Pops all but the root view model off the navigation stack.
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task PopToRootAsync();

        /// <summary>
        /// Pops all but the root view model off the navigation stack.
        /// </summary>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task PopToRootAsync(bool animated);

        /// <summary>
        /// Clears main or master page with new view model on top of the navigation stack.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <returns></returns>
        Task<bool> ClearMainPageAsync<TViewModel>() where TViewModel : BaseViewModel;

        /// <summary>
        /// Clears main or master page with new view model on top of the navigation stack.
        /// </summary>
        /// <param name="mainViewModel">The main view model.</param>
        /// <returns></returns>
        Task<bool> ClearMainPageAsync(Type mainViewModel);

        /// <summary>
        /// Clears main or master page with new view model on top of the navigation stack.
        /// </summary>
        /// <typeparam name="TViewModel">The main view model.</typeparam>
        /// <param name="insertBefore">The types of view model to insert before the mainViewModel.</param>
        /// <returns></returns>
        Task<bool> ClearMainPageAsync<TViewModel>(IEnumerable<Type> insertBefore) where TViewModel : BaseViewModel;

        /// <summary>
        /// Clears main or master page with new view model on top of the navigation stack.
        /// </summary>
        /// <param name="mainViewModel">The main view model</param>
        /// <param name="insertBefore">The types of view model to insert before the mainViewModel.</param>
        /// <returns></returns>
        Task<bool> ClearMainPageAsync(Type mainViewModel, IEnumerable<Type> insertBefore);

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> PushAsync<T>() where T : BaseViewModel;

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T> PushAsync<T>(object parameters) where T : BaseViewModel;

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<T> PushAsync<T>(object parameters, bool animated) where T : BaseViewModel;

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushAsync(Type viewModelType);

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushAsync(Type viewModelType, object parameters);

        /// <summary>
        /// Asynchronously adds a view model to the top of the navigation stack.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameters"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushAsync(Type viewModelType, object parameters, bool animated);

		Task<T> PushTopAsync<T>() where T : BaseViewModel;

		Task<T> PushTopAsync<T>(object parameters) where T : BaseViewModel;

		Task<T> PushTopAsync<T>(object parameters, bool animated) where T : BaseViewModel;

		Task<BaseViewModel> PushTopAsync(Type viewModelType);

		Task<BaseViewModel> PushTopAsync(Type viewModelType, object parameters);

		Task<BaseViewModel> PushTopAsync(Type viewModelType, object parameters, bool animated);

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> PushModalAsync<T>() where T : BaseViewModel;

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<T> PushModalAsync<T>(object parameters) where T : BaseViewModel;

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<T> PushModalAsync<T>(object parameters, bool animated) where T : BaseViewModel;

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushModalAsync(Type viewModelType);

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushModalAsync(Type viewModelType, object parameters);

        /// <summary>
        /// Presents a view model modally.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameters"></param>
        /// <param name="animated"></param>
        /// <returns></returns>
        Task<BaseViewModel> PushModalAsync(Type viewModelType, object parameters, bool animated);

        /// <summary>
        /// Removes the specified view model from the navigation stack.
        /// </summary>
        /// <param name="page"></param>
        void Remove(BaseViewModel viewModel);

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <typeparam name="TDetailViewModel">The detail view model.</typeparam>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>()
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel;

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <typeparam name="TDetailViewModel">The detail view model.</typeparam>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(bool cached)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel;

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <typeparam name="TDetailViewModel">The detail view model.</typeparam>
        /// <param name="parameters">The parameters passed to the detail view model.</param>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(object parameters)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel;

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <typeparam name="TDetailViewModel">The detail view model.</typeparam>
        /// <param name="parameters">The parameters passed to the detail view model.</param>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel, TDetailViewModel>(object parameters, bool cached)
            where TViewModel : BaseViewModel
            where TDetailViewModel : BaseViewModel;

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <param name="detailType">The detail view model.</param>
        /// <param name="parameters">The parameters passed to the detail view model.</param>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel>(Type detailType, object parameters)
            where TViewModel : BaseViewModel;

        /// <summary>
        /// Sets a new master page on the current application.
        /// </summary>
        /// <typeparam name="TViewModel">The master view model.</typeparam>
        /// <param name="detailType">The detail view model.</param>
        /// <param name="parameters">The parameters passed to the detail view model.</param>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<TViewModel> SetMasterPageAsync<TViewModel>(Type detailType, object parameters, bool cached)
            where TViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7, T8>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<BaseViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel, T1, T2, T3, T4, T5, T6, T7, T8, T9>()
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        Task<TMasterViewModel> SetMasterPageAsync<TMasterViewModel, TTabbedViewModel>(params Type[] types)
            where TMasterViewModel : BaseViewModel
            where TTabbedViewModel : BaseViewModel;

        void SetSelectedPage(int selectedIndex);

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <typeparam name="TViewModel">The detail view model.</typeparam>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync<TViewModel>()
            where TViewModel : BaseViewModel;

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <typeparam name="TViewModel">The detail view model.</typeparam>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync<TViewModel>(bool cached)
            where TViewModel : BaseViewModel;

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <typeparam name="TViewModel">The detail view model.</typeparam>
        /// <param name="parameters">The detail view model parameters.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync<TViewModel>(object parameters)
            where TViewModel : BaseViewModel;

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <typeparam name="TViewModel">The detail view model.</typeparam>
        /// <param name="parameters">The detail view model parameters.</param>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync<TViewModel>(object parameters, bool cached)
            where TViewModel : BaseViewModel;

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <param name="viewModelType">The detail view model.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync(Type viewModelType);

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <param name="viewModelType">The detail view model.</param>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, bool cached);

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <param name="viewModelType">The detail view model.</param>
        /// <param name="parameters">The detail view model parameters.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, object parameters);

        /// <summary>
        /// Sets the current detail page when a master page is set.
        /// </summary>
        /// <param name="viewModelType">The detail view model.</param>
        /// <param name="parameters">The detail view model parameters.</param>
        /// <param name="cached">Caches detail page if set.</param>
        /// <returns></returns>
        Task<BaseViewModel> SetDetailPageAsync(Type viewModelType, object parameters, bool cached);

        Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1>() where TViewModel : BaseViewModel;

        Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2>() where TViewModel : BaseViewModel;

        Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3>() where TViewModel : BaseViewModel;

        Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3, T4>() where TViewModel : BaseViewModel;

        Task<BaseViewModel> SetDetailPageAsync<TViewModel, T1, T2, T3, T4, T5>() where TViewModel : BaseViewModel;
        /// <summary>
        /// Resolves view.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        TViewModel ResolveView<TViewModel>() where TViewModel : CommonViewModel;

        /// <summary>
        /// Resolves view with parameters.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        TViewModel ResolveView<TViewModel>(object parameters) where TViewModel : CommonViewModel;

        /// <summary>
        /// Resolves view with parameters.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CommonViewModel ResolveView(Type viewModelType, object parameters);
    }
}
