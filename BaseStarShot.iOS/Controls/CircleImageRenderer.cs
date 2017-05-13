using System;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	public class CircleImageRenderer : ImageControlRenderer
	{
		private void CreateCircle()
		{
			double num = Math.Min(Element.Width, Element.Height);
			Control.Layer.CornerRadius = (float)(num / 2);
			Control.Layer.MasksToBounds = false;
			Control.Layer.BorderColor = ((CircleImage)Element).BorderColor.ToCGColor();
			Control.Layer.BorderWidth = ((CircleImage)Element).BorderThickness;
			Control.ClipsToBounds = true;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<BaseStarShot.Controls.Image> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || Element == null)
				return;
			
			CreateCircle();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == VisualElement.HeightProperty.PropertyName || 
				e.PropertyName == VisualElement.WidthProperty.PropertyName || 
				e.PropertyName == CircleImage.BorderColorProperty.PropertyName || 
				e.PropertyName == CircleImage.BorderThicknessProperty.PropertyName ||
				e.PropertyName == Image.SourceProperty.PropertyName)
			{
				CreateCircle();
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			if (Control != null)
				CreateCircle();
		}
	}
}

