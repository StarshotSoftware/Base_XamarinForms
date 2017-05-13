using BaseStarShot.IO;
using BaseStarShot.Model;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class LastInteractionListener : ILastInteractionListener
    {

        public event EventHandler<DateTime> UpdateTime = delegate
        {
        };

        public void SetLastInteractionTime(DateTime datetime)
        {
            if (UpdateTime != null)
                UpdateTime(this, datetime);
        }
    }
}
