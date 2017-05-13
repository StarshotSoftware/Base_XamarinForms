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
using Android.Graphics.Drawables;

//[assembly: ExportRenderer(typeof(BaseStarShot.Controls.ListView), typeof(BaseStarShot.Controls.ListViewWithPlaceholderRenderer))]
namespace BaseStarShot.Controls
{
    public class ListViewWithPlaceholderRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer, AbsListView.IOnScrollListener
    {
        private Activity _activity;
        private ScrollState previousState = ScrollState.Idle;
        private bool isRefreshEnabled;
        private static bool isScrollingUp = false;
        private static bool skipLastVisibleItem = true;

        public ListViewWithPlaceholderRenderer()
        {
            _activity = this.Context as Activity;
        }

        public ListViewWithPlaceholderRenderer(IntPtr javaReference, JniHandleOwnership transfer)
            : this()
        {
        }

        protected ListView BaseControl { get { return ((BaseStarShot.Controls.ListView)this.Element); } }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
            if (Control == null)
                return;

            if (previousState != ScrollState.Idle && scrollState == ScrollState.Idle)
            {
                int count = view.ChildCount - 2; // visible views count
                int lastVisibleItemPosition = view.LastVisiblePosition - 2;

                if (view.FirstVisiblePosition == 0 && isScrollingUp)
                    BaseControl.OnScrolledTop();
            }

            //if (scrollState == ScrollState.Idle)
            if (!skipLastVisibleItem)
            {
                BaseControl.LastVisiblePosition = view.LastVisiblePosition - 2;
            }

            previousState = scrollState;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            SetSeparator();
            SetPadding();
            SetSelector();
            Control.SetHeaderDividersEnabled(false);
            Control.SetFooterDividersEnabled(false);

            Control.ChoiceMode = ChoiceMode.Single;

            Control.SetOnScrollListener(this);
            Control.SetOnTouchListener(new MyTouchListener());
            BaseControl.ScrollToBottomEvent += BaseControl_ScrollAndroidListView;
            //            BaseControl.ScrollToTopEvent += BaseControl_ScrollToTopEvent;
        }
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case "Padding": SetPadding(); break;
                case "SeparatorVisibility": SetSeparator(); break;
                case "Selector": SetSelector(); break;
                case "ScrollToTopChange":
                    ScrollToTop();
                    break;
                case "ScrollToIndex":
                    if (BaseControl.ScrollToIndex > 0)
                    {
                        DeterminedScrollTo(BaseControl.ScrollToIndex, 3);
                        //Control.SmoothScrollToPosition(BaseControl.ScrollToIndex, 100);
                    }
                    break;
            }
        }

        private void BaseControl_ScrollAndroidListView(object sender, int position)
        {
            if (Control == null)
                return;

            DeterminedScrollTo(position, 3);
            //await Task.Delay(200);
            //Control.SmoothScrollToPosition(position, 100);
            BaseControl.LastVisiblePosition = position;

            //Control.SmoothScrollToPositionFromTop(position, 100);
        }

        private void DeterminedScrollTo(int index, int attempts = 0)
        {
            if (Control == null)
                return;

            if (Control.FirstVisiblePosition != index && attempts < 10)
            {
                attempts++;
                Control.SmoothScrollToPositionFromTop(index, 1, 100);
                Control.PostDelayed(() =>
                {
                    DeterminedScrollTo(index, attempts);
                }, 100);
            }
        }

        private void BaseControl_ScrollToTopEvent(object sender, EventArgs e)
        {
            if (Control == null)
                return;

            Control.SmoothScrollToPositionFromTop(0, 100);
        }

        private ShapeDrawable CreateMyDrawable(Android.Graphics.Color color)
        {
            ShapeDrawable shapeDrawable = new ShapeDrawable();
            shapeDrawable.Paint.Color = color;
            if (color != Android.Graphics.Color.LightGray || color != Android.Graphics.Color.LightBlue || color != Android.Graphics.Color.Transparent)
                shapeDrawable.SetBounds(UIHelper.ConvertDPToPixels(15), 0, Control.Width, 1);

            if (color == Android.Graphics.Color.Transparent)
            {
                shapeDrawable.SetBounds(0, 0, 0, 0);
            }
            return shapeDrawable;
        }

        private async void ScrollToTop()
        {
            if (Control == null)
                return;

            if (BaseControl.ScrollToTopChange)
            {
                Control.SmoothScrollToPositionFromTop(0, 100);
                BaseControl.ScrollToTopChange = false;
            }
        }

        private void SetPadding()
        {
            Control.SetPadding(UIHelper.ConvertDPToPixels(BaseControl.Padding.Left), UIHelper.ConvertDPToPixels(BaseControl.Padding.Top),
                UIHelper.ConvertDPToPixels(BaseControl.Padding.Right), UIHelper.ConvertDPToPixels(BaseControl.Padding.Bottom));
        }

        private void SetSelector()
        {
            switch (BaseControl.Selector)
            {
                case ListViewSelector.None:
                    Control.CacheColorHint = Android.Graphics.Color.Transparent;
                    Control.Selector = CreateMyDrawable(Android.Graphics.Color.Transparent); break;
                case ListViewSelector.Blue:
                    Control.Selector = CreateMyDrawable(Android.Graphics.Color.LightBlue); break;
                case ListViewSelector.Default:
                    break;
            }
        }

        private void SetSeparator()
        {
            if (BaseControl.SeparatorVisibility != SeparatorVisibility.None)
            {
                if (BaseControl.SeparatorBackground != null && BaseControl.SeparatorBackground.File != null)
                    Control.Divider = this.Resources.GetDrawable(UIHelper.GetDrawableResource(BaseControl.SeparatorBackground));
                else
                {
                    Control.Divider = CreateMyDrawable(BaseControl.SeparatorColor.ToAndroid());
                }

                Control.DividerHeight = UIHelper.ConvertDPToPixels(BaseControl.SeparatorHeight);
            }
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
        }

        public class MyTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
        {
            private float initialY, finalY;

            public bool OnTouch(Android.Views.View v, MotionEvent e)
            {
                if (e.Action == MotionEventActions.Down)
                {
                    initialY = e.GetY();
                }
                if (e.Action == MotionEventActions.Up)
                {
                    finalY = e.GetY();
                    if (initialY < finalY)
                        isScrollingUp = true;
                    else if (initialY > finalY)
                        isScrollingUp = false;

                    skipLastVisibleItem = false;
                }

                return false;
            }
        }
    }
}