using System;
using System.ComponentModel;

using Xamarin.Forms.Platform.iOS;

using Foundation;
using UIKit;
using BaseStarShot.Services;
using Base1902;

namespace BaseStarShot.Controls
{
	public class CheckBoxRenderer : ViewRenderer<CheckBox, CheckBoxView>
	{
		UILabel linkLabel;
		
		/// <summary>
		/// Called when [element changed].
		/// </summary>
		/// <param name="e">The e.</param>
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			base.OnElementChanged(e);

			if (Element == null) return;

			BackgroundColor = Element.BackgroundColor.ToUIColor();

			if (Control == null)
			{
                string checkedImageSource = "";
                string uncheckedImageSource = "";

                if (Element.CheckedImage != null && !string.IsNullOrEmpty(Element.CheckedImage.File))
                    checkedImageSource = Element.CheckedImage.File;

                if (Element.UnCheckedImage != null && !string.IsNullOrEmpty(Element.UnCheckedImage.File))
                    uncheckedImageSource = Element.UnCheckedImage.File;

                var checkBox = new CheckBoxView(Bounds, checkedImageSource, uncheckedImageSource, Element);//, Element.WidthRequest, Element.HeightRequest, Element.Spacing);
				checkBox.TouchUpInside += (s, args) => Element.Checked = Control.Checked;

				SetNativeControl(checkBox);
			}

			Control.Frame = Frame;
			Control.Bounds = Bounds;

			linkLabel = new UILabel (Bounds);

			if (!string.IsNullOrEmpty (Element.InlineText)) {
				NSMutableAttributedString text; 
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					text = new NSMutableAttributedString(Element.InlineText);
				}
				else
				{
					text = new NSMutableAttributedString(Element.InlineText + "\n");
					linkLabel.Lines = 2;
				}
				NSRange linkRange = new NSRange (0, Element.InlineText.Length);
				text.AddAttributes (new UIStringAttributes { UnderlineStyle = NSUnderlineStyle.Single }, linkRange);
				linkLabel.AttributedText = text;
				if (!string.IsNullOrEmpty (Element.LinkString)) {
					linkLabel.UserInteractionEnabled = true;
					linkLabel.AddGestureRecognizer (new UITapGestureRecognizer (() => {
						UIApplication.SharedApplication.OpenUrl (new NSUrl (Element.LinkString));
					}));
				}
			} else
				linkLabel.Text = Element.InlineText;
			linkLabel.TextColor = e.NewElement.TextColor.ToUIColor ();
            linkLabel.Center = Control.Center;
			this.Add (linkLabel);

			UpdateFont();

			Control.LineBreakMode = UILineBreakMode.WordWrap;
			Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
			Control.CheckedTitle = string.IsNullOrEmpty(e.NewElement.CheckedText) ? e.NewElement.DefaultText : e.NewElement.CheckedText;
			Control.UncheckedTitle = string.IsNullOrEmpty(e.NewElement.UncheckedText) ? e.NewElement.DefaultText : e.NewElement.UncheckedText;
			Control.Checked = e.NewElement.Checked;
			Control.SetTitleColor(e.NewElement.TextColor.ToUIColor(), UIControlState.Normal);
			Control.SetTitleColor(e.NewElement.TextColor.ToUIColor(), UIControlState.Selected);
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			linkLabel.RemoveFromSuperview ();
			linkLabel.Dispose ();
		}

		/// <summary>
		/// Resizes the text.
		/// </summary>
		private void ResizeText()
		{
			var text = Element.Checked ? string.IsNullOrEmpty(Element.CheckedText) ? Element.DefaultText : Element.CheckedText :
				string.IsNullOrEmpty(Element.UncheckedText) ? Element.DefaultText : Element.UncheckedText;

			var bounds = Control.Bounds;

			var width = Control.TitleLabel.Bounds.Width;

			var height = text.StringHeight(Control.Font, width);

			var minHeight = string.Empty.StringHeight(Control.Font, width);

			var requiredLines = Math.Round(height / minHeight, MidpointRounding.AwayFromZero);

			var supportedLines = Math.Round(bounds.Height / minHeight, MidpointRounding.ToEven);

			if (supportedLines != requiredLines)
			{
				bounds.Height += (float)(minHeight * (requiredLines - supportedLines));
				Control.Bounds = bounds;
				Element.HeightRequest = bounds.Height;
			}

			if (requiredLines == 1) {
				var refFrame = Control.TitleLabel.Frame;
				var x = refFrame.X;
				linkLabel.Frame = new CoreGraphics.CGRect (x, 0, Control.Frame.Width - x, Control.Frame.Height);
			}
		}

		/// <summary>
		/// Draws the specified rect.
		/// </summary>
		/// <param name="rect">The rect.</param>
		public override void Draw(CoreGraphics.CGRect rect)
		{
			base.Draw(rect);
			ResizeText();
		}

		/// <summary>
		/// Updates the font.
		/// </summary>
		private void UpdateFont()
		{
			if (!string.IsNullOrEmpty (Element.FontName)) {
				var font = UIFont.FromName (Element.FontName, (Element.FontSize > 0) ? (float)Element.FontSize : 12.0f);
				if (font != null) {
					Control.Font = font;
					linkLabel.Font = font;
				}
			} else if (Element.FontSize > 0) {
                var fontName = Resolver.Get<IFontService>().GetFontName(FontStyle.Regular);
                if (!string.IsNullOrEmpty(fontName))
                {
                    Control.Font = UIFont.FromName(fontName, (nfloat)Element.FontSize);
                    linkLabel.Font = Control.Font;
                }

    //            var font = UIFont.FromName (Control.Font.Name, (float)Element.FontSize);
				//if (font != null) {
				//	Control.Font = font;
				//	linkLabel.Font = font;
				//}
			}
		}

		/// <summary>
		/// Handles the <see cref="E:ElementPropertyChanged" /> event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			switch (e.PropertyName)
			{
				case "Checked":
					Control.Checked = Element.Checked;
					break;
				case "TextColor":
					Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Normal);
					Control.SetTitleColor(Element.TextColor.ToUIColor(), UIControlState.Selected);
					break;
				case "CheckedText":
					Control.CheckedTitle = string.IsNullOrEmpty(Element.CheckedText) ? Element.DefaultText : Element.CheckedText;
					break;
				case "UncheckedText":
					Control.UncheckedTitle = string.IsNullOrEmpty(Element.UncheckedText) ? Element.DefaultText : Element.UncheckedText;
					break;
				case "FontSize":
					UpdateFont();
					break;
				case "FontName":
					UpdateFont();
					break;
				case "Element":
					break;
				default:
					System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
					return;
			}
		}
	}
}

