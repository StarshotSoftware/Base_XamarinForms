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

    public class NativeCell3 : ViewCell
    {
        public NativeCell3()
        {
        }

        public static readonly BindableProperty Text1Property =
            BindableProperty.Create<NativeCell3, string>(b => b.Text1, null);

        public static BindableProperty ImageAddressProperty =
            BindableProperty.Create<NativeCell3, string>(p => p.ImageAddress, null);

        public static BindableProperty BackgroundImageAddressProperty =
            BindableProperty.Create<NativeCell3, string>(p => p.BackgroundImageAddress, null);

        public static BindableProperty ImageWidthProperty =
            BindableProperty.Create<NativeCell3, double>(p => p.ImageWidth, new PointSize(60));

        public static BindableProperty ImageHeightProperty =
            BindableProperty.Create<NativeCell3, double>(p => p.ImageHeight, new PointSize(60));

        public static BindableProperty BackgroundWidthProperty =
            BindableProperty.Create<NativeCell3, double>(p => p.BackgroundWidth, Globals.LogicalScreenWidth);

        public static BindableProperty BackgroundHeightProperty =
            BindableProperty.Create<NativeCell3, double>(p => p.BackgroundHeight, new PointSize((int)(Globals.LogicalScreenWidth / 3d + 0.5d) * 2d));

        public static BindableProperty FontStyle1Property =
            BindableProperty.Create<NativeCell3, FontStyle>(l => l.FontStyle1, FontStyle.Light);

        public static BindableProperty FontSize1Property =
            BindableProperty.Create<NativeCell3, double>(l => l.FontSize1, new PointSize(8.1));

        public static BindableProperty TextColor1Property =
            BindableProperty.Create<NativeCell3, Color>(l => l.TextColor1, Color.White);

        public string Text1
        {
            get { return (string)GetValue(Text1Property); }
            set { SetValue(Text1Property, value); }
        }

        public string ImageAddress
        {
            get { return (string)GetValue(ImageAddressProperty); }
            set { SetValue(ImageAddressProperty, value); }
        }

        public string BackgroundImageAddress
        {
            get { return (string)GetValue(BackgroundImageAddressProperty); }
            set { SetValue(BackgroundImageAddressProperty, value); }
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

        public double BackgroundHeight
        {
            get { return (double)GetValue(BackgroundHeightProperty); }
            set { SetValue(BackgroundHeightProperty, value); }
        }

        public double BackgroundWidth
        {
            get { return (double)GetValue(BackgroundWidthProperty); }
            set { SetValue(BackgroundWidthProperty, value); }
        }

        public FontStyle FontStyle1
        {
            get { return (FontStyle)GetValue(FontStyle1Property); }
            set { SetValue(FontStyle1Property, value); }
        }

        public double FontSize1
        {
            get { return (double)GetValue(FontSize1Property); }
            set { SetValue(FontSize1Property, value); }
        }

        public Color TextColor1
        {
            get { return (Color)GetValue(TextColor1Property); }
            set { SetValue(TextColor1Property, value); }
        }

    }

}
