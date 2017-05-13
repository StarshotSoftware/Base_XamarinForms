using Base1902;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot
{
    public sealed class BackgroundTask
    {
        const string LogTag = "BackgroundTask";
        const string TaskName = "BackgroundTask";

        BackgroundTask()
        {

        }

        public static async Task RunAsync(Action action)
        {
            await RunAsync(() => new Task(action), null);
        }

        public static async Task RunAsync(Action action, CancellationTokenSource cts)
        {
            await RunAsync(() => new Task(action), cts);
        }

        public static async Task RunAsync(Func<Task> action, CancellationTokenSource cts)
        {
            var service = Resolver.Get<IBackgroundService>();
            if (service != null)
            {
                int id = service.BeginTask(TaskName, cts);

                try
                {
                    if (cts != null)
                        await Task.Run(action, cts.Token);
                    else
                        await Task.Run(action);
                }
                catch (OperationCanceledException)
                {
                    Logger.Write(LogTag, string.Format("Task {0} was cancelled.", id), LogType.Debug);
                }
                catch (Exception ex)
                {
                    Logger.WriteError(LogTag, ex);
                }
                finally
                {
                    service.EndTask(id);
                }
            }
        }
    }
}
