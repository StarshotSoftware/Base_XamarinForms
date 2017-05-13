using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BaseStarShot.Services
{
    public class BackgroundService : IBackgroundService
    {
        const string LogTag = "BackgroundTask";

        static readonly ReaderWriterLockSlim taskLocker = new ReaderWriterLockSlim();
        static volatile int s_taskId = 0;
        static HashSet<int> tasks = new HashSet<int>();

        public int BeginTask(string name, CancellationTokenSource cts)
        {
            taskLocker.EnterWriteLock();
            try
            {
                if (tasks.Count == 0)
                {
                    Logger.Write(LogTag, "Stating LongRunningService", LogType.Debug);
                    Application.Context.StartService(new Intent(Application.Context, typeof(LongRunningService)));
                }
                var taskId = ++s_taskId;
                tasks.Add(taskId);
                return taskId;
            }
            finally
            {
                taskLocker.ExitWriteLock();
            }
        }

        public void EndTask(int taskId)
        {
            taskLocker.EnterWriteLock();
            try
            {
                tasks.Remove(taskId);
                if (tasks.Count == 0)
                {
                    Logger.Write(LogTag, "Stopping LongRunningService", LogType.Debug);
                    Application.Context.StopService(new Intent(Application.Context, typeof(LongRunningService)));
                }
            }
            finally
            {
                taskLocker.ExitWriteLock();
            }
        }
    }
}