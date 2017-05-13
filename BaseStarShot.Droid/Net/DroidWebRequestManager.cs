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

namespace BaseStarShot.Net
{
    public class DroidWebRequestManager : HttpWebRequestManager
    {
        protected override void OnCreate(System.Net.HttpWebRequest request)
        {
            base.OnCreate(request);

            //request.AllowReadStreamBuffering = true;
            request.AllowWriteStreamBuffering = true;
            request.Timeout = 60000;
            request.ReadWriteTimeout = 120000;

            request.UserAgent = Globals.UserAgent;
        }
    }
}