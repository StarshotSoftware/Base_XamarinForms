using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls.Converters
{
	public class StringToIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is int && targetType == typeof(int))
				return value.ToString();
			else if (value is int? && targetType == typeof(int?))
			{
				if (((int?)value).HasValue)
					value.ToString();
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				if (targetType == typeof(int?))
					return null;

				return 0;
			}

			if (value is string)
			{
				if (targetType == typeof(int?))
				{
					int intValue;
					if (int.TryParse((string)value, out intValue))
						return intValue;

					return null;
				} 
				else if (targetType == typeof(int))
				{
					int intValue;
					if (int.TryParse((string)value, out intValue))
						return intValue;

					return 0;
				}
			}

			return value;
		}
	}
}

