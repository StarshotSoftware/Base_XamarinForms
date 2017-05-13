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
    public class BackgroundManagerBinder : Android.OS.Binder
    {
        public BackgroundManagerService Service { get; protected set; }

        public bool IsBound { get; set; }

        public BackgroundManagerBinder(BackgroundManagerService service)
        {
            this.Service = service;
        }
    }
}