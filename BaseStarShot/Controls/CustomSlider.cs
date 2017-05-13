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
    public class CustomSlider : View
    {
        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create<CustomSlider, int>(p => p.Minimum, 0);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create<CustomSlider, int>(p => p.Maximum, 0);

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create<CustomSlider, double>(p => p.Progress, 0);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<CustomSlider, double>(p => p.FontSize, 12);

        public static readonly BindableProperty BackgroundImageProperty =
            BindableProperty.Create<CustomSlider, FileImageSource>(p => p.BackgroundImage, null);

        public static readonly BindableProperty ThumbImageProperty =
            BindableProperty.Create<CustomSlider, FileImageSource>(p => p.ThumbImage, null);

        public static readonly BindableProperty TouchableProperty =
            BindableProperty.Create<CustomSlider, bool>(p => p.Touchable, false);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<CustomSlider, Color>(p => p.TextColor, Color.White);

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

        public CustomSlider()
        {
            if (Device.OS == TargetPlatform.Android)
            {

            }
        }

    }
}
