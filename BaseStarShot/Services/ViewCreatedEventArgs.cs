using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Services
{
    public class ViewCreatedEventArgs : EventArgs
    {
        public View View { get; protected set; }
		public CommonViewModel ViewModel { get; protected set; }

		public ViewCreatedEventArgs(View view, CommonViewModel ViewModel)
        {
            this.View = view;
			this.ViewModel = ViewModel;
        }
    }
}
