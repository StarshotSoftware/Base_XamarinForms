using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace BaseStarShot
{
    public static class UIButtonExtensions
    {
        public static void ApplyTiltingEffect(this UIButton button)
        {
            button.ContentEdgeInsets = UIEdgeInsets.Zero;

            button.TouchUpInside += EdgeInsetNormal;
            button.TouchUpOutside += EdgeInsetNormal;
            button.TouchDragOutside += EdgeInsetNormal;
            button.TouchDown += EdgeInsetHighlight;
        }

        public static void RemoveTiltingEffect(this UIButton button)
        {
            button.TouchUpInside -= EdgeInsetNormal;
            button.TouchUpOutside -= EdgeInsetNormal;
            button.TouchDragOutside -= EdgeInsetNormal;
            button.TouchDown -= EdgeInsetHighlight;
        }

        static void EdgeInsetNormal(object sender, System.EventArgs e)
        {
            ((UIButton)sender).ContentEdgeInsets = UIEdgeInsets.Zero;
        }

        static void EdgeInsetHighlight(object sender, System.EventArgs e)
        {
            ((UIButton)sender).ContentEdgeInsets = new UIEdgeInsets(4, 0, 0, 0);
        }
    }
}