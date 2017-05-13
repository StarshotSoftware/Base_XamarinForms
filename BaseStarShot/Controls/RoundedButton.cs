using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class RoundedButton : Button
    {
        /// <summary>
        /// FOR WINDOWS BUTTON ONLY
        /// </summary>
        public static BindableProperty ReAppearingProperty =
           BindableProperty.Create<RoundedButton, bool>(p => p.ReAppearing, true, BindingMode.TwoWay);
        
        public static BindableProperty HasShadowProperty =
            BindableProperty.Create<RoundedButton, bool>(p => p.HasShadow, false, BindingMode.TwoWay);
        
        public static BindableProperty OrientationProperty =
            BindableProperty.Create<RoundedButton, GradientOrientation>(l => l.Orientation, GradientOrientation.TopBottom);

        public static BindableProperty StartColorProperty =
            BindableProperty.Create<RoundedButton, string>(l => l.GradientColor, "");

        public static BindableProperty HoverColorProperty =
            BindableProperty.Create<RoundedButton, string>(l => l.HoverColor, "");

        public static BindableProperty DisableColorProperty =
            BindableProperty.Create<RoundedButton, string>(l => l.DisableColor, "");

        public static BindableProperty PressedColorProperty =
            BindableProperty.Create<RoundedButton, string>(l => l.PressedColor, "");

        public static BindableProperty GradientTypeProperty =
            BindableProperty.Create<RoundedButton, GradientType>(l => l.GradientStyle, GradientType.Linear);

        public static BindableProperty BackgroundGradientStyleProperty =
            BindableProperty.Create<RoundedButton, BackgroundGradientType>(l => l.BackgroundGradientStyle, BackgroundGradientType.GradientBackground);


        public bool ReAppearing
        {
            get { return (bool)GetValue(ReAppearingProperty); }
            set { SetValue(ReAppearingProperty, value); }
        }

        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }


        public GradientOrientation Orientation
        {
            get { return (GradientOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        
        public string GradientColor
        {
            get { return (string)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }
        public string HoverColor
        {
            get { return (string)GetValue(HoverColorProperty); }
            set { SetValue(HoverColorProperty, value); }
        }

        public string DisableColor
        {
            get { return (string)GetValue(DisableColorProperty); }
            set { SetValue(DisableColorProperty, value); }
        }

        public string PressedColor
        {
            get { return (string)GetValue(PressedColorProperty); }
            set { SetValue(PressedColorProperty, value); }
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

        public RoundedButton()
        {
            SetFontFamily();
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "FontStyle")
                    SetFontFamily();


            };
        }
        
    }
}
