using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class StringToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string && targetType == typeof(FormattedString))
            {
                var val = (string)value;
                if (string.IsNullOrEmpty(val))
                    return null;
                return new FormattedString
                {
                    Spans =
                        {
                            new Span { Text = val }
                        }
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
