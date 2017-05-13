using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    /// <summary>
    /// Specifes which method to use when serializing GET request parameters.
    /// </summary>
    public enum GETRequestSerialization
    {
        /// <summary>
        /// Serializes request data as http://your.domain.com/?{key1}={value1}&{key2}={value2}...
        /// </summary>
        QueryString,
        /// <summary>
        /// Serializes request data as http://your.domain.com/{key1}/{value1}/{key2}/{value2}...
        /// </summary>
        SlashDelimited
    }
}
