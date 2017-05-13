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
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using BaseStarShot.Controls;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Switch), typeof(SwitchControlRenderer))]
namespace BaseStarShot.Controls
{
    using Xamarin.Forms.Platform.Android;

    public class SwitchControlRenderer : Xamarin.Forms.Platform.Android.SwitchRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Switch> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            var customSwitch = this.Element as Switch;

            var control = base.Control;
            control.TextOn = customSwitch.TextOn;
            control.TextOff = customSwitch.TextOff;

            if (customSwitch.TrackDrawable != null && customSwitch.TrackDrawable.File != null)
                control.TrackDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource(customSwitch.TrackDrawable));

            if (customSwitch.ThumbDrawable != null && customSwitch.ThumbDrawable.File != null)
                control.ThumbDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource(customSwitch.ThumbDrawable));

        }

        //protected override void OnElementChanged(ElementChangedEventArgs<CustomSwitch> e)
        //{
        //    base.OnElementChanged(e);
        //    if (e.OldElement != null || this.Element == null)
        //    {
        //        return;
        //    }

        //    var customSwitch = this.Element as CustomSwitch;

        //    //var control = new Switch(Forms.Context)
        //    //{
        //    //    TextOn = customSwitch.TextOn,
        //    //    TextOff = customSwitch.TextOff,
        //    //    TrackDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource("custom_switch_track")),
        //    //    ThumbDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource("custom_switch_thumb")),
        //    //    SwitchMinWidth = 200,
        //    //};
        //    var control = base.Control;
        //    control.TextOn = customSwitch.TextOn;
        //    control.TextOff = customSwitch.TextOff;

        //    if (customSwitch.TrackDrawable != null && customSwitch.TrackDrawable.File != null)
        //        control.TrackDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource(customSwitch.TrackDrawable));

        //    if (customSwitch.ThumbDrawable != null && customSwitch.ThumbDrawable.File != null)
        //        control.ThumbDrawable = Resources.GetDrawable(UIHelper.GetDrawableResource(customSwitch.ThumbDrawable));

        //    //this.SetNativeControl(control);
        //}

    }
}