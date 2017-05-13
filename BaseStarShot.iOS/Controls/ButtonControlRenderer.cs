using BaseStarShot;
using BaseStarShot.Controls;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using Foundation;
using System.Diagnostics;
using CoreAnimation;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Button), typeof(ButtonControlRenderer))]
namespace BaseStarShot.Controls
{
    public class ButtonControlRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            if (base.Control == null)
            {
                SetNativeControl(UIButton.FromType(UIButtonType.Custom));
                Control.TouchUpInside += OnTouchUpInside;
            }

            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var baseElement = Element as BaseStarShot.Controls.Button;
            if (baseElement != null)
            {
                baseElement.PropertyChanged += (s, ev) =>
                {
                    if (Control == null || Element == null)
                        return;

                    var element = (BaseStarShot.Controls.Button)s;
                    switch (ev.PropertyName)
                    {
                        case "FontFamily":
                        case "FontSize": SetFont(element); break;
                        case "TextColor": 
                        case "DisabledTextColor": SetTextColor(element); break;
                        case "BackgroundImage": SetBackgroundImage(element); break;
                        case "BackgroundColor":
                            Control.BackgroundColor = Color.Transparent.ToUIColor();
                            Control.BackgroundColor = element.BackgroundColor.ToUIColor();
                            break;
                        case "BorderColor":
                            SetBorderColor();
                            break;
                        case "HighlightedBackgroundImage": SetHighlightedBackgroundImage(element); break;
                        case "BackgroundImagePattern": SetBackgroundImagePattern(element); break;
                        case "HighlightedBackgroundImagePattern": SetHighlightedBackgroundImagePattern(element); break;
                        case "TiltEnabled": SetTilting(element); break;
                        case "Padding": SetPadding(element); break;
                        case "TextPadding": SetTextPadding(element); break;
                        case "XAlign": SetXAlign(element); break;
                        case "YAlign": SetYAlign(element); break;
                        case "Image":
                        case "ImageLeft":
                        case "ImageRight":
                        case "ImageTop":
                        case "ImageBottom": SetImages(element); break;
                        case "RefitTextEnabled":
                        case "MinimumFontSize": SetAutoShrink(element); break;
                        case "TextLineBreakMode": SetTextLineBreakMode(element); break;
                        case "CornerRadius": SetCornerRadius(element); break;
                        case "IsEnabled": IdentifyBackgroundColor(element); break;
                    }
                };

                SetFont(baseElement);
                SetTextColor(baseElement);
                SetBackgroundImage(baseElement);
                SetHighlightedBackgroundImage(baseElement);
                //SetImages(baseElement);
                SetBackgroundImagePattern(baseElement);
                SetHighlightedBackgroundImagePattern(baseElement);
                SetTilting(baseElement);
                SetPadding(baseElement);
                SetTextPadding(baseElement);
                SetXAlign(baseElement);
                SetYAlign(baseElement);
                SetAutoShrink(baseElement);
                SetTextLineBreakMode(baseElement);
                SetLongPressEvent(baseElement);
                IdentifyBackgroundColor(baseElement);

                SetBorderColor();
            }
        }

        void SetBorderColor()
        {
            if (Element.BorderColor != Xamarin.Forms.Color.Default)
            {
                var color = Element.BorderColor.ToCGColor();
                Control.Layer.RenderButtonBorder(color);
            }
        }

        void IdentifyBackgroundColor(BaseStarShot.Controls.Button element)
        {
            if (Control == null)
                return;

            if (element.IsEnabled)
                Control.BackgroundColor = element.BackgroundColor.ToUIColor();
            else
                Control.BackgroundColor = element.DisabledTextColor.ToUIColor();
        }

        void OnTouchUpInside(object sender, EventArgs e)
        {
            if (Element != null)
            {
				Element?.Raise ("Clicked", EventArgs.Empty);
                var command = Element?.Command;
                if (command != null)
                    command.Execute(Element.CommandParameter);
            }

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var baseElement = Element as BaseStarShot.Controls.Button;
            if (baseElement != null)
            {
                SetAutoShrink(baseElement);
                SetImages(baseElement);

                if (Control?.Frame.Width > 0)
                {
                    SetCornerRadius(baseElement);
                }
            }
        }

        /// <summary>
        /// Sets the button image. 
        /// In iOS, only one image can display per button. Image prioritization follows: Left, Right, Top, Bottom.
        /// </summary>
        /// <param name="element"></param>
        void SetImages(BaseStarShot.Controls.Button element)
        {
            if (Control == null)
                return;

            string imageSource = null;
            if (element.ImageLeft != null && !string.IsNullOrEmpty(element.ImageLeft.File))
                imageSource = element.ImageLeft.File;
            else if (element.ImageRight != null && !string.IsNullOrEmpty(element.ImageRight.File))
                imageSource = element.ImageRight.File;
            else if (element.ImageTop != null && !string.IsNullOrEmpty(element.ImageTop.File))
                imageSource = element.ImageTop.File;
            else if (element.ImageBottom != null && !string.IsNullOrEmpty(element.ImageBottom.File))
                imageSource = element.ImageBottom.File;
            else if (element.Image != null)
                imageSource = element.Image.File;

            UIImage image = imageSource != null ? UIImage.FromBundle(imageSource) : null;
            if (image != null)
            {
                Control.AdjustsImageWhenHighlighted = false;
                if (element.ImageRightWidth > 0 && element.ImageRightHeight > 0)
                {
                    UIGraphics.BeginImageContextWithOptions(new CGSize(element.ImageRightWidth, element.ImageRightHeight), false, 0);
                    image.Draw(new CGRect(0, 0, element.ImageRightWidth, element.ImageRightHeight));
                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();

                    Control.SetImage(resizedImage, UIControlState.Normal);
                }
                else if (element.ImageLeftWidth > 0 && element.ImageLeftHeight > 0)
                {
                    UIGraphics.BeginImageContextWithOptions(new CGSize(element.ImageLeftWidth, element.ImageLeftHeight), false, 0);
                    image.Draw(new CGRect(0, 0, element.ImageLeftWidth, element.ImageLeftHeight));
                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();

                    Control.SetImage(resizedImage, UIControlState.Normal);
                }
                else
                {
                    var maxWidth = element.Padding.Left - element.Padding.Right - Control.Frame.Size.Width;
                    var maxHeight = element.Padding.Top - element.Padding.Bottom - Control.Frame.Size.Height;

                    var imageSize = GetSize(image.Size.Width, image.Size.Height, (nfloat)Math.Abs(maxWidth), (nfloat)Math.Abs(maxHeight));
                    //new CGSize(image.Size.Width * PointSize.Multiplier, image.Size.Height * PointSize.Multiplier);
                    UIGraphics.BeginImageContextWithOptions(imageSize, false, 0);
                    image.Draw(new CGRect(0, 0, imageSize.Width, imageSize.Height));
                    var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                    UIGraphics.EndImageContext();

                    Control.SetImage(resizedImage, UIControlState.Normal);

                    //if (Control.ImageView != null)
                    //{
                    //    Control.ImageView.Frame = new CGRect(0, 0, imageSize.Width, imageSize.Height);
                    //}

                    // Not yet sure if this is acceptable to other apps.
                    // Please make this configurable if needs to disable this.
                    Control.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                }
            }
            else
            {
                Control.SetImage(image, UIControlState.Normal);

                Control.ContentEdgeInsets = new UIEdgeInsets(0, 0, 0, 0);
            }

            if (element.ImageRight != null && element.ImageRight.File != null)
            {
                nfloat topPadding = (nfloat)element.Padding.Top;
                nfloat bottomPadding = (nfloat)element.Padding.Bottom;

                if (!String.IsNullOrWhiteSpace(element.Text))
                {
                    Control.TitleEdgeInsets = new UIEdgeInsets(topPadding, -Control.ImageView.Frame.Size.Width - 10,
                        bottomPadding, Control.ImageView.Frame.Size.Width);

                    Control.ImageEdgeInsets = new UIEdgeInsets(topPadding, Control.TitleLabel.Frame.Size.Width,
                        bottomPadding, -Control.TitleLabel.Frame.Size.Width);
                }
                else
                {
                    Control.ImageEdgeInsets = new UIEdgeInsets(topPadding, 0, bottomPadding, 0);
                }
            }

			if (element.Image != null && element.Image.File != null)
			{
				Control.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			}
			else if (element.ImageLeft != null && element.ImageLeft.File != null)
			{
				Control.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, 20);
			}
        }


        private CGSize GetSize(nfloat imageWidth, nfloat imageHeight, nfloat maxWidth, nfloat maxHeight)
        {
            CGSize size = new CGSize();
            if (maxHeight > 0 && maxWidth > 0 && imageWidth > maxWidth && imageHeight > maxHeight)
            {
                var ratioBitmap = imageWidth / imageHeight;
                var ratioMax = maxWidth / maxHeight;

                var finalWidth = maxWidth;
                var finalHeight = maxHeight;
                if (ratioMax > 1)
                {
                    finalWidth = maxHeight * ratioBitmap;
                }
                else
                {
                    finalHeight = maxWidth / ratioBitmap;
                }

                size.Width = finalWidth;
                size.Height = finalHeight;
            }
            else
            {
                size.Width = imageWidth;
                size.Height = imageHeight;
            }

            return size;
        }

        void SetXAlign(BaseStarShot.Controls.Button element)
        {
            Control.HorizontalAlignment = UIHelper.ConvertToHorizontalAlignment(element.XAlign);

            if (element.XAlign == Xamarin.Forms.TextAlignment.Start)
                Control.TitleLabel.TextAlignment = UITextAlignment.Left;
            else if (element.XAlign == Xamarin.Forms.TextAlignment.Center)
                Control.TitleLabel.TextAlignment = UITextAlignment.Center;
            else if (element.XAlign == Xamarin.Forms.TextAlignment.End)
                Control.TitleLabel.TextAlignment = UITextAlignment.Right;
        }

        void SetYAlign(BaseStarShot.Controls.Button element)
        {
            Control.VerticalAlignment = UIHelper.ConvertToVerticalAlignment(element.YAlign);
        }

        void SetFont(BaseStarShot.Controls.Button element)
        {
            if (!string.IsNullOrWhiteSpace(element.FontFamily))
                Control.Font = UIFont.FromName(element.FontFamily, (nfloat)element.FontSize);
        }

        void SetTextColor(BaseStarShot.Controls.Button element)
        {
            Control.SetTitleColor(element.TextColor.ToUIColor(), UIControlState.Normal);
            Control.SetTitleColor(element.DisabledTextColor.ToUIColor(), UIControlState.Disabled);
        }

        protected virtual void SetBackgroundImage(BaseStarShot.Controls.Button element)
        {
            if (element.BackgroundImage != null && !string.IsNullOrEmpty(element.BackgroundImage.File))
            {
                var image = UIImage.FromBundle(element.BackgroundImage.File);
                if (image == null)
                    throw new ArgumentException("Cannot find image '" + element.BackgroundImage.File + "'.");

                Control.SetBackgroundImage(image, UIControlState.Normal);
            }
            else
            {
                if (element.BackgroundImagePattern == null || string.IsNullOrEmpty(element.BackgroundImagePattern.File))
                    Control.SetBackgroundImage(null, UIControlState.Normal);
            }
        }

        void SetHighlightedBackgroundImage(BaseStarShot.Controls.Button element)
        {
            if (element.HighlightedBackgroundImage != null && !string.IsNullOrEmpty(element.HighlightedBackgroundImage.File))
            {
                var image = UIImage.FromBundle(element.HighlightedBackgroundImage.File);

                Control.SetBackgroundImage(image, UIControlState.Highlighted);
            }
        }

        void SetBackgroundImagePattern(BaseStarShot.Controls.Button element)
        {
            if (element.BackgroundImagePattern != null && !string.IsNullOrEmpty(element.BackgroundImagePattern.File))
            {
                var backgroundImage = UIImage.FromBundle(element.BackgroundImagePattern.File);
                if (backgroundImage == null)
                    throw new ArgumentException("Cannot find image '" + element.BackgroundImagePattern.File + "'.");

                backgroundImage = backgroundImage.CreateResizableImage(new UIEdgeInsets(8, 8 * 2, 8, 8 * 2));

                Control.SetBackgroundImage(backgroundImage, UIControlState.Normal);
            }
        }

        void SetHighlightedBackgroundImagePattern(BaseStarShot.Controls.Button element)
        {
            if (element.HighlightedBackgroundImagePattern != null && !string.IsNullOrEmpty(element.HighlightedBackgroundImagePattern.File))
            {
                var backgroundImage = UIImage.FromBundle(element.HighlightedBackgroundImagePattern.File);
                if (backgroundImage == null)
                    throw new ArgumentException("Cannot find image '" + element.HighlightedBackgroundImagePattern.File + "'.");

                backgroundImage = backgroundImage.CreateResizableImage(new UIEdgeInsets(8, 8 * 2, 8, 8 * 2));

                Control.SetBackgroundImage(backgroundImage, UIControlState.Highlighted);
            }
        }

        void SetTilting(BaseStarShot.Controls.Button element)
        {
            if (element.TiltEnabled)
            {
                Control.ApplyTiltingEffect();
            }
            else
            {
                Control.RemoveTiltingEffect();
            }
        }

        void SetPadding(BaseStarShot.Controls.Button element)
        {
            Control.ContentEdgeInsets = new UIEdgeInsets(
                (nfloat)element.Padding.Top, (nfloat)element.Padding.Left,
                (nfloat)element.Padding.Bottom, (nfloat)element.Padding.Right);
        }

        void SetTextPadding(BaseStarShot.Controls.Button element)
        {
            // spacing between text and image left
            Control.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, (nfloat)element.TextPadding.Right);
            Control.TitleEdgeInsets = new UIEdgeInsets(
                (nfloat)element.TextPadding.Top, (nfloat)element.TextPadding.Left,
                (nfloat)element.TextPadding.Bottom, (nfloat)element.TextPadding.Right);
        }

        void SetAutoShrink(BaseStarShot.Controls.Button element)
        {
            if (Control == null)
                return;

            if (Control.Frame.Width > 0)
            {
                if (element.RefitTextEnabled)
                {
                    Control.TitleLabel.MinimumScaleFactor = (nfloat)(element.MinimumFontSize / element.FontSize);
                    Control.LayoutIfNeeded();
                    Control.TitleLabel.AdjustFontSizeToFit();
                    element.FontSize = Control.Font.PointSize;
                }
            }
        }

        void SetTextLineBreakMode(BaseStarShot.Controls.Button element)
        {
            if (element.TextLineBreakMode == LineBreakMode.NoWrap)
                Control.TitleLabel.LineBreakMode = UILineBreakMode.Clip;
            else if (element.TextLineBreakMode == LineBreakMode.WordWrap)
            {
                Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                Control.TitleLabel.Lines = 2;
            }
            else if (element.TextLineBreakMode == LineBreakMode.CharacterWrap)
            {
                Control.TitleLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
                Control.TitleLabel.Lines = 2;
            }
            else if (element.TextLineBreakMode == LineBreakMode.HeadTruncation)
                Control.TitleLabel.LineBreakMode = UILineBreakMode.HeadTruncation;
            else if (element.TextLineBreakMode == LineBreakMode.TailTruncation)
                Control.TitleLabel.LineBreakMode = UILineBreakMode.TailTruncation;
            else
                Control.TitleLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;
        }

        void SetLongPressEvent(BaseStarShot.Controls.Button element)
        {
            var longPress = new UILongPressGestureRecognizer(recognizer =>
                {
                    if (recognizer.State == UIGestureRecognizerState.Began)
                        element.OnLongPress();
                });
            Control.AddGestureRecognizer(longPress);
        }

        protected virtual void SetCornerRadius(BaseStarShot.Controls.Button element)
        {
            if (element.CornerRadius != 0)
            {
                int radius = 0;
                UIRectCorner forCorners = 0;

                if (element.CornerRadius.Bottom > 0 &&
                    element.CornerRadius.Left > 0 &&
                    element.CornerRadius.Right > 0 &&
                    element.CornerRadius.Top > 0)
                {
                    forCorners = UIRectCorner.AllCorners;
                    radius = Convert.ToInt32(element.CornerRadius.Top);
                }
                else
                {
                    if (element.CornerRadius.Bottom > 0)
                    {
                        forCorners = forCorners | UIRectCorner.BottomRight;
                        radius = Convert.ToInt32(element.CornerRadius.Bottom);
                    }
                    if (element.CornerRadius.Left > 0)
                    {
                        forCorners = forCorners | UIRectCorner.BottomLeft;
                        radius = Convert.ToInt32(element.CornerRadius.Left);
                    }
                    if (element.CornerRadius.Right > 0)
                    {
                        forCorners = forCorners | UIRectCorner.TopRight;
                        radius = Convert.ToInt32(element.CornerRadius.Right);
                    }
                    if (element.CornerRadius.Top > 0)
                    {
                        forCorners = forCorners | UIRectCorner.TopLeft;
                        radius = Convert.ToInt32(element.CornerRadius.Top);
                    }
                }

                Control.SetCornerRadius(radius, forCorners);
                Control.Layer.MasksToBounds = true;
                Control.ClipsToBounds = true;
            }
            else
            {
                Control.SetCornerRadius(0, UIRectCorner.AllCorners);
                Control.ClipsToBounds = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.TouchUpInside -= OnTouchUpInside;
            }

            base.Dispose(disposing);
        }
    }
}