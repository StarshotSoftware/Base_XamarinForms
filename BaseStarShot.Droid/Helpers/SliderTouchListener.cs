using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BaseStarShot.Helpers
{
    public class SliderTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public event EventHandler OnTouchDown;

        public double FinalX
        {
            get;
            set;
        }

        double initialX, initialY;

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            //Android.Widget.TextView MyControl = (TextView)v;

            switch (e.Action & MotionEventActions.Mask)
            {
                case (MotionEventActions.Outside):
                    return false;
                case MotionEventActions.Down:
                    initialX = e.GetX();
                    initialY = e.GetY();
                    FinalX = initialX;
                    OnTouchDown(this, null);

                    return true;
            }

            return false;
        }

    }
}