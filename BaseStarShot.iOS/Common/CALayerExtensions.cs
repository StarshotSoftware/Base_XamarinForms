using System;
using CoreAnimation;
using UIKit;
using CoreGraphics;
using Foundation;
using Xamarin.Forms.Platform.iOS;

namespace BaseStarShot
{
    public static class CALayerExtensions
    {
        public static void RenderButtonShadow(this CALayer layer)
        {
            layer.MasksToBounds = false;
            layer.ShadowColor = UIColor.Black.CGColor;
            layer.ShadowOpacity = 0.5f;
            layer.ShadowRadius = 1.0f;
            layer.ShadowOffset = new CGSize(1.5f, 2.0f);
        }

        public static void RenderButtonBorder(this CALayer layer, CGColor color)
        {
            layer.MasksToBounds = true;
            layer.BorderColor = color;
            layer.BorderWidth = 1f;
            //layer.ShadowOpacity = 0.5f;
            //layer.ShadowRadius = 1.0f;
            //layer.ShadowOffset = new CGSize(1.5f, 2.0f);
        }

        public static void RenderGradientBackground(this CALayer layer, string color)
        {
            CAGradientLayer gradientLayer;
            if (layer.Sublayers != null && layer.Sublayers.Length > 0)
            {
                gradientLayer = layer.Sublayers[0] as CAGradientLayer;
                if (gradientLayer != null)
                {
                    gradientLayer.RemoveFromSuperLayer();
                    gradientLayer = null;
                }
            }

            gradientLayer = new CAGradientLayer();
            var gradientArray = color.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            NSNumber[] offsetColor = new NSNumber[gradientArray.Length];
            CGColor[] colorList = new CGColor[gradientArray.Length];
            for (int i = 0; i < gradientArray.Length; i++)
            {
                var gradientColorOffset = gradientArray[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                colorList[i] = Xamarin.Forms.Color.FromHex(gradientColorOffset[0]).ToCGColor();
                int offsetInt = Int32.Parse(gradientColorOffset[1].Replace("%", ""));
                float offsetFloat = offsetInt / 100f;
                offsetColor[i] = offsetFloat;
            }
            gradientLayer.Colors = colorList;
            gradientLayer.Locations = offsetColor;
            gradientLayer.Frame = layer.Bounds;
            gradientLayer.CornerRadius = layer.CornerRadius;
            gradientLayer.MasksToBounds = false;

            layer.InsertSublayer(gradientLayer, 0);
        }

        public static void RemoveGradientBackground(this CALayer layer)
        {
            if (layer.Sublayers == null)
                return;

            if (layer.Sublayers.Length == 0)
                return;

            var gradientLayer = layer.Sublayers[0] as CAGradientLayer;
            if (gradientLayer != null)
            {
                gradientLayer.RemoveFromSuperLayer();
            }
        }
    }
}

