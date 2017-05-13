using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BaseStarShot.Services;
using Base1902;

namespace BaseStarShot.Controls
{
    public class RoundedComboBox : Xamarin.Forms.View
    {
        public static BindableProperty DisplayTextFieldProperty =
            BindableProperty.Create<RoundedComboBox, string>(p => p.DisplayTextField, "");

        public static BindableProperty TextProperty =
            BindableProperty.Create<RoundedComboBox, string>(p => p.Text, "");

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create<RoundedComboBox, double>(p => p.FontSize, new PointFontSize(6));//Double.NaN);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<RoundedComboBox, FontStyle>(p => p.FontStyle, FontStyle.Regular);

        public static BindableProperty FontFamilyProperty =
            BindableProperty.Create<RoundedComboBox, string>(p => p.FontFamily, "");

        public static BindableProperty BackgroundColorProperty =
            BindableProperty.Create<RoundedComboBox, Color>(p => p.BackgroundColor, Color.Default);

        public static BindableProperty SelectedItemProperty =
            BindableProperty.Create<RoundedComboBox, object>(p => p.SelectedItem, null);

        public static BindableProperty SelectedIndexProperty =
            BindableProperty.Create<RoundedComboBox, int>(p => p.SelectedIndex, -1, BindingMode.TwoWay);

        public static BindableProperty ItemCountProperty =
            BindableProperty.Create<RoundedComboBox, int?>(p => p.ItemCount, null, BindingMode.OneWay);

        public static BindableProperty ItemsPanelBackgroundProperty =
            BindableProperty.Create<RoundedComboBox, Color>(p => p.ItemsPanelBackground, Color.Default);

        public static BindableProperty ItemsSourceProperty =
            BindableProperty.Create<RoundedComboBox, IEnumerable>(p => p.ItemsSource, null);

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create<RoundedComboBox, string>(p => p.Placeholder, "");

        public static BindableProperty PlaceholderColorProperty =
            BindableProperty.Create<RoundedComboBox, Color>(p => p.PlaceholderColor, Color.Default);

        public static BindableProperty TextColorProperty =
            BindableProperty.Create<RoundedComboBox, Color>(p => p.TextColor, Color.Default);

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create<RoundedComboBox, double>(p => p.CornerRadius, new PointSize(15));

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<RoundedComboBox, Color>(p => p.BorderColor, Color.Default);

        public static BindableProperty HorizontalTextAlignmentProperty =
            BindableProperty.Create<RoundedComboBox, Xamarin.Forms.TextAlignment>(p => p.HorizontalTextAlignment, Xamarin.Forms.TextAlignment.Start);

        public static BindableProperty ImageLeftProperty =
            BindableProperty.Create<RoundedComboBox, FileImageSource>(p => p.ImageLeft, null);

        public static BindableProperty ImageRightProperty =
            BindableProperty.Create<RoundedComboBox, FileImageSource>(p => p.ImageRight, null);

        public static BindableProperty ImageRightWidthProperty =
            BindableProperty.Create<RoundedComboBox, double>(p => p.ImageRightWidth, new PointSize(20));

        public static BindableProperty ImageLeftWidthProperty =
            BindableProperty.Create<RoundedComboBox, double>(p => p.ImageRightWidth, new PointSize(20));

        public static BindableProperty ImageLeftHeightProperty =
            BindableProperty.Create<RoundedComboBox, double>(p => p.ImageLeftHeight, new PointSize(20));

        public static BindableProperty TextPaddingProperty =
            BindableProperty.Create<RoundedComboBox, Thickness>(p => p.TextPadding, new PointThickness(10, 5));

        public static BindableProperty IsDialogVisibleProperty =
            BindableProperty.Create<RoundedComboBox, bool>(p => p.IsDialogVisible, defaultBindingMode: BindingMode.OneWayToSource, defaultValue: false);

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayTextField
        {
            get { return (string)GetValue(DisplayTextFieldProperty); }
            set { SetValue(DisplayTextFieldProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public int? ItemCount
        {
            get { return (int?)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }
        public Color ItemsPanelBackground
        {
            get { return (Color)GetValue(ItemsPanelBackgroundProperty); }
            set { SetValue(ItemsPanelBackgroundProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color PlaceholderColor
        {
            get { return (Color)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public Xamarin.Forms.TextAlignment HorizontalTextAlignment
        {
            get { return (Xamarin.Forms.TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        public FileImageSource ImageLeft
        {
            get { return (FileImageSource)GetValue(ImageLeftProperty); }
            set { SetValue(ImageLeftProperty, value); }
        }

        public FileImageSource ImageRight
        {
            get { return (FileImageSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        public double ImageRightWidth
        {
            get { return (double)GetValue(ImageRightWidthProperty); }
            set { SetValue(ImageRightWidthProperty, value); }
        }

        public double ImageLeftWidth
        {
            get { return (double)GetValue(ImageLeftWidthProperty); }
            set { SetValue(ImageLeftWidthProperty, value); }
        }

        public double ImageLeftHeight
        {
            get { return (double)GetValue(ImageLeftHeightProperty); }
            set { SetValue(ImageLeftHeightProperty, value); }
        }

        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public bool IsDialogVisible
        {
            get { return (bool)GetValue(IsDialogVisibleProperty); }
            set { SetValue(IsDialogVisibleProperty, value); }
        }

        public RoundedComboBox()
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
    }
}
