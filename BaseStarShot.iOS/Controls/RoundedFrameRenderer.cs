using BaseStarShot;
using BaseStarShot.Controls;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using Foundation;
using System.Diagnostics;
using CoreAnimation;
using System.Linq;

[assembly: ExportRenderer(typeof(RoundedFrame), typeof(RoundFrameRenderer))]
namespace BaseStarShot.Controls
{
	public class RoundFrameRenderer : ViewRenderer<RoundedFrame, UIView>
	{
		const int ALL_EDGE = 0;
		const int LEFT_EDGE = 1;
		const int RIGHT_EDGE = 2;

		protected override void OnElementChanged(ElementChangedEventArgs<RoundedFrame> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;

			var view = new UIImageView();
			SetNativeControl(view);

			if (!Element.HasBorder)
			{
				if (Element.GestureRecognizers.Any())
				{
					view.AddGestureRecognizer(
						new UITapGestureRecognizer(() =>
								{
									foreach (var gesture in Element.GestureRecognizers)
									{
										if (gesture is TapGestureRecognizer)
										{
											var tapRecognizer = (TapGestureRecognizer)gesture;
											if (tapRecognizer.Command != null)
												tapRecognizer.Command.Execute(tapRecognizer.CommandParameter);

											tapRecognizer.Raise("Tapped", EventArgs.Empty);
										}
									}
								})
					);
					view.UserInteractionEnabled = true;
				}
			}
			else
			{
				view.UserInteractionEnabled = false;
			}

			this.BackgroundColor = Color.Default.ToUIColor();

			SetBackgroundColor();
			SetBorder();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			switch (e.PropertyName)
			{
				case "GradientColor": RenderGradientBackground(); break;
				case "BackgroundColor": SetBackgroundColor(); break;
				case "CornerRadius": SetCornerRadius(); break;
				case "HasBorder":
				case "BorderColor": SetBorder(); break;
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var baseElement = Element as RoundedFrame;
			if (baseElement != null)
			{
				if (Control.Frame.Width > 0)
				{
					if (baseElement.BackgroundImage != null)
					{
						var imageView = new UIImageView(UIImage.FromBundle(baseElement.BackgroundImage));
						imageView.Frame = Control.Bounds;
						imageView.SetCornerRadius((nfloat)Convert.ToDouble(baseElement.CornerRadius), UIRectCorner.AllCorners);
						imageView.Layer.MasksToBounds = true;
						Control.AddSubview(imageView);
					}

					SetCornerRadius();
					RenderGradientBackground();
				}
			}
		}

		void RenderGradientBackground()
		{
			if (string.IsNullOrEmpty(Element.GradientColor))
				Control.Layer.RemoveGradientBackground();
			else
				Control.Layer.RenderGradientBackground(Element.GradientColor);
		}

		void SetBackgroundColor()
		{
			Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
		}

		void SetCornerRadius()
		{
			var test = Element.CornerRadius;

			if (!string.IsNullOrEmpty(Element.CornerRadius))
			{
				var cornerRad = Element.CornerRadius.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				Thickness cornerRadius = new Thickness();

				if (cornerRad.Length == 4)
				{
					cornerRadius.Left = float.Parse(cornerRad[0]);
					cornerRadius.Top = float.Parse(cornerRad[1]);
					cornerRadius.Right = float.Parse(cornerRad[2]);
					cornerRadius.Bottom = float.Parse(cornerRad[3]);


				}
				else if (cornerRad.Length == 2)
				{
					cornerRadius.Left = float.Parse(cornerRad[0]);
					cornerRadius.Right = float.Parse(cornerRad[0]);
					cornerRadius.Top = float.Parse(cornerRad[1]);
					cornerRadius.Bottom = float.Parse(cornerRad[1]);
				}
				else
				{
					int rad;
					if (int.TryParse(Element.CornerRadius, out rad))
					{
						if (rad <= -1)
						{
							cornerRadius.Left = 10;
							cornerRadius.Right = 10;
							cornerRadius.Top = 10;
							cornerRadius.Bottom = 10;
						}
						else
						{
							cornerRadius.Left = rad;
							cornerRadius.Right = rad;
							cornerRadius.Top = rad;
							cornerRadius.Bottom = rad;
						}
					}
				}

				int radius = 0;
				UIRectCorner forCorners = 0;

				if (cornerRadius.Bottom > 0 &&
					cornerRadius.Left > 0 &&
					cornerRadius.Right > 0 &&
					cornerRadius.Top > 0)
				{
					forCorners = UIRectCorner.AllCorners;
					radius = Convert.ToInt32(cornerRadius.Top);
				}
				else
				{
					if (cornerRadius.Bottom > 0)
					{
						forCorners = forCorners | UIRectCorner.BottomRight;
						radius = Convert.ToInt32(cornerRadius.Bottom);
					}
					if (cornerRadius.Left > 0)
					{
						forCorners = forCorners | UIRectCorner.BottomLeft;
						radius = Convert.ToInt32(cornerRadius.Left);
					}
					if (cornerRadius.Right > 0)
					{
						forCorners = forCorners | UIRectCorner.TopRight;
						radius = Convert.ToInt32(cornerRadius.Right);
					}
					if (cornerRadius.Top > 0)
					{
						forCorners = forCorners | UIRectCorner.TopLeft;
						radius = Convert.ToInt32(cornerRadius.Top);
					}
				}

				Control.SetCornerRadius(radius, forCorners);
			}
			else
			{
				Control.SetCornerRadius(0, UIRectCorner.AllCorners);
			}
		}

		void SetBorder()
		{
			if (Element.HasBorder)
			{
				Control.Layer.BorderWidth = 1;
				Control.Layer.BorderColor = Element.BorderColor.ToCGColor();
			}
			else
			{
				Control.Layer.BorderWidth = 0;
			}
		}
	}
}
