using BaseStarShot.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BaseStarShot.Repositories
{
    public interface IErrorRepository
    {
        /// <summary>
        /// Gets all error data.
        /// </summary>
        /// <returns></returns>
        List<ErrorData> GetAll();

        /// <summary>
        /// Geta all pending error date for sending.
        /// </summary>
        /// <returns></returns>
        List<ErrorData> GetAllPending();

        /// <summary>
        /// Saves error data.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        bool Save(ErrorData error);
    }
}
