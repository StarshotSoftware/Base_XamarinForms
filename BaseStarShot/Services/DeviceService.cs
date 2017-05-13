using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class DeviceService : IDeviceService
    {
        public Platform OS { get { return (Platform)Xamarin.Forms.Device.OS; } }

        public virtual bool NetworkActivityIndicatorVisible { get; set; }


        public Idiom Idiom { get { return (Idiom)Xamarin.Forms.Device.Idiom; } }

        public void OpenUri(Uri uri)
        {
            Xamarin.Forms.Device.OpenUri(uri);
        }
    }
}
