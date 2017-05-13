using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using BaseStarShot.Controls;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class TabbedControlRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

			//this.TabBar.Translucent = false;

            SetupBarItems();
        }

        private void SetupBarItems()
        {
            var tabbedPage = (BaseStarShot.Controls.TabbedPage)Element;
            if (tabbedPage.ActiveIcons.Count > TabBar.Items.Count())
                throw new ArgumentException("There are too many active icons specified.");

            for (var i = 0; i < tabbedPage.ActiveIcons.Count; i++)
            {
                var tabBarItem = TabBar.Items[i];

				var normalTextAttributes = new UITextAttributes { TextColor = tabbedPage.TextColor.ToUIColor()};
                tabBarItem.SetTitleTextAttributes(normalTextAttributes, UIControlState.Normal);

				var selectedTextAttributes = new UITextAttributes { TextColor = tabbedPage.SelectedTextColor.ToUIColor() };
                tabBarItem.SetTitleTextAttributes(selectedTextAttributes, UIControlState.Selected);

                var iconNormalSource = tabbedPage.Children[i].Icon;
                if (!string.IsNullOrEmpty(iconNormalSource))
                {
					var iconNormalImage = UIImage.FromFile (iconNormalSource);
					if (iconNormalImage == null)
						iconNormalImage = UIImage.FromBundle(iconNormalSource);

					if (iconNormalImage == null)
						throw new ArgumentException("Cannot find image '" + iconNormalSource + "'.");

					iconNormalImage	= iconNormalImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                    tabBarItem.Image = iconNormalImage;
                }

                var activeIconSource = tabbedPage.ActiveIcons[i];
                if (!string.IsNullOrEmpty(activeIconSource))
                {
					var activeIconImage = UIImage.FromFile (activeIconSource);
					if (activeIconImage == null)
						activeIconImage = UIImage.FromBundle (activeIconSource);

					if (activeIconImage == null)
						throw new ArgumentException("Cannot find image '" + activeIconSource + "'.");

					activeIconImage = activeIconImage.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                    tabBarItem.SelectedImage = activeIconImage;
                }
            }
        }
    }
}