using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using BaseStarShot;
using System.Diagnostics;

namespace BaseStarShot.Controls
{
    public class ProgressViewControlRenderer : Xamarin.Forms.Platform.iOS.LabelRenderer
    {
        const double MinProgressWidth = 23;

        UIImageView thumbImageView;

        protected BaseStarShot.Controls.ProgressView BaseElement
        {
            get { return Element as BaseStarShot.Controls.ProgressView; }
        }

        protected UIView ContainerView
        {
            get { return Control.Superview.Superview; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            if (BaseElement != null)
            {
                BaseElement.PropertyChanged += (s, ev) =>
                {
                    var element = (BaseStarShot.Controls.ProgressView)s;
                    switch (ev.PropertyName)
                    {
                        case "BackgroundImage": SetBackgroundImage(element); break;
                        case "Progress": SetProgress(element); break;
                        case "ThumbImage": SetThumbImage(element); break;
                    }
                };
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (BaseElement != null)
            {
                SetBackgroundImage(BaseElement);
                SetThumbImage(BaseElement);
                SetProgress(BaseElement);
            }

            AddTapGesture();
        }

        void AddTapGesture()
        {
            if (ContainerView != null)
            {
                if (ContainerView.GestureRecognizers == null ||
                    ContainerView.GestureRecognizers.Count() == 0)
                {
                    ContainerView.AddGestureRecognizer(new UITapGestureRecognizer(OnProgressTap));
                    ContainerView.UserInteractionEnabled = true;
                }
            }
        }

        protected virtual void SetBackgroundImage(BaseStarShot.Controls.ProgressView element)
        {
            if (element.BackgroundImage != null && !string.IsNullOrEmpty(element.BackgroundImage.File))
            {
                var image = UIImage.FromBundle(element.BackgroundImage.File);
                Control.BackgroundColor = UIColor.FromPatternImage(image);
            }
            else
            {
                Control.BackgroundColor = null;
            }
        }

        void SetProgress(BaseStarShot.Controls.ProgressView element)
        {
            if (ContainerView != null)
            {
                var width = (element.Progress / element.Maximum) * (ContainerView.Frame.Width - MinProgressWidth);
                element.WidthRequest = width + MinProgressWidth;
            }
        }

        void SetThumbImage(BaseStarShot.Controls.ProgressView element)
        {
            if (element.Bounds.Height > 0)
            {
                if (element.ThumbImage != null && !string.IsNullOrEmpty(element.ThumbImage.File))
                {
                    if (thumbImageView == null)
                    {
                        var image = UIImage.FromBundle(element.ThumbImage.File);

                        thumbImageView = new UIImageView(image);
                        thumbImageView.AddGestureRecognizer(new UIPanGestureRecognizer(OnProgressPan));
                        thumbImageView.UserInteractionEnabled = true;
                        Control.Superview.AddSubview(thumbImageView);
                    }

                    var top = (nfloat)(element.Bounds.Size.Height * 0.15);
                    var height = (nfloat)(element.Bounds.Size.Height * 0.7);
                    var width = height;
                    var left = (nfloat)(element.Bounds.Width - width - top);
                    left = left < 0 ? 0 : left;
                    thumbImageView.Frame = new CGRect(left, top, width, height);
                }
            }
        }

        void OnProgressTap(UITapGestureRecognizer recognizer)
        {
            if (ContainerView != null)
            {
                var point = recognizer.LocationInView(ContainerView);
                UpdateProgressBaseOnPoint(point);
            }
        }

        void OnProgressPan(UIPanGestureRecognizer recognizer)
        {
            if (ContainerView != null)
            {
                var point = recognizer.LocationInView(ContainerView);
                UpdateProgressBaseOnPoint(point);
            }
        }

        void UpdateProgressBaseOnPoint(CGPoint point)
        {
            if (BaseElement != null)
            {
                var adjustedPointX = point.X;
                if (adjustedPointX < MinProgressWidth)
                    adjustedPointX = (nfloat)MinProgressWidth;
                else if (adjustedPointX > ContainerView.Frame.Width)
                    adjustedPointX = ContainerView.Frame.Width;

                var ratio = (adjustedPointX - MinProgressWidth) / (ContainerView.Frame.Width - MinProgressWidth);
                if (ratio < 0)
                    ratio = 0;
                BaseElement.Progress = ratio * BaseElement.Maximum;
            }
        }
    }
}