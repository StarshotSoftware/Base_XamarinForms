using BaseStarShot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface ILastInteractionListener
    {
        /// <summary>
        /// Loads file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        event EventHandler<DateTime> UpdateTime;

        void SetLastInteractionTime(DateTime datetime);
    }
}
