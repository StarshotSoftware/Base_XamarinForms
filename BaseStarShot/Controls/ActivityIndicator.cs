using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ActivityIndicator : Xamarin.Forms.ActivityIndicator
    {
        public static BindableProperty ImageProperty =
            BindableProperty.Create<Button, FileImageSource>(p => p.Image, null);

        public static BindableProperty IndicatorTypeProperty =
            BindableProperty.Create<ActivityIndicator, ActivityIndicatorType>(p => p.IndicatorType, ActivityIndicatorType.Bar);

        /// <summary>
        /// Gets or sets the image or resource to be used as the indeterminate indicator.
        /// </summary>
        public FileImageSource Image
        {
            get { return (FileImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the activity indicator type to Ring or Bar. Currently works on windows only
        /// </summary>
        public ActivityIndicatorType IndicatorType
        {
            get { return (ActivityIndicatorType)GetValue(IndicatorTypeProperty); }
            set { SetValue(IndicatorTypeProperty, value); }
        }
    }
}
