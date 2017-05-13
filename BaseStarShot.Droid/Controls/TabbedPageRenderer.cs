using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BaseStarShot.Controls;
using Android;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.InputMethodServices;
using Android.Views.InputMethods;
using Android.Graphics;
using Android.Util;
[assembly: ExportRenderer(typeof(BaseStarShot.Controls.TabbedPage), typeof(TabbedPageRenderer))]

namespace BaseStarShot.Controls
{
    public class TabbedPageRenderer : TabbedRenderer, BaseStarShot.Controls.OnMenuItemSelectedListener
    {
        TextView tvTemporary;

        private Activity _activity;
        private bool _isFirstDesign = true;
        public static TabbedPage myTabbedPage;
        private ICustomMenu mMenu;
        private Android.Views.View actionBarCustomView;
        private int currentTabIndex = 0;
        int previousIndex = -1;

        private bool isCustomViewAdded = false;
        private int heightDiff;

        //public static List<ToolbarItem> originalToolBarItems;

        private static readonly GlobalLayoutListener listener = new GlobalLayoutListener();
        private static readonly List<CustomMenuItem> menuItems = new List<CustomMenuItem>();

        private List<string> childrenIcons = new List<string>();

        public static double originalPadding;

        protected BaseStarShot.Controls.TabbedPage BaseControl { get { return ((BaseStarShot.Controls.TabbedPage)this.Element); } }

        private int closeKeyboardCount = 0;
        private int openKeyboardCount = 0;

        public TabbedPageRenderer()
        {
            //originalToolBarItems = new List<ToolbarItem>();
            tvTemporary = new TextView(this.Context);
        }

        void listener_GlobalLayout(object sender, EventArgs e)
        {
            _activity.ActionBar.NavigationMode = ActionBarNavigationMode.Standard;

            if (_isFirstDesign)
            {
                ActionBarTabsSetup(_activity.ActionBar);
            }

            Android.Graphics.Rect r = new Android.Graphics.Rect();

            var activityRootView = _activity.Window.DecorView.FindViewById(Android.Resource.Id.Content);
            activityRootView.GetWindowVisibleDisplayFrame(r);

            heightDiff = activityRootView.RootView.Height - (r.Bottom - r.Top);

            if (heightDiff > 100)
            {
                if (openKeyboardCount > 0) { return; }

                openKeyboardCount++;
                closeKeyboardCount = 0;

                if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                {
                    myTabbedPage.Padding = 0;
                    mMenu.Hide();

                    _activity.Window.SetSoftInputMode(SoftInput.AdjustResize);
                }
            }
            else
            {
                if (closeKeyboardCount > 0) { return; }

                openKeyboardCount = 0;
                closeKeyboardCount++;

                if (mMenu.IsShowing())
                {
                    mMenu.UpdateBadgeCount(myTabbedPage.BadgeCount);
                    return;
                }

                mMenu.SetSelectedIndex(currentTabIndex);

                //if (myTabbedPage.Padding.Bottom != 0) //Uncomment this when using MONSO
                {
                    if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                        myTabbedPage.Padding = new Thickness(0, 0, 0, originalPadding);
                    else
                        myTabbedPage.Padding = new Thickness(0, originalPadding, 0, 0);

                    ShowCustomMenu();
                }

                mMenu.UpdateBadgeCount(myTabbedPage.BadgeCount);
            }
        }

        void ShowCustomMenu()
        {
            if (mMenu == null) return;

            var flag = (myTabbedPage.TabScreenLocation == TabGravity.Bottom) ? GravityFlags.Bottom : GravityFlags.Top;
            mMenu.Show(tvTemporary, flag, myTabbedPage.ShowIndicator);

            if (actionBarCustomView == null)
            {
                actionBarCustomView = mMenu.GetView();
                if (actionBarCustomView == null) return;

                AddCustomMenuView();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (!_isFirstDesign) return;

            if (e.OldElement != null)
            {
                listener.GlobalLayout -= listener_GlobalLayout;
                ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            }

            if (e.NewElement != null)
            {
                listener.GlobalLayout += listener_GlobalLayout;
                ViewTreeObserver.AddOnGlobalLayoutListener(listener);
            }

            myTabbedPage = (BaseStarShot.Controls.TabbedPage)Element;

            if (_activity == null)
            {
                _activity = this.Context as Activity;

                if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                    mMenu = new CustomMenuPopUp(this.Context, this, _activity.LayoutInflater);
                else
                    mMenu = new CustomMenu(this.Context, this, _activity.LayoutInflater);

                mMenu.SetBadgeLocation(myTabbedPage.BadgeLocation);
                mMenu.SetItemsCountInLine(myTabbedPage.ActiveIcons.Count);
                if (myTabbedPage.BackgroundDrawable != null && myTabbedPage.SelectedDrawable != null)
                    mMenu.SetBackgroundDrawable(myTabbedPage.BackgroundDrawable.File, myTabbedPage.SelectedDrawable.File);

                if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                    originalPadding = myTabbedPage.Padding.Bottom;
                else
                    originalPadding = myTabbedPage.Padding.Top;
            }

            if (childrenIcons.Count == 0)
            {
                for (int i = 0; i < myTabbedPage.Children.Count; i++)
                {
                    if (myTabbedPage.Children[i].Icon.File != null)
                        childrenIcons.Add(myTabbedPage.Children[i].Icon.File);
                }
            }

            myTabbedPage.Appearing += myTabbedPage_Appearing;
            myTabbedPage.Disappearing += myTabbedPage_Disappearing;

        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var element = sender as BaseStarShot.Controls.TabbedPage;
            //myTabbedPage = element;

            switch (e.PropertyName)
            {
                case "BadgeCount":
                    mMenu.UpdateBadgeCount(element.BadgeCount);
                    break;
                case "ShowTabs":
                    if (element.ShowTabs)
                    {
                        if (mMenu.IsShowing()) return;

                        if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                            myTabbedPage.Padding = new Thickness(0, 0, 0, originalPadding);
                        else
                            originalPadding = myTabbedPage.Padding.Bottom;

                        ShowCustomMenu();
                    }
                    else
                    {
                        myTabbedPage.Padding = 0;
                        mMenu.Hide();
                    }

                    break;

                case "CurrentPage":
                    currentTabIndex = myTabbedPage.Children.IndexOf(element.CurrentPage);

                    SetUpMenu();

                    //if (currentTabIndex != 0)
                    //    SetCustomIcon("empty_layout");

                    break;

                case "SelectedItem":
                    currentTabIndex = myTabbedPage.Children.IndexOf(element.CurrentPage);

                    SetUpMenu();

                    break;
                case "Padding":
                    if (element.Padding == 0)
                    {
                        element.Padding = 0;
                        mMenu.Hide();
                    }
                    else if (element.Padding == -1)
                    {
                        if (element.TabScreenLocation == TabGravity.Bottom)
                            element.Padding = new Thickness(0, 0, 0, originalPadding);
                        else
                            element.Padding = new Thickness(0, originalPadding, 0, 0);

                        ShowCustomMenu();
                    }
                    break;
                case "IsEnabled":
                    mMenu.EnableTabs(element.IsEnabled);
                    break;
            }
        }

        void SetUpMenu()
        {
            if (currentTabIndex == previousIndex) return;

            mMenu.SetSelectedIndex(currentTabIndex);

            mMenu.UpdateSelectedTab(_activity, myTabbedPage.ActiveIcons, childrenIcons);

            previousIndex = currentTabIndex;
        }

        void myTabbedPage_Disappearing(object sender, EventArgs e)
        {
            mMenu.Hide();

            listener.GlobalLayout -= listener_GlobalLayout;
            ViewTreeObserver.RemoveOnGlobalLayoutListener(listener);
            //_isFirstDesign = true;
            //_activity.ActionBar.SetCustomView(UIHelper.GetLayoutResource("empty_layout"));
        }

        protected override void OnWindowVisibilityChanged(ViewStates visibility)
        {
            if (visibility == ViewStates.Gone)
                _isFirstDesign = true;
            //else
            //    _isFirstDesign = false;

            base.OnWindowVisibilityChanged(visibility);
        }

        void myTabbedPage_Appearing(object sender, EventArgs e)
        {
            //_isFirstDesign = true;
            if (listener.Handle != IntPtr.Zero)
                ViewTreeObserver.AddOnGlobalLayoutListener(listener);

            if (heightDiff <= 100)
            {
                if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                    myTabbedPage.Padding = new Thickness(0, 0, 0, originalPadding);
                else
                    myTabbedPage.Padding = new Thickness(0, originalPadding, 0, 0);

                ShowCustomMenu();
            }

            currentTabIndex = this.Element.Children.IndexOf(this.Element.CurrentPage);

            //if (currentTabIndex == 0 && myTabbedPage.Children[0].Navigation.NavigationStack.Count <= 2)
            //    SetCustomIcon("custom_setting");
            //else
            //    SetCustomIcon("empty_layout");

            mMenu.SetSelectedIndex(currentTabIndex);
        }

        private void ActionBarTabsSetup(ActionBar actionBar)
        {
            if (actionBar.TabCount == 0) return;

            if (menuItems.Count > 0) menuItems.Clear();

            mMenu.SetTextColor(myTabbedPage.TextColor.ToAndroid(), myTabbedPage.SelectedTextColor.ToAndroid());

            int ctr = 0;
            foreach (var page in myTabbedPage.Children)
            {
                try
                {
                    var cmi = new CustomMenuItem();
                    cmi.Id = ctr;
                    cmi.Caption = (myTabbedPage.TextVisible) ? page.Title : "";

                    if (myTabbedPage.ActiveIcons.Count > 0)
                        cmi.ActiveImageId = UIHelper.GetDrawableResource(myTabbedPage.ActiveIcons.ElementAt(ctr));

                    if (page.Icon != null && page.Icon.File != null)
                        cmi.ImageResourceId = UIHelper.GetDrawableResource(page.Icon);

                    if (cmi.Id == myTabbedPage.BadgeLocation)
                        cmi.Badge = myTabbedPage.BadgeCount.ToString();

                    menuItems.Add(cmi);

                    if (!mMenu.IsShowing())
                    {
                        try
                        {
                            mMenu.SetMenuItems(menuItems);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }

                    ctr++;
                }
                catch (Exception)
                {
                    continue;
                }

                _isFirstDesign = false;

            }

            mMenu.SetSelectedIndex(currentTabIndex);

            if (mMenu.IsShowing()) return;

            if (myTabbedPage.TabScreenLocation == TabGravity.Bottom)
                myTabbedPage.Padding = new Thickness(0, 0, 0, originalPadding);
            else
                myTabbedPage.Padding = new Thickness(0, originalPadding, 0, 0);

            ShowCustomMenu();

            mMenu.UpdateBadgeCount(myTabbedPage.BadgeCount);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            //if (changed)
            {
                //var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
                if (actionBarCustomView != null)
                {
                    var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);

                    AddCustomMenuView();

                    int x2 = r - l;
                    if (msw <= 0) msw = (int)Globals.ScreenWidth;
                    if (x2 <= 0) x2 = (int)Globals.ScreenWidth;

                    actionBarCustomView.Measure(msw, UIHelper.ConvertDPToPixels(50));//MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly));
                    actionBarCustomView.Layout(0, 0, x2, UIHelper.ConvertDPToPixels(50));//b - t);

                    actionBarCustomView.Visibility = ViewStates.Visible;
                }

            }
        }

        void AddCustomMenuView()
        {
            try
            {
                if (isCustomViewAdded)
                {
                    this.RemoveView(actionBarCustomView);
                    this.RemoveViewInLayout(actionBarCustomView);
                }

                //var layout = new Android.Widget.RelativeLayout.LayoutParams(
                //    Android.Widget.RelativeLayout.LayoutParams.MatchParent, Android.Widget.RelativeLayout.LayoutParams.WrapContent);
                //actionBarCustomView.Id = 2;
                //layout.AddRule(LayoutRules.CenterInParent, actionBarCustomView.Id);

                //Display display = _activity.WindowManager.DefaultDisplay;
                //Android.Graphics.Point size = new Android.Graphics.Point();
                //display.GetSize(size);
                //int height = size.Y;

                var layout = new Android.Widget.LinearLayout.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
                layout.Gravity = GravityFlags.Bottom;

                //layout.SetMargins(0, height, 0, 0);

                //var windowManager = this.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                //var layout = new WindowManagerLayoutParams(
                //    0, 500,
                //    ViewGroup.LayoutParams.MatchParent,
                //    ViewGroup.LayoutParams.WrapContent,
                //    WindowManagerTypes.Phone,
                //    WindowManagerFlags.Fullscreen,
                //    Format.Translucent);
                //layout.Gravity = GravityFlags.Bottom;


                actionBarCustomView.SetBackgroundColor(myTabbedPage.TabBackgroundColor.ToAndroid());

                this.AddView(actionBarCustomView, layout);

                isCustomViewAdded = true;
            }
            catch (Exception)
            {
                this.RemoveView(actionBarCustomView);
                this.RemoveViewInLayout(actionBarCustomView);

                AddCustomMenuView();
            }
        }

        public void MenuItemSelectedEvent(CustomMenuItem selection, List<ImageView> icons, List<TextView> titleTextView)
        {
            int index = selection.Id;
            currentTabIndex = index;

            SetUpMenu();

            myTabbedPage.RaiseTappedIndex(currentTabIndex);

            myTabbedPage.CurrentPage = myTabbedPage.Children[selection.Id];
            myTabbedPage.SelectedItem = myTabbedPage.Children[selection.Id];

        }
    }
}