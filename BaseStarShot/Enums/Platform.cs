using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public enum Platform
    {
        /// <summary>
        /// (Unused) Indicates that app is running on an undefined platform.
        /// </summary>
        Other = 0,
        /// <summary>
        /// Indicates that app is running on an Apple iOS OS.
        /// </summary>
        iOS = 1,
        /// <summary>
        /// Indicates that app is running on a Google Android OS.
        /// </summary>
        Android = 2,
        /// <summary>
        /// Indicates that app is running on a Microsoft WinPhone OS.
        /// </summary>
        WinPhone = 3,
        Windows = 4,
    }
}
