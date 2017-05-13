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

namespace Base1902.Helpers
{
    public class InfoWindowClickListener : Object, Android.Gms.Maps.GoogleMap.IOnInfoWindowClickListener
    {
        public event EventHandler<Android.Gms.Maps.Model.Marker> InfoWindowClicked = delegate
        {
        };

        public void OnInfoWindowClick(Android.Gms.Maps.Model.Marker p0)
        {
            InfoWindowClicked(this, p0);
        }

        public IntPtr Handle
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}