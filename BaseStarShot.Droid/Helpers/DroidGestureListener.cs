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
    public class DroidGestureListener : GestureDetector.SimpleOnGestureListener
    {
        private static int SWIPE_THRESHOLD = 30;
        private static int SWIPE_VELOCITY_THRESHOLD = 30;

        public event EventHandler OnSwipeDown;
        public event EventHandler OnSwipeUp;
        public event EventHandler OnSwipeLeft;
        public event EventHandler OnSwipeRight;

        public float FinalX
        {
            get;
            set;
        }

        public float InitialX
        {
            get;
            set;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            InitialX = e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffX > 0)
                    {
                        if (OnSwipeRight != null)
                        {
                            OnSwipeRight(this, null);
                        }
                    }
                    else
                    {
                        if (OnSwipeLeft != null)
                            OnSwipeLeft(this, null);
                    }
                    FinalX = e2.GetX();
                }
            }
            else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD)
            {
                if (diffY > 0)
                {
                    if (OnSwipeDown != null)
                        OnSwipeDown(this, null);
                }
                else
                {
                    if (OnSwipeUp != null)
                        OnSwipeUp(this, null);
                }
            }
            return true;
        }


    }
}