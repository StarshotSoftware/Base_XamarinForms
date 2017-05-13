using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BaseStarShot.Controls
{
    public class ImageControlRenderer : ViewRenderer<BaseStarShot.Controls.Image, UIImageView>
    //Xamarin.Forms.Platform.iOS.ImageRenderer
    {
        private bool isDisposed;

        UIActivityIndicatorView activityIndicator;

        protected override void OnElementChanged(ElementChangedEventArgs<BaseStarShot.Controls.Image> e)
        {
            if (base.Control == null)
            {
                base.SetNativeControl(new UIImageView(CGRect.Empty)
                {
                    ContentMode = UIViewContentMode.ScaleAspectFit,
                    ClipsToBounds = true
                });
            }
            if (e.NewElement != null)
            {
                this.SetAspect();
                this.SetImage(e.OldElement);
                this.SetOpacity();
                this.SetCommand();
                this.SetCornerRadius();
            }
            base.OnElementChanged(e);
        }

		public override void TouchesBegan (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			if (Element.EnableTouchViewListener) {
				UITouch touch = (UITouch)evt.AllTouches.AnyObject;
				if (touch.View is ImageControlRenderer) {
					CGPoint point = touch.LocationInView (touch.View);
					Element.OnTouchEvent (new TouchEventArgs ((double)point.X, (double)point.Y, TouchState.BEGAN));
				}
			}
		}

		public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			if (Element.EnableTouchViewListener) {
				UITouch touch = (UITouch)evt.AllTouches.AnyObject;
				if (touch.View is ImageControlRenderer) {
					CGPoint point = touch.LocationInView (touch.View);
					Element.OnTouchEvent (new TouchEventArgs ((double)point.X, (double)point.Y, TouchState.MOVED));
				}
			}
		}


		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			if (Element.EnableTouchViewListener) {
				UITouch touch = (UITouch)evt.AllTouches.AnyObject;
				if (touch.View is ImageControlRenderer) {
					CGPoint point = touch.LocationInView (touch.View);
					Element.OnTouchEvent (new TouchEventArgs ((double)point.X, (double)point.Y, TouchState.ENDED));
				}
			}
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Element.ShowActivityIndicatorWhileLoading && activityIndicator == null && Control.Frame.Height > 0)
            {
                activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
                activityIndicator.Frame = new CGRect(0, 0, Control.Frame.Width, Control.Frame.Height);
                Control.AddSubview(activityIndicator);
            }

			    this.SetCornerRadius();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                this.SetImage(null);
                return;
            }
            if (e.PropertyName == Image.IsOpaqueProperty.PropertyName)
            {
                this.SetOpacity();
                return;
            }
            if (e.PropertyName == Image.AspectProperty.PropertyName)
            {
                this.SetAspect();
                return;
            }
            if (e.PropertyName == Image.CommandProperty.PropertyName)
            {
                this.SetCommand();
                return;
            }
            if (e.PropertyName == "CornerRadius")
            {
                this.SetCornerRadius();
                return;
            }
        }

        private void SetCornerRadius()
        {

			if (Element.CornerRadius == 80123)
			{

				UIRectCorner forCorners = 0;
				int radius = 0;
			
				forCorners = forCorners | UIRectCorner.TopRight;
				radius = Convert.ToInt32(10);
					
				forCorners = forCorners | UIRectCorner.TopLeft;
				radius = Convert.ToInt32(10);
				base.Control.SetCornerRadius(10, forCorners);
			}
			else
			{
            base.Control.Layer.CornerRadius = (nfloat)base.Element.CornerRadius;

			}
        }

        private void SetAspect()
        {
            base.Control.ContentMode = base.Element.Aspect.ToUIViewContentMode();
        }

        private async void SetImage(Xamarin.Forms.Image oldElement = null)
        {
            var baseElement = Element as BaseStarShot.Controls.Image;
            baseElement.IsImageLoading = true;
            if (baseElement != null && baseElement.ImagePlaceholder is FileImageSource && !string.IsNullOrEmpty(((FileImageSource)baseElement.ImagePlaceholder).File))
                Control.Image = UIImage.FromBundle(((FileImageSource)baseElement.ImagePlaceholder).File);
            else
                Control.Image = null;
			//await Task.Delay (100);

			if (activityIndicator != null)
			{
				activityIndicator.StartAnimating();
			}

            ImageSource source = Element.Source;
            if (oldElement == null || !object.Equals(oldElement.Source, Element.Source))
            {
                if (source != null)
                {
                    try
                    {
                        UIImage image = await source.GetUIImage();

                        if (Element != null && source != Element.Source)
                            return;

                        if (Control != null)
                        {
                            if (baseElement != null && image == null && baseElement.ImagePlaceholder is FileImageSource && !string.IsNullOrEmpty(((FileImageSource)baseElement.ImagePlaceholder).File))
                                Control.Image = UIImage.FromBundle(((FileImageSource)baseElement.ImagePlaceholder).File);
                            else
                            {
                                Control.Image = image;
                                if (activityIndicator != null)
                                {
                                    activityIndicator.StopAnimating();
                                }
                                baseElement.IsImageLoading = false;
                                baseElement.Raise("ImageLoaded", EventArgs.Empty);
                            }
                        }
                        if (!this.isDisposed)
                        {
                            ((IVisualElementController)base.Element).NativeSizeChanged();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (Control != null)
                        {
                            if (baseElement != null && baseElement.ImagePlaceholder is FileImageSource && !string.IsNullOrEmpty(((FileImageSource)baseElement.ImagePlaceholder).File))
                                Control.Image = UIImage.FromBundle(((FileImageSource)baseElement.ImagePlaceholder).File);
                            else
                                Control.Image = null;
                        }
                    }
                }
                else
                {
                    if (baseElement != null && baseElement.ImagePlaceholder is FileImageSource && !string.IsNullOrEmpty(((FileImageSource)baseElement.ImagePlaceholder).File))
                        Control.Image = UIImage.FromBundle(((FileImageSource)baseElement.ImagePlaceholder).File);
                    else
                        Control.Image = null;
                }
            }
        }

        private void SetOpacity()
        {
            base.Control.Opaque = base.Element.IsOpaque;
        }

        UITapGestureRecognizer commandGesture;
        void SetCommand()
        {
            if (Element.Command != null)
            {
                if (commandGesture == null)
                {
                    commandGesture = new UITapGestureRecognizer(() =>
                    {
                        if (!Element.IsEnabled)
                            return;

                        if (Element.Command.CanExecute(Element.CommandParameter))
                            Element.Command.Execute(Element.CommandParameter);
                    });
                }
                Control.AddGestureRecognizer(commandGesture);
                Control.UserInteractionEnabled = true;
            }
            else
            {
                if (commandGesture != null)
                    Control.RemoveGestureRecognizer(commandGesture);
                Control.UserInteractionEnabled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }
            UIImage image;
            if (disposing && base.Control != null && (image = base.Control.Image) != null)
            {
                image.Dispose();
            }
            this.isDisposed = true;
            base.Dispose(disposing);
        }
    }
}

