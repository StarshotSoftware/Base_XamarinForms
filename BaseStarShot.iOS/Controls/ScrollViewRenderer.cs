using System;

namespace BaseStarShot.Controls
{
	public class ScrollViewRenderer : Xamarin.Forms.Platform.iOS.ScrollViewRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;

			var baseElement = Element as BaseStarShot.Controls.ScrollView;
			if (baseElement != null)
			{
				SetBounces(baseElement);
			}

            if (!baseElement.HasScrollbar)
            {
                ShowsHorizontalScrollIndicator = false;
                ShowsVerticalScrollIndicator = false;
            }
		}

		void SetBounces(BaseStarShot.Controls.ScrollView element)
		{
			Bounces = element.Bounces;		
		}
	}
}

