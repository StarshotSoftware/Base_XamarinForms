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
    public class Label : Xamarin.Forms.Label
    {
        public static BindableProperty TagProperty =
            BindableProperty.Create<Label, object>(l => l.Tag, null);

        public static BindableProperty AutoFitProperty =
            BindableProperty.Create<Label, bool>(l => l.AutoFit, false);

        public static BindableProperty CommandProperty =
            BindableProperty.Create<Label, System.Windows.Input.ICommand>(l => l.Command, null);

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create<Label, object>(l => l.CommandParameter, null);

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<Label, Color>(l => l.BorderColor, Color.Default);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<Label, FontStyle>(l => l.FontStyle, FontStyle.Regular);

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create<Label, string>(l => l.Placeholder, null);

        public static BindableProperty PlaceholderColorProperty =
            BindableProperty.Create<Label, Color>(l => l.PlaceholderColor, Color.Default);

        //public static BindableProperty MaxCharactersProperty = BindableProperty.Create<Label, int?>(l => l.MaxCharacters, null);

        public static BindableProperty MaxLinesProperty =
            BindableProperty.Create<Label, int>(l => l.MaxLines, 0);

        public static BindableProperty MaxWidthProperty =
            BindableProperty.Create<Label, double?>(l => l.MaxWidth, null);

        public static BindableProperty TextStyleProperty =
            BindableProperty.Create<Label, TextStyling>(l => l.TextStyle, TextStyling.None);

        public static BindableProperty RefitTextEnabledProperty =
            BindableProperty.Create<Label, bool>(p => p.RefitTextEnabled, false);

        public static BindableProperty RequiredProperty =
          BindableProperty.Create<Label, bool>(p => p.Required, false);

        public static BindableProperty InputTypeProperty =
            BindableProperty.Create<Label, TextInputType>(p => p.InputType, TextInputType.None);

        public static BindableProperty PaddingProperty =
            BindableProperty.Create<Label, Thickness>(p => p.Padding, new PointThickness(0));

        public bool Required
        {
            get { return (bool)GetValue(RequiredProperty); }
            set { SetValue(RequiredProperty, value); }
        }

        public bool RefitTextEnabled
        {
            get { return (bool)GetValue(RefitTextEnabledProperty); }
            set { SetValue(RefitTextEnabledProperty, value); }
        }

        public TextInputType InputType
        {
            get { return (TextInputType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }

        /// <summary>
        /// Note: This is not implemented in iOS. iOS Label does not have padding property or alike.
        /// </summary>
        /// <value>The padding.</value>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public System.Windows.Input.ICommand Command
        {
            get { return (System.Windows.Input.ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object Tag
        {
            get { return (bool)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        public bool AutoFit
        {
            get { return (bool)GetValue(AutoFitProperty); }
            set { SetValue(AutoFitProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
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

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        //public int? MaxCharacters
        //{
        //    get { return (int?)GetValue(MaxCharactersProperty); }
        //    set { SetValue(MaxCharactersProperty, value); }
        //}

        public double? MaxWidth
        {
            get { return (double?)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        public TextStyling TextStyle
        {
            get { return (TextStyling)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        public Label()
        {
            SetFontFamily();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "FontStyle")
                SetFontFamily();
        }

        protected void SetFontFamily()
        {
            var fontService = Resolver.Get<IFontService>();
            if (fontService == null) throw new InvalidOperationException("IFontService is not implemented.");
            this.FontFamily = fontService.GetFontName(this.FontStyle);
        }
    }
}
