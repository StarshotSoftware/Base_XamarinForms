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

    public class NativeCell2 : ViewCell
    {
        public NativeCell2()
        {
        }

        public static readonly BindableProperty Text1Property =
            BindableProperty.Create<NativeCell2, string>(b => b.Text1, null);

        public static readonly BindableProperty Text2Property =
            BindableProperty.Create<NativeCell2, string>(d => d.Text2, null);

        public static readonly BindableProperty Text3Property =
            BindableProperty.Create<NativeCell2, string>(d => d.Text3, null);

        public static readonly BindableProperty Text4Property =
            BindableProperty.Create<NativeCell2, string>(d => d.Text4, null);

        public static BindableProperty ImageAddressProperty =
            BindableProperty.Create<NativeCell2, string>(p => p.ImageAddress, null);

        public static BindableProperty ImageWidthProperty =
            BindableProperty.Create<NativeCell2, double>(p => p.ImageWidth, new PointSize(60));

        public static BindableProperty ImageHeightProperty =
            BindableProperty.Create<NativeCell2, double>(p => p.ImageHeight, new PointSize(60));

        public static BindableProperty FontStyle1Property =
            BindableProperty.Create<NativeCell2, FontStyle>(l => l.FontStyle1, FontStyle.Light);

        public static BindableProperty FontStyle2Property =
            BindableProperty.Create<NativeCell2, FontStyle>(l => l.FontStyle2, FontStyle.Light);

        public static BindableProperty FontStyle3Property =
            BindableProperty.Create<NativeCell2, FontStyle>(l => l.FontStyle3, FontStyle.Light);

        public static BindableProperty FontStyle4Property =
            BindableProperty.Create<NativeCell2, FontStyle>(l => l.FontStyle4, FontStyle.Light);

        public static BindableProperty FontSize1Property =
            BindableProperty.Create<NativeCell2, double>(l => l.FontSize1, new PointSize(8.1));

        public static BindableProperty FontSize2Property =
            BindableProperty.Create<NativeCell2, double>(l => l.FontSize2, new PointSize(6.3));

        public static BindableProperty FontSize3Property =
            BindableProperty.Create<NativeCell2, double>(l => l.FontSize3, new PointSize(8.1));

        public static BindableProperty FontSize4Property =
            BindableProperty.Create<NativeCell2, double>(l => l.FontSize4, new PointSize(6.3));

        public static BindableProperty TextColor1Property =
            BindableProperty.Create<NativeCell2, Color>(l => l.TextColor1, Color.White);

        public static BindableProperty TextColor2Property =
            BindableProperty.Create<NativeCell2, Color>(l => l.TextColor2, Color.White);

        public static BindableProperty TextColor3Property =
            BindableProperty.Create<NativeCell2, Color>(l => l.TextColor3, Color.White);

        public static BindableProperty TextColor4Property =
            BindableProperty.Create<NativeCell2, Color>(l => l.TextColor4, Color.White);

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

        public string Text3
        {
            get { return (string)GetValue(Text3Property); }
            set { SetValue(Text3Property, value); }
        }

        public string Text4
        {
            get { return (string)GetValue(Text4Property); }
            set { SetValue(Text4Property, value); }
        }

        public string ImageAddress
        {
            get { return (string)GetValue(ImageAddressProperty); }
            set { SetValue(ImageAddressProperty, value); }
        }

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
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

        public FontStyle FontStyle3
        {
            get { return (FontStyle)GetValue(FontStyle3Property); }
            set { SetValue(FontStyle3Property, value); }
        }

        public FontStyle FontStyle4
        {
            get { return (FontStyle)GetValue(FontStyle4Property); }
            set { SetValue(FontStyle4Property, value); }
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

        public double FontSize3
        {
            get { return (double)GetValue(FontSize3Property); }
            set { SetValue(FontSize3Property, value); }
        }

        public double FontSize4
        {
            get { return (double)GetValue(FontSize4Property); }
            set { SetValue(FontSize4Property, value); }
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

        public Color TextColor3
        {
            get { return (Color)GetValue(TextColor3Property); }
            set { SetValue(TextColor3Property, value); }
        }

        public Color TextColor4
        {
            get { return (Color)GetValue(TextColor4Property); }
            set { SetValue(TextColor4Property, value); }
        }
    }

}
