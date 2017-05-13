using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class StreamToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value is Stream && targetType == typeof(ImageSource))
            {
                return ImageSource.FromStream(() => value as Stream);
            }
            else if (value is string && targetType == typeof(ImageSource))
            {
                var val = (string)value;
                return (ImageSource)val;
            }
            else if (value is Uri && targetType == typeof(ImageSource))
            {
                return (UriImageSource)((value as Uri).ToString());
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value is ImageSource && targetType == typeof(string))
            //{
            //    if (value is FileImageSource)
            //    {
            //        return ((FileImageSource)value).File;
            //    }
            //    else if (value is UriImageSource)
            //    {
            //        return ((UriImageSource)value).Uri.ToString();
            //    }
            //}
            return null;
        }
    }
}
