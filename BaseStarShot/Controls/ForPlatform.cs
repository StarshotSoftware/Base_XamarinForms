using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ForPlatform<T>
    {
        public ForPlatform()
        {

        }

        public static implicit operator T(ForPlatform<T> forPlatform)
        {
            T value = forPlatform.Default;
            switch (Device.OS)
            {
                case TargetPlatform.Android:
                    if (forPlatform.hasSetAndroid)
                        value = forPlatform.Android;
                    break;
                case TargetPlatform.iOS:
                    if (forPlatform.hasSetiOS)
                        value = forPlatform.iOS;
                    break;
                case TargetPlatform.WinPhone:
                    if (forPlatform.hasSetWinPhone)
                        value = forPlatform.WinPhone;
                    break;
                case TargetPlatform.Windows:
                    if (Device.Idiom == TargetIdiom.Phone)
                    {
                        if (forPlatform.hasSetWindowsPhone)
                            value = forPlatform.WindowsPhone;
                        break;
                    }
                    if (forPlatform.hasSetWindowsTablet)
                        value = forPlatform.WindowsTablet;
                    break;
            }
            return value;
        }

        private T _android;
        private bool hasSetAndroid;
        /// <summary>
        /// The type as it is implemented on the Android platform.
        /// </summary>
        public T Android
        {
            get { return _android; }
            set { _android = value; hasSetAndroid = true; }
        }

        private T _iOS;
        private bool hasSetiOS;
        /// <summary>
        /// The type as it is implemented on the iOS platform.
        /// </summary>
        public T iOS
        {
            get { return _iOS; }
            set { _iOS = value; hasSetiOS = true; }
        }

        private T _winPhone;
        private bool hasSetWinPhone;
        /// <summary>
        /// The type as it is implemented on the WinPhone platform.
        /// </summary>
        public T WinPhone
        {
            get { return _winPhone; }
            set { _winPhone = value; hasSetWinPhone = true; }
        }

        private T _windowsTablet;
        private bool hasSetWindowsTablet;
        /// <summary>
        /// The type as it is implemented on the WinRT tablet platform.
        /// </summary>
        public T WindowsTablet
        {
            get { return _windowsTablet; }
            set { _windowsTablet = value; hasSetWindowsTablet = true; }
        }

        private T _windowsPhone;
        private bool hasSetWindowsPhone;
        /// <summary>
        /// The type as it is implemented on the WinRT phone platform.
        /// </summary>
        public T WindowsPhone
        {
            get { return _windowsPhone; }
            set { _windowsPhone = value; hasSetWindowsPhone = true; }
        }

        /// <summary>
        /// The default value of the type if not set for the specific platform.
        /// </summary>
        public T Default { get; set; }
    }
}
