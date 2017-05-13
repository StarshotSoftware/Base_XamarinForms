using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BaseStarShot.Services;
using System.Windows.Input;
using Base1902;

namespace BaseStarShot.Controls
{
    public class RoundedEntry : Xamarin.Forms.Entry, IKeyPressHandler
    {
        //public static BindableProperty AndroidTextProperty =
        //    BindableProperty.Create<RoundedEntry, string>(p => p.AndroidText, "", BindingMode.OneWayToSource);

        public static BindableProperty ShouldEndEditingProperty =
            BindableProperty.Create<RoundedEntry, bool>(p => p.ShouldEndEditing, true);

        public static BindableProperty AllowCutCopyPasteProperty =
          BindableProperty.Create<RoundedEntry, bool>(p => p.AllowCutCopyPaste, true);

        public static BindableProperty ShowCursorProperty =
             BindableProperty.Create<RoundedEntry, bool>(p => p.ShowCursor, true);

        public static BindableProperty ShowKeyboardProperty =
           BindableProperty.Create<RoundedEntry, bool>(p => p.ShowKeyboard, false);

        public static BindableProperty TriggerResignResponderProperty =
            BindableProperty.Create<RoundedEntry, int>(p => p.TriggerResignResponder, 0);

        public static BindableProperty TextPaddingProperty =
            BindableProperty.Create<RoundedEntry, Thickness>(p => p.TextPadding, new PointThickness(20, 30));

        public static BindableProperty IsReadOnlyProperty =
            BindableProperty.Create<RoundedEntry, bool>(p => p.IsReadOnly, false);

        public static BindableProperty EnableAutoCorrectProperty =
            BindableProperty.Create<RoundedEntry, bool>(p => p.EnableAutoCorrect, true);

        public static BindableProperty IsSingleLineProperty =
            BindableProperty.Create<RoundedEntry, bool>(p => p.IsSingleLine, true);

        public static BindableProperty InputAccessoryBackgroundColorProperty =
            BindableProperty.Create<RoundedEntry, Color>(p => p.InputAccessoryBackgroundColor, Color.White);

        //public static BindableProperty IsPasswordProperty =
        //    BindableProperty.Create<RoundedEntry, bool>(p => p.IsPassword, false);

        //public static BindableProperty PlaceholderProperty =
        //    BindableProperty.Create<RoundedEntry, string>(p => p.Placeholder, "");

        //public static BindableProperty PlaceholderColorProperty =
        //    BindableProperty.Create<RoundedEntry, Color>(p => p.PlaceholderColor, ForPlatform.Get(Color.Gray, Color.Default, Color.Default, Color.Default, Color.Default));

        //public static BindableProperty TextColorProperty =
        //    BindableProperty.Create<RoundedEntry, Color>(p => p.TextColor, ForPlatform.Get(Color.Black, Color.Default, Color.Default, Color.Default, Color.Default));

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create<RoundedEntry, double>(p => p.CornerRadius, new PointSize(15));

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<RoundedEntry, Color>(p => p.BorderColor, Color.Default);

        //public static BindableProperty HorizontalTextAlignmentProperty =
        //    BindableProperty.Create<RoundedEntry, Xamarin.Forms.TextAlignment>(p => p.HorizontalTextAlignment, Xamarin.Forms.TextAlignment.Start);

        public static BindableProperty ImageRightProperty =
            BindableProperty.Create<RoundedEntry, ImageSource>(p => p.ImageRight, null);

        public static BindableProperty ImageRightWidthProperty =
            BindableProperty.Create<RoundedEntry, double>(p => p.ImageRightWidth, new PointSize(20));

        public static BindableProperty ImageRightHeightProperty =
            BindableProperty.Create<RoundedEntry, double>(p => p.ImageRightHeight, new PointSize(20));

        //public static BindableProperty FontSizeProperty =
        //    BindableProperty.Create<RoundedEntry, double>(p => p.FontSize, new PointFontSize(6));//Double.NaN);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<RoundedEntry, FontStyle>(p => p.FontStyle, FontStyle.Regular);

        //public static BindableProperty FontFamilyProperty =
        //    BindableProperty.Create<RoundedEntry, string>(p => p.FontFamily, "");

        public static BindableProperty MaxCharacterProperty =
            BindableProperty.Create<RoundedEntry, int?>(p => p.MaxCharacter, null);

        public static BindableProperty NextElementProperty =
            BindableProperty.Create<RoundedEntry, VisualElement>(l => l.NextElement, null);

        public static BindableProperty CommandProperty =
            BindableProperty.Create<RoundedEntry, ICommand>(l => l.Command, null);

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create<RoundedEntry, object>(l => l.CommandParameter, null);

        public static BindableProperty ReturnKeyTypeProperty =
            BindableProperty.Create<RoundedEntry, ReturnKeyType>(l => l.ReturnKeyType, ReturnKeyType.Automatic);

        public static BindableProperty TextInputTypeProperty =
            BindableProperty.Create<RoundedEntry, TextInputType>(l => l.TextInputType, TextInputType.None);

        public static BindableProperty TriggerFocusProperty =
            BindableProperty.Create<RoundedEntry, int>(l => l.TriggerFocus, 0);

        //public string AndroidText
        //{
        //    get { return (string)GetValue(AndroidTextProperty); }
        //    set { SetValue(AndroidTextProperty, value); }
        //}

        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
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

        public bool ShouldEndEditing
        {
            get { return (bool)GetValue(ShouldEndEditingProperty); }
            set { SetValue(ShouldEndEditingProperty, value); }
        }

        public bool AllowCutCopyPaste
        {
            get { return (bool)GetValue(AllowCutCopyPasteProperty); }
            set { SetValue(AllowCutCopyPasteProperty, value); }
        }

        public bool ShowCursor
        {
            get { return (bool)GetValue(ShowCursorProperty); }
            set { SetValue(ShowCursorProperty, value); }
        }


        public bool ShowKeyboard
        {
            get { return (bool)GetValue(ShowKeyboardProperty); }
            set { SetValue(ShowKeyboardProperty, value); }
        }


        public bool IsSingleLine
        {
            get { return (bool)GetValue(IsSingleLineProperty); }
            set { SetValue(IsSingleLineProperty, value); }
        }

        public Color InputAccessoryBackgroundColor
        {
            get { return (Color)GetValue(InputAccessoryBackgroundColorProperty); }
            set { SetValue(InputAccessoryBackgroundColorProperty, value); }
        }

        //public bool IsPassword
        //{
        //    get { return (bool)GetValue(IsPasswordProperty); }
        //    set { SetValue(IsPasswordProperty, value); }
        //}

        //public string Placeholder
        //{
        //    get { return (string)GetValue(PlaceholderProperty); }
        //    set { SetValue(PlaceholderProperty, value); }
        //}

        //public Color PlaceholderColor
        //{
        //    get { return (Color)GetValue(PlaceholderColorProperty); }
        //    set { SetValue(PlaceholderColorProperty, value); }
        //}

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        //public Color TextColor
        //{
        //    get { return (Color)GetValue(TextColorProperty); }
        //    set { SetValue(TextColorProperty, value); }
        //}

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        //public Xamarin.Forms.TextAlignment HorizontalTextAlignment
        //{
        //    get { return (Xamarin.Forms.TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
        //    set { SetValue(HorizontalTextAlignmentProperty, value); }
        //}

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

        public double ImageRightHeight
        {
            get { return (double)GetValue(ImageRightHeightProperty); }
            set { SetValue(ImageRightHeightProperty, value); }
        }

        //public double FontSize
        //{
        //    get { return (double)GetValue(FontSizeProperty); }
        //    set { SetValue(FontSizeProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        //public string FontFamily
        //{
        //    get { return (string)GetValue(FontFamilyProperty); }
        //    set { SetValue(FontFamilyProperty, value); }
        //}

        public int? MaxCharacter
        {
            get { return (int?)GetValue(MaxCharacterProperty); }
            set { SetValue(MaxCharacterProperty, value); }
        }

        public int TriggerResignResponder
        {
            get { return (int)GetValue(TriggerResignResponderProperty); }
            set { SetValue(TriggerResignResponderProperty, value); }
        }

        public VisualElement NextElement
        {
            get { return (VisualElement)GetValue(NextElementProperty); }
            set { SetValue(NextElementProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ReturnKeyType ReturnKeyType
        {
            get { return (ReturnKeyType)GetValue(ReturnKeyTypeProperty); }
            set { SetValue(ReturnKeyTypeProperty, value); }
        }

        public TextInputType TextInputType
        {
            get { return (TextInputType)GetValue(TextInputTypeProperty); }
            set { SetValue(TextInputTypeProperty, value); }
        }

        public int TriggerFocus
        {
            get { return (int)GetValue(TriggerFocusProperty); }
            set { SetValue(TriggerFocusProperty, value); }
        }

        internal bool DisableNextFocus { get; set; }

        public RoundedEntry()
        {
            SetFontFamily();
            this.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "FontStyle")
                    SetFontFamily();
            };
        }

        Action<KeyCode> onPerformKeyPress;
        void IKeyPressHandler.SetPerformKeyPress(Action<KeyCode> onKeyPress)
        {
            this.onPerformKeyPress = onKeyPress;
        }

        public void PerformKeyPress(KeyCode key)
        {
            if (this.onPerformKeyPress != null)
                this.onPerformKeyPress(key);
        }

        public void FocusFixed()
        {
            if (Device.OS == TargetPlatform.Windows)
            {
                TriggerFocus += 1;
            }
            else
                Focus();
        }

        protected void SetFontFamily()
        {
            var fontService = Resolver.Get<IFontService>();
            if (fontService == null) throw new InvalidOperationException("IFontService is not implemented.");
            this.FontFamily = fontService.GetFontName(this.FontStyle);
        }
    }
}
