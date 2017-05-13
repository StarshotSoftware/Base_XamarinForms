using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot
{
    public static class LifecycleEvents
    {
        public static event EventHandler Start = delegate { };
        public static event EventHandler Sleep = delegate { };
        public static event EventHandler Resume = delegate { };

        public static void RaiseStart()
        {
            Start(Application.Current, EventArgs.Empty);
        }

        public static void RaiseSleep()
        {
            Sleep(Application.Current, EventArgs.Empty);
        }

        public static void RaiseResume()
        {
            Resume(Application.Current, EventArgs.Empty);
        }
    }
}
