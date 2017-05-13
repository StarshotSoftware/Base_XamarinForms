using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public interface IGlobals
    {
        /// <summary>
        /// The current resolution scale used by the device.
        /// </summary>
        ResolutionScale ResolutionScale { get; }

        /// <summary>
        /// The screen width.
        /// </summary>
        double ScreenHeight { get; }

        /// <summary>
        /// The screen height.
        /// </summary>
        double ScreenWidth { get; }

        /// <summary>
        /// The application name.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// The package name.
        /// </summary>
        string PackageName { get; }

        /// <summary>
        /// The package family name.
        /// </summary>
        string PackageFamilyName { get; }

        /// <summary>
        /// The version of the application.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The OS and version number.
        /// </summary>
        string OSVersion { get; }

        /// <summary>
        /// The model of the device.
        /// </summary>
        string Model { get; }

        /// <summary>
        /// The build of the device.
        /// </summary>
        string Build { get; }

        /// <summary>
        /// Ths manufacturer of the device.
        /// </summary>
        string Manufacturer { get; }

        /// <summary>
        /// The User-Agent string used for HTTP web requests.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// The documents folder of the app where user content could be placed.
        /// </summary>
        string DocumentsFolder { get; }

        /// <summary>
        /// The library folder of the app where system generated files could be placed.
        /// </summary>
        string LibraryFolder { get; }

        /// <summary>
        /// The databases folder of the app where database files could be placed.
        /// </summary>
        string DatabasesFolder { get; }

        /// <summary>
        /// Gets if application is running in the background.
        /// </summary>
        bool RunningInBackground { get; }

        /// <summary>
        /// Gets or sets the primary language to be used for resource files and formatting.
        /// </summary>
        string Language { get; set; }

		/// <summary>
		/// Gets or sets the default phone country code.
		/// </summary>
		string CountryCode { get; set; }

		/// <summary>
		/// Gets or sets the default phone country display name.
		/// </summary>
		string CountryDisplayName { get; set; }

		/// <summary>
		/// Gets or sets the device uniqueId
		/// </summary>
		string DeviceUniqueId { get; set; }

    }
}
