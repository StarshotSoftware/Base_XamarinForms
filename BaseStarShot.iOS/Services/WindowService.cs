using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BaseStarShot.Services
{
    public class WindowService : IWindowService
    {
        public double GetNavigationBarHeight()
        {
            return 44;
        }

        public double GetSoftwareButtonsBarHeight()
        {
            return 0;
        }

        public double GetSoftwareButtonsBarWidth()
        {
            return 0;
        }

        public double GetStatusBarHeight()
        {
            return 20;
        }

        public double GetStatusBarWidth()
        {
            return 0;
        }

		public double GetBottomAppBarHeight()
		{
			return 0;
		}

        public event EventHandler<Rect> OnVisibleScreenSizeChangedEvent;
        public void OnVisibleSizeChanged(Rect rect)
        {
            if (OnVisibleScreenSizeChangedEvent != null)
            {
                OnVisibleScreenSizeChangedEvent(this, rect);
            }
        }
    }
}