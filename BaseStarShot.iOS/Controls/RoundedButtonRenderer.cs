using BaseStarShot.Controls;
using CoreGraphics;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundedButton), typeof(RoundedButtonRenderer))]
namespace BaseStarShot.Controls
{
    public class RoundedButtonRenderer : ButtonControlRenderer
    {
        protected RoundedButton BaseElement { get { return (RoundedButton)Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            if (BaseElement != null)
            {
                SetShadow(BaseElement);
                if (Control != null)
                {
                    Control.ImageView.ContentMode = UIViewContentMode.ScaleToFill;

                    Control.TouchDown += (s, evt) =>
                    {
                        if (Control != null)
                        {
                            Control.Selected = true;
                        }
                    };

                    Control.TouchDownRepeat += (s, evt) =>
                    {
                        if (Control != null)
                        {
                            Control.Selected = true;
                        }
                    };

                    Control.TouchCancel += (s, evt) =>
                    {
                        if (Control != null)
                        {
                            Control.Selected = false;
                        }
                    };

                    Control.TouchUpInside += async (s, evt) =>
                    {
                        if (Control != null)
                        {
                            Control.Selected = true;
                            await Task.Delay(500);
                            if (Control != null)
                            {
                                Control.Selected = false;
                            }
                        }
                    };
                }
            }

            //Control.UserInteractionEnabled = true;
            //Control.AddTarget((sender, ev) => { Control.Selected = true; }, UIControlEvent.TouchDown | UIControlEvent.TouchDownRepeat);
            //Control.AddTarget((sender, ev) => { Control.Selected = false; }, UIControlEvent.TouchCancel);
            //Control.AddTarget(async (sender, ev) =>
            //{
            //    Control.Selected = true;
            //    await Task.Delay(500);
            //    Control.Selected = false;
            //}, UIControlEvent.TouchUpInside);

            SetDisabledTextColor();
        }

        //void OnTouchUpInside(object sender, EventArgs e)
        //{
        //    if (Element != null)
        //        Element.Raise("Clicked", EventArgs.Empty);
        //}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (BaseElement != null)
            {
                if (Control.Frame.Height > 0)
                {
                    //Control.Layer.CornerRadius = 5;

                    SetPressedColor();
                    SetHoverColor();
                    SetDisabledColor();
                    SetGradientColor();

                    //SetCornerRadius();
                    //Control.Layer.RenderGradientBackground(baseElement.GradientColor);
                    //if (overlay != null)
                    //{
                    //    overlay.Frame = Control.Layer.Bounds;
                    //    overlay.CornerRadius = Control.Layer.CornerRadius;
                    //}

                    SetShadow(BaseElement);
                }

            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = (RoundedButton)sender;
            switch (e.PropertyName)
            {
                //case "IsEnabled": SetGradientLayer(); break;
                case "HasShadow": SetShadow(element); break;
                case "HoverColor":
                    SetHoverColor();
                    break;
                case "PressedColor":
                    SetPressedColor();
                    break;
                case "GradientColor":
                    SetGradientColor();
                    break;
                case "DisableColor":
                    SetDisabledColor();
                    break;
                case "DisabledTextColor":
                    SetDisabledTextColor();
                    break;
            }

        }

        void SetShadow(RoundedButton element)
        {
            if (element.HasShadow)
            {
                Control.Layer.RenderButtonShadow();
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (Control != null)
        //    {
        //        Control.TouchUpInside -= OnTouchUpInside;
        //    }

        //    base.Dispose(disposing);
        //}

        void SetPressedColor()
        {
            var pressedColor = ImageFromGradient(BaseElement.PressedColor);
            Control.SetBackgroundImage(pressedColor, UIControlState.Highlighted | UIControlState.Selected);
        }

        void SetHoverColor()
        {
            var hoverColor = ImageFromGradient(BaseElement.HoverColor);
            //Control.SetBackgroundImage(hoverColor, UIControlState.Focused);

        }

        void SetDisabledColor()
        {
            var disabledColor = ImageFromGradient(BaseElement.DisableColor);
            Control.SetBackgroundImage(disabledColor, UIControlState.Disabled);

        }

        void SetGradientColor()
        {
            var gradientColor = ImageFromGradient(BaseElement.GradientColor);
            Control.SetBackgroundImage(gradientColor, UIControlState.Normal);
        }

        void SetDisabledTextColor()
        {
            Control.SetTitleColor(BaseElement.DisabledTextColor.ToUIColor(), UIControlState.Disabled);
        }

        public UIImage ImageFromGradient(string color)
        {
            var gradientArray = color.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            nfloat[] locations = new nfloat[gradientArray.Length];
            CGColor[] colorList = new CGColor[gradientArray.Length];
            for (int i = 0; i < gradientArray.Length; i++)
            {
                var gradientColorOffset = gradientArray[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                colorList[i] = Xamarin.Forms.Color.FromHex(gradientColorOffset[0]).ToCGColor();
                int offsetInt = Int32.Parse(gradientColorOffset[1].Replace("%", ""));
                float offsetFloat = offsetInt / 100f;
                locations[i] = offsetFloat;
            }

            var rect = new CoreGraphics.CGRect(0, 0, Control.Frame.Width, Control.Frame.Height);

            var colorSpace = CGColorSpace.CreateDeviceRGB();
            UIGraphics.BeginImageContext(rect.Size);
            CoreGraphics.CGGradient gradient = new CGGradient(colorSpace, colorList, locations);

            colorSpace.Dispose();

            var context = UIGraphics.GetCurrentContext();

            context.DrawLinearGradient(gradient, new CGPoint(0, 0), new CGPoint(0, Control.Frame.Height), CGGradientDrawingOptions.None);

            gradient.Dispose();

            try
            {
                var img = UIGraphics.GetImageFromCurrentImageContext();

                UIGraphics.EndImageContext();

                return img;
            }
            catch
            {
                return null;
            }
        }


    }
}
