using Base1902;
using Base1902.Controls;
using Xamarin.Forms;

namespace TwinTechsLib.Droid.TwinTechs.Droid.Helper
{
    public static class SizeExtensions
    {
        public static byte ConvertDoubleToByteColor(this double value)
        {
            return (byte)(value * 255);
        }

        public static int ConvertDPToPixels(this double value)
        {
            double scale = (double)Globals.ResolutionScale / 100d;
            return (int)(value * scale + 0.5d);
        }
    }

    public static class ResourceExtensions
    {
        public static int GetDrawableResource(this FileImageSource source)
        {
            return UIHelper.GetDrawableResource(source);
        }
    }
}