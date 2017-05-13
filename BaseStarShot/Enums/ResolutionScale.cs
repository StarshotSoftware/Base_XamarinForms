using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public enum ResolutionScale
    {
        /// <summary>
        /// Specifies the scale of a display is invalid.
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Android ldpi.
        /// </summary>
        Scale75Percent = 75,
        /// <summary>
        /// Specifies the scale of a display as 100 percent. This percentage indicates
        /// that the display pixel density is less than 174 pixels per inch (DPI/PPI),
        /// or the screen resolution is less than 1920x1080, or both.
        /// Android mdpi.
        /// </summary>
        Scale100Percent = 100,
        /// <summary>
        /// This value isn't used.
        /// </summary>
        Scale120Percent = 120,
        /// <summary>
        /// Specifies the scale of a display as 140 percent. This percentage indicates
        /// that the display has a pixel density greater than or equal to 174 PPI and
        /// a screen resolution greater than or equal to 1920x1080.
        /// </summary>
        Scale140Percent = 140,
        /// <summary>
        /// Windows Phone only. Specifies the scale of a display as 150 percent. This
        /// percentage indicates that a phone display has a resolution of 1280x720.
        /// Android hdpi.
        /// </summary>
        Scale150Percent = 150,
        /// <summary>
        /// Windows Phone only. Specifies the scale of a display as 160 percent. This
        /// percentage indicates that a phone display has a resolution of 1280x768.
        /// </summary>
        Scale160Percent = 160,
        /// <summary>
        /// Specifies the scale of a display as 180 percent. This percentage indicates
        /// that the display has a pixel density greater than or equal to 240 PPI and
        /// a screen resolution greater than or equal to 2560x1440.
        /// </summary>
        Scale180Percent = 180,
        /// <summary>
        /// Android xhdpi. iOS Retina @2x.
        /// </summary>
        Scale200Percent = 200,
        /// <summary>
        /// Windows Phone 1920x1080.
        /// </summary>
        Scale225Percent = 225,
        /// <summary>
        /// Android xxhdpi, iOS Retina @3x.
        /// </summary>
        Scale300Percent = 300,
        /// <summary>
        /// Android xxxhdpi.
        /// </summary>
        Scale400Percent = 400
    }
}
