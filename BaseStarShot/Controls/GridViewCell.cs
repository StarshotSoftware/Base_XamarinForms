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

    public class GridViewCell : ViewCell
    {
        public GridViewCell()
        {
        }

        public static readonly BindableProperty Text1Property =
            BindableProperty.Create<GridViewCell, string>(b => b.Text1, null);

        public static readonly BindableProperty Text2Property =
            BindableProperty.Create<GridViewCell, string>(b => b.Text2, null);

        public static readonly BindableProperty Text3Property =
            BindableProperty.Create<GridViewCell, string>(b => b.Text3, null);

        public static BindableProperty ImageAddressProperty =
            BindableProperty.Create<GridViewCell, string>(p => p.ImageAddress, null);

        public static BindableProperty ImageWidthProperty =
            BindableProperty.Create<GridViewCell, double>(p => p.ImageWidth, new PointSize(60));

        public static BindableProperty ImageHeightProperty =
            BindableProperty.Create<GridViewCell, double>(p => p.ImageHeight, new PointSize(60));

        public static BindableProperty FontStyle1Property =
            BindableProperty.Create<GridViewCell, FontStyle>(l => l.FontStyle1, FontStyle.Light);

        public static BindableProperty FontSize1Property =
            BindableProperty.Create<GridViewCell, double>(l => l.FontSize1, new PointSize(8.1));

        public static BindableProperty TextColor1Property =
            BindableProperty.Create<GridViewCell, Color>(l => l.TextColor1, Color.White);

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
