using Android.Content.Res;
using Android.Support.V4.Content;
using Xamarin.Forms;
using AColor = Android.Graphics.Color;

namespace BaseStarShot.Controls.Extensions
{
    public static class ColorExtensions
    {
        public static readonly int[][] States = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };

        public static AColor ToAndroid(this Color self)
        {
            return new AColor((byte)(byte.MaxValue * self.R), (byte)(byte.MaxValue * self.G), (byte)(byte.MaxValue * self.B), (byte)(byte.MaxValue * self.A));
        }

        public static AColor ToAndroid(this Color self, int defaultColorResourceId)
        {
            if (self == Color.Default)
            {
                return new AColor(ContextCompat.GetColor(Forms.Context, defaultColorResourceId));
            }

            return ToAndroid(self);
        }

        public static AColor ToAndroid(this Color self, Color defaultColor)
        {
            if (self == Color.Default)
                return defaultColor.ToAndroid();

            return ToAndroid(self);
        }

        public static ColorStateList ToAndroidPreserveDisabled(this Color color, ColorStateList defaults)
        {
            int disabled = defaults.GetColorForState(States[1], color.ToAndroid());
            return new ColorStateList(States, new[] { color.ToAndroid().ToArgb(), disabled });
        }
    }
}