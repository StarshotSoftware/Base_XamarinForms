using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ActivityIndicatorControlRenderer : ActivityIndicatorRenderer
    {
        const string RotationKey = "ImageRotation";
        const double Speed = 4.0d;
		UIImageView image;

		Page page;
		NavigationPage navigPage;

		public ActivityIndicatorControlRenderer()
		{
			LifecycleEvents.Sleep += OnSleep;
			LifecycleEvents.Resume += OnResume;
		}

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ActivityIndicator> e)
        {
            base.OnElementChanged(e);

			if (e.OldElement == null)
			{
				AttachPageEvents(e.NewElement);
			}

            if (e.OldElement != null || Element == null)
                return;

            var element = (BaseStarShot.Controls.ActivityIndicator)Element;

            SetImage(element);
			RenderAnimation(element.IsRunning);
        }

		void OnSleep(object sender, EventArgs e)
		{
			if (Element != null)
				RenderAnimation(false);
		}

		void OnResume(object sender, EventArgs e)
		{
			if (Element != null)
			{
				var element = (BaseStarShot.Controls.ActivityIndicator)Element;
				RenderAnimation(element.IsRunning);
			}
		}

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var element = (ActivityIndicator)Element;

            if (e.PropertyName == ActivityIndicator.ImageProperty.PropertyName)
            {
                SetImage(element);
            }
            else if (e.PropertyName == Xamarin.Forms.ActivityIndicator.IsRunningProperty.PropertyName)
            {
				RenderAnimation(element.IsRunning);
            }
        }

        protected void SetImage(ActivityIndicator element)
        {
            if (!string.IsNullOrEmpty(element.Image))
            {
				if (image != null)
				{
					image.RemoveFromSuperview();
					image.Dispose();
				}

                image = new UIImageView(UIImage.FromBundle(element.Image));
                if (element.WidthRequest > 0)
                    image.Frame = new CoreGraphics.CGRect(0, 0, (nfloat)element.WidthRequest, (nfloat)element.HeightRequest);
                image.Hidden = true;
                Control.Superview.AddSubview(image);

				// disables/hides the display of default loader
				Control.Color = Color.Transparent.ToUIColor();
            }
            else
            {
                Control.Hidden = false;
                image.RemoveFromSuperview();
                image.Dispose();
                image = null;
            }
        }

		void RenderAnimation(bool animate)
		{
			if (image != null)
			{
				if (animate)
				{
					if (image.Frame.Width <= 0)
						image.Frame = Control.Frame;
					image.Hidden = false;
					Control.Hidden = true;

					var rotationAnimation = new CoreAnimation.CABasicAnimation();
					rotationAnimation.KeyPath = "transform.rotation.z";
					rotationAnimation.To = new NSNumber(Math.PI / 2d * Speed);
					rotationAnimation.Duration = 1d;
					rotationAnimation.Cumulative = true;
					rotationAnimation.RepeatCount = float.MaxValue;

					if (image.Layer.AnimationForKey(RotationKey) != null)
					{
						image.Layer.RemoveAnimation(RotationKey);
					}

					image.Layer.AddAnimation(rotationAnimation, RotationKey);
				}
				else
				{
					image.Hidden = true;

					if (image.Layer.AnimationForKey(RotationKey) != null)
					{
						image.Layer.RemoveAnimation(RotationKey);
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			LifecycleEvents.Sleep -= OnSleep;
			LifecycleEvents.Resume -= OnResume;
		}

		private void AttachPageEvents(Element element)
		{
			page = GetContainingPage(element);
			if (page == null)
			{
				var root = GetRootElement(element);
				root.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "Parent")
					{
						AttachPageEvents(root);
					}
				};
				return;
			}

			if (page.Parent is TabbedPage)
			{
				page.Appearing += (object sender, EventArgs e) => 
				{

				};
				return;
			}

			if (navigPage != null)
			{
				if (navigPage.Parent is TabbedPage)
				{
					page.Appearing += (object sender, EventArgs e) => 
					{
						if (image != null && !image.Hidden)
							RenderAnimation(true);
					};
				}
			}
		}

		private NavigationPage GetContainingNavigationPage(Element element)
		{
			Element parentElement = element.Parent;

			if (parentElement == null)
				return null;

			if (typeof(NavigationPage).IsAssignableFrom(parentElement.GetType()))
				return (NavigationPage)parentElement;
			else
				return GetContainingNavigationPage(parentElement);
		}

		private VisualElement GetRootElement(Xamarin.Forms.Element element)
		{
			VisualElement parentElement = element.ParentView;

			while (parentElement.ParentView != null)
				parentElement = parentElement.ParentView;

			return parentElement;
		}

		private Page GetContainingPage(Xamarin.Forms.Element element)
		{
			Element parentElement = element.ParentView;

			if (parentElement == null)
				return null;

			if (typeof(Page).IsAssignableFrom(parentElement.GetType()))
				return (Page)parentElement;
			else
				return GetContainingPage(parentElement);
		}
    }
}