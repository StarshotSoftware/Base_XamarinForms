using Base1902;
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
    public class FormattedLabelChild : Xamarin.Forms.Label
    {
        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<FormattedLabelChild, FontStyle>(l => l.FontStyle, FontStyle.Regular);

        public static BindableProperty ClickableIndexProperty =
            BindableProperty.Create<FormattedLabelChild, int>(l => l.ClickableIndex, -1);

        public static BindableProperty TextStyleProperty =
            BindableProperty.Create<FormattedLabelChild, TextStyle>(l => l.TextStyle, TextStyle.None);

        public static BindableProperty UnderlineColorProperty =
            BindableProperty.Create<FormattedLabelChild, Color>(l => l.UnderlineColor, Color.Transparent);

        public static BindableProperty ClickWholeTextProperty =
            BindableProperty.Create<FormattedLabelChild, bool>(l => l.ClickWholeText, false);

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public TextStyle TextStyle
        {
            get { return (TextStyle)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public int ClickableIndex
        {
            get { return (int)GetValue(ClickableIndexProperty); }
            set { SetValue(ClickableIndexProperty, value); }
        }

        public Color UnderlineColor
        {
            get { return (Color)GetValue(UnderlineColorProperty); }
            set { SetValue(UnderlineColorProperty, value); }
        }

        public bool ClickWholeText
        {
            get { return (bool)GetValue(ClickWholeTextProperty); }
            set { SetValue(ClickWholeTextProperty, value); }
        }

        public FormattedLabelChild()
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

        public static BindableProperty CommandProperty =
            BindableProperty.Create<FormattedLabelChild, ICommand>(l => l.Command, null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
    }
}
