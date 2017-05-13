using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class StringExtensions
    {
        /// <summary>
        /// Encodes special characters to html
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHtml(this string s)
        {
            string[] common = { "Æ", "Ø", "Å", "Ö", "æ", "ø", "å", "ö" };
            string[] converted = { "&AElig;", "&Oslash;", "&Aring;", "&Ouml;", "&aelig;", "&oslash;", "&aring;", "&ouml;" };

            for (int i = 0; i < common.Length; i++)
            {
                s = s.Replace(common[i], converted[i]);
            }

            return s;
        }
    }
}
