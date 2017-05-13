using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot;
using Xamarin.Forms;

namespace BaseStarShot.Services
{
    /// <summary>
    /// Provides window services.
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// Gets the logical height of the navigation bar, this may change depending on the scale and orientation of the device.
        /// </summary>
        /// <returns></returns>
        double GetNavigationBarHeight();

        /// <summary>
        /// Gets the logical height of the software buttons bar.
        /// </summary>
        /// <returns></returns>
        double GetSoftwareButtonsBarHeight();

        /// <summary>
        /// Gets the logical width of the software buttons bar.
        /// </summary>
        /// <returns></returns>
        double GetSoftwareButtonsBarWidth();

        /// <summary>
        /// Gets the logical height of the status bar.
        /// </summary>
        /// <returns></returns>
        double GetStatusBarHeight();

        /// <summary>
        /// Gets the logical height of the bottom app bar
        /// </summary>
        /// <returns></returns>
        double GetBottomAppBarHeight();

        /// <summary>
        /// Gets the logical width of the status bar.
        /// </summary>
        /// <returns></returns>
        double GetStatusBarWidth();

        /// <summary>
        /// Fire visible changes on current screen
        /// </summary>
        /// <returns></returns>
        event EventHandler<Rect> OnVisibleScreenSizeChangedEvent;
        void OnVisibleSizeChanged(Rect rect);

    }
}
