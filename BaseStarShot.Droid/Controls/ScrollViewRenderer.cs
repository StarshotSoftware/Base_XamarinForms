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
using Android;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Support.V4.Widget;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.ScrollView), typeof(BaseStarShot.Controls.ScrollViewRenderer))]
namespace BaseStarShot.Controls
{

    public class ScrollViewRenderer : Xamarin.Forms.Platform.Android.ScrollViewRenderer
    {

        protected ScrollView BaseControl { get { return ((BaseStarShot.Controls.ScrollView)this.Element); } }
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            if (!BaseControl.HasScrollbar)
            {
                this.HorizontalScrollBarEnabled = false;
                this.VerticalScrollBarEnabled = false;
            }

        }

      
    }
}