using BaseStarShot.Collections;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using BaseStarShot;

namespace BaseStarShot.Controls
{
    public class ListView : Xamarin.Forms.ListView
    {
        public static BindableProperty SeparatorBackgroundProperty =
            BindableProperty.Create<ListView, FileImageSource>(p => p.SeparatorBackground, null);

        //public static BindableProperty SeparatorColorProperty =
        //    BindableProperty.Create<ListView, Color>(p => p.SeparatorColor, Color.Transparent);

        public static BindableProperty SeparatorHeightProperty =
            BindableProperty.Create<ListView, double>(p => p.SeparatorHeight, new PointSize(1));

        public static BindableProperty PaddingProperty =
            BindableProperty.Create<ListView, Thickness>(p => p.Padding, 0);

        public static BindableProperty TransparentSeparatorProperty =
            BindableProperty.Create<ListView, FileImageSource>(p => p.TransparentSeparator, null);

        public static BindableProperty DataTemplateTypeProperty =
            BindableProperty.Create<ListView, Type>(l => l.DataTemplateType, null);

        public static BindableProperty EmptyTextProperty =
            BindableProperty.Create<ListView, string>(l => l.EmptyText, "");


		public static BindableProperty ScrollToTopChangeProperty =
			BindableProperty.Create<ListView, bool>(l => l.ScrollToTopChange, false,BindingMode.TwoWay);

        public static BindableProperty EmptyTextFontStyleProperty =
            BindableProperty.Create<ListView, FontStyle>(l => l.EmptyTextFontStyle, FontStyle.Italic);

        public static BindableProperty EmptyTextFontSizeProperty =
            BindableProperty.Create<ListView, double>(l => l.EmptyTextFontSize, new PointSize(17));

        public static BindableProperty EmptyTextColorProperty =
            BindableProperty.Create<ListView, Color>(l => l.EmptyTextColor, Color.Default);

        public static BindableProperty EmptyTextBackgroundProperty =
            BindableProperty.Create<ListView, FileImageSource>(l => l.EmptyTextBackground, null);

        public static BindableProperty SelectorProperty =
            BindableProperty.Create<ListView, ListViewSelector>(l => l.Selector, ListViewSelector.Default);

        public static BindableProperty ClickableProperty =
            BindableProperty.Create<ListView, bool>(l => l.Clickable, true);

        public static BindableProperty DismissKeyboardOnDragProperty =
            BindableProperty.Create<ListView, bool>(l => l.DismissKeyboardOnDrag, true);

        public static BindableProperty UseCustomRendererProperty =
            BindableProperty.Create<ListView, bool>(l => l.UseCustomRenderer, true);

        public static BindableProperty SelectOnLongClickProperty =
            BindableProperty.Create<ListView, bool>(l => l.SelectItemOnLongClick, false);

        public static BindableProperty SwipeEnabledProperty =
            BindableProperty.Create<ListView, bool>(l => l.SwipeEnabled, false);

        public static BindableProperty ScrollToIndexProperty =
            BindableProperty.Create<ListView, int>(p => p.ScrollToIndex, -1);

        public static BindableProperty LastVisiblePositionProperty =
            BindableProperty.Create<ListView, int>(p => p.LastVisiblePosition, -1);

        public static BindableProperty LoadMoreOffsetProperty =
            BindableProperty.Create<ListView, int>(l => l.LoadMoreOffset, 2);

        public static BindableProperty ItemContainerStyleProperty =
            BindableProperty.Create<ListView, string>(l => l.ItemContainerStyle, null);

        /// <summary>
        /// Add reverse collection property if enabled. E.G. Messaging ListView
        /// </summary>
        public static BindableProperty ReverseCollectionEnabledProperty =
            BindableProperty.Create<ListView, bool>(l => l.ReverseCollectionEnabled, false);
        public bool ReverseCollectionEnabled
        {
            get { return (bool)GetValue(ReverseCollectionEnabledProperty); }
            set { SetValue(ReverseCollectionEnabledProperty, value); }
        }

        public Type DataTemplateType
        {
            get { return (Type)GetValue(DataTemplateTypeProperty); }
            set { SetValue(DataTemplateTypeProperty, value); }
        }

        public string EmptyText
        {
            get { return (string)GetValue(EmptyTextProperty); }
            set { SetValue(EmptyTextProperty, value); }
        }


		public bool ScrollToTopChange
		{
			get { return (bool)GetValue(ScrollToTopChangeProperty); }
			set { SetValue(ScrollToTopChangeProperty, value); }
		}


        public FontStyle EmptyTextFontStyle
        {
            get { return (FontStyle)GetValue(EmptyTextFontStyleProperty); }
            set { SetValue(EmptyTextFontStyleProperty, value); }
        }

        public double EmptyTextFontSize
        {
            get { return (double)GetValue(EmptyTextFontSizeProperty); }
            set { SetValue(EmptyTextFontSizeProperty, value); }
        }

        public Color EmptyTextColor
        {
            get { return (Color)GetValue(EmptyTextColorProperty); }
            set { SetValue(EmptyTextColorProperty, value); }
        }

        public FileImageSource EmptyTextBackground
        {
            get { return (FileImageSource)GetValue(EmptyTextBackgroundProperty); }
            set { SetValue(EmptyTextBackgroundProperty, value); }
        }

        public ListViewSelector Selector
        {
            get { return (ListViewSelector)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        public bool Clickable
        {
            get { return (bool)GetValue(ClickableProperty); }
            set { SetValue(ClickableProperty, value); }
        }

        /// <summary>
        /// ios only
        /// </summary>
        public bool DismissKeyboardOnDrag
        {
            get { return (bool)GetValue(DismissKeyboardOnDragProperty); }
            set { SetValue(DismissKeyboardOnDragProperty, value); }
        }

        /// <summary>
        /// used for ios twintech only(note: dataSource override)
        /// </summary>
        public bool UseCustomRenderer
        {
            get { return (bool)GetValue(UseCustomRendererProperty); }
            set { SetValue(UseCustomRendererProperty, value); }
        }

        public bool SelectItemOnLongClick
        {
            get { return (bool)GetValue(SelectOnLongClickProperty); }
            set { SetValue(SelectOnLongClickProperty, value); }
        }

        public bool SwipeEnabled
        {
            get { return (bool)GetValue(SwipeEnabledProperty); }
            set { SetValue(SwipeEnabledProperty, value); }
        }

        /// <summary>
        /// android use only
        /// </summary>
        public int ScrollToIndex
        {
            get { return (int)GetValue(ScrollToIndexProperty); }
            set { SetValue(ScrollToIndexProperty, value); }
        }

        public int LoadMoreOffset
        {
            get { return (int)GetValue(LoadMoreOffsetProperty); }
            set { SetValue(LoadMoreOffsetProperty, value); }
        }

        public int LastVisiblePosition
        {
            get { return (int)GetValue(LastVisiblePositionProperty); }
            set { SetValue(LastVisiblePositionProperty, value); }
        }
        
        public FileImageSource SeparatorBackground
        {
            get { return (FileImageSource)GetValue(SeparatorBackgroundProperty); }
            set { SetValue(SeparatorBackgroundProperty, value); }
        }

        //public Color SeparatorColor
        //{
        //    get { return (Color)GetValue(SeparatorColorProperty); }
        //    set { SetValue(SeparatorColorProperty, value); }
        //}

        /// <summary>
        /// for android only
        /// </summary>
        public double SeparatorHeight
        {
            get { return (double)GetValue(SeparatorHeightProperty); }
            set { SetValue(SeparatorHeightProperty, value); }
        }

        /// <summary>
        /// for android only
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public FileImageSource TransparentSeparator
        {
            get { return (FileImageSource)GetValue(TransparentSeparatorProperty); }
            set { SetValue(TransparentSeparatorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ListView.ItemContainerStyle from a resource with a matching key for winRT.
        /// </summary>
        public string ItemContainerStyle
        {
            get { return (string)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public event EventHandler LongClick;
		public event EventHandler<int> ScrollToBottomEvent=delegate{};
		public event EventHandler ScrollToTopEvent = delegate{};

        public void OnLongClick()
        {
            EventHandler handler = LongClick;
            if (handler != null)
                LongClick(this, null);
        }

        public void OnScrolledTop()
        {
            if (ReverseCollectionEnabled)
            {
                if (ItemsSource is ISupportReverseIncrementalLoading)
                {
                    var loaderSource = (ISupportReverseIncrementalLoading)ItemsSource;
                    if (!loaderSource.IsTopLoading && loaderSource.Count >= LoadMoreOffset)
                    {
                        loaderSource.LoadMoreTopItemsAsync(0);
                    }
                }
            }
        }

        public async void ScrollToTop()
        {
            var loaderSource = ItemsSource;
            if (ItemsSource == null)
                return;

            var prop = ItemsSource.GetType().GetRuntimeProperty("Count");

            if (prop != null && prop.GetValue(ItemsSource) != null)
            {
                var count = int.Parse(prop.GetValue(ItemsSource).ToString());

                if (count > 0)
                {
//                    if (Device.OS == TargetPlatform.iOS)
//                        await Task.Delay(1000);

                    //ScrollToTopEvent(null, null);


					ScrollToTopChange = true;
                    //var source = (IList)ItemsSource;
                    //await Task.Delay(500);
                    //ScrollTo(source[0], ScrollToPosition.Start, false);
                }
            }

        }

        public async void ScrollToBottom()
        {
            var loaderSource = ItemsSource;
            if (ItemsSource == null)
                return;

            var prop = ItemsSource.GetType().GetRuntimeProperty("Count");

            if (prop != null && prop.GetValue(ItemsSource) != null)
            {
                var count = int.Parse(prop.GetValue(ItemsSource).ToString());

                if (count > 0)
                {
                    var index = 0;
                    if (!this.IsGroupingEnabled)
                    {
                        index = count - 1;
                    }
                    else
                    {
                        var source = (IList)ItemsSource;
                        var cnt0 = count - 1;
                        var item = source[cnt0] as IList;
                        index = item.Count - 1;
                    }

//                    if (Device.OS == TargetPlatform.iOS)
//                        await Task.Delay(1000);

                    if (index > 0)
                        ScrollToBottomEvent(this, index);

                }
            }

        }

        public ListView()
        //: base(ListViewCachingStrategy.RecycleElement)
        {
            //if (Xamarin.Forms.Device.OS != TargetPlatform.Android)
            this.ItemAppearing += ListView_ItemAppearing;
        }

        void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            HandleItemAppearing(e);
        }

        protected virtual void HandleItemAppearing(ItemVisibilityEventArgs e)
        {
            if (IsGroupingEnabled)
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    var loaderSource = (ISupportIncrementalLoading)ItemsSource;

                    var enumSource = (System.Collections.IList)ItemsSource;
                    if (!loaderSource.IsLoading)
                    {
                        if (loaderSource.Count > 0 && LoadMoreOffset >= 0)
                        {
                            try
                            {
                                var lastIndex = loaderSource.Count - 1;
                                if (lastIndex < 0)
                                    return;

                                var listLastGroup = (System.Collections.IList)enumSource[lastIndex];

								var index = listLastGroup.Count - LoadMoreOffset;
								if (index < 0){
									return;
								}

                                if (e.Item == listLastGroup[index])
                                    loaderSource.LoadMoreItemsAsync(0);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    var loaderSource = (ISupportIncrementalLoading)ItemsSource;

                    if (!loaderSource.IsLoading && loaderSource.Count >= LoadMoreOffset && LoadMoreOffset >= 0 && (loaderSource.Count - LoadMoreOffset - 1 > 0))
                    {
                        if (e.Item == loaderSource[loaderSource.Count - LoadMoreOffset - 1])
                        {
                            loaderSource.LoadMoreItemsAsync(0);
                        }
                    }
                }
            }
        }

        public void LoadMore()
        {
            if (IsGroupingEnabled)
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    var loaderSource = (ISupportIncrementalLoading)ItemsSource;
                    var enumSource = (System.Collections.IList)ItemsSource;
                    if (!loaderSource.IsLoading)
                    {
                        if (loaderSource.Count > 0 && LoadMoreOffset >= 0)
                        {
                            loaderSource.LoadMoreItemsAsync(0);
                        }
                    }
                }
            }
            else
            {
                if (ItemsSource is ISupportIncrementalLoading)
                {
                    var loaderSource = (ISupportIncrementalLoading)ItemsSource;

                    if (!loaderSource.IsLoading && loaderSource.Count > 0 && LoadMoreOffset >= 0)
                    {
                        loaderSource.LoadMoreItemsAsync(0);
                    }
                }
            }
        }

    }
}
