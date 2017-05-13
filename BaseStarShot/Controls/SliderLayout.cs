using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    [ContentProperty("Children")]
    public class SliderLayout : ContentView
    {
        RelativeLayout relativeLayout = new RelativeLayout();
        Frame mainFrame;

        readonly int defaultPadding = 5;
        readonly string semi_transparent = "#58595b";

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create<SliderLayout, int>(p => p.Minimum, 0);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create<SliderLayout, int>(p => p.Maximum, 0);

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create<SliderLayout, double>(p => p.Progress, 0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<SliderLayout, double>(p => p.FontSize, 12);

        public static readonly BindableProperty BackgroundImageProperty =
            BindableProperty.Create<SliderLayout, FileImageSource>(p => p.BackgroundImage, null);

        public static readonly BindableProperty ThumbImageProperty =
            BindableProperty.Create<SliderLayout, FileImageSource>(p => p.ThumbImage, null);

        public static readonly BindableProperty TouchableProperty =
            BindableProperty.Create<SliderLayout, bool>(p => p.Touchable, false);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<SliderLayout, Color>(p => p.TextColor, Color.White);

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public FileImageSource ThumbImage
        {
            get { return (FileImageSource)GetValue(ThumbImageProperty); }
            set { SetValue(ThumbImageProperty, value); }
        }

        public bool Touchable
        {
            get { return (bool)GetValue(TouchableProperty); }
            set { SetValue(TouchableProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public SliderLayout()
        {
            if (Device.OS != TargetPlatform.Android)
            {
                relativeLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

                var maxLabel = new BaseStarShot.Controls.Label
                {
                    Text = this.Maximum.ToString(),
                    YAlign = Xamarin.Forms.TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    XAlign = Xamarin.Forms.TextAlignment.End,
                    FontStyle = FontStyle.Light
                };
                maxLabel.SetBinding(Label.TextProperty, new Binding { Source = this, Path = "Maximum" });
                maxLabel.SetBinding(Label.FontSizeProperty, new Binding { Source = this, Path = "FontSize" });
                maxLabel.SetBinding(Label.TextColorProperty, new Binding { Source = this, Path = "TextColor" });

                mainFrame = new Frame
                {
                    Padding = new Thickness(new PointSize(defaultPadding), 0, new PointSize(defaultPadding), 0),
                    BackgroundColor = Color.FromHex(semi_transparent),
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Content = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = {
                        maxLabel
                    }
                    },
                };
                mainFrame.SetBinding(BaseStarShot.Controls.Frame.WidthRequestProperty, new Binding { Source = this, Path = "WidthRequest" });

                relativeLayout.Children.Add(mainFrame,
                    Constraint.Constant(0),
                    Constraint.Constant(0),
                    widthConstraint: Constraint.RelativeToParent((parent) =>
                    {
                        return parent.Width;
                    })
                );


                var progressView = new ProgressView
                {
                    PaddingLeft = new PointSize(defaultPadding),
                    PaddingRight = new PointSize(defaultPadding),
                    VerticalTextAlignment = Xamarin.Forms.TextAlignment.Center,
                    HorizontalTextAlignment = Xamarin.Forms.TextAlignment.Start,
                    FontStyle = FontStyle.Light
                };

                progressView.SetBinding(ProgressView.ThumbImageProperty, new Binding { Source = this, Path = "ThumbImage" });
                progressView.SetBinding(ProgressView.ProgressProperty, new Binding { Source = this, Path = "Progress", Mode = BindingMode.TwoWay });
                progressView.SetBinding(ProgressView.BackgroundImageProperty, new Binding { Source = this, Path = "BackgroundImage" });
                progressView.SetBinding(ProgressView.MaximumProperty, new Binding { Source = this, Path = "Maximum" });
                progressView.SetBinding(ProgressView.ParentWidthProperty, new Binding { Source = this, Path = "WidthRequest" });
                progressView.SetBinding(ProgressView.TouchableProperty, new Binding { Source = this, Path = "Touchable" });
                progressView.SetBinding(Label.FontSizeProperty, new Binding { Source = this, Path = "FontSize" });
                progressView.SetBinding(Label.TextColorProperty, new Binding { Source = this, Path = "TextColor" });

                relativeLayout.Children.Add(progressView,
                    Constraint.Constant(0),
                    Constraint.Constant(0),
                    heightConstraint: Constraint.RelativeToView(mainFrame, (parent, sibling) =>
                    {
                        return sibling.Height;
                    })
                );

                this.Content = relativeLayout;
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
    }
}
