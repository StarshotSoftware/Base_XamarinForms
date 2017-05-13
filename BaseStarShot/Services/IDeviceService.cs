using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IDeviceService
    {
        /// <summary>
        /// Gets the OS the app is working on.
        /// </summary>
        Platform OS { get; }

        /// <summary>
        /// Gets the Idiom the app is working on.
        /// </summary>
        Idiom Idiom { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BaseStarShot.Services.IDeviceService"/> network activity
        /// indicator is visible.
        /// </summary>
        /// <value><c>true</c> if network activity indicator visible; otherwise, <c>false</c>.</value>
        bool NetworkActivityIndicatorVisible { get; set; }

        /// <summary> 
        /// Request the device to open the Uri.
        /// </summary>
        /// <param name="uri"></param>
        void OpenUri(Uri uri);
    }
}
