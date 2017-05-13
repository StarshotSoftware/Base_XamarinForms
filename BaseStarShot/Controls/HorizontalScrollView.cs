using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class HorizontalScrollView : Xamarin.Forms.ScrollView
    {

        public HorizontalScrollView()
        {
            
        }

        public event EventHandler SwipeLeft;
        public event EventHandler SwipeRight;

        public void OnSwipeLeft()
        {
            EventHandler handler = SwipeLeft;
            if (handler != null)
                SwipeLeft(this, null);
        }

        public void OnSwipeRight()
        {
            EventHandler handler = SwipeRight;
            if (handler != null)
                SwipeRight(this, null);
        }

        public double FinalX
        {
            get;
            set;
        }

        public double InitialX
        {
            get;
            set;
        }
    }
}
