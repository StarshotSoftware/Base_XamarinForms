using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace BaseStarShot.Util
{
    public class FontCache
    {
        private static Dictionary<string, Typeface> fontDictionary = new Dictionary<string, Typeface>();

        public static Typeface GetTypeFace(string fontName)
        {
            if (FontCache.fontDictionary.ContainsKey(fontName))
            {
                Typeface typeFace;
                FontCache.fontDictionary.TryGetValue(fontName, out typeFace);
                return typeFace;
            }
            else
            {
                Typeface typeF = Typeface.CreateFromAsset(Forms.Context.Assets, fontName);
                FontCache.fontDictionary.Add(fontName, typeF);
                return typeF;
            }
        }
    }
}