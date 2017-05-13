using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using SDWebImage;
using Foundation;
using TwinTechs.Controls;
using Xamarin.Forms;
using TwinTechs.Ios.Controls;

[assembly: ExportRenderer(typeof(FastImage), typeof(FastImageRenderer))]
namespace TwinTechs.Ios.Controls
{
    public class FastImageRenderer : ImageRenderer, IFastImageProvider
    {
        protected FastImage BaseControl { get { return ((FastImage)this.Element); } }

        public FastImageRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            //			if (e.OldElement != null) {
            //				((FastImage)e.OldElement).ImageProvider = null;
            //			}
            if (e.NewElement != null)
            {
                var fastImage = e.NewElement as FastImage;
                SetImageUrl(fastImage.ImageUrl);
                this.SetCornerRadius();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "ImageUrl")
            {
                var fastImage = Element as FastImage;
                SetImageUrl(fastImage.ImageUrl);
            }

            if (e.PropertyName == "CornerRadius")
            {
                this.SetCornerRadius();
                return;
            }
        }


        private void SetCornerRadius()
        {
            base.Control.Layer.CornerRadius = (nfloat)BaseControl.CornerRadius;
            if (BaseControl.BorderColor != Xamarin.Forms.Color.Default)
            {
                base.Control.Layer.BorderColor = BaseControl.BorderColor.ToCGColor();
                base.Control.Layer.BorderWidth = 1;
            }
        }

        #region FastImageProvider implementation

        bool? IsValidUrl(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString))
                return null;

            Uri uriResult;
            bool result = Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out uriResult);
                //&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (uriResult == null)
                return null;

            if (uriResult.IsAbsoluteUri)
                return true;

            return false;
        }

        public void SetImageUrl(string imageUrl)
        {
            if (Control == null || Element == null)
            {
                return;
            }

            UIImage placeholder = null;
            if (BaseControl.ImagePlaceholder != null && BaseControl.ImagePlaceholder is FileImageSource)
            {
                if (!string.IsNullOrWhiteSpace(((FileImageSource)BaseControl.ImagePlaceholder).File))
                    placeholder = UIImage.FromBundle(((FileImageSource)BaseControl.ImagePlaceholder).File);
            }

            if (IsValidUrl(imageUrl) == null)
            {
                Control.Image = placeholder;
                return;
            }

            if (!IsValidUrl(imageUrl).Value)
            {
                Control.Image = UIImage.FromBundle(imageUrl); ;
                return;
            }
			var test = new NSUrl(imageUrl);
            if (imageUrl != null)
            {
                Control.SetImage(
                    url: new NSUrl(imageUrl),
                    placeholder: placeholder
                );
            }
            else
            {
                Control.Image = placeholder; 
            }
        }

        #endregion
    }
}

