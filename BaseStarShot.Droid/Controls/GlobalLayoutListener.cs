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

namespace BaseStarShot.Controls
{
    public class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public event EventHandler GlobalLayout;

        public void OnGlobalLayout()
        {
            if (GlobalLayout != null)
                GlobalLayout(this, EventArgs.Empty);
        }
    }
}