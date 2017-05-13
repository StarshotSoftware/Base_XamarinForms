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
using Android.Graphics.Drawables;

namespace BaseStarShot.Controls.ImageHelper
{
    public class UrlImageCache : SoftReferenceHashTable<string, Drawable>
    {
        static UrlImageCache instance;

        public static UrlImageCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new UrlImageCache();

                return instance;
            }
        }


    }
}