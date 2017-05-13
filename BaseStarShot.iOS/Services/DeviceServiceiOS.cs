using System;

using UIKit;
namespace BaseStarShot.Services
{
	public class DeviceServiceiOS : DeviceService
	{
		public DeviceServiceiOS ()
		{
		}

		public override bool NetworkActivityIndicatorVisible {
			get {
				return UIApplication.SharedApplication.NetworkActivityIndicatorVisible;
			}
			set {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
			}
		}
	}
}

