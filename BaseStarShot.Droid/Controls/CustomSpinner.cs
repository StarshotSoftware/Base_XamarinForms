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
    public class CustomSpinner : Spinner
    {
        public CustomSpinner(Context context)
            : base(context)
        {

        }

        public override bool PerformClick()
        {


            return base.PerformClick();
        }
    }
}