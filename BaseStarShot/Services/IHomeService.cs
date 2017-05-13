using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IHomeService
    {
        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        event EventHandler<bool> HomeCalled;

        bool GoHome();
    }
}
