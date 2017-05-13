using System;

using Xamarin.Forms;
using System.Diagnostics;

namespace TwinTechs.Controls
{
    public interface IFastImageProvider
    {
        void SetImageUrl(string imageUrl);
    }

    public class FastImage : Image
    {

        public FastImage()
        {

        }
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create<FastImage, double>(p => p.CornerRadius, 0);

        public static readonly BindableProperty ImagePlaceholderProperty = BindableProperty.Create<FastImage, ImageSource>(p => p.ImagePlaceholder, null);

        public static readonly BindableProperty ImageUrlProperty = BindableProperty.Create<FastImage, string>(w => w.ImageUrl, null);

        public static readonly BindableProperty TagProperty = BindableProperty.Create<FastImage, object>(w => w.Tag, null);

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create<FastImage, Color>(w => w.BorderColor, Color.Default);

        /// <summary>
        /// sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set
            {
                SetValue(ImageUrlProperty, value);
            }
        }

        public object Tag
        {
            get { return (object)GetValue(TagProperty); }
            set
            {
                SetValue(TagProperty, value);
            }

        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public ImageSource ImagePlaceholder
        {
            get { return (ImageSource)base.GetValue(ImagePlaceholderProperty); }
            set { base.SetValue(ImagePlaceholderProperty, value); }
        }

        public double CornerRadius
        {
            get { return (double)base.GetValue(CornerRadiusProperty); }
            set { base.SetValue(CornerRadiusProperty, value); }
        }
    }

}


