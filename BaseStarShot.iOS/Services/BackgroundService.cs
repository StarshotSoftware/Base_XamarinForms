using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Threading;

namespace BaseStarShot.Services
{
    public class BackgroundService : IBackgroundService
    {
        const string LogTag = "BackgroundTask";

        public int BeginTask(string name, CancellationTokenSource cts)
        {
            int taskId = 0;
            taskId = (int)UIApplication.SharedApplication.BeginBackgroundTask(name, () =>
                {
					Logger.Write(LogTag, string.Format("Task {0} timeout occurred, canceled.", taskId), LogType.Debug);
                    if (cts != null)
                        cts.Cancel();
                });
			Logger.Write(LogTag, string.Format("Begin Task {0}.", taskId), LogType.Debug);
            return taskId;
        }

        public void EndTask(int taskId)
        {
			Logger.Write(LogTag, string.Format("End Task {0}.", taskId), LogType.Debug);
			UIApplication.SharedApplication.EndBackgroundTask((nint)taskId);
        }
    }
}