using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public abstract class CommonGlobals : IGlobals
    {
        public virtual ResolutionScale ResolutionScale { get; set; }

        public virtual double ScreenHeight { get; set; }

        public virtual double ScreenWidth { get; set; }

        public virtual string ApplicationName { get; set; }

        public virtual string PackageName { get; set; }

        public virtual string PackageFamilyName { get; set; }

        public virtual Version Version { get; set; }

        public virtual string OSVersion { get; set; }

        public virtual string Model { get; set; }

        public virtual string Build { get; set; }

        public virtual string Manufacturer { get; set; }

        public virtual string UserAgent { get; set; }

        public virtual string DocumentsFolder { get; set; }

        public virtual string LibraryFolder { get; set; }

        public virtual string DatabasesFolder { get; set; }

		public virtual string CountryCode { get; set; }

		public virtual string CountryDisplayName { get; set; }

		public virtual string DeviceUniqueId { get; set; }

        public virtual bool RunningInBackground { get; set; }

        public virtual string Language
        {
            get
            {
                if (System.Globalization.CultureInfo.DefaultThreadCurrentCulture == null)
                {
                    System.Globalization.CultureInfo ci;
                    try
                    {
                        ci = new System.Globalization.CultureInfo(GetDefaultLanguage());
                    }
                    catch (System.Globalization.CultureNotFoundException)
                    {
                        ci = new System.Globalization.CultureInfo("en-US");
                    }
                    System.Globalization.CultureInfo.DefaultThreadCurrentUICulture =
                            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = ci;
                }
                return System.Globalization.CultureInfo.DefaultThreadCurrentCulture.Name;
            }
            set
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture =
                    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(value);
            }
        }

	

		public virtual string GetDefaultLanguage()
        {
            return "en-US";
        }
    }
}
