using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot.Services;

namespace BaseStarShot
{
    public abstract class CommonViewModel : BindableBase
    {
        /// <summary>
        /// Called after dependencies have been asigned.
        /// </summary>
        public virtual void Init()
        {

        }
    }
}
