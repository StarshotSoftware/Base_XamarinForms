using Base1902;
using BaseStarShot;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class Entry : Xamarin.Forms.Entry, IKeyPressHandler
    {
        /// <summary>
        /// Default entry padding.
        /// </summary>
        public static readonly Thickness DefaultPadding =
             ForPlatform.Get(new Thickness(0), new Thickness(12d, 0d), new Thickness(12d, 8d), new Thickness(12d, 8d), new Thickness(12d, 8d));

        /// <summary>
        /// Default text padding.
        /// </summary>
        public static readonly Thickness DefaultTextPadding = new Thickness(0);

        public static BindableProperty TagProperty =
            BindableProperty.Create<Entry, string>(p => p.Tag, "true");

        public static BindableProperty MaxCharacterProperty =
            BindableProperty.Create<Entry, int?>(p => p.MaxCharacter, null);

        public static BindableProperty BackgroundImageProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.BackgroundImage, null);

        //public static BindableProperty FontFamilyProperty =
        //    BindableProperty.Create<Entry, string>(l => l.FontFamily, null);

        //public static BindableProperty FontSizeProperty =
        //    BindableProperty.Create<Entry, double>(l => l.FontSize, Device.GetNamedSize(NamedSize.Default, typeof(Xamarin.Forms.Entry)));

        public static BindableProperty ImageLeftWidthProperty =
            BindableProperty.Create<Entry, double>(l => l.ImageLeftWidth, 0);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<Entry, FontStyle>(l => l.FontStyle, FontStyle.Regular);

#if XAMARIN_FORMS_1_5
        public static BindableProperty TextAlignmentProperty =
            BindableProperty.Create<Entry, TextAlignment>(p => p.TextAlignment, BaseStarShot.TextAlignment.Left);
#endif

        public static BindableProperty TextPaddingProperty =
            BindableProperty.Create<Button, Thickness>(p => p.TextPadding, DefaultTextPadding);

        public static BindableProperty PaddingProperty =
            BindableProperty.Create<Button, Thickness>(p => p.Padding, DefaultPadding);

        public static BindableProperty MarginProperty =
            BindableProperty.Create<Entry, Thickness>(p => p.Margin, new PointThickness(0));

        public static BindableProperty SuppressKeyboardProperty =
            BindableProperty.Create<Entry, bool>(l => l.SuppressKeyboard, false);

        public static BindableProperty ClearFocusProperty =
            BindableProperty.Create<Entry, bool>(l => l.ClearFocus, false);

        public static BindableProperty ClearFocusTriggerProperty =
            BindableProperty.Create<Entry, int>(l => l.ClearFocusTrigger, 0);

        public static BindableProperty NextElementProperty =
            BindableProperty.Create<Entry, VisualElement>(l => l.NextElement, null);

        public static BindableProperty ReturnKeyTypeProperty =
            BindableProperty.Create<Entry, ReturnKeyType>(l => l.ReturnKeyType, ReturnKeyType.Automatic);

        public static BindableProperty TextInputTypeProperty =
            BindableProperty.Create<Entry, TextInputType>(l => l.TextInputType, TextInputType.None);



        public static BindableProperty ImageLeftProperty =
            BindableProperty.Create<Entry, FileImageSource>(p => p.ImageLeft, null);

        public static BindableProperty ImageTopProperty =
            BindableProperty.Create<Entry, FileImageSource>(p => p.ImageTop, null);

        public static BindableProperty ImageRightProperty =
            BindableProperty.Create<Entry, FileImageSource>(p => p.ImageRight, null);

        public static BindableProperty ImageBottomProperty =
            BindableProperty.Create<Entry, FileImageSource>(p => p.ImageBottom, null);

        public static BindableProperty ImageCenterProperty =
            BindableProperty.Create<Entry, FileImageSource>(p => p.ImageCenter, null);

        public static BindableProperty BorderRadiusProperty =
            BindableProperty.Create<Entry, float>(p => p.BorderRadius, 1f);

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<Entry, Color>(p => p.BorderColor, Color.Default);

        public static BindableProperty IsReadOnlyProperty =
            BindableProperty.Create<Entry, bool>(p => p.IsReadOnly, false);

        public static BindableProperty IsSingleLineProperty =
           BindableProperty.Create<Entry, bool>(p => p.IsSingleLine, false);

        //public static BindableProperty HintColorProperty =
        //    BindableProperty.Create<Entry, Color>(p => p.HintColor, Color.Gray);

        /// <summary>
        /// Set Hint Color
        /// </summary>
        //public Color HintColor
        //{
        //    get { return (Color)GetValue(HintColorProperty); }
        //    set { SetValue(HintColorProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the image or resource displayed on the left side of the text.
        /// </summary>
        public float BorderRadius
        {
            get { return (float)GetValue(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or set the border color of the entry. Currently implemented in windows.shared only
        /// </summary>
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// get or set if entry is editable. It does not change the color to disabled.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// get or set if entry is editable. It does not change the color to disabled.
        /// </summary>
        public bool IsSingleLine
        {
            get { return (bool)GetValue(IsSingleLineProperty); }
            set { SetValue(IsSingleLineProperty, value); }
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

        /// <summary>
        /// Gets or sets the image or resource displayed at the center.
        /// </summary>
        public FileImageSource ImageCenter
        {
            get { return (FileImageSource)GetValue(ImageCenterProperty); }
            set { SetValue(ImageCenterProperty, value); }
        }


        // Summary:
        //     Gets or sets the command that is run when the menu is clicked.
        //
        // Remarks:
        //     To be added.
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
        }

        public int? MaxCharacter
        {
            get { return (int?)GetValue(MaxCharacterProperty); }
            set { SetValue(MaxCharacterProperty, value); }
        }
        //
        // Summary:
        //     Identifies the command bound property.
        //
        // Remarks:
        //     To be added.
        public static BindableProperty CommandProperty =
            BindableProperty.Create<Entry, ICommand>(l => l.Command, null);

        //public static BindableProperty Text1Property =
        //    BindableProperty.Create<Entry, string>(l => l.Text1, "");

        //public static BindableProperty PaddingProperty =
        //    BindableProperty.Create<Entry, Thickness>(p => p.Padding, DefaultPadding);


        Action<KeyCode> onPerformKeyPress;

        /// <summary>
        /// Gets or sets the image or resource to be used as the background of the entry.
        /// </summary>
        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        ///// <summary>
        ///// Gets the font family to which the font for the entry belongs.
        ///// </summary>
        //public string FontFamily
        //{
        //    get { return (string)GetValue(FontFamilyProperty); }
        //    set { SetValue(FontFamilyProperty, value); }
        //}

        ///// <summary>
        ///// Gets the size of the font for the entry.
        ///// </summary>
        //public double FontSize
        //{
        //    get { return (double)GetValue(FontSizeProperty); }
        //    set { SetValue(FontSizeProperty, value); }
        //}

        public double ImageLeftWidth
        {
            get { return (double)GetValue(ImageLeftWidthProperty); }
            set { SetValue(ImageLeftWidthProperty, value); }
        }
        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

#if XAMARIN_FORMS_1_5
        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
#endif

        /// <summary>
        /// Gets or sets if the soft keyboard should be suppressed when entry gets focus.
        /// </summary>
        public bool SuppressKeyboard
        {
            get { return (bool)GetValue(SuppressKeyboardProperty); }
            set { SetValue(SuppressKeyboardProperty, value); }
        }

        public int ClearFocusTrigger
        {
            get { return (int)GetValue(ClearFocusTriggerProperty); }
            set { SetValue(ClearFocusTriggerProperty, value); }
        }

        public bool ClearFocus
        {
            get { return (bool)GetValue(ClearFocusProperty); }
            set { SetValue(ClearFocusProperty, value); }
        }

        public IList<BaseStarShot.Controls.Entry> GroupElement { get; protected set; }

        public VisualElement NextElement
        {
            get { return (VisualElement)GetValue(NextElementProperty); }
            set { SetValue(NextElementProperty, value); }
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

        /// <summary>
        /// Gets or sets the padding between the text and image inside the entry.
        /// </summary>
        public Thickness TextPadding
        {
            get { return (Thickness)GetValue(TextPaddingProperty); }
            set { SetValue(TextPaddingProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// Implemented for WinRT NextElement focus implementation.
        /// </summary>
        internal bool DisableNextFocus { get; set; }

        /// <summary>
        /// Gets or sets the padding within the entry. Not implemented, use TextAlignment for placement instead.
        /// </summary>
        //public Thickness Padding
        //{
        //    get { return (Thickness)GetValue(PaddingProperty); }
        //    set { SetValue(PaddingProperty, value); }
        //}

        public Entry()
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
