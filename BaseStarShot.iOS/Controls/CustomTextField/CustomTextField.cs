using System;
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace BaseStarShot
{
    public class CustomTextField : UITextField
    {
        public UIEdgeInsets EdgeInsets { get; set; }
        public bool ShowCursor { get; set; }

        public CustomTextField()
        {
            EdgeInsets = UIEdgeInsets.Zero;
        }

        public CustomTextField(IntPtr intPtr) : base(intPtr)
        {
            EdgeInsets = UIEdgeInsets.Zero;
        }

        public override CGRect GetCaretRectForPosition(UITextPosition position)
        {
            if (ShowCursor)
                return base.GetCaretRectForPosition(position);
            else
                return CGRect.Empty;
        }

        public override CoreGraphics.CGRect TextRect(CoreGraphics.CGRect forBounds)
        {
            return base.TextRect(InsetRect(forBounds, EdgeInsets));
        }

        public override CoreGraphics.CGRect EditingRect(CoreGraphics.CGRect forBounds)
        {
            return base.EditingRect(InsetRect(forBounds, EdgeInsets));
        }

        public override CoreGraphics.CGRect RightViewRect(CoreGraphics.CGRect forBounds)
        {
            return base.RightViewRect(InsetRectRight(forBounds, EdgeInsets));
        }

//        public override CoreGraphics.CGRect LeftViewRect(CoreGraphics.CGRect forBounds)
//        {
//            return base.LeftViewRect(InsetRectLeft(forBounds, EdgeInsets));
//            //return base.LeftViewRect(forBounds);
//        }

        // Workaround until this method is available in Xamarin.iOS
        public static CoreGraphics.CGRect InsetRect(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        {
            return new CoreGraphics.CGRect(rect.X + insets.Left,
                rect.Y + insets.Top,
                rect.Width - insets.Left - insets.Right,
                rect.Height - insets.Top - insets.Bottom);
        }

        public static CoreGraphics.CGRect InsetRectRight(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        {
            return new CoreGraphics.CGRect(rect.X + insets.Left,
                rect.Y + insets.Top,
                rect.Width - (insets.Left + (insets.Right - 5)),
                rect.Height - insets.Top - insets.Bottom);
        }

        public static CoreGraphics.CGRect InsetRectLeft(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        {
            return new CoreGraphics.CGRect(rect.X + insets.Left + 100,
                rect.Y + insets.Top,
                rect.Width - insets.Left - insets.Right - 100,
                rect.Height - insets.Top - insets.Bottom);
        }
    }

    public class CustomTextView : UITextView
    {
        public bool ShowCursor { get; set; }

        public CustomTextView()
        {

        }

        public override CGRect GetCaretRectForPosition(UITextPosition position)
        {
            if (ShowCursor)
                return base.GetCaretRectForPosition(position);
            else
                return CGRect.Empty;
        }
        
        //public override CoreGraphics.CGRect TextRect(CoreGraphics.CGRect forBounds)
        //{
        //    return base.TextRect(InsetRect(forBounds, EdgeInsets));
        //}

        //public override CoreGraphics.CGRect EditingRect(CoreGraphics.CGRect forBounds)
        //{
        //    return base.EditingRect(InsetRect(forBounds, EdgeInsets));
        //}

        //public override CoreGraphics.CGRect RightViewRect(CoreGraphics.CGRect forBounds)
        //{
        //    return base.RightViewRect(InsetRectRight(forBounds, EdgeInsets));
        //}

        //        public override CoreGraphics.CGRect LeftViewRect(CoreGraphics.CGRect forBounds)
        //        {
        //            return base.LeftViewRect(InsetRectLeft(forBounds, EdgeInsets));
        //            //return base.LeftViewRect(forBounds);
        //        }

        // Workaround until this method is available in Xamarin.iOS
        //public static CoreGraphics.CGRect InsetRect(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        //{
        //    return new CoreGraphics.CGRect(rect.X + insets.Left,
        //        rect.Y + insets.Top,
        //        rect.Width - insets.Left - insets.Right,
        //        rect.Height - insets.Top - insets.Bottom);
        //}

        //public static CoreGraphics.CGRect InsetRectRight(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        //{
        //    return new CoreGraphics.CGRect(rect.X + insets.Left,
        //        rect.Y + insets.Top,
        //        rect.Width - (insets.Left + (insets.Right - 5)),
        //        rect.Height - insets.Top - insets.Bottom);
        //}

        //public static CoreGraphics.CGRect InsetRectLeft(CoreGraphics.CGRect rect, UIEdgeInsets insets)
        //{
        //    return new CoreGraphics.CGRect(rect.X + insets.Left + 100,
        //        rect.Y + insets.Top,
        //        rect.Width - insets.Left - insets.Right - 100,
        //        rect.Height - insets.Top - insets.Bottom);
        //}
    }
}

