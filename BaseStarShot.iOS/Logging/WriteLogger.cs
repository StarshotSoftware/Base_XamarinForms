using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using Foundation;

using Xamarin.Forms;
using BaseStarShot.Logging;

namespace BaseStarShot.Logging
{
    class WriteLogger : IWriteLogger
    {
        public void Write(string tag, string message, LogType type)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0}| {1}: {2}", type, tag, message));
        }
    }
}