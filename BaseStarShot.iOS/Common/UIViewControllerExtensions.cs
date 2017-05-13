using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

namespace BaseStarShot
{
	public static class UIViewControllerExtensions
	{
		static nfloat navigatigationBarLeftRightPadding = 16;

		public static void SetBackButtonImage(this PageRenderer pageRenderer, string path)
		{
			if (pageRenderer.NavigationController != null &&
				!pageRenderer.NavigationController.TopViewController.NavigationItem.HidesBackButton &&
				pageRenderer.Element.Navigation.NavigationStack[0] != pageRenderer.Element)
			{
				pageRenderer.NavigationController.TopViewController.NavigationItem.SetHidesBackButton(true, false);

				var backImage = UIImage.FromBundle(path);
				var customBackButton = new UIButton(UIButtonType.Custom);
				customBackButton.SetImage(backImage, UIControlState.Normal);
				customBackButton.SetImage(backImage, UIControlState.Highlighted);
				customBackButton.Frame = new CoreGraphics.CGRect(0, 0, backImage.Size.Width + navigatigationBarLeftRightPadding, 44);
				customBackButton.ContentEdgeInsets = new UIEdgeInsets(0, navigatigationBarLeftRightPadding, 0, 0);
				customBackButton.AddTarget((sender, args) => 
				{
					var page = ((Page)pageRenderer.Element);
					if (page.Navigation != null)
					{
						page.SendBackButtonPressed();
						page.Navigation.PopAsync(true);
					}
				}, UIControlEvent.TouchUpInside);

				var barBackButton = new UIBarButtonItem(customBackButton);

				// Remove left padding of back button
				var negativeSeparator = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
				negativeSeparator.Width = -navigatigationBarLeftRightPadding;

				pageRenderer.NavigationController.TopViewController.NavigationItem.SetLeftBarButtonItems(new[] { negativeSeparator, barBackButton }, false);
			}
		}

		public static void HideBackButton(this PageRenderer pageRenderer, bool value)
		{
			foreach (var item in pageRenderer.NavigationController.TopViewController.NavigationItem.LeftBarButtonItems)
			{
				if (item.CustomView != null)
				{
					item.CustomView.Hidden = value;
				}
			}
		}
	}
}

