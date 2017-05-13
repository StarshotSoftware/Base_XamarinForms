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
    public class DroidTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        //public event EventHandler OnSwipeLeft;
        //public event EventHandler OnSwipeRight;
        public event EventHandler OnTouchDown;

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

        double initialX = 0, initialY = 0;

        //readonly int DIRECTION_LEFT = 0;
        //readonly int DIRECTION_RIGHT = 2;

        //int direction;

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            //Android.Widget.TextView MyControl = (TextView)v;

            switch (e.Action)
            {
                //case (MotionEventActions.Outside):
                //    return false;
                case MotionEventActions.Down:
                    InitialX = e.GetX();
                    //initialX = e.GetX();
                    //initialY = e.GetY();

                    //if (FinalX <= 0)
                    //    FinalX = initialX;

                    //OnTouchDown(this, null);

                    return true;
                case MotionEventActions.Move:

                    if (Math.Abs(e.GetY() - initialY) < Math.Abs(e.GetX() - initialX))
                    {
                        //InitialX = InitialX;
                        FinalX = e.GetX();
                        OnTouchDown(this, null);

                        //return true;
                    }

                    initialX = e.GetX();
                    initialY = e.GetY();

                    InitialX = e.GetX();

                    //double finalX = e.GetX();
                    //double finalY = e.GetY();

                    //InitialX = initialX;
                    //FinalX = finalX;

                    //if (initialX < finalX)
                    //{
                    //    direction = DIRECTION_RIGHT;
                    //}

                    //if (initialX > finalX)
                    //{
                    //    direction = DIRECTION_LEFT;
                    //}

                    //try
                    //{
                    //    if (v.Background != null)
                    //        if (e.RawX >= (v.Right - v.Background.Bounds.Width()))
                    //        {
                    //            switch (direction)
                    //            {
                    //                case 0: OnSwipeLeft(this, null); return true;
                    //                case 2: OnSwipeRight(this, null); return true;
                    //            }
                    //        }
                    //}
                    //catch (Exception)
                    //{
                    //    break;
                    //}

                    break;
            }

            return false;
        }


    }
}