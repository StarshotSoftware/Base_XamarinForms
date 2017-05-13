using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BaseStarShot.Controls
{
    public static partial class UIHelper
    {
        public static UIControlContentHorizontalAlignment ConvertToHorizontalAlignment(Xamarin.Forms.TextAlignment alignment)
        {
            switch (alignment)
            {
                default:
                case Xamarin.Forms.TextAlignment.Center: return UIControlContentHorizontalAlignment.Center;
                case Xamarin.Forms.TextAlignment.Start: return UIControlContentHorizontalAlignment.Left;
                case Xamarin.Forms.TextAlignment.End: return UIControlContentHorizontalAlignment.Right;
            }
        }

        public static UIControlContentVerticalAlignment ConvertToVerticalAlignment(Xamarin.Forms.TextAlignment alignment)
        {
            switch (alignment)
            {
                default:
                case Xamarin.Forms.TextAlignment.Center: return UIControlContentVerticalAlignment.Center;
                case Xamarin.Forms.TextAlignment.Start: return UIControlContentVerticalAlignment.Top;
                case Xamarin.Forms.TextAlignment.End: return UIControlContentVerticalAlignment.Bottom;
            }
        }
    }
}