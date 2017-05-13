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
    public class SearchBar : Xamarin.Forms.SearchBar
    {
        public event EventHandler<FocusEventArgs> FocusChanged;

        public const string FocusMessage = "SearchBarFocus";
        public const string UnfocusMessage = "SearchBarUnfocus";

        public static BindableProperty SearchIconProperty =
            BindableProperty.Create<SearchBar, FileImageSource>(p => p.SearchIcon, null);

        public static BindableProperty ClearTextProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.ClearText, false, BindingMode.TwoWay);

        public static BindableProperty SearchOnTypeProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.SearchOnType, true);

        public static BindableProperty SearchFieldBackgroundProperty =
            BindableProperty.Create<SearchBar, Color>(p => p.SearchFieldBackground, Color.Default);

        public static BindableProperty BackgroundProperty =
            BindableProperty.Create<SearchBar, FileImageSource>(p => p.Background, null);

        public static BindableProperty PanelBackgroundProperty =
            BindableProperty.Create<SearchBar, Color>(p => p.PanelBackground, Color.White);

        //public static BindableProperty TextColorProperty =
        //    BindableProperty.Create<SearchBar, Color>(p => p.TextColor, Color.Default);


        //        public static BindableProperty HintTextColorProperty =
        //            BindableProperty.Create<SearchBar, Color>(p => p.HintTextColor, Color.Default);

        public static BindableProperty TextInputTypeProperty =
            BindableProperty.Create<SearchBar, TextInputType>(p => p.TextInputType, TextInputType.None);

        public static BindableProperty CornerRadiusProperty =
            BindableProperty.Create<SearchBar, double>(p => p.CornerRadius, 0);

        public static BindableProperty BorderColorProperty =
            BindableProperty.Create<SearchBar, Color>(p => p.BorderColor, Color.Default);

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<SearchBar, FontStyle>(p => p.FontStyle, FontStyle.Regular);

        public static BindableProperty AlignSearchIconRightProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.AlignSearchIconRight, false);

        public static BindableProperty ShowsCancelButtonProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.ShowsCancelButton, false);

        public static BindableProperty ShowsClearButtonProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.ShowsClearButton, false);

        public static BindableProperty ShowsDoneButtonProperty =
            BindableProperty.Create<SearchBar, bool>(p => p.ShowsDoneButton, false);

        public static BindableProperty DoneCommandProperty =
            BindableProperty.Create<SearchBar, ICommand>(p => p.DoneCommand, null);

        public FileImageSource SearchIcon
        {
            get { return (FileImageSource)GetValue(SearchIconProperty); }
            set { SetValue(SearchIconProperty, value); }
        }

        public bool ClearText
        {
            get { return (bool)GetValue(ClearTextProperty); }
            set { SetValue(ClearTextProperty, value); }
        }

        /// <summary>
        /// Enables searching while typing on windows desktop only
        /// </summary>
        public bool SearchOnType
        {
            get { return (bool)GetValue(SearchOnTypeProperty); }
            set { SetValue(SearchOnTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the search field background. For ios only
        /// </summary>
        /// <value>The search field background.</value>
        public Color SearchFieldBackground
        {
            get { return (Color)GetValue(SearchFieldBackgroundProperty); }
            set { SetValue(SearchFieldBackgroundProperty, value); }
        }

        //public Color TextColor
        //{
        //    get { return (Color)GetValue(TextColorProperty); }
        //    set { SetValue(TextColorProperty, value); }
        //}


        //        public Color HintTextColor
        //        {
        //            get { return (Color)GetValue(HintTextColorProperty); }
        //            set { SetValue(HintTextColorProperty, value); }
        //        }

        public FileImageSource Background
        {
            get { return (FileImageSource)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Set suggestions panel background, for windows only
        /// </summary>
        public Color PanelBackground
        {
            get { return (Color)GetValue(PanelBackgroundProperty); }
            set { SetValue(PanelBackgroundProperty, value); }
        }

        public TextInputType TextInputType
        {
            get { return (TextInputType)GetValue(TextInputTypeProperty); }
            set { SetValue(TextInputTypeProperty, value); }
        }

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// Get or set search icon to the right. Used for Android only
        /// </summary>
        public bool AlignSearchIconRight
        {
            get { return (bool)GetValue(AlignSearchIconRightProperty); }
            set { SetValue(AlignSearchIconRightProperty, value); }
        }

        /// <summary>
        /// iOS only.
        /// Gets or sets a value indicating whether this <see cref="T:BaseStarShot.Controls.SearchBar"/> shows cancel button.
        /// </summary>
        /// <value><c>true</c> if shows cancel button; otherwise, <c>false</c>.</value>
        public bool ShowsCancelButton
        {
            get { return (bool)GetValue(ShowsCancelButtonProperty); }
            set { SetValue(ShowsCancelButtonProperty, value); }
        }

        public bool ShowsClearButton
        {
            get { return (bool)GetValue(ShowsClearButtonProperty); }
            set { SetValue(ShowsClearButtonProperty, value); }
        }

        public bool ShowsDoneButton
        {
            get { return (bool)GetValue(ShowsDoneButtonProperty); }
            set { SetValue(ShowsDoneButtonProperty, value); }
        }

        public ICommand DoneCommand
        {
            get { return (ICommand)GetValue(DoneCommandProperty); }
            set { SetValue(DoneCommandProperty, value); }
        }

        public SearchBar()
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

        public void ChangeFocus(bool isFocus)
        {
            if (FocusChanged != null)
                FocusChanged(this, new FocusEventArgs(this, isFocus));
        }

        public new void Focus()
        {
            base.Focus();

            MessagingCenter.Send<SearchBar>(this, FocusMessage);
        }

        public new void Unfocus()
        {
            base.Unfocus();

            MessagingCenter.Send<SearchBar>(this, UnfocusMessage);
        }
    }
}
