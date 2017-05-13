//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;
//using Android.Text;
//using BaseStarShot.Controls;
//using Android.Graphics.Drawables;
//using Android.Util;
//using Android;
//using Android.Graphics;
//using BaseStarShot.Helpers;


//[assembly: ExportRenderer(typeof(ProgressView), typeof(ProgressViewRenderer))]
//namespace BaseStarShot.Controls
//{
//    public class ProgressViewRenderer : LabelRenderer
//    {
//        private readonly DroidTouchListener _listener;
//        double parentWidth;

//        protected BaseStarShot.Controls.ProgressView Base { get { return (BaseStarShot.Controls.ProgressView)Element; } }
//        protected Android.Widget.TextView MyControl { get { return (this.Control); } }

//        public ProgressViewRenderer()
//        {
//            _listener = new DroidTouchListener();
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
//        {
//            base.OnElementChanged(e);

//            if (e.NewElement == null)
//            {
//                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
//                _listener.OnSwipeRight -= HandleOnSwipeRight;
//                _listener.OnTouchDown -= _listener_OnTouchDown;
//            }

//            if (e.OldElement == null)
//            {
//                _listener.OnSwipeLeft += HandleOnSwipeLeft;
//                _listener.OnSwipeRight += HandleOnSwipeRight;
//                _listener.OnTouchDown += _listener_OnTouchDown;
//            }

//            Init();
//        }

//        private void Init()
//        {
//            if (Base.Progress < Base.Minimum)
//                Base.Progress = Base.Minimum;

//            if (Base.Progress > Base.Maximum)
//                Base.Progress = Base.Maximum;

//            int backgroundID = UIHelper.GetDrawableResource(Base.BackgroundImage);
//            int drawableRightID = UIHelper.GetDrawableResource(Base.ThumbImage);

//            int paddingLeft = (Math.Round(Base.PaddingLeft) == 0) ? 5 : (int)Math.Round(Base.PaddingLeft);
//            int paddingRight = (Math.Round(Base.PaddingRight) == 0) ? 5 : (int)Math.Round(Base.PaddingRight);

//            MyControl.SetPadding(paddingLeft, 0, 0, paddingRight);
//            MyControl.SetBackgroundDrawable(this.Resources.GetDrawable(backgroundID));
//            MyControl.SetCompoundDrawablesWithIntrinsicBounds(null, null, this.Resources.GetDrawable(drawableRightID), null);
//            MyControl.Text = Base.Progress.ToString();
//            MyControl.Gravity = GravityFlags.CenterVertical;

//            Base.MinimumWidthRequest = PixelsToDp(MyControl.GetCompoundDrawables()[2].IntrinsicWidth + 60);

//            parentWidth = DpToPixel(Base.ParentWidth) - DpToPixel(20);

//            double controlWidth = (Base.Progress / Base.Maximum) * parentWidth;

//            if (Base.Touchable)
//            {
//                MyControl.SetOnTouchListener(_listener);
//            }

//            Base.WidthRequest = PixelsToDp(controlWidth);
//            MyControl.SetWidth((int)(controlWidth));
//        }

//        void _listener_OnTouchDown(object sender, EventArgs e)
//        {
//            if (PixelsToDp(_listener.FinalX) <= Base.MinimumWidthRequest)
//            {
//                Base.WidthRequest = Base.MinimumWidthRequest;
//                Base.Progress = Base.Minimum;
//                Base.Text = Base.Minimum.ToString();
//                return;
//            }

//            if (PixelsToDp(_listener.FinalX) >= PixelsToDp(parentWidth))
//            {
//                Base.WidthRequest = PixelsToDp(parentWidth);
//                Base.Progress = Base.Maximum;
//                Base.Text = Base.Maximum.ToString();
//                return;
//            }

//            UpdateUI();
//        }

//        void HandleOnSwipeLeft(object sender, EventArgs e)
//        {
//            if (Base.WidthRequest <= Base.MinimumWidthRequest)
//            {
//                Base.WidthRequest = Base.MinimumWidthRequest;
//                Base.Progress = Base.Minimum;
//                MyControl.Text = Base.Minimum.ToString();
//                return;
//            }

//            Base.OnSwipeLeft();
//            UpdateUI();
//        }

//        void HandleOnSwipeRight(object sender, EventArgs e)
//        {
//            if (Base.WidthRequest >= PixelsToDp(parentWidth))
//            {
//                Base.WidthRequest = PixelsToDp(parentWidth);
//                Base.Progress = Base.Maximum;
//                MyControl.Text = Base.Maximum.ToString();
//                return;
//            }

//            Base.OnSwipeRight();
//            UpdateUI();
//        }

//        void UpdateUI()
//        {
//            Base.InitialX = PixelsToDp(_listener.InitialX);
//            Base.FinalX = PixelsToDp(_listener.FinalX);

//            if (Base.WidthRequest == PixelsToDp(Base.FinalX))
//            {
//                if (PixelsToDp(Base.FinalX) <= Base.MinimumWidthRequest)
//                    Base.WidthRequest = Base.MinimumWidthRequest;
//                else
//                    Base.WidthRequest = Base.FinalX;
//            }
//            else
//                Base.WidthRequest = Base.FinalX;

//            double prog = ((DpToPixel(Base.WidthRequest) - DpToPixel(Base.MinimumWidthRequest))
//               / (parentWidth - DpToPixel(Base.MinimumWidthRequest))) * Base.Maximum;

//            if (prog >= 0 && prog <= Base.Maximum)
//            {
//                Base.Progress = (int)Math.Round(prog);
//                Base.Text = Base.Progress.ToString();
//            }
//        }

//        private double DpToPixel(double dp)
//        {
//            return dp * (DeviceDpi() / 160d);
//        }

//        private double PixelsToDp(double px)
//        {
//            return px / (DeviceDpi() / 160d);
//        }

//        private int DeviceDpi()
//        {
//            switch (this.Resources.DisplayMetrics.DensityDpi)
//            {
//                case DisplayMetricsDensity.Low:
//                    return 120;
//                case DisplayMetricsDensity.Medium:
//                    return 160;
//                case DisplayMetricsDensity.High:
//                    return 240;
//                case DisplayMetricsDensity.Xhigh:
//                    return 320;
//                case DisplayMetricsDensity.Xxhigh:
//                    return 480;
//                case DisplayMetricsDensity.Xxxhigh:
//                    return 640;
//                case DisplayMetricsDensity.Tv:
//                    return 213;
//            }
//            return -1;
//        }
//    }
//}