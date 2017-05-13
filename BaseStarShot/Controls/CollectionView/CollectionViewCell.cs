using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class CollectionViewCell : ViewCell
    {
        public static readonly BindableProperty ItemsInRowProperty =
            BindableProperty.Create<CollectionViewCell, int>(p => p.ItemsInRow, 1);

        public static readonly BindableProperty ItemViewFactoryProperty =
            BindableProperty.Create<CollectionViewCell, Func<View>>(p => p.ItemViewFactory, null);

        public static readonly BindableProperty ColumnSpacingProperty =
            BindableProperty.Create<CollectionViewCell, double>(p => p.ColumnSpacing, 0);

        public static readonly BindableProperty PaddingProperty =
            BindableProperty.Create<CollectionViewCell, Thickness>(p => p.Padding, 0);

        public int ItemsInRow
        {
            get { return (int)GetValue(ItemsInRowProperty); }
            set { SetValue(ItemsInRowProperty, value); }
        }

        public Func<View> ItemViewFactory
        {
            get { return (Func<View>)GetValue(ItemViewFactoryProperty); }
            set { SetValue(ItemViewFactoryProperty, value); }
        }

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public event EventHandler<ItemTappedEventArgs> ItemTapped = delegate { };

        Grid grid;
		bool isInitialized;

        public CollectionViewCell()
        {
            grid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                RowSpacing = 0,
                RowDefinitions = 
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
            };

            this.View = grid;
            this.View.BackgroundColor = Color.Transparent;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

			if (isInitialized)
				return;

			var collectionView = this.Parent as CollectionView;
			if (collectionView != null)
            {
                this.SetBinding(CollectionViewCell.ItemsInRowProperty, new Binding("ItemsInRow", source: collectionView));
                this.SetBinding(CollectionViewCell.ColumnSpacingProperty, new Binding("ColumnSpacing", source: collectionView));
                this.SetBinding(CollectionViewCell.PaddingProperty, new Binding("RowPadding", source: collectionView));

                if (collectionView.ItemTemplate == null) return;

                this.ItemViewFactory = () =>
                {
                    var content = collectionView.ItemTemplate.CreateContent();
                    View view;
                    if (content is ViewCell)
                        view = (content as ViewCell).View;

                    else
                        view = content as View;
                    return view;
                };

				grid.SetBinding(Grid.ColumnSpacingProperty, new Binding("ColumnSpacing", source: this));
				grid.SetBinding(Grid.PaddingProperty, new Binding("Padding", source: this));
                grid.SetBinding(Grid.WidthRequestProperty, new Binding("Width", source: collectionView));

				isInitialized = true;
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemViewFactoryProperty.PropertyName
                || propertyName == ItemsInRowProperty.PropertyName)
            {
                CreateView();
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            BindContext();
        }

        void CreateView()
        {
            if (ItemViewFactory == null)
                return;

            if (grid.ColumnDefinitions.Count > ItemsInRow)
            {
                int count = grid.ColumnDefinitions.Count;
                for (int i = ItemsInRow; i < count; i++)
                {
                    grid.Children.RemoveAt(ItemsInRow);
                    grid.ColumnDefinitions.RemoveAt(ItemsInRow);
                }
            }
            else
            {
                var list = (IEnumerable)this.BindingContext;
                var enumerator = list.GetEnumerator();

                for (int i = 0; i < ItemsInRow; i++)
                {
                    object context = null;
                    if (enumerator.MoveNext())
                        context = enumerator.Current;

                    if (grid.ColumnDefinitions.Count > i)
                        continue;
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    if (context == null)
                        continue;

                    var view = ItemViewFactory();

                    if (view != null)
                    {
                        view.BindingContext = context;
                        var tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.CommandParameter = view;
                        tapGestureRecognizer.Command = new RelayCommand<View>(v =>
                        {
                            ItemTapped(this, new ItemTappedEventArgs(null, v.BindingContext));
                        });
                        view.GestureRecognizers.Add(tapGestureRecognizer);

                        grid.Children.Add(view, i, 0);
                    }
                    else
                    {
                        var tempView = new Xamarin.Forms.StackLayout();
                        grid.Children.Add(tempView, i, 0);
                    }
                    //else
                    //{
                    //    var collectionView = this.Parent as CollectionView;
                    //    var content = collectionView.ItemTemplate.CreateContent();

                    //    (content as NativeCell4).View
                    //}

                }
            }
        }

        void BindContext()
        {
            if (grid.Children.Count == 0)
                return;
            if (this.BindingContext is IEnumerable)
            {
                var list = (IEnumerable)this.BindingContext;
                var enumerator = list.GetEnumerator();
                for (int i = 0; i < ItemsInRow; i++)
                {
                    if (grid.Children.Count < i + 1)
                        break;
                    var child = grid.Children[i];
                    object context = null;
                    if (enumerator.MoveNext())
                        context = enumerator.Current;
                    child.IsVisible = context != null;
                    if (child.BindingContext != context)
                        child.BindingContext = context;
                }
            }
        }
    }
}
