using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	public class CircleImage : BaseStarShot.Controls.Image
    {
        /// <summary>
        /// Thickness property of border
        /// </summary>
		public static readonly BindableProperty BorderThicknessProperty =
			BindableProperty.Create<CircleImage, int>(p => p.BorderThickness, 0);

        /// <summary>
        /// Color property of border
        /// </summary>
        public static readonly BindableProperty BorderColorProperty =
			BindableProperty.Create<CircleImage, Color>(p => p.BorderColor, Color.White);

        /// <summary>
        /// Render as circle property, applicable to android only.
        /// </summary>
        public static readonly BindableProperty RenderCircleProperty =
            BindableProperty.Create<CircleImage, bool>(p => p.RenderCircle, true);

        /// <summary>
        /// Border thickness of circle image
        /// </summary>
        public int BorderThickness
        {
            get { return (int)base.GetValue(BorderThicknessProperty); }
            set { base.SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Border Color of circle image
        /// </summary>
        public Color BorderColor
        {
            get { return (Color)base.GetValue(BorderColorProperty); }
            set { base.SetValue(BorderColorProperty, value); }
        }

        /// <summary>
        /// Gets or set if image is rendered as circle
        /// </summary>
        public bool RenderCircle
        {
            get { return (bool)base.GetValue(RenderCircleProperty); }
            set { base.SetValue(RenderCircleProperty, value); }
        }
    }
}
