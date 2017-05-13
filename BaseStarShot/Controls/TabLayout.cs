using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;
using BaseStarShot.Services;
using BaseStarShot.Model;

namespace BaseStarShot.Controls
{
    [ContentProperty("Children")]
    public class TabLayout : ContentView
    {
        Grid GridView;
        Grid TabContentView;
        RowDefinition tabHeight;

        bool isSettingContent;

        double smallestFontSize = -1;

        public static readonly BindableProperty TabDividerColorProperty =
            BindableProperty.Create<TabLayout, Color>(p => p.TabDividerColor, Color.Default);

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create<TabLayout, Color>(p => p.LineColor, Color.Transparent);

        public static readonly BindableProperty AnimateTabSwitchProperty =
            BindableProperty.Create<TabLayout, bool>(p => p.AnimateTabSwitch, true);

        public static readonly BindableProperty SeparatorHeightProperty =
            BindableProperty.Create<TabLayout, double>(p => p.SeparatorHeight, new PointSize(2));

        public static readonly BindableProperty TabDividerWidthProperty =
            BindableProperty.Create<TabLayout, double>(p => p.TabDividerWidth, 0);

        public static readonly BindableProperty LineHeightProperty =
            BindableProperty.Create<TabLayout, GridLength>(p => p.LineHeight, new PointGridLength(1));

        public static readonly BindableProperty TabHeightProperty =
            BindableProperty.Create<TabLayout, double>(p => p.TabHeight, new PointSize(50));

        public static readonly BindableProperty TabSpacingProperty =
            BindableProperty.Create<TabLayout, double>(p => p.TabSpacing, 0);

        public static readonly BindableProperty ScrollEnabledProperty =
            BindableProperty.Create<TabLayout, bool>(p => p.ScrollEnabled, false);

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create<TabLayout, int>(p => p.SelectedIndex, -1);



        private ObservableCollection<TabChild> _tabChildren = new ObservableCollection<TabChild>();
        public IList<TabChild> Children
        {
            get { return _tabChildren; }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public bool AnimateTabSwitch
        {
            get { return (bool)GetValue(AnimateTabSwitchProperty); }
            set { SetValue(AnimateTabSwitchProperty, value); }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public Color TabDividerColor
        {
            get { return (Color)GetValue(TabDividerColorProperty); }
            set { SetValue(TabDividerColorProperty, value); }
        }

        public double SeparatorHeight
        {
            get { return (double)GetValue(SeparatorHeightProperty); }
            set { SetValue(SeparatorHeightProperty, value); }
        }

        public GridLength LineHeight
        {
            get { return (GridLength)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public double TabHeight
        {
            get { return (double)GetValue(TabHeightProperty); }
            set { SetValue(TabHeightProperty, value); }
        }

        public double TabSpacing
        {
            get { return (double)GetValue(TabSpacingProperty); }
            set { SetValue(TabSpacingProperty, value); }
        }

        public double TabDividerWidth
        {
            get { return (double)GetValue(TabDividerWidthProperty); }
            set { SetValue(TabDividerWidthProperty, value); }
        }

        public bool ScrollEnabled
        {
            get { return (bool)base.GetValue(ScrollEnabledProperty); }
            set { SetValue(ScrollEnabledProperty, value); }
        }

        private HorizontalScrollView CurrentScrollView;

        public TabLayout()
        {
            this.GridView = new Grid
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 0,
            };
            this.GridView.SetBinding(Grid.ColumnSpacingProperty, new Binding(path: "TabSpacing", source: this));

            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.Padding = 0;

            TabContentView = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowSpacing = 0
            };
            TabContentView.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            TabContentView.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            View view = null;
            if (ScrollEnabled)
            {
                view = new HorizontalScrollView
                {
                    Padding = 0,
                    Orientation = ScrollOrientation.Horizontal,
                    Content = this.GridView
                };

                CurrentScrollView = view as HorizontalScrollView;
            }
            else
            {
                view = this.GridView;
            }

            var box = new BoxView();
            box.SetBinding(BoxView.HeightRequestProperty, new Binding() { Source = this, Path = "SeparatorHeight" });
            box.SetBinding(BoxView.ColorProperty, new Binding() { Source = this, Path = "LineColor" });

            var box1 = new BoxView();
            box1.SetBinding(BoxView.HeightRequestProperty, new Binding() { Source = this, Path = "SeparatorHeight" });
            box1.SetBinding(BoxView.ColorProperty, new Binding() { Source = this, Path = "LineColor" });

            //this.GridView.Children.Add(box, idx, 1);

            isSettingContent = true;
            var contentLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Padding = 0,
                Spacing = 0,
                Children =
                {
                    box,
                    view,
                    box1
                },
            };
            TabContentView.Children.Add(contentLayout, 0, 0);
            this.Content = TabContentView;

            isSettingContent = false;

            tabHeight = new RowDefinition();
            tabHeight.Height = new GridLength(this.TabHeight);
            //tabHeight.SetBinding(RowDefinition.HeightProperty, new Binding { Source = this, Path = "HeightRequest" });
            //this.GridView.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            this.GridView.RowDefinitions.Add(tabHeight);

            var lineRow = new RowDefinition();
            lineRow.SetBinding(RowDefinition.HeightProperty, new Binding { Source = this, Path = "LineHeight" });
            //this.GridView.RowDefinitions.Add(lineRow);
            //this.GridView.ColumnSpacing = 0;
            //this.GridView.RowSpacing = 0;
            _tabChildren.CollectionChanged += _tabChildren_CollectionChanged;

            // TODO: implement XChanged as an event from listview
            //Resolver.Get<IScrollListener>().XChanged += TabLayout_XChanged;
        }

        #region old XChanged code
        //void TabLayout_XChanged(object sender, ScrollX scroll)
        //{
        //    if (CurrentScrollView == null) return;

        //    if (Device.OS == TargetPlatform.Android)
        //    {
        //        var myScrollX = CurrentScrollView.ScrollX + scroll.X1 - scroll.X2;

        //        CurrentScrollView.ScrollToAsync(CurrentScrollView.ScrollX + scroll.X1 - scroll.X2, 0, false);




        //    }
        //    else
        //    {
        //        var maxPosition = CurrentScrollView.ContentSize.Width - CurrentScrollView.Width;
        //        if (maxPosition < 0)
        //            maxPosition = 0;

        //        var movedDistance = scroll.X1 - scroll.X2;

        //        var newXPosition = CurrentScrollView.ScrollX + movedDistance;
        //        if (newXPosition < 0)
        //            newXPosition = 0;
        //        else if (newXPosition > maxPosition)
        //            newXPosition = maxPosition;
        //        CurrentScrollView.ScrollToAsync(newXPosition, 0, false);
        //    }



        //    var padding = new PointSize(130);

        //    int center = (int)Math.Abs((CurrentScrollView.ScrollX + (CurrentScrollView.Width / 2d)) - padding); //CurrentScrollView.ContentSize.Width
        //    int chilrenNum = this.GridView.Children.Count;
        //    for (int i = 0; i < chilrenNum; i++)
        //    {
        //        View v = this.GridView.Children.ElementAt(i);

        //        if (v.GetType() != typeof(BaseStarShot.Controls.Button))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            var button = v as BaseStarShot.Controls.Button;

        //            int viewLeft = (int)button.X;
        //            int viewWidth = (int)button.Bounds.Width;

        //            //Debug.WriteLine("button: " + button.Text + ", viewLeft: " + viewLeft + ", center: " + center);
        //            if (center >= viewLeft && center <= (viewLeft) + viewWidth)
        //            {
        //                //CurrentScrollView.ScrollToAsync((viewLeft + viewWidth / 2) - center, 0, false);
        //                SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }
        //}
        #endregion


        int oldSelectedIndex = -1;
        protected override void OnPropertyChanging(string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            if (propertyName == "SelectedIndex")
            {
                oldSelectedIndex = SelectedIndex;
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (!isSettingContent && propertyName == "Content") throw new InvalidOperationException("Cannot set Content of TabLayout");
            if (propertyName == "SelectedIndex")
            {
                if (oldSelectedIndex > -1 && this.Children.Count > oldSelectedIndex)
                {
                    var oldTab = this.Children.ElementAt(oldSelectedIndex);
                    oldTab.SetSelected(false);
                }

                if (SelectedIndex > -1 && this.Children.Count > SelectedIndex)
                {
                    var newTab = this.Children.ElementAt(SelectedIndex);
                    newTab.SetSelected(true);
                    this.TabContentView.RaiseChild(newTab);
                }

                SetTabChildrenBackground();
            }
            else if (propertyName == "ScrollEnabled")
            {
                var stackLayout = (StackLayout)this.Content;
                stackLayout.Children.RemoveAt(0);
                View view;
                if (ScrollEnabled)
                {
                    view = new HorizontalScrollView
                    {
                        Padding = 0,
                        Orientation = ScrollOrientation.Horizontal,
                        Content = this.GridView
                    };

                    CurrentScrollView = view as HorizontalScrollView;
                    CurrentScrollView.Padding = new PointThickness { Value = new PointThickness(130, 0, 130, 0) };
                }
                else
                {
                    view = this.GridView;

                    CurrentScrollView = null;
                }
                stackLayout.Children.Insert(0, view);
            }

            switch (propertyName)
            {
                case "BackgroundColor":
                case "SelectedBackgroundColor":
                case "SelectedBackgroundImage": SetTabChildrenBackground(); break;
                case "BackgroundImage": SetTabChildrenBackground(); break;
                case "TabHeight": tabHeight.Height = new GridLength(this.TabHeight); break;
            }

            base.OnPropertyChanged(propertyName);
        }


        void _tabChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int index = e.NewStartingIndex;
                        foreach (TabChild tc in e.NewItems)
                        {
                            tc.SetBinding(TabChild.AnimateTabSwitchProperty, new Binding { Source = this, Path = "AnimateTabSwitch" });

                            int idx = index++;
                            var column = new ColumnDefinition();
                            column.SetBinding(ColumnDefinition.WidthProperty, new Binding { Source = tc, Path = "ColumnWidth" });
                            this.GridView.ColumnDefinitions.Insert(idx, column);

                            var element = new BaseStarShot.Controls.Button
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                            };

                            if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
                            {
                                element.BackgroundImage = "ButtonTabStyle";
                                element.BorderWidth = 0;
                            }

                            element.XAlign = Xamarin.Forms.TextAlignment.Center;
                            element.SetBinding(Button.RefitTextEnabledProperty, new Binding { Source = tc, Path = "RefitTextEnabled" });
                            element.SetBinding(Button.TextProperty, new Binding { Source = tc, Path = "Text" });
                            element.SetBinding(Button.TextColorProperty, new Binding { Source = tc, Path = "TextColor" });
                            element.SetBinding(Button.CornerRadiusProperty, new Binding { Source = tc, Path = "CornerRadius" });
                            element.SetBinding(Button.FontSizeProperty, new Binding { Source = tc, Path = "FontSize" });
                            element.SetBinding(Button.FontStyleProperty, new Binding { Source = tc, Path = "FontStyle" });
                            element.SetBinding(Button.BorderRadiusProperty, new Binding { Source = tc, Path = "BorderRadius" });
                            element.SetBinding(Button.ImageLeftHeightProperty, new Binding { Source = tc, Path = "ImageLeftHeight" });
                            element.SetBinding(Button.ImageLeftWidthProperty, new Binding { Source = tc, Path = "ImageLeftWidth" });
                            element.SetBinding(Button.ImageRightHeightProperty, new Binding { Source = tc, Path = "ImageRightHeight" });
                            element.SetBinding(Button.ImageRightWidthProperty, new Binding { Source = tc, Path = "ImageRightWidth" });
                            element.SetBinding(Button.ImageLeftProperty, new Binding { Source = tc, Path = "ImageLeft" });
                            element.SetBinding(Button.ImageRightProperty, new Binding { Source = tc, Path = "ImageRight" });
                            element.SetBinding(Button.HeightRequestProperty, new Binding("TabHeight", source: this));
                            element.SetBinding(Button.CenterImageProperty, new Binding { Source = tc, Path = "CenterImage" });
                            element.SetBinding(Button.TextPaddingProperty, new Binding { Source = tc, Path = "TextPadding" });
                            element.SetBinding(Button.PaddingProperty, new Binding { Source = tc, Path = "Padding" });
                            element.SetBinding(Button.SelectedImageLeftProperty, new Binding { Source = tc, Path = "SelectedImageLeft" });
                            element.SetBinding(Button.SelectedImageRightProperty, new Binding { Source = tc, Path = "SelectedImageRight" });


                            //if (Device.OS == TargetPlatform.iOS)
                            element.PropertyChanged += ButtonPropertyChanged;
                            element.Clicked += element_Clicked;


                            var box = new BoxView();// { HorizontalOptions = LayoutOptions.End };
                            box.SetBinding(BoxView.WidthRequestProperty, new Binding() { Source = this, Path = "TabDividerWidth" });
                            box.SetBinding(BoxView.ColorProperty, new Binding() { Source = this, Path = "TabDividerColor" });

                            var layout = new Grid
                            {
                                ColumnSpacing = 0,
                                Padding = 0
                            };
                            if (e.NewStartingIndex > 0)
                            {
                                layout.ColumnDefinitions = new ColumnDefinitionCollection
                                    {
                                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Auto)},
                                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)}
                                    };
                                layout.Children.Add(box, 0, 0);
                                layout.Children.Add(element, 1, 0);
                            }
                            else
                            {
                                layout.ColumnDefinitions = new ColumnDefinitionCollection
                                    {
                                        new ColumnDefinition{ Width = new GridLength(1, GridUnitType.Star)}
                                    };
                                layout.Children.Add(element, 0, 0);
                            }

                            this.GridView.Children.Add(layout, idx, 0);


                            var subText = new BaseStarShot.Controls.Label
                            {
                                VerticalOptions = LayoutOptions.End,
                                VerticalTextAlignment = Xamarin.Forms.TextAlignment.End,
                                HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                                MaxLines = 1,
                                LineBreakMode = LineBreakMode.TailTruncation
                            };
                            subText.SetBinding(BaseStarShot.Controls.Label.TextProperty, new Binding(path: "SubText", source: tc));
                            subText.SetBinding(BaseStarShot.Controls.Label.FontSizeProperty, new Binding(path: "SubTextFontSize", source: tc));
                            subText.SetBinding(BaseStarShot.Controls.Label.TextColorProperty, new Binding(path: "SubTextColor", source: tc));
                            subText.SetBinding(BaseStarShot.Controls.Label.IsVisibleProperty, new Binding(path: "SubText", source: tc, converter: new StringToBoolConverter()));

                            this.GridView.Children.Add(subText, idx, 0);

                            this.TabContentView.Children.Add(tc, 0 , 1);

                            if (SelectedIndex == -1)
                            {
                                SelectedIndex = idx;
                            }
                            else
                            {
                                tc.Opacity = 0;
                                tc.IsVisible = false;
                                this.TabContentView.LowerChild(tc);
                            }

                            if (tc.SetCurrentSelected)
                            {
                                SelectedIndex = idx;
                            }
                            if (idx == SelectedIndex)
                            {
                                if (tc.SelectedBackgroundImage != null && tc.SelectedBackgroundImage.File != null)
                                    element.BackgroundImage = tc.SelectedBackgroundImage;

                                element.BackgroundColor = tc.SelectedBackgroundColor;
                                element.TextColor = tc.SelectedTextColor;
                            }
                            else
                            {
                                if (tc.BackgroundImage != null && tc.BackgroundImage.File != null)
                                    element.BackgroundImage = tc.BackgroundImage;

                                element.BackgroundColor = tc.BackgroundColor;
                                element.TextColor = tc.TextColor;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {

                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        int index = e.OldStartingIndex;
                        this.GridView.ColumnDefinitions.RemoveAt(index);

                        var button = this.GridView.Children.FirstOrDefault(c => Grid.GetColumn(c) == index && Grid.GetRow(c) == 0);
                        var boxView = this.GridView.Children.FirstOrDefault(c => Grid.GetColumn(c) == index && Grid.GetRow(c) == 1);
                        this.GridView.Children.Remove(button);
                        this.GridView.Children.Remove(boxView);

                        for (int i = e.OldStartingIndex + 1; i < this.GridView.Children.Count; i++)
                        {
                            button = this.GridView.Children.FirstOrDefault(c => Grid.GetColumn(c) == i && Grid.GetRow(c) == 0);
                            boxView = this.GridView.Children.FirstOrDefault(c => Grid.GetColumn(c) == i && Grid.GetRow(c) == 1);

                            Grid.SetColumn(button, i - 1);
                            Grid.SetColumn(boxView, i - 1);
                        }
                        this.TabContentView.Children.RemoveAt(index + 1);
                        RemoveTabChild(index);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        SelectedIndex = -1;
                        this.GridView.ColumnDefinitions.Clear();
                        this.GridView.Children.Clear();
                        while (this.TabContentView.Children.Count > 0)
                            RemoveTabChild(0);
                    }
                    break;
            }
        }

        void ButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FontSize")
            {
                var button = (Button)sender;
                if (smallestFontSize.Equals(-1))
                {
                    smallestFontSize = button.FontSize;
                    return;
                }

                if (button.FontSize < smallestFontSize)
                    smallestFontSize = button.FontSize;
            }

            if (smallestFontSize.Equals(-1))
                return;

            foreach (var child in GridView.Children)
            {
                var button = child as Button;
                if (button == null)
                    continue;

                button.FontSize = smallestFontSize;
            }
        }

        void RemoveTabChild(int index)
        {
            var tabChild = (TabChild)this.TabContentView.Children[index + 1];
            if (tabChild.Content is Xamarin.Forms.ListView)
                tabChild.Content = null;
            else if (tabChild.Content != null)
            {
                RemoveAllListViews(tabChild.Content);
            }
            this.TabContentView.Children.RemoveAt(index + 1);
        }

        void RemoveAllListViews(Element element)
        {
            if (element is Xamarin.Forms.ContentView)
            {
                var contentView = (Xamarin.Forms.ContentView)element;
                if (contentView.Content != null)
                {
                    if (contentView.Content is Xamarin.Forms.ListView)
                        contentView.Content = null;
                    else
                        RemoveAllListViews(contentView.Content);
                }
            }
            else if (element is Layout<View>)
            {
                var layout = (Layout<View>)element;
                foreach (var child in layout.Children.ToList())
                {
                    if (child is Xamarin.Forms.ListView)
                        layout.Children.Remove(child);
                    else
                        RemoveAllListViews(element);
                }
            }
        }

        void SetTabChildrenBackground()
        {
            for (int i = 0; i < this.Children.Count; i++)
            {
                SetTabChildBackground(i, i == SelectedIndex);

            }
        }

        void SetTabChildBackground(int index, bool selected)
        {
            int childIndex = index > 0 ? 1 : 0;
            var button = (BaseStarShot.Controls.Button)(this.GridView.Children.FirstOrDefault(c => Grid.GetColumn(c) == index && Grid.GetRow(c) == 0) as Grid).Children[childIndex];
            if (button != null)
            {
                var tabChild = this.Children[index];

                if (tabChild.SelectedBackgroundImage != null && tabChild.SelectedBackgroundImage.File != null &&
                    tabChild.BackgroundImage != null && tabChild.BackgroundImage.File != null)
                    button.BackgroundImage = selected ? tabChild.SelectedBackgroundImage : tabChild.BackgroundImage;

                button.BackgroundColor = selected ? tabChild.SelectedBackgroundColor : tabChild.BackgroundColor;
                button.TextColor = selected ? tabChild.SelectedTextColor : tabChild.TextColor;

                if (ChangeBackground(button, true))
                {
                    button.ImageLeft = selected ? tabChild.SelectedImageLeft : tabChild.ImageLeft;
                }

                if (ChangeBackground(button, false))
                {
                    button.ImageRight = selected ? tabChild.SelectedImageRight : tabChild.ImageRight;
                }
            }
        }

        bool ChangeBackground(Button button, bool leftImage)
        {
            bool change = false;
            if (leftImage)
                change = button.ImageLeft != null && button.SelectedImageLeft != null;
            else
                change = button.ImageRight != null && button.SelectedImageRight != null;

            return change;
        }

        void element_Clicked(object sender, EventArgs e)
        {
            int idx = Grid.GetColumn(((BaseStarShot.Controls.Button)sender).Parent);
            SelectedIndex = idx;
        }

    }
}
