using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;
using CoreAnimation;
using Xamarin.Forms;

namespace BaseStarShot
{
    public static class UIViewExtensions
    {
        public static void SetCornerRadius(this UIView view, System.nfloat radius, UIRectCorner forCorners)
        {
            if (forCorners == UIRectCorner.AllCorners)
            {
                view.Layer.CornerRadius = radius;
                view.Layer.Mask = null;
            }
            else
            {
                UIBezierPath maskPath = UIBezierPath.FromRoundedRect(view.Bounds, forCorners, new CGSize(radius, radius));
                CAShapeLayer maskLayer = new CAShapeLayer();
                maskLayer.Frame = view.Bounds;
                maskLayer.Path = maskPath.CGPath;
                view.Layer.Mask = maskLayer;
            }
        }

    }
}