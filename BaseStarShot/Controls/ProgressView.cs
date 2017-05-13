using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ProgressView : Label
    {
        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create<ProgressView, int>(p => p.Minimum, 0);

        public static readonly BindableProperty BackgroundImageProperty =
            BindableProperty.Create<ProgressView, FileImageSource>(p => p.BackgroundImage, null);

        public static readonly BindableProperty ThumbImageProperty =
            BindableProperty.Create<ProgressView, FileImageSource>(p => p.ThumbImage, null);

        public static readonly BindableProperty PaddingLeftProperty =
            BindableProperty.Create<ProgressView, double>(p => p.PaddingLeft, 0);

        public static readonly BindableProperty PaddingRightProperty =
            BindableProperty.Create<ProgressView, double>(p => p.PaddingRight, 0);

        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create<ProgressView, double>(p => p.Progress, 0, BindingMode.TwoWay);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create<ProgressView, double>(p => p.Maximum, 0);

        public static readonly BindableProperty ParentWidthProperty =
            BindableProperty.Create<ProgressView, double>(p => p.ParentWidth, 0);

        public static readonly BindableProperty TouchableProperty =
            BindableProperty.Create<ProgressView, bool>(p => p.Touchable, true);

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public FileImageSource ThumbImage
        {
            get { return (FileImageSource)GetValue(ThumbImageProperty); }
            set { SetValue(ThumbImageProperty, value); }
        }

        public double PaddingLeft
        {
            get { return (double)GetValue(PaddingLeftProperty); }
            set { SetValue(PaddingLeftProperty, value); }
        }

        public double PaddingRight
        {
            get { return (double)GetValue(PaddingRightProperty); }
            set { SetValue(PaddingRightProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double ParentWidth
        {
            get { return (double)GetValue(ParentWidthProperty); }
            set { SetValue(ParentWidthProperty, value); }
        }

        public bool Touchable
        {
            get { return (bool)GetValue(TouchableProperty); }
            set { SetValue(TouchableProperty, value); }
        }

        //Swipe events

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
