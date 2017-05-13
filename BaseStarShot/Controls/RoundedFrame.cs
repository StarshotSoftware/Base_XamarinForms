using BaseStarShot;
using BaseStarShot.Controls;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class RoundedFrame : Grid
    {
        public static BindableProperty BackgroundImageProperty =
          BindableProperty.Create<RoundedFrame, FileImageSource>(p => p.BackgroundImage, null);

        //public static BindableProperty BackgroundColorProperty =
        //    BindableProperty.Create<RoundedFrame, Color>(p => p.BackgroundColor, Color.Default);

        public static BindableProperty StartColorProperty =
            BindableProperty.Create<RoundedFrame, string>(l => l.GradientColor, "");

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<RoundedFrame, Color>(l => l.BorderColor, Color.FromHex("#07381a"));

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create<RoundedFrame, string>(l => l.CornerRadius, "0,0,0,0");

        public static BindableProperty HasBorderProperty =
            BindableProperty.Create<RoundedFrame, bool>(l => l.HasBorder, false);

        public RoundedFrame()
        {
            if (Device.OS == TargetPlatform.iOS)
                Children.Insert(0, new ContentView());
        }

        public string GradientColor
        {
            get { return (string)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        //public Color BackgroundColor
        //{
        //    get { return (Color)GetValue(BackgroundColorProperty); }
        //    set { SetValue(BackgroundColorProperty, value); }
        //}

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public string CornerRadius
        {
            get { return (string)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }

        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }
    }
}
