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
using BaseStarShot.Controls;
using Android.Graphics.Drawables;

using Android.Views.Animations;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.ActivityIndicator), typeof(ActivityIndicatorControlRenderer))]
namespace BaseStarShot.Controls
{
    public class ActivityIndicatorControlRenderer : ActivityIndicatorRenderer
    {
        static Android.Graphics.Drawables.Drawable DefaultImage;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ActivityIndicator> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;

            var control = (BaseStarShot.Controls.ActivityIndicator)Element;

            control.PropertyChanged += (s, ev) =>
            {
                switch (ev.PropertyName)
                {
                    case "Image": SetImage(control); break;
                }
            };

            if (control.Image != null && control.Image.File != null)
                SetImage(control);
        }

        void SetImage(BaseStarShot.Controls.ActivityIndicator element)
        {
            if (DefaultImage != null)
                DefaultImage = Control.IndeterminateDrawable;

            if (element.Image != null)
            {
                var resourceId = UIHelper.GetDrawableResource(element.Image);
                if (resourceId > 0)
                {
                    //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                    //{
                    //    RotateDrawable rotateDrawable = new RotateDrawable();
                    //    rotateDrawable.Drawable = Resources.GetDrawable(UIHelper.GetDrawableResource(element.Image));
                    //    rotateDrawable.FromDegrees = 0;
                    //    rotateDrawable.ToDegrees = 1280;
                    //    rotateDrawable.PivotXRelative = true;
                    //    rotateDrawable.PivotX = 0.50f;
                    //    rotateDrawable.PivotYRelative = true;
                    //    rotateDrawable.PivotY = 0.50f;

                    //    Control.IndeterminateDrawable = rotateDrawable;
                    //}
                    //else
                    {
                        Control.IndeterminateDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource(element.Image));

                        var rotateAnimation = new RotateAnimation(0f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
                        rotateAnimation.Interpolator = new LinearInterpolator();
                        rotateAnimation.RepeatCount = RotateAnimation.Infinite;
                        rotateAnimation.Duration = 1000;
                        Control.Animation = rotateAnimation;
                        Control.StartAnimation(rotateAnimation);
                    }
                }
                else
                {
                    Control.IndeterminateDrawable = DefaultImage;
                }
            }
            else
            {
                Control.IndeterminateDrawable = DefaultImage;
            }
        }
    }
}