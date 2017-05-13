using Base1902;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	public class FormattedTextLabel : Xamarin.Forms.Label
    {
        public string HTMLString
        {
            get { return (string)GetValue(HTMLStringProperty); }
            set { SetValue(HTMLStringProperty, value); }
        }

        public double DefaultFontSize
        {
            get { return (double)GetValue(DefaultFontSizeProperty); }
            set { SetValue(DefaultFontSizeProperty, value); }
        }

        public double DefaultHFontSize
        {
            get { return (double)GetValue(DefaultHFontSizeProperty); }
            set { SetValue(DefaultHFontSizeProperty, value); }
        }

		public Color DefaultTextColor
		{
			get { return (Color)GetValue(DefaultTextColorProperty); }
			set { SetValue(DefaultTextColorProperty, value); }
		}

        //public double DefaultPFontSize
        //{
        //    get { return (double)GetValue(DefaultPFontSizeProperty); }
        //    set { SetValue(DefaultPFontSizeProperty, value); }
        //}

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

		public int MaxLines
		{
			get { return (int)GetValue(MaxLinesProperty); }
			set { SetValue(MaxLinesProperty, value); }
		}

        /// <summary>
        /// Gets or sets the font style for the text using the mapped fonts in IFontService.
        /// </summary>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

		public static BindableProperty DefaultTextColorProperty =
			BindableProperty.Create<FormattedTextLabel, Color>(l => l.DefaultTextColor, Color.White);

        public static BindableProperty HTMLStringProperty =
            BindableProperty.Create<FormattedTextLabel, string>(l => l.HTMLString, null);

        public static BindableProperty DefaultFontSizeProperty =
            BindableProperty.Create<FormattedTextLabel, double>(l => l.DefaultFontSize, new PointSize(6));

        public static BindableProperty DefaultHFontSizeProperty =
            BindableProperty.Create<FormattedTextLabel, double>(l => l.DefaultHFontSize, -1);

		public static BindableProperty MaxLinesProperty =
			BindableProperty.Create<FormattedTextLabel, int>(l => l.MaxLines, 0);

        //public static BindableProperty DefaultPFontSizeProperty =
        //    BindableProperty.Create<FormattedTextLabel, double>(l => l.DefaultPFontSize, new PointSize(6));

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<FormattedTextLabel, FontStyle>(p => p.FontStyle, FontStyle.Regular);

        public static BindableProperty FontFamilyProperty =
            BindableProperty.Create<FormattedTextLabel, string>(p => p.FontFamily, "");

        public FormattedTextLabel()
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
