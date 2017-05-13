using System;
namespace BaseStarShot.Services
{
    public interface IDispatcherService
    {
        /// <summary>
        /// Invokes an action on the main thread.
        /// </summary>
        /// <param name="action"></param>
        void BeginInvokeOnMainThread(Action action);
    }
}
