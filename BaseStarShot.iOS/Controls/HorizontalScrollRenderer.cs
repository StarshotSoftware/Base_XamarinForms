using System;
using Xamarin.Forms.Platform.iOS;

namespace BaseStarShot.Controls
{
	public class HorizontalScrollRenderer : ScrollViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			PanGestureRecognizer.DelaysTouchesBegan = DelaysContentTouches;
		}
	}
}

