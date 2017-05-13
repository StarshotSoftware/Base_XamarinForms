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
using Android.Util;
using Android.Content.Res;

namespace TwinTechsLib.Droid.TwinTechs.Droid.Controls
{
    public static class UnitsExtensions
    {
        public static int DpToPixels(this double dp)
        {
            DisplayMetrics metrics = Resources.System.DisplayMetrics;
            double px = dp * ((float)metrics.DensityDpi / 160f);
            return (int)Math.Floor(px);
        }

        public static int PixelsToDp(this double px)
        {
            if (px == 0d)
                return 0;

            DisplayMetrics metrics = Resources.System.DisplayMetrics;
            double dp = px / ((float)metrics.DensityDpi / 160f);
            return (int)Math.Floor(dp);
        }

        public static int DpToPixels(this int dp)
        {
            return DpToPixels((double)dp);
        }

        public static int PixelsToDp(this int px)
        {
            return PixelsToDp((double)px);
        }
    }
}