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

namespace BaseStarShot.Services
{
    public class HttpBackgroundManagerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        IHttpBackgroundManagerClient client;

        public event EventHandler ServiceConnected = delegate { };
        public event EventHandler ServiceDisconnected = delegate { };

        public HttpBackgroundManagerServiceConnection(IHttpBackgroundManagerClient client)
        {
            if (client != null)
                this.client = client;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as HttpBackgroundManagerBinder;
            if (serviceBinder != null)
            {
                this.client.BackgroundBinder = serviceBinder;
                this.client.BackgroundBinder.IsBound = true;

                ServiceConnected(this, EventArgs.Empty);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            ServiceDisconnected(this, EventArgs.Empty);
            this.client.BackgroundBinder.IsBound = false;
        }
    }
}