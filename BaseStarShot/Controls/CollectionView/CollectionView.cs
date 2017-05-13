using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BaseStarShot.Collections;

namespace BaseStarShot.Controls
{
    public class CollectionView : BaseStarShot.Controls.ListView
    {
        public static new readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create<CollectionView, IEnumerable>(p => p.ItemsSource, null);

        public static new readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create<CollectionView, DataTemplate>(p => p.ItemTemplate, null);

        public static readonly BindableProperty ItemsInRowProperty =
            BindableProperty.Create<CollectionView, int>(p => p.ItemsInRow, 1);

        public static readonly BindableProperty RowPaddingProperty =
            BindableProperty.Create<CollectionView, Thickness>(p => p.RowPadding, 0);

        public static readonly BindableProperty ColumnSpacingProperty =
            BindableProperty.Create<CollectionView, double>(p => p.ColumnSpacing, 0);

        public new static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create<CollectionView, object>(p => p.SelectedItem, 0, BindingMode.TwoWay);

        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public new DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public int ItemsInRow
        {
            get { return (int)GetValue(ItemsInRowProperty); }
            set { SetValue(ItemsInRowProperty, value); }
        }

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public Thickness RowPadding
        {
            get { return (Thickness)GetValue(RowPaddingProperty); }
            set { SetValue(RowPaddingProperty, value); }
        }

        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                var oldItem = SelectedItem;
                SetValue(SelectedItemProperty, value);
                if (value != null && oldItem != value)
                    ItemSelected(this, new SelectedItemChangedEventArgs(value));
            }
        }

        ObservableCollection<List<object>> items;

        public new event EventHandler<ItemVisibilityEventArgs> ItemAppearing = delegate { };
        public new event EventHandler<ItemVisibilityEventArgs> ItemDisappearing = delegate { };
        public new event EventHandler<SelectedItemChangedEventArgs> ItemSelected = delegate { };
        public new event EventHandler<ItemTappedEventArgs> ItemTapped = delegate { };

        public CollectionView()
        {
            items = new ObservableCollection<List<object>>();
            base.SwipeEnabled = true;
            base.ItemsSource = items;
            base.SeparatorVisibility = SeparatorVisibility.None;
            base.ItemAppearing += CollectionView_ItemAppearing;
            base.ItemDisappearing += CollectionView_ItemDisappearing;
            base.ItemTapped += CollectionView_ItemTapped;
            Selector = ListViewSelector.None;
        }

        void CollectionView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            base.SelectedItem = null;
        }

        void CollectionView_ItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            var items = e.Item as List<object>;
            foreach (var item in items)
            {
                ItemDisappearing(this, new ItemVisibilityEventArgs(item));
            }
        }

        void CollectionView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            base.SeparatorVisibility = Xamarin.Forms.SeparatorVisibility.None;

            var items = e.Item as List<object>;
            foreach (var item in items)
            {
                ItemAppearing(this, new ItemVisibilityEventArgs(item));
            }
        }

        protected override void OnPropertyChanging(string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                items.Clear();
                if (ItemsSource is INotifyCollectionChanged)
                {
                    var observable = ItemsSource as INotifyCollectionChanged;
                    observable.CollectionChanged -= observable_CollectionChanged;
                }
            }
        }

        bool isBaseTemplateChanging;
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (!isBaseTemplateChanging && propertyName == ItemTemplateProperty.PropertyName)
            {
                SetListViewTemplate();
            }

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                SetItemsSource();
                SetItems();
            }
        }

        void SetItemsSource()
        {
            if (ItemsSource is INotifyCollectionChanged)
            {
                var observable = ItemsSource as INotifyCollectionChanged;
                observable.CollectionChanged += observable_CollectionChanged;
            }
        }

        void observable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource is IEnumerable<object>)
            {
                if (e.NewStartingIndex > -1 || e.OldStartingIndex > -1)
                {
                    var list = ((IEnumerable<object>)ItemsSource).ToList();
                    var index = e.NewStartingIndex > -1 ? e.NewStartingIndex : e.OldStartingIndex;

                    int rows = (int)Math.Ceiling((double)list.Count / ItemsInRow);
                    int startingRow = (int)Math.Ceiling((double)(index + 1) / ItemsInRow);
                    // Removing last row.
                    if (e.OldStartingIndex >= list.Count && rows < startingRow)
                        rows++;
                    for (int i = startingRow - 1; i < rows; i++)
                    {
                        var rowList = new List<object>();
                        var baseIndex = i * ItemsInRow;
                        for (int j = 0; baseIndex + j < list.Count && j < ItemsInRow; j++)
                        {
                            rowList.Add(list[baseIndex + j]);
                        }
                        if (items.Count < i)
                            items.Add(rowList);
                        else
                        {
                            if (rowList.Count > 0)
                                items.Insert(i, rowList);
                            else
                                items.RemoveAt(i);
                            if (items.Count > i + 1)
                                items.RemoveAt(i + 1);
                        }
                    }
                }
                else
                    SetItems();
            }
        }

        void SetItems()
        {
            if (ItemsSource is IEnumerable<object>)
            {
                items.Clear();
                var list = ((IEnumerable<object>)ItemsSource).ToList();

                for (int i = 0; i < list.Count; i += ItemsInRow)
                {
                    var rowList = new List<object>();
                    for (int j = 0; i + j < list.Count && j < ItemsInRow; j++)
                    {
                        rowList.Add(list[i + j]);
                    }
                    items.Add(rowList);
                }
            }
        }

        void SetListViewTemplate()
        {
            isBaseTemplateChanging = true;

            base.ItemTemplate = new DataTemplate(() =>
            {
                var collectionViewCell = new CollectionViewCell();
                collectionViewCell.ItemTapped += (s, e) =>
                {
                    SelectedItem = e.Item;
                    ItemTapped(this, new ItemTappedEventArgs(e.Group, e.Item));
                };
                return collectionViewCell;
            });
            isBaseTemplateChanging = false;
        }

		protected override void HandleItemAppearing(ItemVisibilityEventArgs e)
		{
			if (ItemsSource is ISupportIncrementalLoading)
			{
				var loaderSource = (ISupportIncrementalLoading)ItemsSource;

				if (!loaderSource.IsLoading && loaderSource.Count >= LoadMoreOffset)
				{
					var item = e.Item as List<object>;
					if (item.Any(x => x == loaderSource[loaderSource.Count - LoadMoreOffset]))
					{
						loaderSource.LoadMoreItemsAsync(0);
					}
				}
			}
		}
    }
}
