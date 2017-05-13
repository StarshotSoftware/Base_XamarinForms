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

    public class NativeCell : ViewCell
    {
        public NativeCell()
        {

        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create<NativeCell, RelayCommand<string>>(b => b.Command, null);

        public static readonly BindableProperty Text1Property =
            BindableProperty.Create<NativeCell, string>(b => b.Text1, null);

        public static readonly BindableProperty Text2Property =
            BindableProperty.Create<NativeCell, string>(d => d.Text2, null);

        public static readonly BindableProperty Text3Property =
            BindableProperty.Create<NativeCell, string>(d => d.Text3, null);

        public static BindableProperty ImageAddressProperty =
            BindableProperty.Create<NativeCell, string>(p => p.ImageAddress, null);

        public static BindableProperty ImageWidthProperty =
            BindableProperty.Create<NativeCell, double>(p => p.ImageWidth, new PointSize(60));

        public static BindableProperty ImageHeightProperty =
            BindableProperty.Create<NativeCell, double>(p => p.ImageHeight, new PointSize(60));

        public static BindableProperty FontStyle1Property =
            BindableProperty.Create<NativeCell, FontStyle>(l => l.FontStyle1, FontStyle.Light);

        public static BindableProperty FontStyle2Property =
            BindableProperty.Create<NativeCell, FontStyle>(l => l.FontStyle2, FontStyle.Light);

        public static BindableProperty FontStyle3Property =
            BindableProperty.Create<NativeCell, FontStyle>(l => l.FontStyle3, FontStyle.Light);

        public static BindableProperty FontSize1Property =
            BindableProperty.Create<NativeCell, double>(l => l.FontSize1, new PointSize(8.1));

        public static BindableProperty FontSize2Property =
            BindableProperty.Create<NativeCell, double>(l => l.FontSize2, new PointSize(6.3));

        public static BindableProperty FontSize3Property =
            BindableProperty.Create<NativeCell, double>(l => l.FontSize3, new PointSize(8.1));

        public static BindableProperty TextColor1Property =
            BindableProperty.Create<NativeCell, Color>(l => l.TextColor1, Color.Black);

        public static BindableProperty TextColor2Property =
            BindableProperty.Create<NativeCell, Color>(l => l.TextColor2, Color.Black);

        public static BindableProperty TextColor3Property =
            BindableProperty.Create<NativeCell, Color>(l => l.TextColor3, Color.Black);

        public static BindableProperty IsClickableProperty =
            BindableProperty.Create<NativeCell, bool>(l => l.IsClickable, true);

        public RelayCommand<string> Command
        {
            get { return (RelayCommand<string>)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

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

        public bool IsClickable
        {
            get { return (bool)GetValue(IsClickableProperty); }
            set { SetValue(IsClickableProperty, value); }
        }
    }

}
