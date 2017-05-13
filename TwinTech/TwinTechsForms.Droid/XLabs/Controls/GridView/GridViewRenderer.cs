using System;
using Xamarin.Forms.Platform.Android;
using Android.Support.V7.Widget;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Android.Content.Res;
using Android.Views;
using LabsGridView = XLabs.Forms.Controls.GridView;
using System.Collections;
using Android.Widget;
using Base1902;
using Base1902.Services;
using Android.Graphics;
using System.Reflection;

[assembly: ExportRenderer(typeof(LabsGridView), typeof(GridViewRenderer))]
namespace XLabs.Forms.Controls
{
    public class GridViewRenderer : ViewRenderer<LabsGridView, Android.Widget.GridLayout>
    {
        private readonly Android.Content.Res.Orientation _orientation = Android.Content.Res.Orientation.Undefined;

        ScrollRecyclerView _recyclerView;

        private RecyclerView.LayoutManager _layoutManager;
        private GridViewAdapter _adapter;

        RecyclerView.ItemDecoration _paddingDecoration;
        private TextView emptyTextView = null;

        public GridViewRenderer()
        {
        }


        #region overridden

        protected override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (newConfig.Orientation != _orientation)
                OnElementChanged(new ElementChangedEventArgs<LabsGridView>(Element, Element));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XLabs.Forms.Controls.GridView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                CreateRecyclerView();

                if (emptyTextView == null)
                {
                    emptyTextView = new Android.Widget.TextView(Xamarin.Forms.Forms.Context);
                    emptyTextView.SetScrollContainer(true);
                    emptyTextView.Gravity = GravityFlags.Center;
                    emptyTextView.Text = Element.EmptyText;
                    emptyTextView.SetTextColor(Android.Graphics.Color.Black);
                    emptyTextView.Visibility = ViewStates.Gone;

                    var fontName = Resolver.Get<IFontService>().GetFontName(FontStyle.Italic);
                    if (!string.IsNullOrEmpty(fontName))
                        emptyTextView.Typeface = Typeface.CreateFromAsset(this.Context.Assets, fontName);
                }

                var grid = new Android.Widget.GridLayout(this.Context);
                grid.RowCount = 1;
                grid.ColumnCount = 1;

                var gridParams0 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.Fill), GridLayout.InvokeSpec(0, GridLayout.Fill));
                var gridParams1 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.TopAlighment), GridLayout.InvokeSpec(0, GridLayout.Fill));

                grid.AddView(emptyTextView, 0, gridParams0);
                grid.AddView(_recyclerView, 1, gridParams0);

                base.SetNativeControl(grid);
            }
            //TODO unset
            //			this.Unbind (e.OldElement);
            //			this.Bind (e.NewElement);

        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == GridView.RefreshingProperty.PropertyName)
            {
                if (!Element.Refreshing)
                {
                    var prop = Element.ItemsSource.GetType().GetRuntimeProperty("Count");

                    if (prop != null && prop.GetValue(Element.ItemsSource) != null)
                    {
                        if (Element.PullToRefreshEnabled)
                        {
                            var count = int.Parse(prop.GetValue(Element.ItemsSource).ToString());

                            if (count == 0)
                            {
                                emptyTextView.Visibility = ViewStates.Visible;
                                //_recyclerView.Visibility = ViewStates.Gone;
                            }
                            else
                            {
                                emptyTextView.Visibility = ViewStates.Gone;
                                //_recyclerView.Visibility = ViewStates.Visible;
                            }

                        }

                        //Control.RequestLayout();
                        //Control.Invalidate();
                    }
                    else
                    {
                        emptyTextView.Visibility = ViewStates.Visible;
                    }
                }
            }

            if (e.PropertyName == "ItemsSource")
            {
                _adapter.Items = Element.ItemsSource;
            }

            if (e.PropertyName == "IsScrollEnabled")
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _recyclerView.Enabled = Element.IsScrollEnabled;
                });
            }
        }

        #endregion

        void DestroyRecyclerview()
        {
            //TODO
            _recyclerView.Touch -= _recyclerView_Touch;
        }

        void CreateRecyclerView()
        {
            _recyclerView = new ScrollRecyclerView(Android.App.Application.Context);
            _recyclerView.Touch += _recyclerView_Touch;
            var scrollListener = new GridViewScrollListener(Element, _recyclerView);
            _recyclerView.AddOnScrollListener(scrollListener);
            if (Element.IsHorizontal)
            {
                var linearLayoutManager = new LinearLayoutManager(Context, OrientationHelper.Horizontal, false);
                _layoutManager = linearLayoutManager;

            }
            else
            {
                var gridlayoutManager = new GridLayoutManager(Context, 1);

                _layoutManager = gridlayoutManager;

            }
            _recyclerView.SetLayoutManager(_layoutManager);
            _recyclerView.SetItemAnimator(null);
            _recyclerView.HasFixedSize = true;

            _recyclerView.HorizontalScrollBarEnabled = Element.IsHorizontal;
            _recyclerView.VerticalScrollBarEnabled = !Element.IsHorizontal;

            _adapter = new GridViewAdapter(Element.ItemsSource, _recyclerView, Element, Resources.DisplayMetrics);

            _recyclerView.SetAdapter(_adapter);
            UpdatePadding();
        }


        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdatePadding();
        }

        void UpdatePadding()
        {
            _recyclerView.SetPadding((int)Element.ContentPaddingLeft,
                (int)Element.ContentPaddingTop,
                (int)Element.ContentPaddingRight,
                (int)Element.ContentPaddingBottom);
            if (Element.IsHorizontal)
            {
                if (_paddingDecoration != null)
                {
                    _recyclerView.RemoveItemDecoration(_paddingDecoration);
                }
                var source = Element.ItemsSource as ICollection;
                var numberOfItems = source == null ? 0 : source.Count;

                _paddingDecoration = new HorizontalSpacesItemDecoration(ConvertDpToPixels((float)Element.ColumnSpacing / 2), ConvertDpToPixels((int)Element.RowSpacing));
                _recyclerView.AddItemDecoration(_paddingDecoration);

            }
            else
            {
                UpdateGridLayout();
            }

        }

        void UpdateGridLayout()
        {
            var source = Element.ItemsSource as ICollection;
            var numberOfItems = source == null ? 0 : source.Count;

            var horizontalPadding = 0;
            int numberOfItemsToUse = 1;
            if (_layoutManager != null)
            {
                if (Element.Width > 0)
                {
                    float width = ConvertDpToPixels((float)Element.Width);
                    var itemWidth = ConvertDpToPixels(Element.ItemWidth);
                    var columnSpacing = ConvertDpToPixels(Element.ColumnSpacing);
                    var contentPaddingLeft = ConvertDpToPixels(Element.ContentPaddingLeft);
                    var contentPaddingRight = ConvertDpToPixels(Element.ContentPaddingRight);

                    //					float width = (float)_recyclerView.Width - 2;
                    if (Element.IsContentCentered)
                    {
                        int numberOfItemsThatFit = (int)Math.Floor((width) / (itemWidth + columnSpacing));
                        numberOfItemsToUse = Element.CenterAsFilledRow ? numberOfItemsThatFit : (int)Math.Min(numberOfItemsThatFit, numberOfItems);
                        if (Element.MaxItemsPerRow != -1)
                        {
                            numberOfItemsToUse = Element.MaxItemsPerRow;
                        }
                        var remainingWidth = (width - (contentPaddingLeft + contentPaddingRight)) - ((numberOfItemsToUse * itemWidth) + ((numberOfItemsToUse) * (columnSpacing)));

                        horizontalPadding = (int)(remainingWidth / (numberOfItemsToUse + 1));
                    }
                    else
                    {
                        horizontalPadding = columnSpacing;
                    }
                }
            }


            var gridLayoutManager = _layoutManager as GridLayoutManager;
            if (gridLayoutManager != null)
            {
                //TODO calculate
                gridLayoutManager.SpanCount = Math.Max(1, numberOfItemsToUse);
            }
            //TODO
            if (_paddingDecoration != null)
            {
                _recyclerView.RemoveItemDecoration(_paddingDecoration);
            }
            _paddingDecoration = new SpacesItemDecoration(horizontalPadding, ConvertDpToPixels((int)Element.RowSpacing),
                numberOfItems, numberOfItemsToUse,
                ConvertDpToPixels((int)Element.ContentPaddingTop), ConvertDpToPixels((int)Element.ContentPaddingBottom));

            //			_paddingDecoration = new SpacesItemDecoration (horizontalPadding, (int)Element.RowSpacing, 
            //				numberOfItems, numberOfItemsToUse, 
            //				(int)Element.ContentPaddingTop, (int)Element.ContentPaddingBottom);
            _recyclerView.AddItemDecoration(_paddingDecoration);

        }


        private int ConvertDpToPixels(double dpValue)
        {
            var pixels = (int)((dpValue) * Resources.DisplayMetrics.Density);
            return pixels;
        }

        float _startEventY;
        float _heightChange;


        void _recyclerView_Touch(object sender, TouchEventArgs e)
        {
            var ev = e.Event;
            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            switch (action)
            {
                case MotionEventActions.Down:
                    _startEventY = ev.GetY();
                    _heightChange = 0;
                    Element.RaiseOnStartScroll();
                    break;
                case MotionEventActions.Move:
                    float delta = (ev.GetY() + _heightChange) - _startEventY;
                    Element.RaiseOnScroll(delta, _recyclerView.GetVerticalScrollOffset());
                    break;
                case MotionEventActions.Up:
                    Element.RaiseOnStopScroll();
                    break;
            }
            e.Handled = false;

        }
    }

    public class SpacesItemDecoration : RecyclerView.ItemDecoration
    {
        int _columnSpacing;
        int _rowSpacing;
        int _numberOfItemsPerRow;
        int _numberOfItems;
        int _topSpacing;
        int _bottomSpacing;

        public SpacesItemDecoration(int columnSpacing, int rowSpacing, int numberOfItems, int numberOfItemsPerRow, int topSpacing, int bottomSpacing)
        {
            _rowSpacing = rowSpacing;
            _columnSpacing = columnSpacing;
            _numberOfItems = numberOfItems;
            _numberOfItemsPerRow = numberOfItemsPerRow;
            _topSpacing = topSpacing;
            _bottomSpacing = bottomSpacing;
        }

        public override void GetItemOffsets(Android.Graphics.Rect outRect, int itemPosition, RecyclerView parent)
        {
            //TODO - work out if the rectangle is the last/first row/column
            if (itemPosition % _numberOfItemsPerRow == 0)
            {
                //first col
                outRect.Left = _columnSpacing;
            }
            else
            {
                outRect.Left = _columnSpacing / 2;
            }
            if (itemPosition % _numberOfItemsPerRow == (_numberOfItemsPerRow - 1))
            {
                //last col
                outRect.Right = _columnSpacing;
            }
            else
            {
                outRect.Right = _columnSpacing / 2;
            }
            //TODO write a custom layout for android
            //			if (itemPosition < _numberOfItemsPerRow) {
            //				outRect.Top = _topSpacing;
            //			} 
            //			if (itemPosition > (_numberOfItems - _numberOfItemsPerRow)) {
            //				outRect.Bottom = _bottomSpacing;
            //			} else {
            outRect.Bottom = _rowSpacing;
            //			}
        }
    }

    public class HorizontalSpacesItemDecoration : RecyclerView.ItemDecoration
    {
        int _columnSpacing;
        int _rowSpacing;

        public HorizontalSpacesItemDecoration(int columnSpacing, int rowSpacing)
        {
            _rowSpacing = rowSpacing;
            _columnSpacing = columnSpacing;
            _columnSpacing = columnSpacing;
        }

        public override void GetItemOffsets(Android.Graphics.Rect outRect, int itemPosition, RecyclerView parent)
        {
            outRect.Left = _columnSpacing / 2;
            outRect.Right = _columnSpacing / 2;
            outRect.Bottom = _rowSpacing;
        }
    }

    public class ScrollRecyclerView : RecyclerView
    {
        public ScrollRecyclerView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }


        public ScrollRecyclerView(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }


        public ScrollRecyclerView(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
        {
        }


        public ScrollRecyclerView(Android.Content.Context context) : base(context)
        {
        }

        public int GetVerticalScrollOffset()
        {
            return ComputeVerticalScrollOffset();
        }

        public int GetHorizontalScrollOffset()
        {
            return ComputeHorizontalScrollOffset();
        }
    }

    public class GridViewScrollListener : RecyclerView.OnScrollListener
    {
        LabsGridView _gridView;

        ScrollRecyclerView _recyclerView;

        public GridViewScrollListener(GridView gridView, ScrollRecyclerView recyclerView)
        {
            _gridView = gridView;
            _recyclerView = recyclerView;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            _gridView.RaiseOnScroll(dy, _recyclerView.GetVerticalScrollOffset());

            if (dy > 0)
            {
                var manager = (_recyclerView.GetLayoutManager() is GridLayoutManager) ?
                    (GridLayoutManager)_recyclerView.GetLayoutManager() : (LinearLayoutManager)_recyclerView.GetLayoutManager();

                int lastVisibleItemPosition = manager.FindLastVisibleItemPosition();
                int visibleItemCount = recyclerView.ChildCount;
                int totalItemCount = manager.ItemCount;

                if (lastVisibleItemPosition == totalItemCount - 1)
                {
                    _gridView.InvokeLoadMoreEvent();
                }

            }
        }
    }
}








//using System;
//using Xamarin.Forms.Platform.Android;
//using Android.Support.V7.Widget;
//using Xamarin.Forms;
//using XLabs.Forms.Controls;
//using Android.Content.Res;
//using Android.Views;
//using LabsGridView = XLabs.Forms.Controls.GridView;
//using System.Reflection;

//[assembly: ExportRenderer(typeof(LabsGridView), typeof(GridViewRenderer))]
//namespace XLabs.Forms.Controls
//{
//    using Android.Content;
//    using Android.Graphics;
//    using Android.Support.V4.Widget;
//    using Android.Util;
//    using Android.Widget;
//    using Base1902;
//    using Base1902.Controls;
//    using Base1902.Services;
//    using ICollection = System.Collections.ICollection;
//    using Math = System.Math;
//    using System.Collections.Specialized;

//    public class GridViewRenderer :  ViewRenderer<LabsGridView, SwipeRefreshLayout>
//    {
//		private readonly Android.Content.Res.Orientation _orientation = Android.Content.Res.Orientation.Undefined;

//		ScrollRecyclerView _recyclerView;
//        ScrollChildSwipeRefreshLayout swipeLayout;
//        private RecyclerView.LayoutManager _layoutManager;
//		private GridViewAdapter _adapter;

//		RecyclerView.ItemDecoration _paddingDecoration;

//        private TextView emptyTextView = null;
//        private Android.Graphics.Color recyclerViewColor;

//        public GridViewRenderer ()
//		{
//		}


//		#region overridden

//		protected override void OnConfigurationChanged (Configuration newConfig)
//		{
//			base.OnConfigurationChanged (newConfig);
//			if (newConfig.Orientation != _orientation)
//				OnElementChanged (new ElementChangedEventArgs<LabsGridView> (Element, Element));
//		}

//		protected override void OnElementChanged (ElementChangedEventArgs<XLabs.Forms.Controls.GridView> e)
//		{
//			base.OnElementChanged (e);
//			if (e.NewElement != null)
//            {
//                swipeLayout = new ScrollChildSwipeRefreshLayout(this.Context);

//                CreateRecyclerView ();

//                swipeLayout.SetLayoutManager(_recyclerView.GetLayoutManager());
//                swipeLayout.Enabled = Element.PullToRefreshEnabled;

//                if (swipeLayout.Enabled)
//                {
//                    swipeLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight,
//                    Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);

//                    swipeLayout.Refresh += SwipeLayout_Refresh;
//                }

//                if (emptyTextView == null)
//                {
//                    emptyTextView = new Android.Widget.TextView(Xamarin.Forms.Forms.Context);
//                    emptyTextView.SetScrollContainer(true);
//                    emptyTextView.Gravity = GravityFlags.Center;
//                    emptyTextView.Text = Element.EmptyText;
//                    emptyTextView.SetTextColor(Android.Graphics.Color.Black);
//                    emptyTextView.Visibility = ViewStates.Gone;

//                    var fontName = Resolver.Get<IFontService>().GetFontName(FontStyle.Italic);
//                    if (!string.IsNullOrEmpty(fontName))
//                        emptyTextView.Typeface = Typeface.CreateFromAsset(this.Context.Assets, fontName);
//                }

//                var param = new Android.Support.V4.Widget.SwipeRefreshLayout.LayoutParams(
//                      Android.Support.V4.Widget.SwipeRefreshLayout.LayoutParams.MatchParent,
//                      Android.Support.V4.Widget.SwipeRefreshLayout.LayoutParams.MatchParent);
//                //swipeLayout.AddView(_recyclerView, 0, param);
//                //swipeLayout.AddView(emptyTextView, 0, param);


//                var grid = new Android.Widget.GridLayout(this.Context);
//                //grid.LayoutParameters = param;
//                grid.RowCount = 1;
//                grid.ColumnCount = 1;

//                var gridParams0 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.Fill), GridLayout.InvokeSpec(0, GridLayout.Fill));
//                var gridParams1 = new GridLayout.LayoutParams(GridLayout.InvokeSpec(0, GridLayout.TopAlighment), GridLayout.InvokeSpec(0, GridLayout.Fill));

//                grid.AddView(emptyTextView, 0, gridParams0);
//                grid.AddView(_recyclerView, 1, gridParams1);

//                swipeLayout.AddView(grid, param);

//                base.SetNativeControl (swipeLayout);


//            }
//			//TODO unset
//			//			this.Unbind (e.OldElement);
//			//			this.Bind (e.NewElement);
//            SetItemSource();
//		}

//        INotifyCollectionChanged itemsSourceChange;

//        private void SetItemSource()
//        {
//            if (itemsSourceChange != null)
//                itemsSourceChange.CollectionChanged -= GridViewRenderer_CollectionChanged;

//            if (Element.ItemsSource is INotifyCollectionChanged)
//            {
//                itemsSourceChange = (Element.ItemsSource as INotifyCollectionChanged);
//                itemsSourceChange.CollectionChanged += GridViewRenderer_CollectionChanged;
//            }
//            if(Element.ItemsSource != null)
//                _adapter.Items = Element.ItemsSource;
//        }

//        void GridViewRenderer_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
//        {
//            _adapter.Items = Element.ItemsSource;
//        }

//        private void SwipeLayout_Refresh(object sender, EventArgs e)
//        {
//            if (Element.RefreshCommand != null && Element.RefreshCommand.CanExecute(null))
//                Element.RefreshCommand.Execute(null);
//        }

//        protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
//		{
//			base.OnElementPropertyChanged (sender, e);
//			if (e.PropertyName == "ItemsSource")
//            {
//                SetItemSource();
//			}

//            if (e.PropertyName == "Refreshing")
//            {
//                if (!Element.Refreshing)
//                {
//                    var prop = Element.ItemsSource.GetType().GetRuntimeProperty("Count");

//                    if (prop != null && prop.GetValue(Element.ItemsSource) != null)
//                    {
//                        if (Element.PullToRefreshEnabled)
//                        {
//                            var count = int.Parse(prop.GetValue(Element.ItemsSource).ToString());

//                            if (count == 0)
//                            {
//                                emptyTextView.Visibility = ViewStates.Visible;
//                                //_recyclerView.Visibility = ViewStates.Gone;
//                            }
//                            else
//                            {
//                                emptyTextView.Visibility = ViewStates.Gone;
//                                //_recyclerView.Visibility = ViewStates.Visible;
//                            }

//                            Control.Post(() =>
//                            {
//                                Control.Refreshing = false;
//                            });

//                        }

//                        //Control.RequestLayout();
//                        //Control.Invalidate();
//                    }
//                    else
//                    {
//                        emptyTextView.Visibility = ViewStates.Visible;
//                    }
//                }
//                else
//                {
//                    if (Element.PullToRefreshEnabled)
//                    {
//                        Control.Post(() =>
//                        {
//                            Control.Refreshing = true;
//                        });

//                        emptyTextView.Visibility = ViewStates.Gone;
//                    }
//                }
//            }

//			if (e.PropertyName == "IsScrollEnabled") {
//				Device.BeginInvokeOnMainThread (() => {
//					_recyclerView.Enabled = Element.IsScrollEnabled;
////					Debug.WriteLine ("scroll enabled changed to " + _gridCollectionView.ScrollEnabled);
//				}
//				);


//			}
//		}

//		#endregion

//		void DestroyRecyclerview ()
//		{
//			//TODO
//			_recyclerView.Touch -= _recyclerView_Touch;
//		}

//		void CreateRecyclerView ()
//		{
//			_recyclerView = new ScrollRecyclerView (Android.App.Application.Context);
//            _recyclerView.Touch += _recyclerView_Touch;


//            if (Element.IsHorizontal) {
//				var linearLayoutManager = new LinearLayoutManager (Context, OrientationHelper.Horizontal, false);
//				_layoutManager = linearLayoutManager;

//			} else {
//				var gridlayoutManager = new GridLayoutManager (Context, 1);

//				_layoutManager = gridlayoutManager;

//			}

//            var scrollListener = new GridViewScrollListener(Element, _recyclerView, _layoutManager, swipeLayout);
//            _recyclerView.AddOnScrollListener(scrollListener);

//            _recyclerView.SetLayoutManager (_layoutManager);
//			_recyclerView.SetItemAnimator (null);
//			_recyclerView.HasFixedSize = true;

//			_recyclerView.HorizontalScrollBarEnabled = Element.IsHorizontal;
//			_recyclerView.VerticalScrollBarEnabled = !Element.IsHorizontal;

//			_adapter = new GridViewAdapter (Element.ItemsSource, _recyclerView, Element, Resources.DisplayMetrics);

//			_recyclerView.SetAdapter (_adapter);

//            //_recyclerView.GetAdapter().RegisterAdapterDataObserver(new PositionUpdater(this.Context));

//			UpdatePadding ();
//		}

//        protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
//		{
//			base.OnSizeChanged (w, h, oldw, oldh);
//			UpdatePadding ();
//		}

//		void UpdatePadding ()
//		{
//			_recyclerView.SetPadding ((int)Element.ContentPaddingLeft, 
//				(int)Element.ContentPaddingTop, 
//				(int)Element.ContentPaddingRight, 
//				(int)Element.ContentPaddingBottom);
//			if (Element.IsHorizontal) {
//				if (_paddingDecoration != null) {
//					_recyclerView.RemoveItemDecoration (_paddingDecoration);
//				}
//				var source = Element.ItemsSource as ICollection;
//				var numberOfItems = source == null ? 0 : source.Count;
//				_paddingDecoration = new HorizontalSpacesItemDecoration (ConvertDpToPixels ((float)Element.ColumnSpacing / 2), ConvertDpToPixels ((int)Element.RowSpacing));
//				_recyclerView.AddItemDecoration (_paddingDecoration);

//			} else {
//				UpdateGridLayout ();
//			}

//		}

//		void UpdateGridLayout ()
//		{
//			var source = Element.ItemsSource as ICollection;
//			var numberOfItems = source == null ? 0 : source.Count;

//			var horizontalPadding = 0;
//			int numberOfItemsToUse = 1;
//			if (_layoutManager != null) {

//                var w = UIHelper.ConvertDPToPixels(Element.Width);
//                var p = UIHelper.ConvertDPToPixels(Element.WidthRequest);

//                if (Element.Width > 0) {
//					//					float width = (float)_recyclerView.Width - 2;
//					if (Element.IsContentCentered) {

//						float width = (float)Element.Width;
//						int numberOfItemsThatFit = (int)Math.Floor ((width) / (Element.ItemWidth + Element.ColumnSpacing));
//						numberOfItemsToUse = Element.CenterAsFilledRow ? numberOfItemsThatFit : (int)Math.Min (numberOfItemsThatFit, numberOfItems);
//						if (Element.MaxItemsPerRow != -1) {
//							numberOfItemsToUse = Element.MaxItemsPerRow;
//						}
//						var remainingWidth = (width - (Element.ContentPaddingLeft + Element.ContentPaddingRight)) - ((numberOfItemsToUse * Element.ItemWidth) + ((numberOfItemsToUse) * (Element.ColumnSpacing)));
//                        remainingWidth = Math.Abs(remainingWidth);
//                        horizontalPadding = (int)(remainingWidth / (numberOfItemsToUse + 1));
//					} else {
//						horizontalPadding = (int)Element.ColumnSpacing;
//					}

//					//Console.WriteLine (" width {0} items using {1} padding {2} iwdith {3} ", _recyclerView.Width, numberOfItemsToUse, horizontalPadding, Element.ItemWidth);
//				}
//			}


//			var gridLayoutManager = _layoutManager as GridLayoutManager;
//			if (gridLayoutManager != null) {
//				//TODO calculate
//				gridLayoutManager.SpanCount = Math.Max (1, numberOfItemsToUse);
//			}
//			//TODO
//			if (_paddingDecoration != null) {
//				_recyclerView.RemoveItemDecoration (_paddingDecoration);
//			}
//			_paddingDecoration = new SpacesItemDecoration (ConvertDpToPixels (horizontalPadding), ConvertDpToPixels ((int)Element.RowSpacing), 
//				numberOfItems, numberOfItemsToUse, 
//				ConvertDpToPixels ((int)Element.ContentPaddingTop), ConvertDpToPixels ((int)Element.ContentPaddingBottom));

//			//			_paddingDecoration = new SpacesItemDecoration (horizontalPadding, (int)Element.RowSpacing, 
//			//				numberOfItems, numberOfItemsToUse, 
//			//				(int)Element.ContentPaddingTop, (int)Element.ContentPaddingBottom);
//			_recyclerView.AddItemDecoration (_paddingDecoration);

//		}


//		private int ConvertDpToPixels (float dpValue)
//		{
//			var pixels = (int)((dpValue) * Resources.DisplayMetrics.Density);
//			return pixels;
//		}

//		float _startEventY;
//		float _heightChange;


//		void _recyclerView_Touch (object sender, TouchEventArgs e)
//		{
//			//Console.WriteLine ("ExtendedWebViewRenderer_Touch");
//			var ev = e.Event;
//			MotionEventActions action = ev.Action & MotionEventActions.Mask;
//			switch (action) {
//			case MotionEventActions.Down:
//				_startEventY = ev.GetY ();
//				_heightChange = 0;
//				Element.RaiseOnStartScroll ();
//				//				Console.WriteLine ("START start ", _startEventY);
//				break;
//			case MotionEventActions.Move:
//				float delta = (ev.GetY () + _heightChange) - _startEventY;
//				Element.RaiseOnScroll (delta, _recyclerView.GetVerticalScrollOffset ());

//				//				Console.WriteLine ("scrolling delta is {0}, change {1}, start {2}", delta, _heightChange, _startEventY);
//				//				Console.WriteLine ("SCROLLY  {0},", _recyclerView.GetVerticalScrollOffset ());
//				break;
//			case MotionEventActions.Up:
//				Element.RaiseOnStopScroll ();
//				break;
//			}
//			e.Handled = false;

//		}
//	}

//    //public class PositionUpdater : RecyclerView.AdapterDataObserver
//    //{
//    //    public PositionUpdater(Context context) : base(context)
//    //    {

//    //    }

//    //    public PositionUpdater(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
//    //    {
//    //    }

//    //    //protected AdapterDataObserver(IntPtr javaReference, JniHandleOwnership transfer);

//    //    //protected override IntPtr ThresholdClass { get; }
//    //    //protected override Type ThresholdType { get; }

//    //    public override void OnChanged()
//    //    {
//    //        //Adapter <?> adapter = getAdapter();
//    //        //if (adapter != null && emptyView != null)
//    //        //{
//    //        //    if (adapter.getItemCount() == 0)
//    //        //    {
//    //        //        emptyView.setVisibility(View.VISIBLE);
//    //        //        RecyclerViewEmptySupport.this.setVisibility(View.GONE);
//    //        //    }
//    //        //    else
//    //        //    {
//    //        //        emptyView.setVisibility(View.GONE);
//    //        //        RecyclerViewEmptySupport.this.setVisibility(View.VISIBLE);
//    //        //    }
//    //        }
//    //    }

//    //}

//    public class SpacesItemDecoration : RecyclerView.ItemDecoration
//	{
//		int _columnSpacing;
//		int _rowSpacing;
//		int _numberOfItemsPerRow;
//		int _numberOfItems;
//		int _topSpacing;
//		int _bottomSpacing;

//		public SpacesItemDecoration (int columnSpacing, int rowSpacing, int numberOfItems, int numberOfItemsPerRow, int topSpacing, int bottomSpacing)
//		{
//			_rowSpacing = rowSpacing;
//			_columnSpacing = columnSpacing;
//			_numberOfItems = numberOfItems;
//			_numberOfItemsPerRow = numberOfItemsPerRow;
//			_topSpacing = topSpacing;
//			_bottomSpacing = bottomSpacing;
//		}

//		public override void GetItemOffsets (Android.Graphics.Rect outRect, int itemPosition, RecyclerView parent)
//		{
//			//TODO - work out if the rectangle is the last/first row/column
//			if (itemPosition % _numberOfItemsPerRow == 0) {
//				//first col
//				outRect.Left = _columnSpacing;
//			} else {
//				outRect.Left = _columnSpacing / 2;
//			}
//			if (itemPosition % _numberOfItemsPerRow == (_numberOfItemsPerRow - 1)) {
//				//last col
//				outRect.Right = _columnSpacing;
//			} else {
//				outRect.Right = _columnSpacing / 2;
//			}
//			//TODO write a custom layout for android
//			//			if (itemPosition < _numberOfItemsPerRow) {
//			//				outRect.Top = _topSpacing;
//			//			} 
//			//			if (itemPosition > (_numberOfItems - _numberOfItemsPerRow)) {
//			//				outRect.Bottom = _bottomSpacing;
//			//			} else {
//			outRect.Bottom = _rowSpacing;
//			//			}
//		}
//	}

//	public class HorizontalSpacesItemDecoration : RecyclerView.ItemDecoration
//	{
//		int _columnSpacing;
//		int _rowSpacing;

//		public HorizontalSpacesItemDecoration (int columnSpacing, int rowSpacing)
//		{
//			_rowSpacing = rowSpacing;
//			_columnSpacing = columnSpacing;
//			_columnSpacing = columnSpacing;
//		}

//		public override void GetItemOffsets (Android.Graphics.Rect outRect, int itemPosition, RecyclerView parent)
//		{
//			outRect.Left = _columnSpacing / 2;
//			outRect.Right = _columnSpacing / 2;
//			outRect.Bottom = _rowSpacing;
//		}
//	}

//	public class ScrollRecyclerView : RecyclerView
//	{
//		public ScrollRecyclerView (IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base (javaReference, transfer)
//		{
//		}


//		public ScrollRecyclerView (Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
//		{
//		}


//		public ScrollRecyclerView (Android.Content.Context context, Android.Util.IAttributeSet attrs) : base (context, attrs)
//		{
//		}


//		public ScrollRecyclerView (Android.Content.Context context) : base (context)
//		{
//		}

//		public int GetVerticalScrollOffset ()
//		{
//			return ComputeVerticalScrollOffset ();
//		}

//		public int GetHorizontalScrollOffset ()
//		{
//			return ComputeHorizontalScrollOffset ();
//		}
//	}

//	public class GridViewScrollListener : RecyclerView.OnScrollListener
//	{
//		LabsGridView _gridView;

//		ScrollRecyclerView _recyclerView;
//        GridLayoutManager _gridManager;
//        LinearLayoutManager _linearManager;
//        ScrollChildSwipeRefreshLayout control;

//        public GridViewScrollListener (GridView gridView, ScrollRecyclerView recyclerView, 
//            RecyclerView.LayoutManager layoutManager, ScrollChildSwipeRefreshLayout control)
//		{
//            this.control = control;
//			_gridView = gridView;
//			_recyclerView = recyclerView;

//            if (layoutManager is GridLayoutManager)
//            {
//                _gridManager = layoutManager as GridLayoutManager;
//            }
//            else
//            {
//                _linearManager = layoutManager as LinearLayoutManager;
//            }
//		}

//        private bool isIdle = false;
//		public override void OnScrolled (RecyclerView recyclerView, int dx, int dy)
//		{
//			base.OnScrolled (recyclerView, dx, dy);

//            if (dy > 0)
//            {
//                //if (isIdle)
//                {
//                    var manager = (_gridManager == null) ? _linearManager : _gridManager;

//                    int lastVisibleItemPosition = manager.FindLastVisibleItemPosition();
//                    int visibleItemCount = recyclerView.ChildCount;
//                    int totalItemCount = manager.ItemCount;

//                    if (lastVisibleItemPosition == totalItemCount - 1)
//                    {
//                        _gridView.InvokeLoadMoreEvent();
//                    }
//                }
//            }
//            _gridView.RaiseOnScroll (dy, _recyclerView.GetVerticalScrollOffset ());
//		}

//        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
//        {
//            base.OnScrollStateChanged(recyclerView, newState);
//            isIdle = newState == RecyclerView.ScrollStateIdle;

//            control.ScrollState = newState;
//        }

//        //private static bool IsLoading = true;

//        //public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
//        //{

//        //    if (firstVisibleItem + visibleItemCount == totalItemCount && totalItemCount != 0)
//        //    {
//        //        if (!IsLoading)
//        //            Element.InvokeLoadMoreEvent(this);
//        //    }
//        //}

//        //public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
//        //{
//        //    IsLoading = scrollState == ScrollState.TouchScroll;
//        //}
//    }


//    public class ScrollChildSwipeRefreshLayout : SwipeRefreshLayout//, Android.Views.View.IOnTouchListener
//    {
//        GridLayoutManager _gridManager;
//        LinearLayoutManager _linearManager;

//        public ScrollChildSwipeRefreshLayout(Context context)
//            : base(context)
//        {


//            //this.SetOnTouchListener(this);
//        }

//        public void SetLayoutManager(RecyclerView.LayoutManager layoutManager)
//        {
//            if (layoutManager is GridLayoutManager)
//            {
//                _gridManager = layoutManager as GridLayoutManager;
//            }
//            else
//            {
//                _linearManager = layoutManager as LinearLayoutManager;
//            }
//        }

//        public ScrollChildSwipeRefreshLayout(Context context, IAttributeSet attrs)
//            : base(context, attrs)
//        {

//        }

//        private int previousStatus = -1;

//        // The current SwipeRefreshLayout only check its immediate child scrollability.
//        // In our case, ListFragment uses a ListView inside a parent FrameLayout which breaks this.
//        public override bool CanChildScrollUp()
//        {
//            var manager = (_gridManager != null) ? _gridManager : _linearManager;

//            var firstVisibleItem = manager.FindFirstVisibleItemPosition();
//            if (manager.ChildCount == 0)
//                return false;

//            var p = manager.FindFirstCompletelyVisibleItemPosition();

//            var scrollState = this.ScrollState;

//            if (p == 0)
//            {
//                if (previousStatus == 0 && scrollState == 1)
//                {
//                    previousStatus = -1;
//                    return false;
//                }
//            }

//            if (previousStatus == -1)
//            {
//                if (p != 0)
//                    previousStatus = scrollState;
//            }
//            else
//            {
//                previousStatus = scrollState;
//            }

//            return true;
//        }

//        public int ScrollState { get; set; }
//        //public bool OnTouch(Android.Views.View v, MotionEvent e)
//        //{
//        //    MotionEventActions action = e.Action;
//        //    switch (action)
//        //    {
//        //        case MotionEventActions.Down:
//        //            // Disallow ScrollView to intercept touch events.
//        //            v.Parent.RequestDisallowInterceptTouchEvent(true);
//        //            break;

//        //        case MotionEventActions.Up:
//        //            // Allow ScrollView to intercept touch events.
//        //            v.Parent.RequestDisallowInterceptTouchEvent(false);
//        //            break;
//        //    }

//        //    // Handle ListView touch events.
//        //    v.OnTouchEvent(e);
//        //    return true;
//        //}
//    }
//}

