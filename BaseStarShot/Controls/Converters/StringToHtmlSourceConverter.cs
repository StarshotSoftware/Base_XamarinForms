using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	public class StringToHtmlSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null) {
				if (value is string && targetType == typeof(WebViewSource))
				{
					var val = (string)value;
					var source = new HtmlWebViewSource ();
					source.Html = val;
					return source;
				}
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is HtmlWebViewSource && targetType == typeof(string))
			{
				HtmlWebViewSource source = value as HtmlWebViewSource;
				return source.Html;
			}
			return null;
		}
	}
}

