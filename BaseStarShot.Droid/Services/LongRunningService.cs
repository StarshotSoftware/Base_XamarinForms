using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Threading;

namespace BaseStarShot.Services
{
    [Service]
    public class LongRunningService : Service
    {
        const string LogTag = "BackgroundTask";
        volatile bool isRunning;

        public LongRunningService()
        {
            Logger.Write(LogTag, "LongRunningService constructed", LogType.Debug);
        }

        public override void OnCreate()
        {
            Logger.Write(LogTag, "LongRunningService created", LogType.Debug);
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            isRunning = true;
            Logger.Write(LogTag, string.Format("LongRunningService OnStartCommand - {0}", startId), LogType.Debug);

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            isRunning = false;
            Logger.Write(LogTag, "LongRunningService destoryed", LogType.Debug);
            base.OnDestroy();
        }
    }
}