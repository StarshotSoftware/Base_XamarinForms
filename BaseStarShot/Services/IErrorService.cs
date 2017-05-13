using BaseStarShot.Logging;
using System;
using System.Threading.Tasks;
namespace BaseStarShot.Services
{
    public interface IErrorService
    {
        /// <summary>
        /// Logs exception.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        bool LogError(ErrorData error);

        /// <summary>
        /// Logs error data.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool LogError(Exception ex);

        /// <summary>
        /// Sends all pending error data for sending.
        /// </summary>
        /// <returns></returns>
        Task<int> SendAllPendingAsync();
    }
}
