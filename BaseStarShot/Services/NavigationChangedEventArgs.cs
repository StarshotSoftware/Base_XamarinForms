using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class NavigationChangedEventArgs : EventArgs
    {
        public BaseViewModel ViewModel { get; protected set; }

        public bool modal { get; protected set; }

        public NavigationChangedEventArgs(BaseViewModel viewModel, bool modal)
        {
            this.ViewModel = viewModel;
            this.modal = modal;
        }
    }
}
