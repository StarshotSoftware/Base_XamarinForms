using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Model
{
    public class ScrollX : BindableBase
    {
        private int _x1;
        public int X1
        {
            get { return _x1; }
            set { SetProperty(ref _x1, value); }
        }

        private int _x2;
        public int X2
        {
            get { return _x2; }
            set { SetProperty(ref _x2, value); }
        }



        public ScrollX()
        {
        }


    }
}
