using System;
using Xamarin.Forms.Platform.iOS;
using BaseStarShot;
using BaseStarShot.Controls;
using UIKit;
using CoreGraphics;

namespace BaseStarShot.Controls
{
	public class RoundedBoxViewRenderer : BoxRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.BoxView> e)
		{
			base.OnElementChanged(e);

			var baseElement = Element as RoundedBoxView;
			if (baseElement != null)
			{
				baseElement.PropertyChanged += (s, ev) =>
				{
					var element = (RoundedBoxView)s;
					switch (ev.PropertyName)
					{
						case "CornerRadius": SetCornerRadius(element); break;
					}
				};

				SetCornerRadius(baseElement);
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var baseElement = Element as RoundedBoxView;
			if (baseElement != null)
			{
				SetCornerRadius(baseElement);
			}
		}

		void SetCornerRadius(RoundedBoxView element)
		{
			if (Frame.Width > 0)
			{
				this.SetCornerRadius((nfloat)element.CornerRadius, UIRectCorner.AllCorners);
                ClipsToBounds = true;
			}
		}
	}
}

