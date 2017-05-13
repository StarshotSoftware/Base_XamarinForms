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
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BaseStarShot.Controls;
using Android;
using Android.Graphics.Drawables;
using System.Threading.Tasks;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.HorizontalScrollView), typeof(HorizontalScrollRenderer))]
namespace BaseStarShot.Controls
{
    public class HorizontalScrollRenderer : Xamarin.Forms.Platform.Android.ScrollViewRenderer
    {
        float StartX, StartY;
        int IsHorizontal = -1;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= NewElement_PropertyChanged;

            if (((HorizontalScrollView)e.NewElement).Orientation == ScrollOrientation.Horizontal) IsHorizontal = 1;


            e.NewElement.PropertyChanged += NewElement_PropertyChanged;
        }

        void NewElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ChildCount > 0)
            {
                GetChildAt(0).HorizontalScrollBarEnabled = false;
                GetChildAt(0).VerticalScrollBarEnabled = false;
            }
        }



        public override bool DispatchTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    StartX = e.RawX;
                    StartY = e.RawY;
                    this.RequestDisallowInterceptTouchEvent(true);
                    break;
                case MotionEventActions.Move:
                    if (IsHorizontal * Math.Abs(StartX - e.RawX) < IsHorizontal * Math.Abs(StartY - e.RawY))
                        this.RequestDisallowInterceptTouchEvent(false);


                    break;
                case MotionEventActions.Up:
                    this.RequestDisallowInterceptTouchEvent(false);
                    break;
            }
            return base.DispatchTouchEvent(e);
        }

    }
}