using System;
using System.Runtime.InteropServices;
using UIKit;

namespace BaseStarShot
{
	public static class DeviceHardware
	{
		public const string HardwareProperty = "hw.machine";

		[DllImport ("libc", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property, // name of the property
			IntPtr output, // output
			IntPtr oldLen, // IntPtr.Zero
			IntPtr newp, // IntPtr.Zero
			uint newlen // 0
		);
		public static string Version
		{
			get
			{
				try 
				{
					// get the length of the string that will be returned
					var pLen = Marshal.AllocHGlobal(sizeof(int));
					sysctlbyname(HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

					var length = Marshal.ReadInt32(pLen);

					// check to see if we got a length
					if (length == 0)
					{
						Marshal.FreeHGlobal(pLen);
						return "Unknown";
					}

					// get the hardware string
					var pStr = Marshal.AllocHGlobal(length);
					sysctlbyname(HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

					// convert the native string into a C# string
					var hardwareStr = Marshal.PtrToStringAnsi(pStr);

					// cleanup
					Marshal.FreeHGlobal(pLen);
					Marshal.FreeHGlobal(pStr);

					return hardwareStr;
				} 
				catch (Exception ex) 
				{
					Console.WriteLine("DeviceHardware.Version Ex: " + ex.Message);
				}

				return "Unknown";
			}
		}

		public static HardwareVersion GetHardwareVersion(string version)
		{
			HardwareVersion ret = HardwareVersion.Unknown;
			switch (version) {
			case "iPhone1,1":
				ret = HardwareVersion.iPhone;
				break;
			case "iPhone1,2":
				ret = HardwareVersion.iPhone3G;
				break;
			case "iPhone2,1":
				ret = HardwareVersion.iPhone3GS;
				break;
			case "iPhone3,1":
			case "iPhone3,2":
			case "iPhone3,3":
				ret = HardwareVersion.iPhone4;
				break;
			case "iPhone4,1":
				ret = HardwareVersion.iPhone4S;
				break;
			case "iPhone5,1":
			case "iPhone5,2":
				ret = HardwareVersion.iPhone5;
				break;
			case "iPhone5,3":
			case "iPhone5,4":
				ret = HardwareVersion.iPhone5C;
				break;
			case "iPhone6,1":
			case "iPhone6,2":
				ret = HardwareVersion.iPhone5S;
				break;
			case "iPhone7,2":
				ret = HardwareVersion.iPhone6;
				break;
			case "iPhone7,1":
				ret = HardwareVersion.iPhone6Plus;
				break;
			case "iPhone8,1":
				ret = HardwareVersion.iPhone6S;
				break;
			case "iPhone8,2":
				ret = HardwareVersion.iPhone6SPlus;
				break;
			case "iPhone8,4":
				ret = HardwareVersion.iPhoneSE;
				break;
            case "iPhone9,1":
            case "iPhone9,3":
                ret = HardwareVersion.iPhone7;
                break;
            case "iPhone9,2":
            case "iPhone9,4":
                ret = HardwareVersion.iPhone7Plus;
                break;
            case "iPad1,1":
				ret = HardwareVersion.iPad;
				break;
			case "iPad2,1":
			case "iPad2,2":
			case "iPad2,3":
			case "iPad2,4":
				ret = HardwareVersion.iPad2;
				break;
            case "iPad2,5":
            case "iPad2,6":
            case "iPad2,7":
                ret = HardwareVersion.iPadMini;
                break;
            case "iPad3,1":
			case "iPad3,2":
			case "iPad3,3":
				ret = HardwareVersion.iPad3;
				break;
			case "iPad3,4":
			case "iPad3,5":
			case "iPad3,6":
				ret = HardwareVersion.iPad4;
				break;
			case "iPad4,1":
			case "iPad4,2":
			case "iPad4,3":
				ret = HardwareVersion.iPadAir;
				break;
			case "iPad5,3":
			case "iPad5,4":
				ret = HardwareVersion.iPadAir2;
				break;
            case "iPad6,3":
            case "iPad6,4":
            case "iPad6,7":
			case "iPad6,8":
				ret = HardwareVersion.iPadPro;
				break;			
			case "iPad4,4":
			case "iPad4,5":
			case "iPad4,6":
				ret = HardwareVersion.iPadMini2;
				break;
			case "iPad4,7":
			case "iPad4,8":
			case "iPad4,9":
				ret = HardwareVersion.iPadMini3;
				break;
			case "iPad5,1":
			case "iPad5,2":
				ret = HardwareVersion.iPadMini4;
				break;
			case "iPod1,1":
				ret = HardwareVersion.iPod1G;
				break;
			case "iPod2,1":
				ret = HardwareVersion.iPod2G;
				break;
			case "iPod3,1":
				ret = HardwareVersion.iPod3G;
				break;
			case "iPod4,1":
				ret = HardwareVersion.iPod4G;
				break;
			case "iPod5,1":
				ret = HardwareVersion.iPod5G;
				break;
			case "iPod7,1":
				ret = HardwareVersion.iPod6G;
				break;
            case "AppleTV5,3":
                ret = HardwareVersion.AppleTV;
                break;
            case "i386":
			case "x86_64":
				if (UIDevice.CurrentDevice.Model.Contains ("iPhone"))
					ret = HardwareVersion.iPhoneSimulator;
				else
					ret = HardwareVersion.iPadSimulator;
				break;
			}

			return ret;
		}

		public static string GetDisplayVersion(HardwareVersion hVersion, string version)
		{
			switch (hVersion) {
			case HardwareVersion.iPhone:
				return "iPhone";
			case HardwareVersion.iPhone3G:
				return "iPhone 3G";
			case HardwareVersion.iPhone3GS:
				return "iPhone 3GS";
			case HardwareVersion.iPhone4:
				return "iPhone 4";
			case HardwareVersion.iPhone4S:
				return "iPhone 4S";
			case HardwareVersion.iPhone5:
				return "iPhone 5";
            case HardwareVersion.iPhone5C:
                return "iPhone 5C";
            case HardwareVersion.iPhone5S:
				return "iPhone 5S";
			case HardwareVersion.iPhone6:
				return "iPhone 6";
			case HardwareVersion.iPhone6Plus:
				return "iPhone 6 Plus";
			case HardwareVersion.iPhone6S:
				return "iPhone 6S";
			case HardwareVersion.iPhone6SPlus:
				return "iPhone 6S Plus";
			case HardwareVersion.iPhoneSE:
				return "iPhone SE";
            case HardwareVersion.iPhone7:
                return "iPhone 7";
            case HardwareVersion.iPhone7Plus:
                return "iPhone 7 Plus";
            case HardwareVersion.iPad:
			    return "iPad";
			case HardwareVersion.iPad2:
				return "iPad 2";
			case HardwareVersion.iPad3:
				return "iPad 3";
			case HardwareVersion.iPad4:
				return "iPad 4";
			case HardwareVersion.iPadAir:
				return "iPad Air";
			case HardwareVersion.iPadAir2:
				return "iPad Air 2";
			case HardwareVersion.iPadPro:
				return "iPad Pro";
			case HardwareVersion.iPadMini:
				return "iPad Mini";
			case HardwareVersion.iPadMini2:
				return "iPad Mini 2";
			case HardwareVersion.iPadMini3:
				return "iPad Mini 3";
			case HardwareVersion.iPadMini4:
				return "iPad Mini 4";
			case HardwareVersion.iPod1G:
				return "iPod Touch";
			case HardwareVersion.iPod2G:
				return "iPod Touch 2G";
			case HardwareVersion.iPod3G:
				return "iPod Touch 3G";
			case HardwareVersion.iPod4G:
				return "iPod Touch 4G";
			case HardwareVersion.iPod5G:
				return "iPod Touch 5G";
			case HardwareVersion.iPod6G:
				return "iPod Touch 6G";
            case HardwareVersion.AppleTV:
                return "Apple TV";
            case HardwareVersion.iPhoneSimulator:
				return "iPhone Simulator";
			case HardwareVersion.iPadSimulator:
				return "iPad Simulator";
			default:
				return version;
			}
		}
	}
}

