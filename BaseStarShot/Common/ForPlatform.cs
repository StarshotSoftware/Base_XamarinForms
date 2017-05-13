using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot
{
    public static class ForPlatform
    {
        /// <summary>
        /// Returns different values depending on the current platform.
        /// </summary>
        /// <returns></returns>
        public static T Get<T>(T iOS, T Android, T WinPhone, T WindowsTablet, T WindowsPhone)
        {
            switch (Device.OS)
            {
                case TargetPlatform.Android:
                    return Android;
                case TargetPlatform.iOS:
                    return iOS;
                case TargetPlatform.WinPhone:
                    return WinPhone;
                case TargetPlatform.Windows:
                    if (Device.Idiom == TargetIdiom.Phone)
                        return WindowsPhone;
                    return WindowsTablet;
            }
            return default(T);
        }

        /// <summary>
        /// Executes different Actions depending on the current platform.
        /// </summary>
        /// <param name="iOS"></param>
        /// <param name="Android"></param>
        /// <param name="WinPhone"></param>
        /// <param name="WindowsTablet"></param>
        /// <param name="WindowsPhone"></param>
        /// <param name="Default"></param>
        public static void Execute(Action iOS = null, Action Android = null, Action WinPhone = null, Action WindowsTablet = null, Action WindowsPhone = null, Action Default = null)
        {
            Action action = null;
            switch (Device.OS)
            {
                case TargetPlatform.Android:
                    action = Android;
                    break;
                case TargetPlatform.iOS:
                    action = iOS;
                    break;
                case TargetPlatform.WinPhone:
                    action = WinPhone;
                    break;
                case TargetPlatform.Windows:
                    if (Device.Idiom == TargetIdiom.Phone)
                        action = WindowsPhone;
                    else
                        action = WindowsTablet;
                    break;
            }
            if (action == null)
                action = Default;
            if (action != null)
                action.Invoke();
        }
    }
}
