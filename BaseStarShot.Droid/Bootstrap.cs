using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using BaseStarShot.Net;
using BaseStarShot.Services;
using BaseStarShot.Logging;
using Android.Util;
using BaseStarShot.Services.Media;
using PushSharp.Client;
using Base1902;
using Base1902.Dependencies;
using Base1902.Security;
using BaseStarShot.Security.RNCryptor;

namespace BaseStarShot
{
    public class Bootstrap
    {
        static AndroidGlobals globals;

        public static void Init(Context context)
        {
            Init(context, Resolver.Current);
        }

        public static void Init(Context context, IDependencyResolver resolver)
        {
            BootstrapBase.Init(resolver);

            if (!Globals.HasGlobals)
            {
                globals = new AndroidGlobals();
                var display = context.Resources.DisplayMetrics;
                int scaleFactor = (int)(display.Density * 100);
                globals.ResolutionScale = (ResolutionScale)scaleFactor;

                IWindowManager windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                double scale = ((double)scaleFactor / 100d);
                DisplayMetrics metrics = new DisplayMetrics();
                windowManager.DefaultDisplay.GetRealMetrics(metrics);

                globals.ScreenHeight = metrics.HeightPixels; //display.HeightPixels;
                globals.ScreenWidth = metrics.WidthPixels; //display.WidthPixels;
                Android.Content.PM.PackageInfo pInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
                globals.PackageName = context.PackageName;
                globals.PackageFamilyName = context.PackageName;
                globals.ApplicationName = context.PackageManager.GetApplicationLabel(pInfo.ApplicationInfo);
                globals.Version = new System.Version(pInfo.VersionName);
                globals.OSVersion = "Android " + Android.OS.Build.VERSION.Release;
                globals.Model = Android.OS.Build.Model;
                globals.Build = Android.OS.Build.Display;
                globals.Manufacturer = Capitalize(Android.OS.Build.Manufacturer);
                globals.UserAgent = string.Format("{0}/{1} (Linux; {2}; {3} {4}/{5}) {6}", globals.ApplicationName,
                    globals.Version, globals.OSVersion, globals.Manufacturer, globals.Model, globals.Build, Xamarin.Forms.Device.Idiom);

                globals.LibraryFolder = globals.DocumentsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                globals.CountryCode = GetCountryCode();
                Globals.SetGlobals(globals);

                BaseStarShot.Repositories.BaseRepository.SqlitePlatform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();

                LifecycleEvents.Sleep += LifecycleEvents_Sleep;
                LifecycleEvents.Resume += LifecycleEvents_Resume;
            }

            RegisterDependencies(context, resolver);
            
            // Force setting of default language.
            var lng = Globals.Language;
        }

        public static string GetCountryCode()
        {
            var countryCodeArray = (Java.Util.Locale.Default + "").Split('_');
            if (countryCodeArray != null && countryCodeArray.Length > 1)
                return countryCodeArray[1].ToLower();

            return "";
        }

        static string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            if (char.IsUpper(s[0]))
                return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        static void LifecycleEvents_Resume(object sender, EventArgs e)
        {
            globals.RunningInBackground = false;
        }

        static void LifecycleEvents_Sleep(object sender, EventArgs e)
        {
            globals.RunningInBackground = true;
        }

        public static void RegisterDependencies(Context context)
        {
            RegisterDependencies(context, Resolver.Current);
        }

        public static void RegisterDependencies(Context context, IDependencyResolver resolver)
        {
            resolver.Register<IWriteLogger, WriteLogger>();
            resolver.Register<IWebRequestManager, DroidWebRequestManager>();
            resolver.Register<IPingManager, PingManager>();
            resolver.Register<IBackgroundService, BackgroundService>();
            resolver.Register<IMediaPicker, MediaPicker>();
            resolver.Register(typeof(IWindowService), new WindowService(context));
            resolver.Register<IEncryptor, Security.RNCryptor.Encryptor>();
            resolver.Register<IDecryptor, Security.RNCryptor.Decryptor>();
            resolver.Register<IImageService, ImageService>();
            resolver.Register<IEmailService, EmailService>();
            if (context is Activity)
                Resolver.Register(typeof(IPlatformService), new PlatformService((Activity)context));
            resolver.Register<IHomeService, HomeService>();
            resolver.Register(typeof(ISoundService), new SoundService(context));
            resolver.Register<ISocialMediaService, SocialMediaService>();
            resolver.Register<IFileService, FileService>();
            resolver.Register<IInAppPurchaseService, InAppBilling>();
        }

        class AndroidGlobals : CommonGlobals
        {
            private string dbFolder;
            public override string DatabasesFolder
            {
                get
                {
                    if (string.IsNullOrEmpty(dbFolder))
                    {
                        dbFolder = System.IO.Path.Combine(this.LibraryFolder, "Databases");
                        if (!System.IO.Directory.Exists(dbFolder))
                        {
                            System.IO.Directory.CreateDirectory(dbFolder);
                        }
                    }
                    return dbFolder;
                }
                set
                {

                }
            }
            
            public override string GetDefaultLanguage()
            {
                return (Java.Util.Locale.Default + "").Replace("_", "-");
            }
        }
    }
}
