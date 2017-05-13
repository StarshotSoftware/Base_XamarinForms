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
    public class Editor : Xamarin.Forms.Editor, IKeyPressHandler
    {
        /// <summary>
        /// Default Editor padding.
        /// </summary>
        public static readonly Thickness DefaultPadding =
             ForPlatform.Get(new Thickness(0), new Thickness(12d, 8d), new Thickness(12d, 8d), new Thickness(12d, 8d), new Thickness(12d, 8d));

        public static BindableProperty BackgroundImageProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.BackgroundImage, null);

        public static BindableProperty MaxHeightProperty =
            BindableProperty.Create<Editor, double?>(p => p.MaxHeight, null);

        public static BindableProperty MinHeightProperty =
            BindableProperty.Create<Editor, double?>(p => p.MinHeight, null);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<Editor, FontStyle>(l => l.FontStyle, FontStyle.Regular);

        public static BindableProperty TextAlignmentProperty =
            BindableProperty.Create<Editor, TextAlignment>(p => p.TextAlignment, BaseStarShot.TextAlignment.Left);

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create<Editor, string>(p => p.Placeholder, "");

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<Editor, Color>(p => p.BorderColor, Color.Default);

        public static BindableProperty IsReadOnlyProperty =
            BindableProperty.Create<Editor, bool>(p => p.IsReadOnly, false);

        public static BindableProperty EnableAutoCorrectProperty =
            BindableProperty.Create<Editor, bool>(p => p.EnableAutoCorrect, true);

        public static BindableProperty InputAccessoryBackgroundColorProperty =
            BindableProperty.Create<Editor, Color>(p => p.InputAccessoryBackgroundColor, Color.White);

        public static BindableProperty SuppressKeyboardProperty =
            BindableProperty.Create<Editor, bool>(l => l.SuppressKeyboard, false);

        public static BindableProperty MaxCharacterProperty =
            BindableProperty.Create<Editor, int?>(p => p.MaxCharacter, null);

        //public static BindableProperty Text1Property =
        //    BindableProperty.Create<Editor, string>(l => l.Text1, "");

        //public static BindableProperty PaddingProperty =
        //    BindableProperty.Create<Editor, Thickness>(p => p.Padding, DefaultPadding);


        Action<KeyCode> onPerformKeyPress;

        /// <summary>
        /// Gets or sets the image or resource to be used as the background of the entry.
        /// </summary>
        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
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
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// Gets or set the placeholder text of editor. Currently implemented in windows.shared only
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// For ios only
        /// </summary>
        public bool EnableAutoCorrect
        {
            get { return (bool)GetValue(EnableAutoCorrectProperty); }
            set { SetValue(EnableAutoCorrectProperty, value); }
        }

        public Color InputAccessoryBackgroundColor
        {
            get { return (Color)GetValue(InputAccessoryBackgroundColorProperty); }
            set { SetValue(InputAccessoryBackgroundColorProperty, value); }
        }

        public double? MaxHeight
        {
            get { return (double?)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        public double? MinHeight
        {
            get { return (double?)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        ///// <summary>
        ///// Gets or sets the color of the text.
        ///// </summary>
        //public Color TextColor
        //{
        //    get { return (Color)GetValue(TextColorProperty); }
        //    set { SetValue(TextColorProperty, value); }
        //}

        /// <summary>
        /// Gets or sets if the soft keyboard should be suppressed when entry gets focus.
        /// </summary>
        public bool SuppressKeyboard
        {
            get { return (bool)GetValue(SuppressKeyboardProperty); }
            set { SetValue(SuppressKeyboardProperty, value); }
        }

        public int? MaxCharacter
        {
            get { return (int?)GetValue(MaxCharacterProperty); }
            set { SetValue(MaxCharacterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the padding within the entry. Not implemented, use TextAlignment for placement instead.
        /// </summary>
        //public Thickness Padding
        //{
        //    get { return (Thickness)GetValue(PaddingProperty); }
        //    set { SetValue(PaddingProperty, value); }
        //}

        public Editor()
        {
            SetFontFamily();
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "FontStyle")
                    SetFontFamily();
            };
        }

        void IKeyPressHandler.SetPerformKeyPress(Action<KeyCode> onKeyPress)
        {
            this.onPerformKeyPress = onKeyPress;
        }

        public void PerformKeyPress(KeyCode key)
        {
            if (this.onPerformKeyPress != null)
                this.onPerformKeyPress(key);
        }

        protected void SetFontFamily()
        {
            var fontService = Resolver.Get<IFontService>();
            if (fontService == null) throw new InvalidOperationException("IFontService is not implemented.");
            this.FontFamily = fontService.GetFontName(this.FontStyle);
        }
    }
}
