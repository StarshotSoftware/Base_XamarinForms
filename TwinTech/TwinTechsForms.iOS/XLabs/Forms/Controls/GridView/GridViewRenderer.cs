using Xamarin.Forms;

using XLabs.Forms.Controls;
using System.Collections.Generic;
using CoreGraphics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TwinTechs.Controls;
using System.Diagnostics;
using System.Reflection;
using Base1902.Controls;

[assembly: ExportRenderer(typeof(XLabs.Forms.Controls.GridView), typeof(GridViewRenderer))]
namespace XLabs.Forms.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Linq;
    using Base1902;
    using Foundation;
    using TwinTechsLib.Ios;
    using UIKit;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.iOS;
    using Base1902.Services;
    /// <summary>
    /// Class GridViewRenderer.
    /// </summary>
    public class GridViewRenderer : ViewRenderer<GridView, GridCollectionView>, IGridViewProvider
    {
        private UILabel emptyLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridViewRenderer"/> class.
        /// </summary>l
        public GridViewRenderer()
        {
        }

        private FormsUIRefreshControl refreshControl;
        GridCollectionView _gridCollectionView;
        UIEdgeInsets _edgeInsets;
        int? _initialIndex;

        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<GridView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null)
            {
                return;
            }

            if (refreshControl != null)
                return;

            e.NewElement.GridViewProvider = this;

            _gridCollectionView = new GridCollectionView();
            _gridCollectionView.AllowsMultipleSelection = false;
            _gridCollectionView.SelectionEnable = e.NewElement.SelectionEnabled;



            _gridCollectionView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            _gridCollectionView.ItemSize = new CoreGraphics.CGSize((float)Element.ItemWidth, (float)Element.ItemHeight);
            _gridCollectionView.RowSpacing = Element.RowSpacing;
            _gridCollectionView.ColumnSpacing = Element.ColumnSpacing;

            if (e.NewElement.IsHorizontal)
            {
                UICollectionViewFlowLayout flowLayout = (UICollectionViewFlowLayout)_gridCollectionView.CollectionViewLayout;
                flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            }
            Unbind(e.OldElement);
            Bind(e.NewElement);

            _gridCollectionView.Source = (e.NewElement.ItemsSource != null) ? DataSource : null;
            _gridCollectionView.Delegate = new GridViewDelegate(ItemSelected, HandleOnScrolled);
            ScrollToInitialIndex();
            //			UpdatePadding ();
            InvalidatePadding();

            SetNativeControl(_gridCollectionView);
            Control.AlwaysBounceVertical = true;

            if (Element.PullToRefreshEnabled)
            {
                refreshControl = new FormsUIRefreshControl(_gridCollectionView);
                refreshControl.TintColor = Xamarin.Forms.Color.Gray.ToUIColor();
                refreshControl.RefreshCommand = Element.RefreshCommand;
                //refreshControl.Message = pullToRefreshListView.Message;
                Control.AddSubview(refreshControl);
                refreshControl.IsRefreshing = Element.Refreshing;
            }
        }

        void Refresh(object sender, EventArgs e)
        {
            if (Element.RefreshCommand == null)
                return;

            if (Element.RefreshCommand as RelayCommand != null)
            {
                var command = Element.RefreshCommand as RelayCommand;
                if (command.CanRun && command.CanExecute(null))
                    command.Execute(null);
            }
            else
            {
                if (Element.RefreshCommand.CanExecute(null))
                    Element.RefreshCommand.Execute(null);
            }
        }

        bool _isPaddingInvalid;

        bool IsPaddingInvalid
        {
            get { return _isPaddingInvalid; }
            set { _isPaddingInvalid = value; }
        }

        void InvalidatePadding()
        {
            IsPaddingInvalid = true;
            SetNeedsLayout();
        }

        nfloat _previousWidth;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            SetEmptyLabel();

            _gridCollectionView.Frame = this.Bounds;
            bool widthChanged = _previousWidth != _gridCollectionView.Frame.Width;
            if (widthChanged)
            {
                _previousWidth = _gridCollectionView.Frame.Width;
            }
            if (IsPaddingInvalid || widthChanged)
            {
                UpdatePadding();
            }

        }

        void SetEmptyLabel()
        {
            if (Superview != null && Superview.Frame.Width > 0)
            {
                if (emptyLabel == null)
                {
                    emptyLabel = new UILabel();
                    emptyLabel.Center = Center;
                    emptyLabel.Bounds = Bounds;
                    emptyLabel.TextAlignment = UITextAlignment.Center;
                    emptyLabel.LineBreakMode = UILineBreakMode.WordWrap;
                    emptyLabel.Lines = 10;

                    var fontService = Resolver.Get<IFontService>();
                    if (fontService != null)
                    {
                        var font = UIFont.FromName(fontService.GetFontName(FontStyle.Italic), 20);
                        if (font != null)
                            emptyLabel.Font = font;
                    }
                    emptyLabel.Hidden = true;
                    Control.BackgroundView = emptyLabel;
                }

                emptyLabel.Text = Element.EmptyText;
            }
        }


        /// <summary>
        /// Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "ItemsSource")
            {
                var gridView = sender as GridView;
                if (gridView.ItemsSource != null)
                {
                    //					_gridCollectionView.SetContentOffset (new CGPoint (-_edgeInsets.Left, -_edgeInsets.Top), false);
                    _gridCollectionView.Source = DataSource;
                    ReloadData();
                    ScrollToInitialIndex();
                }
            }

            if (e.PropertyName == "RefreshCommand")
            {
                if (refreshControl != null)
                    refreshControl.RefreshCommand = Element.RefreshCommand;
            }
            if (e.PropertyName == "Refreshing")
            {
                if (refreshControl != null)
                {
                    refreshControl.IsRefreshing = Element.Refreshing;
                    if (!Element.Refreshing && emptyLabel != null)
                    {
                        var prop = Element.ItemsSource.GetType().GetRuntimeProperty("Count");

                        if (prop != null && prop.GetValue(Element.ItemsSource) != null)
                        {
                            var count = int.Parse(prop.GetValue(Element.ItemsSource).ToString());

                            if (count == 0)
                            {
                                emptyLabel.Hidden = false;
                            }
                            else
                            {
                                emptyLabel.Hidden = true;
                            }

                            //Control.RequestLayout();
                            //Control.Invalidate();
                        }

                        UpdatePadding();
                    }


                }
                //if (Element.Refreshing)
                //    refreshControl.BeginRefreshing ();
                //else
                //    refreshControl.EndRefreshing ();
            }

            if (e.PropertyName == "IsScrollEnabled")
            {
                Device.BeginInvokeOnMainThread(() => {
                    _gridCollectionView.ScrollEnabled = Element.IsScrollEnabled;
                    //Debug.WriteLine ("scroll enabled changed to " + _gridCollectionView.ScrollEnabled);
                }
                );

            }

            if (e.PropertyName == "ItemSize")
            {
                var gridView = sender as GridView;
                _gridCollectionView.ItemSize = gridView.ItemSize.ToSizeF();
                //				foreach (GridViewCell nativeCell in _gridCollectionView.VisibleCells) {
                //					nativeCell.SetNeedsLayout ();
                //				}

                InvalidatePadding();
            }
            if (e.PropertyName == "IsHorizontal")
            {
                UICollectionViewFlowLayout flowLayout = (UICollectionViewFlowLayout)_gridCollectionView.CollectionViewLayout;
                flowLayout.ScrollDirection = Element.IsHorizontal ? UICollectionViewScrollDirection.Horizontal : UICollectionViewScrollDirection.Vertical;
            }
            if (e.PropertyName == "IsContentCentered")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "ContentPaddingLeft")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "ContentPaddingTop")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "ContentPaddingBottom")
            {
                InvalidatePadding();
            }

            if (e.PropertyName == "CenterAsFilledRow")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "ContentPaddingRight")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "SectionPaddingBottom")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "MaxItemsPerRow")
            {
                InvalidatePadding();
            }
            if (e.PropertyName == "SectionPaddingTop")
            {
                InvalidatePadding();
            }

        }

        /// <summary>
        /// Unbinds the specified old element.
        /// </summary>
        /// <param name="oldElement">The old element.</param>
        private void Unbind(GridView oldElement)
        {
            if (oldElement != null)
            {
                oldElement.PropertyChanging += ElementPropertyChanging;
                oldElement.PropertyChanged -= ElementPropertyChanged;

                var itemsSource = oldElement.ItemsSource as INotifyCollectionChanged;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Binds the specified new element.
        /// </summary>
        /// <param name="newElement">The new element.</param>
        private void Bind(GridView newElement)
        {
            if (newElement != null)
            {
                newElement.PropertyChanging += ElementPropertyChanging;
                newElement.PropertyChanged += ElementPropertyChanged;
                if (newElement.ItemsSource is INotifyCollectionChanged)
                {
                    (newElement.ItemsSource as INotifyCollectionChanged).CollectionChanged += DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Elements the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var itemsSource = Element != null ? Element.ItemsSource as INotifyCollectionChanged : null;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged -= DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Elements the property changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangingEventArgs"/> instance containing the event data.</param>
        private void ElementPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
            {
                var itemsSource = Element != null ? Element.ItemsSource as INotifyCollectionChanged : null;
                if (itemsSource != null)
                {
                    itemsSource.CollectionChanged += DataCollectionChanged;
                }
            }
        }

        /// <summary>
        /// Datas the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (Control != null)
                {
                    ReloadData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error " + ex.Message);
            }
        }

        /// <summary>
        /// The _data source
        /// </summary>
        private GridDataSource _dataSource;

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        private GridDataSource DataSource
        {
            get
            {
                return _dataSource ??
                (_dataSource =
                        new GridDataSource(GetCell, RowsInSection, ItemSelected));
            }
        }

        void HandleOnScrolled(UIScrollView scrollView)
        {

            foreach (GridViewCell nativeCell in _gridCollectionView.VisibleCells)
            {
                nativeCell.ViewCell.OnScroll(scrollView.ContentOffset.ToPoint(), new Xamarin.Forms.Point(nativeCell.Frame.X, nativeCell.Frame.Y));
            }
            Element.RaiseOnScroll(0, (float)scrollView.ContentOffset.Y);

            var bottomEdge = scrollView.ContentOffset.Y + scrollView.Frame.Size.Height;

			if(scrollView.ContentSize.Height > 0)
            if (Math.Ceiling(bottomEdge) >= Math.Ceiling(scrollView.ContentSize.Height))
            {
                Element.InvokeLoadMoreEvent();
            }

            //Photo model = null; 
            //              var incremental = gridView.ItemsSource as IncrementalCollection<Photo>;
            //              if (incremental.Any())
            //              {
            //                  model = (Photo)incremental[indexPath.Row];
            //                  if (incremental.Count >= AlbumViewModel.PageSize)
            //                  {
            //                      if (model == incremental[incremental.Count - (AlbumViewModel.PageSize / 2)])
            //                      {
            //                          incremental.LoadMoreItemsAsync(0);
            //                      }
            //                  }
            //              }
        }

        void ScrollToInitialIndex()
        {
            if (_initialIndex.HasValue && _gridCollectionView != null && _gridCollectionView.DataSource != null)
            {
                ScrollToItemWithIndex(_initialIndex.Value, false);
                _initialIndex = null;
            }
        }

        /// <summary>
        /// Rowses the in section.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        /// <param name="section">The section.</param>
        /// <returns>System.Int32.</returns>
        public int RowsInSection(UICollectionView collectionView, nint section)
        {
            //			var property = Element.ItemsSource.GetType ().GetProperty ("InstanceId");
            //			string instanceId = property?.GetValue (Element.ItemsSource)?.ToString ();
            //			Console.WriteLine (">>>>> countfrom  collection {0} is {1}", instanceId, ((ICollection)Element.ItemsSource).Count);
            var numberOfItems = ((ICollection)Element.ItemsSource).Count;
            return numberOfItems;
        }

        /// <summary>
        /// Items the selected.
        /// </summary>
        /// <param name="tableView">The table view.</param>
        /// <param name="indexPath">The index path.</param>
        public void ItemSelected(UICollectionView tableView, NSIndexPath indexPath)
        {
            var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            Element.InvokeItemSelectedEvent(this, item);
        }

        NSString cellId;

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="collectionView">Collection view.</param>
        /// <param name="indexPath">Index path.</param>
        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            cellId = cellId ?? new NSString(GridViewCell.Key);
            var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
            var collectionCell = collectionView.DequeueReusableCell(cellId, indexPath) as GridViewCell;

            collectionCell.RecycleCell(item, Element.ItemTemplate, Element);
            if (collectionCell.GestureRecognizers != null)
            {
                foreach(var gesture in collectionCell.GestureRecognizers)
                {
                    collectionCell.RemoveGestureRecognizer(gesture);
                }
            }

            var tapRecognizer = new UITapGestureRecognizer(() =>
            {
                ItemSelected(collectionView, indexPath);
            });
            tapRecognizer.NumberOfTapsRequired = 1;
            tapRecognizer.Delegate = new GestureDelegate();

            collectionCell.AddGestureRecognizer(tapRecognizer);

            return collectionCell;
        }

        public class GestureDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
            {
                return true;
            }

            public override bool ShouldBegin(UIGestureRecognizer recognizer)
            {
                return true;
            }

            public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
            {
                return true;
            }
        }

        /// <summary>
        /// Reloads the data.
        /// </summary>
        public void ReloadData()
        {
            if (_gridCollectionView != null)
            {
                InvokeOnMainThread(() => {
                    UpdatePadding();
                    _gridCollectionView.ReloadData();
                    _gridCollectionView.Delegate = new GridViewDelegate(ItemSelected, HandleOnScrolled);
                }
                );
            }
        }


        //TODO this method/mechanism needs some more thought
        /// <summary>
        /// Scrolls the index of the to item with.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public void ScrollToItemWithIndex(int index, bool animated)
        {
            if (_gridCollectionView != null && _gridCollectionView.NumberOfItemsInSection(0) > index)
            {
                var indexPath = NSIndexPath.FromRowSection(index, 0);
                InvokeOnMainThread(() => {
                    _gridCollectionView.ScrollToItem(indexPath, UICollectionViewScrollPosition.Top, animated);
                });
            }
            else
            {
                _initialIndex = index;
            }
        }

        /// <summary>
        /// Updates the padding for when we center the content in the gridview
        /// </summary>
        void UpdatePadding()
        {
            if (Element == null || (ICollection)Element.ItemsSource == null)
            {
                return;
            }
            var numberOfItems = ((ICollection)Element.ItemsSource).Count;
            UICollectionViewFlowLayout flowLayout = _gridCollectionView != null ? (UICollectionViewFlowLayout)_gridCollectionView.CollectionViewLayout : null;
            if (flowLayout != null)
            {
                if (Element.IsContentCentered && numberOfItems > 0 && _gridCollectionView.Frame.Width > 0)
                {
                    flowLayout.InvalidateLayout();
                    float width = (float)_gridCollectionView.Frame.Width - 2;
                    int numberOfItemsThatFit = (int)Math.Floor(width / (_gridCollectionView.ItemSize.Width));
                    int numberOfItemsToUse = Element.CenterAsFilledRow ? numberOfItemsThatFit : (int)Math.Min(numberOfItemsThatFit, numberOfItems);
                    if (Element.MaxItemsPerRow != -1)
                    {
                        numberOfItemsToUse = Element.MaxItemsPerRow;
                    }
                    var remainingWidth = width - (numberOfItemsToUse * (_gridCollectionView.ItemSize.Width));
                    var padding = remainingWidth / (numberOfItemsToUse + 1);

                    Console.WriteLine(" width {0} items using {1} padding {2} iwdith {3} ", _gridCollectionView.Frame.Width, numberOfItemsToUse, padding, _gridCollectionView.ItemSize.Width);
                    _gridCollectionView.ColumnSpacing = padding;
                    _edgeInsets = new UIEdgeInsets((float)Element.ContentPaddingTop, (float)padding, (float)padding + 4, (float)padding);
                    Console.WriteLine("final insets " + _edgeInsets);
                    _gridCollectionView.ContentInset = _edgeInsets;
                }
                flowLayout.SectionInset = new UIEdgeInsets((float)Element.SectionPaddingTop, 0, (float)Element.SectionPaddingBottom, 0);
                if (_gridCollectionView.Frame.Width > 0 && _gridCollectionView.Frame.Height > 0)
                {
                    IsPaddingInvalid = false;
                }
            }
        }
    }
}