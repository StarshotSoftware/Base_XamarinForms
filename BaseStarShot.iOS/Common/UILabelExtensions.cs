using System;
using UIKit;
using System.Drawing;
using Foundation;

namespace BaseStarShot
{
	public static class UILabelExtensions
	{
		public static void AdjustFontSizeToFit(this UILabel label)
		{
			if (string.IsNullOrEmpty (label.Text))
				return;
			
			var font = label.Font;
			var size = label.Frame.Size;

			for (var maxSize = label.Font.PointSize; maxSize >= label.MinimumScaleFactor * label.Font.PointSize; maxSize -= 1f)
			{
				font = font.WithSize(maxSize);
				var constraintSize = new SizeF((float)size.Width, float.MaxValue);
				var labelSize = (new NSString(label.Text)).StringSize(font, constraintSize, UILineBreakMode.WordWrap);

				if (labelSize.Height <= size.Height)
				{
					label.Font = font;
					label.SetNeedsLayout();
					break;
				}
			}

			// set the font to the minimum size anyway
			label.Font = font;
			label.SetNeedsLayout();
		}
	}
}

