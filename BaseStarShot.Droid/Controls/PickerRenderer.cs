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
using Android.Text;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Picker), typeof(BaseStarShot.Controls.PickerRenderer))]
namespace BaseStarShot.Controls
{
    public class PickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
        protected BaseStarShot.Controls.Picker Base { get { return (BaseStarShot.Controls.Picker)Element; } }
        protected Android.Widget.EditText MyControl { get { return (this.Control); } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);

            MyControl.SetTextColor(Android.Graphics.Color.White);
            MyControl.SetBackgroundDrawable(this.Resources.GetDrawable("button_white_outline"));
            MyControl.SetCompoundDrawablesWithIntrinsicBounds(null, null, this.Resources.GetDrawable("dropdown"), null);
        }
    }
}