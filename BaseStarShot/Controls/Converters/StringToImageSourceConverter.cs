using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null) {
                if (value is string && targetType == typeof(ImageSource))
                {
                    var val = (string)value;
                    var source = (ImageSource)val;
                    if (source == null)
                        return null;

                    if (source is UriImageSource){
                        UriImageSource imgSource = (UriImageSource)source;
                        imgSource.CachingEnabled = Globals.ImageCachedEnable;
                        imgSource.CacheValidity = Globals.ImageCachedValidity;
                        return imgSource;
                    }
                    return source;
                }
            }
           
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImageSource && targetType == typeof(string))
            {
                if (value is FileImageSource)
                {
                    return ((FileImageSource)value).File;
                }
                else if (value is UriImageSource)
                {
                    return ((UriImageSource)value).Uri.ToString();
                }
            }
            return null;
        }
    }
}
