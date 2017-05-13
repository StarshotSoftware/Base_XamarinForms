using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using Foundation;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace BaseStarShot.Net
{
    public class iOSWebRequestManager : HttpWebRequestManager
    {
		private volatile int requestCount = 0;
		
        protected override void OnCreate(System.Net.HttpWebRequest request)
        {
            base.OnCreate(request);

            //request.AllowReadStreamBuffering = true;
            request.AllowWriteStreamBuffering = true;
			request.Timeout = 60000;
			request.ReadWriteTimeout = 120000;

            request.UserAgent = Globals.UserAgent;
        }

		public override async Task<Response> GetResponseAsync (HttpWebRequest webRequest, object data)
		{
			Interlocked.Increment (ref requestCount);
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			try
			{
				return await base.GetResponseAsync (webRequest, data);
			}
			finally {
				if (Interlocked.Decrement (ref requestCount) == 0) {
					UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				}
			}
		}
    }
}