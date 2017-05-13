using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IBackgroundService
    {
        int BeginTask(string name, CancellationTokenSource cts);

        void EndTask(int taskId);
    }
}
