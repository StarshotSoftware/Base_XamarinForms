using System;
using Xamarin.Forms.Platform.iOS;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using BaseStarShot.Controls;

namespace BaseStarShot.Controls
{
	public class GradientViewControlRenderer : ImageRenderer
	{
		bool hasGradientLayer;

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;

			RenderGradientLayer((GradientView)Element);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			RenderGradientLayer((GradientView)Element);
		}

		void RenderGradientLayer(GradientView element)
		{
			if (element.Bounds.Height > 0)
			{
				var startColor = element.StartColor.ToCGColor();
				var endColor = element.EndColor.ToCGColor();

				if (hasGradientLayer)
					Control.Layer.Sublayers[0].RemoveFromSuperLayer();

				var gradientLayer = (CAGradientLayer)CAGradientLayer.Create();
				gradientLayer.Frame = new CGRect(0, 0, (nfloat)element.Bounds.Size.Width, (nfloat)element.Bounds.Size.Height);
				gradientLayer.Colors = new CGColor[] { startColor, endColor };
                if (element.Orientation == GradientOrientation.LeftRight)
                {
                    gradientLayer.StartPoint = new CGPoint(0, 0);
                    gradientLayer.EndPoint = new CGPoint(1, 0);
                }
				Control.Layer.InsertSublayer(gradientLayer, 0);
				hasGradientLayer = true;
			}
		}
	}
}

