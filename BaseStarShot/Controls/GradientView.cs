using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class GradientView : Xamarin.Forms.Image
    {
        public static BindableProperty OrientationProperty =
            BindableProperty.Create<GradientView, GradientOrientation>(l => l.Orientation, GradientOrientation.TopBottom);

        public static BindableProperty StartColorProperty =
            BindableProperty.Create<GradientView, Color>(l => l.StartColor, Color.Transparent);

        public static BindableProperty EndColorViewProperty =
            BindableProperty.Create<GradientView, Color>(l => l.EndColor, Color.Transparent);

        public static BindableProperty GradientTypeProperty =
            BindableProperty.Create<GradientView, GradientType>(l => l.GradientStyle, GradientType.Linear);

        public static BindableProperty BackgroundGradientStyleProperty =
            BindableProperty.Create<GradientView, BackgroundGradientType>(l => l.BackgroundGradientStyle, BackgroundGradientType.GradientBackground);

        public static readonly BindableProperty ImagePlaceholderProperty =
            BindableProperty.Create<GradientView, ImageSource>(p => p.ImagePlaceholder, null);

        public GradientOrientation Orientation
        {
            get { return (GradientOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        public Color EndColor
        {
            get { return (Color)GetValue(EndColorViewProperty); }
            set { SetValue(EndColorViewProperty, value); }
        }

        public GradientType GradientStyle
        {
            get { return (GradientType)GetValue(GradientTypeProperty); }
            set { SetValue(GradientTypeProperty, value); }
        }

        public BackgroundGradientType BackgroundGradientStyle
        {
            get { return (BackgroundGradientType)GetValue(BackgroundGradientStyleProperty); }
            set { SetValue(BackgroundGradientStyleProperty, value); }
        }

        public ImageSource ImagePlaceholder
        {
            get { return (ImageSource)base.GetValue(ImagePlaceholderProperty); }
            set { base.SetValue(ImagePlaceholderProperty, value); }
        }
    }
}
