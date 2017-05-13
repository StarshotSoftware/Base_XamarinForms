using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Controls
{
    using BaseStarShot;
    using System;
    using System.Linq.Expressions;
    using Xamarin.Forms;

    public class TwoColumnTextCell : ViewCell
    {
        public TwoColumnTextCell()
        {
        }

        public static readonly BindableProperty Text1Property =
            BindableProperty.Create<TwoColumnTextCell, string>(b => b.Text1, null);

        public static readonly BindableProperty Text2Property =
            BindableProperty.Create<TwoColumnTextCell, string>(d => d.Text2, null);

        public static BindableProperty FontStyle1Property =
            BindableProperty.Create<TwoColumnTextCell, FontStyle>(l => l.FontStyle1, FontStyle.Light);

        public static BindableProperty FontStyle2Property =
            BindableProperty.Create<TwoColumnTextCell, FontStyle>(l => l.FontStyle2, FontStyle.Light);

        public static BindableProperty FontSize1Property =
            BindableProperty.Create<TwoColumnTextCell, double>(l => l.FontSize1, new PointSize(8.1));

        public static BindableProperty FontSize2Property =
            BindableProperty.Create<TwoColumnTextCell, double>(l => l.FontSize2, new PointSize(6.3));

        public static BindableProperty TextColor1Property =
            BindableProperty.Create<TwoColumnTextCell, Color>(l => l.TextColor1, Color.Black);

        public static BindableProperty TextColor2Property =
            BindableProperty.Create<TwoColumnTextCell, Color>(l => l.TextColor2, Color.Black);

        public string Text1
        {
            get { return (string)GetValue(Text1Property); }
            set { SetValue(Text1Property, value); }
        }

        public string Text2
        {
            get { return (string)GetValue(Text2Property); }
            set { SetValue(Text2Property, value); }
        }

        public FontStyle FontStyle1
        {
            get { return (FontStyle)GetValue(FontStyle1Property); }
            set { SetValue(FontStyle1Property, value); }
        }

        public FontStyle FontStyle2
        {
            get { return (FontStyle)GetValue(FontStyle2Property); }
            set { SetValue(FontStyle2Property, value); }
        }

        public double FontSize1
        {
            get { return (double)GetValue(FontSize1Property); }
            set { SetValue(FontSize1Property, value); }
        }

        public double FontSize2
        {
            get { return (double)GetValue(FontSize2Property); }
            set { SetValue(FontSize2Property, value); }
        }

        public Color TextColor1
        {
            get { return (Color)GetValue(TextColor1Property); }
            set { SetValue(TextColor1Property, value); }
        }

        public Color TextColor2
        {
            get { return (Color)GetValue(TextColor2Property); }
            set { SetValue(TextColor2Property, value); }
        }

    }

}
