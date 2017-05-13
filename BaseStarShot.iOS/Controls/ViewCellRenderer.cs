using System;
using UIKit;
using BaseStarShot.Controls;

namespace BaseStarShot.Controls
{
	public class ViewCellRenderer : Xamarin.Forms.Platform.iOS.ViewCellRenderer
	{
		public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableViewCell reusableCell, UITableView tv)
		{
			var cell = base.GetCell(item, reusableCell, tv);

			if (item is CollectionViewCell)
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			return cell;
		}
	}
}

