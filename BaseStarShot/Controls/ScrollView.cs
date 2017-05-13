using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ScrollView : Xamarin.Forms.ScrollView
    {
		public static readonly BindableProperty BouncesProperty =
			BindableProperty.Create<ScrollView, bool>(p => p.Bounces, true);

        public static readonly BindableProperty HasScrollbarProperty =
          BindableProperty.Create<ScrollView, bool>(p => p.HasScrollbar, true);

        public static readonly BindableProperty MinZoomFactorProperty =
            BindableProperty.Create<ScrollView, double>(p => p.MinZoomFactor, 1d);

        public static readonly BindableProperty MaxZoomFactorProperty =
            BindableProperty.Create<ScrollView, double>(p => p.MaxZoomFactor, 1d);

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BaseStarShot.Controls.ScrollView"/> bounces. Use for iOS.
		/// </summary>
		public bool Bounces
		{
			get { return (bool)GetValue(BouncesProperty); }
			set { SetValue(BouncesProperty, value); }
		}


        public bool HasScrollbar
        {
            get { return (bool)GetValue(HasScrollbarProperty); }
            set { SetValue(HasScrollbarProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates the minimum permitted run-time value of ZoomFactor.
        /// </summary>
        public double MinZoomFactor
        {
            get { return (double)GetValue(MinZoomFactorProperty); }
            set { SetValue(MinZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates the maximum permitted run-time value of ZoomFactor.
        /// </summary>
        public double MaxZoomFactor
        {
            get { return (double)GetValue(MaxZoomFactorProperty); }
            set { SetValue(MaxZoomFactorProperty, value); }
        }
    }
}
