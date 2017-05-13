using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class Globals
    {
        private static IGlobals current;

        /// <summary>
        /// Checks if globals provider has been set.
        /// </summary>
        public static bool HasGlobals { get { return current != null; } }

        /// <summary>
        /// The current resolution scale used by the device.
        /// </summary>
        public static ResolutionScale ResolutionScale { get { return current.ResolutionScale; } }

        /// <summary>
        /// The screen height.
        /// </summary>
        public static double ScreenHeight { get { return current.ScreenHeight; } }

        /// <summary>
        /// The screen width.
        /// </summary>
        public static double ScreenWidth { get { return current.ScreenWidth; } }

        /// <summary>
        /// The screen height in density pixels.
        /// </summary>
        public static double LogicalScreenHeight { get { return (double)(current.ScreenHeight / ((double)current.ResolutionScale / 100d)); } }

        /// <summary>
        /// The screen width in density pixels.
        /// </summary>
        public static double LogicalScreenWidth { get { return (double)(current.ScreenWidth / ((double)current.ResolutionScale / 100d)); } }

        public static double LogicalScreenWidthDouble { get { return (double)(current.ScreenWidth / ((double)current.ResolutionScale / 100d)); } }

        /// <summary>
        /// The application name.
        /// </summary>
        public static string ApplicationName { get { return current.ApplicationName; } }

        /// <summary>
        /// The package name.
        /// </summary>
        public static string PackageName { get { return current.PackageName; } }

        /// <summary>
        /// The package family name.
        /// </summary>
		public static string PackageFamilyName { get { return current == null ? null : current.PackageFamilyName; } }

        /// <summary>
        /// The version of the application.
        /// </summary>
        public static Version Version { get { return current.Version; } }

        /// <summary>
        /// The OS and version number.
        /// </summary>
        public static string OSVersion { get { return current.OSVersion; } }

        /// <summary>
        /// The model of the device.
        /// </summary>
        public static string Model { get { return current.Model; } }

        /// <summary>
        /// The build of the device.
        /// </summary>
        public static string Build { get { return current.Build; } }

        /// <summary>
        /// Ths manufacturer of the device.
        /// </summary>
        public static string Manufacturer { get { return current.Manufacturer; } }

        /// <summary>
        /// The User-Agent string used for HTTP web requests.
        /// </summary>
        public static string UserAgent { get { return current.UserAgent; } }

        /// <summary>
        /// The documents folder of the app where user content could be placed.
        /// </summary>
        public static string DocumentsFolder { get { return current.DocumentsFolder; } }

        /// <summary>
        /// The library folder of the app where system generated files could be placed.
        /// </summary>
        public static string LibraryFolder { get { return current.LibraryFolder; } }

        /// <summary>
        /// The databases folder of the app where database files could be placed.
        /// </summary>
        public static string DatabasesFolder { get { return current.DatabasesFolder; } }

        /// <summary>
        /// The format used to encode System.DateTime in url or post data.
        /// </summary>
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Gets if application is running in the background.
        /// </summary>
        public static bool RunningInBackground { get { return current.RunningInBackground; } }

        private static SmtpInfo _smtp;
        /// <summary>
        /// The Smtp setup information.
        /// </summary>
        public static SmtpInfo Smtp { get { return _smtp ?? (_smtp = new SmtpInfo()); } }

        private static ErrorInfo _error;
        /// <summary>
        /// The error logging setup information.
        /// </summary>
        public static ErrorInfo Error { get { return _error ?? (_error = new ErrorInfo()); } }

        /// <summary>
        /// Gets or sets the primary language to be used for resource files and formatting.
        /// </summary>
        public static string Language { get { return current.Language; } set { current.Language = value; } }

		/// <summary>
		/// Gets or sets the primary language to be used for resource files and formatting.
		/// </summary>
		public static string CountryCode { get { return current.CountryCode; } set { current.CountryCode = value; } }

		/// <summary>
		/// Gets or sets the primary language to be used for resource files and formatting.
		/// </summary>
		public static string CountryDisplayName { get { return current.CountryDisplayName; } set { current.CountryDisplayName = value; } }

		/// <summary>
		/// Gets or sets the device unique identifier.
		/// </summary>
		/// <value>The device unique identifier.</value>
		public static string DeviceUniqueId { get { return current.DeviceUniqueId; } set { current.DeviceUniqueId = value; } }

        /// <summary>
        /// Sets the current globals provider.
        /// </summary>
        /// <param name="globals"></param>
        public static void SetGlobals(IGlobals globals)
        {
            current = globals;
        }

        /// <summary>
        /// Set InAppPurchase to SandBox Mode
        /// </summary>
        public static bool SandBoxInAppPurchase { get { 
            #if DEBUG 
            return true;
            #else
            return false;
            #endif
        }}

        /// <summary>
        /// Enable image caching
        /// </summary>
        public static bool ImageCachedEnable = true;

        /// <summary>
        /// TimeSpan of file to be cached 
        /// </summary>
        public static TimeSpan ImageCachedValidity =
#if DEBUG
 TimeSpan.FromSeconds(3600);
#else
        TimeSpan.FromDays(5);
#endif

    }
}
