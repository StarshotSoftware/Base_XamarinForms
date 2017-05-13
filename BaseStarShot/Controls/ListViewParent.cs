using System;
using Xamarin.Forms;
using System.Reflection;
using System.Collections.Specialized;

namespace BaseStarShot.Controls
{
    [ContentProperty("Children")]
    public class ListViewParent : ContentView
    {
        private static ListView listView = null;
        private static event EventHandler<View> UpdateLayoutEvent = delegate { };
        private static event EventHandler<View> UpdateCustomPlaceholderEvent = delegate { };

        public ListViewParent()
        {
            UpdateLayoutEvent = (s, view) =>
            {
                if (this.Content == null)
                    this.Content = view;
            };

            UpdateCustomPlaceholderEvent = (s, view) =>
            {
                if (view != null && this.Content != null)
                {
                    view.InputTransparent = true;
                    var layout = (AbsoluteLayout)this.Content;
                    layout.Children.Add(view, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                }
            };
        }

        static void ListViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((newValue as ListView) != null)
            {
                if (listView == null)
                {
                    listView = newValue as ListView;

                    var placeholderView = new Label
                    {
                        InputTransparent = true,
                        VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                        HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center
                    };

                    placeholderView.SetBinding(Label.TextProperty, new Binding("EmptyText", source: listView));
                    placeholderView.SetBinding(Label.TextColorProperty, new Binding("EmptyTextColor", source: listView));
                    placeholderView.SetBinding(Label.FontStyleProperty, new Binding("EmptyTextFontStyle", source: listView));

                    var collectionChanged = listView.ItemsSource as INotifyCollectionChanged;
                    if (collectionChanged != null)
                    {
                        if (!(listView.ItemsSource is INotifyCollectionChanged))
                            return;

                        collectionChanged = listView.ItemsSource as INotifyCollectionChanged;

                        collectionChanged.CollectionChanged += (s, ev) =>
                        {
                            if (!listView.IsRefreshing)
                            {
                                placeholderView.IsVisible = ev.NewItems.Count == 0;
                            }
                        };
                    }

                    listView.PropertyChanged += (s, ev) =>
                    {
                        if (ev.PropertyName == ListView.IsRefreshingProperty.PropertyName)
                        {
                            if (listView.IsRefreshing)
                                placeholderView.IsVisible = false;
                            else
                            {
                                placeholderView.IsVisible = GetItemsSourceCount() == 0;
                            }
                        }
                        else if (ev.PropertyName == ListView.ItemsSourceProperty.PropertyName)
                        {
                            if (collectionChanged == null)
                            {
                                if (!(listView.ItemsSource is INotifyCollectionChanged))
                                    return;

                                collectionChanged = listView.ItemsSource as INotifyCollectionChanged;

                                if (collectionChanged != null)
                                {
                                    collectionChanged.CollectionChanged += (s1, ev1) =>
                                    {
                                        if (!listView.IsRefreshing)
                                        {
                                            placeholderView.IsVisible = ev1.NewItems.Count == 0;
                                        }
                                    };
                                }
                            }
                        }
                    };

                    var layout = new AbsoluteLayout();
                    layout.Children.Add(listView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
                    layout.Children.Add(placeholderView, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

                    UpdateLayoutEvent(null, layout);
                }
            }
        }

        static void CustomPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != null)
            {

            }
        }

        private static int GetItemsSourceCount()
        {
            if (listView == null)
                return 0;

            if (listView.ItemsSource != null)
            {
                var prop = listView.ItemsSource.GetType().GetRuntimeProperty("Count");

                if (prop != null && prop.GetValue(listView.ItemsSource) != null)
                {
                    var count = int.Parse(prop.GetValue(listView.ItemsSource).ToString());
                    return count;
                }
            }

            return 0;
        }

        public static BindableProperty CustomPlaceholderProperty =
            BindableProperty.Create<ListViewParent, View>(p => p.CustomPlaceholder, null, propertyChanged: CustomPlaceholderChanged);

        public static BindableProperty ListviewProperty =
            BindableProperty.Create<ListViewParent, ListView>(p => p.Listview, null, propertyChanged: ListViewChanged);

        public ListView Listview
        {
            get { return (ListView)GetValue(ListviewProperty); }
            set { SetValue(ListviewProperty, value); }
        }

        public View CustomPlaceholder
        {
            get { return (View)GetValue(CustomPlaceholderProperty); }
            set { SetValue(CustomPlaceholderProperty, value); }
        }
    }
}
