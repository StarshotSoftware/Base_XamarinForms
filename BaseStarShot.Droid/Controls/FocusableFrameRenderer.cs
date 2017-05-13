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

[assembly: ExportRenderer(typeof(FocusableFrame), typeof(FocusableFrameRenderer))]
namespace BaseStarShot.Controls
{
    public class FocusableFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);

            this.Focusable = true;
            this.FocusableInTouchMode = true;

            this.FocusChange += FocusableFrameRenderer_FocusChange;
        }

        void FocusableFrameRenderer_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            Element.Raise("Focused", new FocusEventArgs(Element, e.HasFocus));
        }
    }
}