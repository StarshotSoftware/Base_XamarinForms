using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    public interface IPingManager
    {
        /// <summary>
        /// Ping Host and return true if success
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task<bool> PingURL(string host, string port);

    }
}
