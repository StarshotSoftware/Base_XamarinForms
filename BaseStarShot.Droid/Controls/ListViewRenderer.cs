using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Base1902;
using BaseStarShot.Services;
using BaseStarShot.Util;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.ListView), typeof(BaseStarShot.Controls.ListViewRenderer))]
namespace BaseStarShot.Controls
{
    public class ListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer, AbsListView.IOnScrollListener
    {
        private Activity _activity;
        private TextView _emptyViewTv;
        private ScrollChildSwipeRefreshLayout _swipeContainer;
        private INotifyCollectionChanged collectionChanged = null;

        private ScrollState previousState = ScrollState.Idle;
        private bool isRefreshEnabled;
        private static bool isScrollingUp = false;
        private static bool skipLastVisibleItem = true;

        public ListViewRenderer()
        {
            _activity = this.Context as Activity;
        }

        public ListViewRenderer(IntPtr javaReference, JniHandleOwnership transfer)
            : this()
        {
        }

        protected ListView BaseControl { get { return ((BaseStarShot.Controls.ListView)this.Element); } }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (Control == null)
                return;

            if (previousState != ScrollState.Idle && scrollState == ScrollState.Idle)
            {
                int count = view.ChildCount - 2; // visible views count
                int lastVisibleItemPosition = view.LastVisiblePosition - 2;

                if (view.FirstVisiblePosition == 0 && isScrollingUp)
                    BaseControl.OnScrolledTop();
            }

            //if (scrollState == ScrollState.Idle)
            if (!skipLastVisibleItem)
            {
                BaseControl.LastVisiblePosition = view.LastVisiblePosition - 2;
            }

            previousState = scrollState;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var swipeRefreshParam = new SwipeRefreshLayout.LayoutParams(SwipeRefreshLayout.LayoutParams.MatchParent, SwipeRefreshLayout.LayoutParams.WrapContent);
            var gridParams0 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.Center), GridLayout.InvokeSpec(0, GridLayout.Fill));
            var gridParams1 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.TopAlighment), GridLayout.InvokeSpec(0, GridLayout.Fill));

            if (Element.IsPullToRefreshEnabled)
            {
                isRefreshEnabled = Element.IsPullToRefreshEnabled;
                Element.IsPullToRefreshEnabled = false;

                CreateEmptyView();

                var parent = (Android.Support.V4.Widget.SwipeRefreshLayout)Control.Parent;
                parent.RemoveAllViews();

                _swipeContainer = new ScrollChildSwipeRefreshLayout(this.Context);
                _swipeContainer.Enabled = isRefreshEnabled;

                _swipeContainer.LayoutParameters = swipeRefreshParam;
                _swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight);

                //_swipeContainer.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
                //    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                _swipeContainer.Refresh += SwipeContainer_Refresh;

                var grid = new Android.Widget.GridLayout(this.Context);
                grid.LayoutParameters = swipeRefreshParam;
                grid.RowCount = 1;
                grid.ColumnCount = 1;

                grid.AddView(_emptyViewTv, 0, gridParams0);
                grid.AddView(Control, 1, gridParams1);

                _swipeContainer.AddView(grid);
                parent.AddView(_swipeContainer, swipeRefreshParam);
                parent.RequestLayout();

                //Control.EmptyView = _emptyViewTv;
            }
            else
            {
                isRefreshEnabled = Element.IsPullToRefreshEnabled;
                CreateEmptyView();
            }

            SetSeparator();
            SetPadding();
            SetSelector();
            Control.SetHeaderDividersEnabled(false);
            Control.SetFooterDividersEnabled(false);

            Control.ChoiceMode = ChoiceMode.Single;

            if (isRefreshEnabled)
            {
                //if (Element.IsRefreshing)
                //    shouldExecuteCommand = false;

                Control.Post(() =>
                {
                    try
                    {
                        _swipeContainer.Refreshing = Element.IsRefreshing;

                    }
                    catch (Exception ex)
                    {

                    }
                });
            }

            SetItemsSource(true);
            Control.SetOnScrollListener(this);
            Control.SetOnTouchListener(new MyTouchListener());
            BaseControl.ScrollToBottomEvent += BaseControl_ScrollAndroidListView;
            //            BaseControl.ScrollToTopEvent += BaseControl_ScrollToTopEvent;
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "ItemsSource":
                    SetItemsSource(); break;
                case "Padding": SetPadding(); break;
                case "SeparatorVisibility": SetSeparator(); break;
                case "Selector": SetSelector(); break;
                case "EmptyText":
                    if (_emptyViewTv != null && BaseControl != null)
                    {
                        _emptyViewTv.Text = BaseControl.EmptyText;
                    }
                    break;

                case "IsRefreshing":

                    UpdateEmptyLabelVisibility();
                    //if (Element.IsRefreshing)
                    //    shouldExecuteCommand = false;
                    if (_swipeContainer != null && Element != null)
                    {
                        if(Control != null)
                        {
                            Control.Post(() =>
                            {
								if(Element != null)
                                	_swipeContainer.Refreshing = Element.IsRefreshing;
                            });
                        }
                        
                    }
                    break;

                case "Height":
                    if (!isRefreshEnabled)
                        if (Element != null && _emptyViewTv != null && !string.IsNullOrWhiteSpace(BaseControl.EmptyText))
                            _emptyViewTv.SetHeight(UIHelper.ConvertDPToPixels(Element.Height));

                    break;

                case "ScrollToTopChange":
                    ScrollToTop();
                    break;
                case "ScrollToIndex":
                    if (BaseControl.ScrollToIndex > 0)
                    {
                        DeterminedScrollTo(BaseControl.ScrollToIndex, 3);
                        //Control.SmoothScrollToPosition(BaseControl.ScrollToIndex, 100);
                    }
                    break;
            }
        }

        private void BaseControl_ScrollAndroidListView(object sender, int position)
        {
            if (Control == null)
                return;

            DeterminedScrollTo(position, 3);
            //await Task.Delay(200);
            //Control.SmoothScrollToPosition(position, 100);
            BaseControl.LastVisiblePosition = position;

            //Control.SmoothScrollToPositionFromTop(position, 100);
        }

        private void DeterminedScrollTo(int index, int attempts = 0)
        {
            if (Control == null)
                return;

            if (Control.FirstVisiblePosition != index && attempts < 10)
            {
                attempts++;
                Control.SmoothScrollToPositionFromTop(index, 1, 100);
                Control.PostDelayed(() =>
                {
                    DeterminedScrollTo(index, attempts);
                }, 100);
            }
        }

        private void BaseControl_ScrollToTopEvent(object sender, EventArgs e)
        {
            if (Control == null)
                return;

            Control.SmoothScrollToPositionFromTop(0, 100);
        }

        private void CreateEmptyView()
        {
            _emptyViewTv = new TextView(this.Context);
            _emptyViewTv.Text = BaseControl.EmptyText;
            _emptyViewTv.TextAlignment = Android.Views.TextAlignment.Center;
            _emptyViewTv.Gravity = GravityFlags.Center;
            _emptyViewTv.SetTextColor(BaseControl.EmptyTextColor.ToAndroid());
            var fontName = Resolver.Get<IFontService>().GetFontName(BaseControl.EmptyTextFontStyle);
            if (!string.IsNullOrEmpty(fontName))
                _emptyViewTv.Typeface = FontCache.GetTypeFace(fontName);

            SetVisibility(ViewStates.Gone);
        }

        private ShapeDrawable CreateMyDrawable(Android.Graphics.Color color)
        {
            ShapeDrawable shapeDrawable = new ShapeDrawable();
            shapeDrawable.Paint.Color = color;
            if (color != Android.Graphics.Color.LightGray || color != Android.Graphics.Color.LightBlue || color != Android.Graphics.Color.Transparent)
                shapeDrawable.SetBounds(UIHelper.ConvertDPToPixels(15), 0, Control.Width, 1);

            if (color == Android.Graphics.Color.Transparent)
            {
                shapeDrawable.SetBounds(0, 0, 0, 0);
            }
            return shapeDrawable;
        }

        private int GetItemsSourceCount()
        {
            if (Element == null)
                return 0;

            if (Element.ItemsSource != null)
            {
                var prop = Element.ItemsSource.GetType().GetRuntimeProperty("Count");

                if (prop != null && prop.GetValue(Element.ItemsSource) != null)
                {
                    var count = int.Parse(prop.GetValue(Element.ItemsSource).ToString());
                    return count;
                }
            }

            return 0;
        }

        private async void ScrollToTop()
        {
            if (Control == null)
                return;

            if (BaseControl.ScrollToTopChange)
            {
                Control.SmoothScrollToPositionFromTop(0, 100);
                BaseControl.ScrollToTopChange = false;
            }
        }

        private void SetItemsSource(bool isInitLoad = false)
        {
            var element = Element;
            if (Element == null || Element.ItemsSource == null)
            {
                if (_emptyViewTv != null)
                    SetVisibility(ViewStates.Visible);
                return;
            }

            if (collectionChanged == null)
            {
                if (!(Element.ItemsSource is INotifyPropertyChanged))
                    return;

                collectionChanged = Element.ItemsSource as INotifyCollectionChanged;

                int itemCount = 0;
                collectionChanged.CollectionChanged += (s, ev) =>
                {
                    if (Element == null)
                        return;

                    itemCount = ev.NewStartingIndex;
                    if (Element != null)
                    {
                        if (ev.Action == NotifyCollectionChangedAction.Add)
                        {
                            if (_emptyViewTv != null && _emptyViewTv.Visibility == ViewStates.Visible && !Element.IsRefreshing)
                                SetVisibility(ViewStates.Gone);
                        }
                        else if (ev.Action == NotifyCollectionChangedAction.Remove)
                        {
                            if (ev.NewItems != null && ev.OldItems != null && !Element.IsRefreshing)
                            {
                                if (_emptyViewTv != null && ev.NewItems.Count - ev.OldItems.Count < 0)
                                    SetVisibility(ViewStates.Visible);
                            }
                        }
                        else if (ev.Action == NotifyCollectionChangedAction.Reset)
                        {
                            if (_emptyViewTv != null && !Element.IsRefreshing)
                                SetVisibility(ViewStates.Visible);
                        }
                    }
                };
            }

            UpdateEmptyLabelVisibility(isInitLoad);
        }

        private void SetPadding()
        {
            Control.SetPadding(UIHelper.ConvertDPToPixels(BaseControl.Padding.Left), UIHelper.ConvertDPToPixels(BaseControl.Padding.Top),
                UIHelper.ConvertDPToPixels(BaseControl.Padding.Right), UIHelper.ConvertDPToPixels(BaseControl.Padding.Bottom));
        }

        private void SetSelector()
        {
            switch (BaseControl.Selector)
            {
                case ListViewSelector.None:
                    Control.CacheColorHint = Android.Graphics.Color.Transparent;
                    Control.Selector = CreateMyDrawable(Android.Graphics.Color.Transparent); break;
                case ListViewSelector.Blue:
                    Control.Selector = CreateMyDrawable(Android.Graphics.Color.LightBlue); break;
                case ListViewSelector.Default:
                    break;
            }
        }

        private void SetSeparator()
        {
            if (BaseControl.SeparatorVisibility != SeparatorVisibility.None)
            {
                if (BaseControl.SeparatorBackground != null && BaseControl.SeparatorBackground.File != null)
                    Control.Divider = this.Resources.GetDrawable(UIHelper.GetDrawableResource(BaseControl.SeparatorBackground));
                else
                {
                    Control.Divider = CreateMyDrawable(BaseControl.SeparatorColor.ToAndroid());
                }

                Control.DividerHeight = UIHelper.ConvertDPToPixels(BaseControl.SeparatorHeight);
            }
        }

        private void SetVisibility(ViewStates state)
        {
            if (Element == null || Control == null)
                return;

            if (string.IsNullOrWhiteSpace(BaseControl.EmptyText))
                return;

            if (isRefreshEnabled)
                _emptyViewTv.Visibility = state;
            else
            {
                if (state == ViewStates.Visible)
                    Control.AddFooterView(_emptyViewTv, null, false);
                else
                    Control.RemoveFooterView(_emptyViewTv);
            }
        }

        private async void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            if (Element == null || Control == null)
                return;

            SetVisibility(ViewStates.Gone);

            await Task.Delay(500);

            if (Element.RefreshCommand != null && Element.RefreshCommand.CanExecute(null))
                Element.RefreshCommand.Execute(null);

        }

        private async void UpdateEmptyLabelVisibility(bool isInitLoad = false)
        {
            if (Element.IsRefreshing)
                SetVisibility(ViewStates.Gone);
            else
            {
                if (isInitLoad)
                    await Task.Delay(5000);

                var count = GetItemsSourceCount();
                SetVisibility((GetItemsSourceCount() > 0) ? ViewStates.Gone : ViewStates.Visible);
            }
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
			if (Control != null && Control.ChildCount > 0 && _swipeContainer != null)
            {
                var y = Control.GetChildAt(0).GetY();

                if (y >= -10 && y <= 0f)
					_swipeContainer.Enabled = isRefreshEnabled;
				else
					_swipeContainer.Enabled = false;
			}

        }

        public class MyTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
        {
            private float initialY, finalY;

			private static float OVERSCROLL_THRESHOLD_IN_PIXELS = 100;

			private float downY;


            public bool OnTouch(Android.Views.View v, MotionEvent e)
            {
                if (e.Action == MotionEventActions.Down)
                {
                    initialY = e.GetY();
                }
                if (e.Action == MotionEventActions.Up)
                {
                    finalY = e.GetY();
                    if (initialY < finalY)
                        isScrollingUp = true;
                    else if (initialY > finalY)
                        isScrollingUp = false;

                    skipLastVisibleItem = false;
                }

                return false;
            }
        }
    }
}

public class ScrollChildSwipeRefreshLayout : SwipeRefreshLayout//, Android.Views.View.IOnTouchListener
{
    public ScrollChildSwipeRefreshLayout(Context context)
        : base(context)
    {
        //this.SetOnTouchListener(this);
    }

    public ScrollChildSwipeRefreshLayout(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
    }

    // The current SwipeRefreshLayout only check its immediate child scrollability.
    // In our case, ListFragment uses a ListView inside a parent FrameLayout which breaks this.
    public override bool CanChildScrollUp()
    {
        GridLayout child = GetChildAt(0) as GridLayout;

        if (child != null)
        {
            var listView = child.GetChildAt(1) as Android.Widget.ListView;

            if (listView != null)
            {
                if (listView.FirstVisiblePosition == 0)
                {
                    //var subChild = child.GetChildAt(0);
                    //var scroll = subChild != null && subChild.Top != 0;
                    return false;
                }

                return true;
            }
        }

        return false;
    }

}