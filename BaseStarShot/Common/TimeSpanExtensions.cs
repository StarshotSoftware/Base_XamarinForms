using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts the TimeSpan object to its string represenation using DateTime formatting.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this TimeSpan timeSpan, string format)
        {
            return new DateTime().Add(timeSpan).ToString(format);
        }

        /// <summary>
        /// Converts the TimeSpan object to its string represenation using DateTime formatting.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this TimeSpan? timeSpan, string format)
        {
            return ToDateTimeString(timeSpan.GetValueOrDefault(), format);
        }
    }
}
