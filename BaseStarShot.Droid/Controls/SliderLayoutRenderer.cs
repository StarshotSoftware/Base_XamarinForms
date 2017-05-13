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


//[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Frame), typeof(SliderLayoutRenderer))]
//namespace BaseStarShot.Controls
//{
//    public class SliderLayoutRenderer : FrameRenderer
//    {
//        ProgressView progressView;
//        double parentWidth;

//        private readonly DroidTouchListener _listener;

//        public SliderLayoutRenderer()
//        {
//            _listener = new DroidTouchListener();
//        }

//        protected BaseStarShot.Controls.Frame Base { get { return (BaseStarShot.Controls.Frame)Element; } }

//        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
//        {

//            base.OnElementChanged(e);

//            if (e.NewElement == null)
//            {
//                _listener.OnTouchDown -= _listener_OnTouchDown;
//                _listener.OnSwipeLeft -= _listener_OnSwipeLeft;
//                _listener.OnSwipeRight -= _listener_OnSwipeRight;
//            }

//            if (e.OldElement == null)
//            {
//                _listener.OnTouchDown += _listener_OnTouchDown;
//                _listener.OnSwipeLeft += _listener_OnSwipeLeft;
//                _listener.OnSwipeRight += _listener_OnSwipeRight;
//            }

//            parentWidth = DpToPixel(Base.WidthRequest) - DpToPixel(20);

//            Xamarin.Forms.RelativeLayout rl = Base.ParentView as Xamarin.Forms.RelativeLayout;

//            foreach (Xamarin.Forms.View view in rl.Children)
//            {
//                if (view.GetType() == typeof(BaseStarShot.Controls.ProgressView))
//                {
//                    progressView = (BaseStarShot.Controls.ProgressView)view;
//                    break;
//                }
//            }

//            if (progressView == null) return;

//            if (progressView.Touchable)
//                this.SetOnTouchListener(_listener);
//        }

//        void _listener_OnSwipeRight(object sender, EventArgs e)
//        {
//            if (PixelsToDp(_listener.FinalX) >= PixelsToDp(parentWidth))
//            {
//                progressView.WidthRequest = PixelsToDp(parentWidth);
//                progressView.Progress = progressView.Maximum;
//                progressView.Text = progressView.Maximum.ToString();
//                return;
//            }

//            //Base.OnSwipeLeft();
//            UpdateUI();
//        }

//        void _listener_OnSwipeLeft(object sender, EventArgs e)
//        {
//            if (PixelsToDp(_listener.FinalX) <= progressView.MinimumWidthRequest)
//            {
//                progressView.WidthRequest = progressView.MinimumWidthRequest;
//                progressView.Progress = progressView.Minimum;
//                progressView.Text = progressView.Minimum.ToString();
//                return;
//            }

//            //Base.OnSwipeRight();
//            UpdateUI();
//        }

//        void _listener_OnTouchDown(object sender, EventArgs e)
//        {
//            if (PixelsToDp(_listener.FinalX) <= progressView.MinimumWidthRequest)
//            {
//                progressView.WidthRequest = progressView.MinimumWidthRequest;
//                progressView.Progress = progressView.Minimum;
//                progressView.Text = progressView.Minimum.ToString();
//                return;
//            }

//            if (PixelsToDp(_listener.FinalX) >= PixelsToDp(parentWidth))
//            {
//                progressView.WidthRequest = PixelsToDp(parentWidth);
//                progressView.Progress = progressView.Maximum;
//                progressView.Text = progressView.Maximum.ToString();
//                return;
//            }

//            UpdateUI();

//        }

//        void UpdateUI()
//        {
//            if (progressView.WidthRequest == PixelsToDp(_listener.FinalX))
//            {
//                if (PixelsToDp(_listener.FinalX) <= progressView.MinimumWidthRequest)
//                    progressView.WidthRequest = progressView.MinimumWidthRequest;
//                else
//                    progressView.WidthRequest = PixelsToDp(_listener.FinalX);
//            }
//            else
//                progressView.WidthRequest = PixelsToDp(_listener.FinalX);

//            double prog = ((DpToPixel(progressView.WidthRequest) - DpToPixel(progressView.MinimumWidthRequest))
//                / (parentWidth - DpToPixel(progressView.MinimumWidthRequest))) * progressView.Maximum;

//            if (prog >= 0 && prog <= progressView.Maximum)
//            {
//                progressView.Progress = (int)Math.Round(prog);
//                progressView.Text = progressView.Progress.ToString();
//            }
//        }


//        private double PixelsToDp(double px)
//        {
//            return px / (DeviceDpi() / 160d);
//        }

//        private double DpToPixel(double dp)
//        {
//            return dp * (DeviceDpi() / 160d);
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