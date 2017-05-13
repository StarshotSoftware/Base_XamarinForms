using Base1902;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class Button : Xamarin.Forms.Button
    {
        /// <summary>
        /// Default button padding.
        /// </summary>
        public static readonly Thickness DefaultPadding =
             ForPlatform.Get(new Thickness(0), new Thickness(0), new Thickness(12d, 8d), new Thickness(12d, 8d), new Thickness(12d, 8d));

        /// <summary>
        /// Default text padding.
        /// </summary>
        public static readonly Thickness DefaultTextPadding = new Thickness(0);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<Button, FontStyle>(p => p.FontStyle, FontStyle.Regular);

        public static BindableProperty DisabledTextColorProperty =
            BindableProperty.Create<Button, Color>(p => p.DisabledTextColor, Color.Default);

        public static BindableProperty DisabledBackgroundColorProperty =
            BindableProperty.Create<Button, Color>(p => p.DisabledBackgroundColor, Color.Default);

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create<Button, Thickness>(p => p.CornerRadius, 0);

        public static BindableProperty ImageHeightProperty =
            BindableProperty.Create<Button, double>(p => p.ImageHeight, new PointSize(20));

        public static BindableProperty BackgroundImageProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.BackgroundImage, null);

        public static BindableProperty HighlightedBackgroundImageProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.HighlightedBackgroundImage, null);

        public static BindableProperty BackgroundImagePatternProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.BackgroundImagePattern, null);

        public static BindableProperty HighlightedBackgroundImagePatternProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.HighlightedBackgroundImagePattern, null);

        public static BindableProperty ImageLeftProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.ImageLeft, null);

        public static BindableProperty SelectedImageLeftProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.SelectedImageLeft, null);

        public static BindableProperty ImageTopProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.ImageTop, null);

        public static BindableProperty ImageRightProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.ImageRight, null);

        public static BindableProperty ImageBottomProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.ImageBottom, null);

        public static BindableProperty SelectedImageRightProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.SelectedImageRight, null);

        public static BindableProperty PaddingProperty =
            BindableProperty.Create<Button, Thickness>(p => p.Padding, 0);//DefaultPadding);

        public static BindableProperty TextPaddingProperty =
            BindableProperty.Create<Button, Thickness>(p => p.TextPadding, DefaultTextPadding);

        public static BindableProperty ImageRightHeightProperty =
            BindableProperty.Create<Button, double>(p => p.ImageRightHeight, Double.NaN);

        public static BindableProperty ImageRightWidthProperty =
            BindableProperty.Create<Button, double>(p => p.ImageRightWidth, Double.NaN);

        public static BindableProperty ImageLeftHeightProperty =
            BindableProperty.Create<Button, double>(p => p.ImageLeftHeight, Double.NaN);

        public static BindableProperty ImageLeftWidthProperty =
            BindableProperty.Create<Button, double>(p => p.ImageLeftWidth, Double.NaN);

        public static BindableProperty TiltEnabledProperty =
            BindableProperty.Create<Button, bool>(p => p.TiltEnabled, false);

        public static BindableProperty CenterImageProperty =
            BindableProperty.Create<Button, bool>(p => p.CenterImage, false);

        public static BindableProperty RefitTextEnabledProperty =
            BindableProperty.Create<Button, bool>(p => p.RefitTextEnabled, false);

        public static BindableProperty MinimumFontSizeProperty =
            BindableProperty.Create<Button, double>(p => p.MinimumFontSize, new PointSize(8));

        public static BindableProperty TextLineBreakModeProperty =
            BindableProperty.Create<Button, LineBreakMode>(p => p.TextLineBreakMode, LineBreakMode.MiddleTruncation);

        public static BindableProperty XAlignProperty =
            BindableProperty.Create<Button, Xamarin.Forms.TextAlignment>(p => p.XAlign, Xamarin.Forms.TextAlignment.Center);

        public static BindableProperty YAlignProperty =
            BindableProperty.Create<Button, Xamarin.Forms.TextAlignment>(p => p.YAlign, Xamarin.Forms.TextAlignment.Center);


        public static BindableProperty BorderlessProperty =
            BindableProperty.Create<Button, bool>(p => p.Borderless, false);

        //		public static BindableProperty BorderRadiusProperty =
        //			BindableProperty.Create<Button, double>(p => p.BorderRadius, 0f);


        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        public Xamarin.Forms.TextAlignment XAlign
        {
            get { return (Xamarin.Forms.TextAlignment)GetValue(XAlignProperty); }
            set { SetValue(XAlignProperty, value); }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment.
        /// </summary>
        public Xamarin.Forms.TextAlignment YAlign
        {
            get { return (Xamarin.Forms.TextAlignment)GetValue(YAlignProperty); }
            set { SetValue(YAlignProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public Thickness CornerRadius
        {
            get { return (Thickness)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Color DisabledTextColor
        {
            get { return (Color)GetValue(DisabledTextColorProperty); }
            set { SetValue(DisabledTextColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the disabled background.
        /// </summary>
        public Color DisabledBackgroundColor
        {
            get { return (Color)GetValue(DisabledBackgroundColorProperty); }
            set { SetValue(DisabledBackgroundColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets imageheight of drawables. Used in android only
        /// </summary>
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image or resource to be used as the background of the button.
        /// </summary>
        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        /// <summary>
        /// For iOS only. Gets or sets the image or resource to be used as the background of the button when highlighted.
        /// </summary>
        public FileImageSource HighlightedBackgroundImage
        {
            get { return (FileImageSource)GetValue(HighlightedBackgroundImageProperty); }
            set { SetValue(HighlightedBackgroundImageProperty, value); }
        }

        /// <summary>
        /// For iOS only. Gets or sets the image or resource to be used as the background pattern of the button.
        /// </summary>
        public FileImageSource BackgroundImagePattern
        {
            get { return (FileImageSource)GetValue(BackgroundImagePatternProperty); }
            set { SetValue(BackgroundImagePatternProperty, value); }
        }

        /// <summary>
        /// For iOS only. Gets or sets the image or resource to be used as the background pattern of the button when highlighted.
        /// </summary>
        public FileImageSource HighlightedBackgroundImagePattern
        {
            get { return (FileImageSource)GetValue(HighlightedBackgroundImagePatternProperty); }
            set { SetValue(HighlightedBackgroundImagePatternProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image or resource displayed on the left side of the text.
        /// </summary>
        public FileImageSource ImageLeft
        {
            get { return (FileImageSource)GetValue(ImageLeftProperty); }
            set { SetValue(ImageLeftProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image or resource displayed on top of the text.
        /// </summary>
        public FileImageSource ImageTop
        {
            get { return (FileImageSource)GetValue(ImageTopProperty); }
            set { SetValue(ImageTopProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image or resource displayed on the right side of the text.
        /// </summary>
        public FileImageSource ImageRight
        {
            get { return (FileImageSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image or resource displayed below the text.
        /// </summary>
        public FileImageSource ImageBottom
        {
            get { return (FileImageSource)GetValue(ImageBottomProperty); }
            set { SetValue(ImageBottomProperty, value); }
        }

        public FileImageSource SelectedImageRight
        {
            get { return (FileImageSource)GetValue(SelectedImageRightProperty); }
            set { SetValue(SelectedImageRightProperty, value); }
        }

        public FileImageSource SelectedImageLeft
        {
            get { return (FileImageSource)GetValue(SelectedImageLeftProperty); }
            set { SetValue(SelectedImageLeftProperty, value); }
        }
        /// <summary>
        /// Gets or sets the image or resource displayed at the center.
        /// </summary>
        //public FileImageSource ImageCenter
        //{
        //    get { return (FileImageSource)GetValue(ImageCenterProperty); }
        //    set { SetValue(ImageCenterProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the padding within the button.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the padding between the text and image inside the button.
        /// </summary>
        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public double ImageRightHeight
        {
            get { return (double)GetValue(ImageRightHeightProperty); }
            set { SetValue(ImageRightHeightProperty, value); }
        }

        public double ImageRightWidth
        {
            get { return (double)GetValue(ImageRightWidthProperty); }
            set { SetValue(ImageRightWidthProperty, value); }
        }

        public double ImageLeftHeight
        {
            get { return (double)GetValue(ImageLeftHeightProperty); }
            set { SetValue(ImageLeftHeightProperty, value); }
        }

        public double ImageLeftWidth
        {
            get { return (double)GetValue(ImageLeftWidthProperty); }
            set { SetValue(ImageLeftWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the tilt effect in windows.
        /// Default to true.
        /// </summary>
        public bool TiltEnabled
        {
            get { return (bool)GetValue(TiltEnabledProperty); }
            set { SetValue(TiltEnabledProperty, value); }
        }

        /// <summary>
        /// Set if image is center aligned. For android use only
        /// </summary>
        public bool CenterImage
        {
            get { return (bool)GetValue(CenterImageProperty); }
            set { SetValue(CenterImageProperty, value); }
        }

        /// <summary>
        /// Indicates whether the button's text will resize to fit with the button's width.
        /// </summary>
        public bool RefitTextEnabled
        {
            get { return (bool)GetValue(RefitTextEnabledProperty); }
            set { SetValue(RefitTextEnabledProperty, value); }
        }

        /// <summary>
        /// The minimum font size of the button.
        /// Will be ignored if RefitTextEnabled property is set to false.
        /// </summary>
        public double MinimumFontSize
        {
            get { return (double)GetValue(MinimumFontSizeProperty); }
            set { SetValue(MinimumFontSizeProperty, value); }
        }

        /// <summary>
        /// The line break mode of the button.
        /// </summary>
        public LineBreakMode TextLineBreakMode
        {
            get { return (LineBreakMode)GetValue(TextLineBreakModeProperty); }
            set { SetValue(TextLineBreakModeProperty, value); }
        }

        /// <summary>
        /// Removes border on windows.
        /// </summary>
        public bool Borderless
        {
            get { return (bool)GetValue(BorderlessProperty); }
            set { SetValue(BorderlessProperty, value); }
        }

        public Button()
        {
            SetFontFamily();
            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "FontStyle")
                        SetFontFamily();
                };
        }

        protected void SetFontFamily()
        {
            var fontService = Resolver.Get<IFontService>();
            if (fontService == null) throw new InvalidOperationException("IFontService is not implemented.");
            this.FontFamily = fontService.GetFontName(this.FontStyle);
        }

        public event EventHandler LongPress;

        public void OnLongPress()
        {
            EventHandler handler = LongPress;
            if (handler != null)
                LongPress(this, null);
        }
    }
}
