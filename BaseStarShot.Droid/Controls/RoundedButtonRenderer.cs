using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using BaseStarShot.Controls;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedButton), typeof(RoundedButtonRenderer))]
namespace BaseStarShot.Controls
{
    public class RoundedButtonRenderer : ButtonControlRenderer
    {
        private Android.Graphics.Rect textBounds = new Android.Graphics.Rect();
        private Android.Graphics.Rect drawableBounds = new Android.Graphics.Rect();
        private int currentWidth = 0;

        protected RoundedButton BaseControl { get { return (RoundedButton)Element; } }

        private StateListDrawable gradientDrawable = null;
        private float[] CornerRadiusArray = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null || e.NewElement == null) return;

            var buttonControl = (RoundedButton)Element;
            gradientDrawable = new StateListDrawable();
            SetCorners();

            Control.Focusable = false;

            buttonControl.PropertyChanged += (s, ev) =>
            {
                if (Control == null || Element == null)
                    return;

                var element = (RoundedButton)s;
                switch (ev.PropertyName)
                {
                    case "IsEnabled":
                        Control.SetTextColor(element.TextColor.ToAndroid()); // for samsung s4 and note5
                        break;
                    case "Text":
                        Control.Text = element.Text;
                        var width = Control.Width;
                        Control.SetWidth(width);
                        break;
                    case "HoverColor":
                        SetHoverColor();
                        Control.Background = gradientDrawable;
                        break;
                    case "PressedColor":
                        SetPressedColor();
                        Control.Background = gradientDrawable;
                        break;
                    case "GradientColor":
                        UpdateStateDrawable();

                        break;
                    case "DisableColor":
                        SetDisabledColor();
                        Control.Background = gradientDrawable;
                        break;

                    case "BorderColor":
                        UpdateStateDrawable();
                        break;
                    //case "Width":
                    //    SetImages(buttonControl);
                    //    break;
                    case "DisabledTextColor":
                        SetDisabledTextColor();
                        break;
                }
            };

            //SetTypeface(buttonControl);
            //SetBackground(buttonControl);
            //SetImages(buttonControl);
            //SetPadding(buttonControl);
            //SetTitlePadding(buttonControl);
            //SetTextAlignment(buttonControl);

            currentWidth = Control.Width;

            if (buttonControl.WidthRequest > 0)
                Control.SetWidth(UIHelper.ConvertDPToPixels(buttonControl.WidthRequest));

            if (buttonControl.HeightRequest > 0)
                Control.SetHeight(UIHelper.ConvertDPToPixels(buttonControl.HeightRequest));

            SetPressedColor();
            SetHoverColor();
            SetDisabledColor();
            SetGradientColor();
            SetDisabledTextColor();

            Control.Background = gradientDrawable;

            this.SetWillNotDraw(false);
        }

        void UpdateStateDrawable()
        {
            gradientDrawable.Dispose();

            gradientDrawable = new StateListDrawable();
            Control.Background = null;

            SetPressedColor();
            SetHoverColor();
            SetDisabledColor();
            SetGradientColor();

            Control.Background = gradientDrawable;

        }

        void SetCorners()
        {
            if (BaseElement.CornerRadius != 0)
            {
                CornerRadiusArray = new float[]
                {
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Top),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Top),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Right),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Right),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Bottom),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Bottom),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Left),
                    UIHelper.ConvertDPToPixels(BaseElement.CornerRadius.Left)
                };

            }
            else
            {
                CornerRadiusArray = new float[] { 5, 5, 5, 5, 5, 5, 5, 5 };
            }
        }
        void SetDisabledTextColor()
        {
            if (BaseControl.DisabledTextColor != Xamarin.Forms.Color.Default)
            {
                ColorStateList textColorStateList = new ColorStateList(
                    new int[][]
                    {
                        new int[]{Android.Resource.Attribute.StatePressed},
                        new int[]{Android.Resource.Attribute.StateEnabled},
                        new int[]{Android.Resource.Attribute.StateFocused, Android.Resource.Attribute.StatePressed},
                        new int[]{-Android.Resource.Attribute.StateEnabled},
                        new int[]{}
                    },
                    new int[]
                    {
                        Element.TextColor.ToAndroid(),
                        Element.TextColor.ToAndroid(),
                        Element.TextColor.ToAndroid(),
                        BaseControl.DisabledTextColor.ToAndroid(),
                        Element.TextColor.ToAndroid()
                    });

                Control.SetTextColor(textColorStateList);
            }
        }

        void SetDisabledColor()
        {
            if (!string.IsNullOrWhiteSpace(BaseControl.DisableColor))
            {
                gradientDrawable.AddState(new[] { -Android.Resource.Attribute.StateEnabled }, GradientConverter(BaseControl.DisableColor));
            }
        }

        void SetPressedColor()
        {
            if (!string.IsNullOrWhiteSpace(BaseControl.PressedColor))
            {
                gradientDrawable.AddState(new[] { Android.Resource.Attribute.StatePressed }, GradientConverter(BaseControl.PressedColor));
                gradientDrawable.AddState(new[] { Android.Resource.Attribute.StateFocused }, GradientConverter(BaseControl.PressedColor));
            }
        }

        void SetHoverColor()
        {
            if (!string.IsNullOrWhiteSpace(BaseControl.HoverColor))
            {
                gradientDrawable.AddState(new[] { Android.Resource.Attribute.StateHovered }, GradientConverter(BaseControl.HoverColor));
            }
        }

        void SetGradientColor()
        {
            if (!string.IsNullOrWhiteSpace(BaseControl.GradientColor))
            {
                gradientDrawable.AddState(new int[] { }, GradientConverter(BaseControl.GradientColor));
            }
        }


        protected LayerDrawable GradientConverter(String Gradient)
        {
            var roundRect = new RoundRectShape(CornerRadiusArray, null, null);

            ShapeDrawable sf = new ShapeDrawable(roundRect);
            sf.SetIntrinsicHeight(100);
            sf.SetIntrinsicWidth(200);
            sf.SetShaderFactory(new GradientShader(Gradient));

            ShapeDrawable shadow = new ShapeDrawable(roundRect);
            shadow.SetIntrinsicHeight(100);
            shadow.SetIntrinsicWidth(200);
            shadow.SetShaderFactory(new ShadowShader(BaseControl));

            LayerDrawable ld = null;
            if (Element.BorderColor != Xamarin.Forms.Color.Default)
            {
                GradientDrawable stroke = new GradientDrawable();
                stroke.SetCornerRadii(CornerRadiusArray);
                stroke.SetStroke(2, Element.BorderColor.ToAndroid());

                ld = new LayerDrawable(new Drawable[] { stroke, shadow, sf });

                ld.SetLayerInset(0, 0, 0, 3, 3);
                ld.SetLayerInset(1, 5, 5, 0, 0); // inset the shadow so it doesn't start right at the left/top
                ld.SetLayerInset(2, 2, 2, 5, 5);
            }
            else
            {
                ld = new LayerDrawable(new Drawable[] { shadow, sf });
                ld.SetLayerInset(0, 5, 5, 0, 0); // inset the shadow so it doesn't start right at the left/top
                ld.SetLayerInset(1, 0, 0, 5, 5);
            }

            return ld;
        }

        public class GradientShader : ShapeDrawable.ShaderFactory
        {
            string[] gradientColors;
            public GradientShader(String Gradient)
            {
                gradientColors = Gradient.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            }

            public override Shader Resize(int width, int height)
            {
                int[] colorList = new int[gradientColors.Count()];
                float[] offsetColor = new float[gradientColors.Count()];
                for (int i = 0; i < gradientColors.Count(); i++)
                {

                    var gradientColorOffset = gradientColors[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    colorList[i] = Android.Graphics.Color.ParseColor(gradientColorOffset[0]);
                    int offsetInt = Int32.Parse(gradientColorOffset[1].Replace("%", ""));
                    float offsetFloat = offsetInt / 100f;
                    offsetColor[i] = offsetFloat;
                }

                LinearGradient gradient = new LinearGradient(0, 0, 0, height, colorList, offsetColor, Shader.TileMode.Clamp);
                return gradient;
            }
        }

        public class ShadowShader : ShapeDrawable.ShaderFactory
        {
            RoundedButton element;
            public ShadowShader(RoundedButton element)
            {
                this.element = element;
            }
            public override Shader Resize(int width, int height)
            {
                LinearGradient lg = null;
                if (element.HasShadow)
                {
                    lg = new LinearGradient(0, 0, 0, height,
                            new int[] { Android.Graphics.Color.ParseColor("#50000000"),
                             Android.Graphics.Color.ParseColor("#50000000"),
                             Android.Graphics.Color.ParseColor("#50000000"),
                             Android.Graphics.Color.ParseColor("#50000000") },
                             new float[] { 0, 0.50f, 0.50f, 1 }, Shader.TileMode.Repeat);
                }
                else
                {
                    lg = new LinearGradient(0, 0, 0, height,
                        new int[] { Android.Graphics.Color.Transparent,
                             Android.Graphics.Color.Transparent,
                             Android.Graphics.Color.Transparent,
                             Android.Graphics.Color.Transparent },
                             new float[] { 0, 0.50f, 0.50f, 1 }, Shader.TileMode.Repeat);
                }

                return lg;
            }
        }



    }
}