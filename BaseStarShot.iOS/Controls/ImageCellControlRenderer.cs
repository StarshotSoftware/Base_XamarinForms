using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using CoreGraphics;
using BaseStarShot;
using System.Threading.Tasks;
using Foundation;

namespace BaseStarShot.Controls
{
	public class ImageCellControlRenderer : TextCellRenderer
	{
		const double defaultImageViewLeftPadding = 15;
		const double leftPadding = 15;
		const double topPadding = 10;
		const double bottomPadding = 10;
		const double rightPadding = 15;

		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cellTableViewCell = (CellTableViewCell)base.GetCell(item, reusableCell, tv);
			var cell = (ImageCell)item;

			SetImage(cell, cellTableViewCell);
			RenderCircle(cell, cellTableViewCell);
			SetTextFont(cell, cellTableViewCell);
			SetDetailFont(cell, cellTableViewCell);

			cellTableViewCell.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			cellTableViewCell.DetailTextLabel.Lines = 0;

			cell.Height = ComputeHeight(cell, cellTableViewCell);

			return cellTableViewCell;
		}

		private double ComputeHeight(ImageCell element, CellTableViewCell target)
		{
			double height = 0;
			var labelPreferredWidth = Globals.LogicalScreenWidth
			                          - defaultImageViewLeftPadding
			                          - element.ImageWidth
			                          - leftPadding
			                          - rightPadding;

			// text 
			var textSize = new NSString(target.TextLabel.Text).GetBoundingRect(
				new CGSize((nfloat)labelPreferredWidth, 0),
				NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes 
			{
				Font = target.TextLabel.Font
			}, null);
			height += textSize.Height;

			// detail
			var detailSize = new NSString(target.DetailTextLabel.Text).GetBoundingRect(
				new CGSize((nfloat)labelPreferredWidth, 0),
				NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes
			{
				Font = target.DetailTextLabel.Font
			}, null);
			height += detailSize.Height;

			height = Math.Max(height, element.ImageHeight);

			height += topPadding + bottomPadding;

			return height;
		}

		protected override void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
		{
			var cellTableViewCell = (CellTableViewCell)sender;
			var cell = (ImageCell)cellTableViewCell.Cell;

			base.HandlePropertyChanged(sender, args);

			switch (args.PropertyName)
			{
				case "ImageSource":
					SetImage(cell, cellTableViewCell);
					RenderCircle(cell, cellTableViewCell);
					break;
				case "RenderCircle":
				case "ImageWidth":
				case "ImageHeight":
					RenderCircle(cell, cellTableViewCell);
					break;
				case "TextFontFamily":
				case "TextFontSize":
				case "TextFontStyle":
					SetTextFont(cell, cellTableViewCell);
					break;
				case "DetailFontFamily":
				case "DetailFontSize":
				case "DetailFontStyle":
					SetDetailFont(cell, cellTableViewCell);
					break;
			}
		}

		private void RenderCircle(ImageCell element, CellTableViewCell target)
		{
			if (element.RenderCircle)
			{
				double num = Math.Min(element.ImageWidth, element.ImageHeight);
				target.ImageView.Layer.CornerRadius = (nfloat)(num / 2);
				target.ImageView.Layer.MasksToBounds = false;
				target.ImageView.ClipsToBounds = true;
			} 
			else
			{
				target.ImageView.Layer.CornerRadius = 0;
			}
		}

		private void SetTextFont(ImageCell element, CellTableViewCell target)
		{
			target.TextLabel.Font = UIFont.FromName(element.TextFontFamily, (nfloat)element.TextFontSize);
		}

		private void SetDetailFont(ImageCell element, CellTableViewCell target)
		{
			target.DetailTextLabel.Font = UIFont.FromName(element.DetailFontFamily, (nfloat)element.DetailFontSize);
		}

		private async void SetImage(ImageCell cell, CellTableViewCell target)
		{
			ImageSource imageSource = cell.ImageSource;
			if (imageSource != null)
			{
				UIImage uiimage;
				try
				{
					uiimage = await cell.ImageSource.GetUIImage();
				}
				catch (TaskCanceledException)
				{
					uiimage = null;
				}

				if (uiimage == null)
				{
					if (cell.ImagePlaceholder != null)
						uiimage = UIImage.FromBundle(cell.ImagePlaceholder.File);
				}

				NSRunLoop.Main.BeginInvokeOnMainThread(delegate
				{
					if (uiimage != null && cell.ImageWidth > 0 && cell.ImageHeight > 0)
						target.ImageView.Image = uiimage.Scale(new CGSize((nfloat)cell.ImageWidth, (nfloat)cell.ImageHeight), 0);
					else
						target.ImageView.Image = uiimage;

					target.SetNeedsLayout();
				});
			}
			else
			{
				target.ImageView.Image = null;
			}
		}
	}
}

