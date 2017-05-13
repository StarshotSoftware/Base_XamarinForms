using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Util;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.GradientView), typeof(BaseStarShot.Controls.GradientViewControlRenderer))]
namespace BaseStarShot.Controls
{
    public class GradientViewControlRenderer : ImageRenderer
    {
        protected BaseStarShot.Controls.GradientView BaseControl { get { return ((BaseStarShot.Controls.GradientView)this.Element); } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);

            int startColor = BaseControl.StartColor.ToAndroid().ToArgb();
            int endColor = BaseControl.EndColor.ToAndroid().ToArgb();

            int[] colors = { startColor, endColor };
            //float[] radii = { 5, 5, 5, 5, 5, 5, 5, 5 };

            GradientDrawable.Orientation orientation = null;
            Android.Graphics.Drawables.GradientType gradientType;

            switch (BaseControl.Orientation)
            {
                case GradientOrientation.BottomTop: orientation = GradientDrawable.Orientation.BottomTop; break;
                case GradientOrientation.LeftRight: orientation = GradientDrawable.Orientation.LeftRight; break;
                case GradientOrientation.RightLeft: orientation = GradientDrawable.Orientation.RightLeft; break;
                case GradientOrientation.TopBottom: orientation = GradientDrawable.Orientation.TopBottom; break;
                default: orientation = GradientDrawable.Orientation.TopBottom; break;
            }

            switch (BaseControl.GradientStyle)
            {
                case GradientType.Linear: gradientType = Android.Graphics.Drawables.GradientType.LinearGradient; break;
                case GradientType.Sweep: gradientType = Android.Graphics.Drawables.GradientType.SweepGradient; break;
                case GradientType.Radial: gradientType = Android.Graphics.Drawables.GradientType.RadialGradient; break;
                default: gradientType = gradientType = Android.Graphics.Drawables.GradientType.LinearGradient; break;
            }

            GradientDrawable drawable = new GradientDrawable(orientation, colors);
            drawable.SetGradientType(gradientType);
            //drawable.SetCornerRadii(radii);
            //drawable.SetStroke(1, Android.Graphics.Color.ParseColor("#FF000000"));
            drawable.SetCornerRadius(0f);
            this.Control.SetBackgroundDrawable(drawable);


        }

        private int GetColor(string stringColor)
        {
            try
            {
                return Android.Graphics.Color.ParseColor(stringColor);
            }
            catch (Exception)
            {
                return Android.Graphics.Color.ParseColor("#B3000000");
            }
        }

    }
}