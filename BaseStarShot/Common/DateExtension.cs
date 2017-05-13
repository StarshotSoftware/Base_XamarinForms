using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class DateExtension
    {
        private static DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToMilliseconds(this DateTime dateTime)
        {
            return (long)(dateTime - _unixEpoch).TotalMilliseconds;
        }

        public static long ToMilliseconds(this TimeSpan timeSpan)
        {
            return (long)timeSpan.TotalMilliseconds;
        }

        public static DateTime ToUniversalDateTime(this long milliseconds)
        {
            return _unixEpoch.AddMilliseconds(milliseconds);
        }

        public static TimeSpan ToUniversalTimeSpan(this long milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

		public static DateTime? ToNullableDateTime( this DateTime value )
		{
			if ( value == DateTime.MinValue )
			{
				return null;
			}
			else
			{
				return value;
			}
		}
    }
}
