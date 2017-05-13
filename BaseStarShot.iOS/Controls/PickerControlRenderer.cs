using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace BaseStarShot.Controls
{
	public class PickerControlRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Xamarin.Forms.Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;

			var element = Element as BaseStarShot.Controls.Picker;
			if (element != null)
			{
				SetTextColor(element);
				SetBorderColor(element);
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			var element = Element as BaseStarShot.Controls.Picker;
			if (element != null)
			{
				if (e.PropertyName == "TextColor")
					SetTextColor(element);
				else if (e.PropertyName == "BorderColor")
					SetBorderColor(element);
			}
		}

		void SetTextColor(BaseStarShot.Controls.Picker element)
		{
			Control.TextColor = element.TextColor.ToUIColor();
		}

		void SetBorderColor(BaseStarShot.Controls.Picker element)
		{
			if (element.BorderColor == Color.Transparent)
				Control.BorderStyle = UIKit.UITextBorderStyle.None;
			else
				Control.Layer.BorderColor = element.BorderColor.ToCGColor();
		}
	}
}
