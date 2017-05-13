using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using BaseStarShot.Logging;

namespace BaseStarShot.Logging
{
    class WriteLogger : IWriteLogger
    {
        public void Write(string tag, string message, LogType type)
        {
            switch (type)
            {
                case LogType.Info: Android.Util.Log.Info(tag, message); break;
                case LogType.Debug: Android.Util.Log.Debug(tag, message); break;
                case LogType.Error: Android.Util.Log.Error(tag, message); break;
            }
        }
    }
}