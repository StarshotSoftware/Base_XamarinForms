using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class NumberValidatorBehavior : Behavior<Entry>
    {
        private int visitCounter = 0;
        public NumberValidatorBehavior()
        {
            visitCounter++;
        }
        // Creating BindableProperties with Limited write access: http://iosapi.xamarin.com/index.aspx?link=M%3AXamarin.Forms.BindableObject.SetValue(Xamarin.Forms.BindablePropertyKey%2CSystem.Object) 

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(NumberValidatorBehavior), false);

        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        public static readonly BindableProperty BackgroundImageProperty =
            BindableProperty.Create("BackgroundImage", typeof(FileImageSource), typeof(NumberValidatorBehavior), null);

        public static readonly BindableProperty ErrorBackgroundImageProperty =
            BindableProperty.Create("ErrorBackgroundImage", typeof(FileImageSource), typeof(NumberValidatorBehavior), null);

        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        public FileImageSource ErrorBackgroundImage
        {
            get { return (FileImageSource)GetValue(ErrorBackgroundImageProperty); }
            set { SetValue(ErrorBackgroundImageProperty, value); }
        }

        //private bool defaultDisplay = true;

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result;
            //if (int.TryParse(e.NewTextValue.Substring(0, e.NewTextValue.Length - 1), out result0))
            //{

            //}

            IsValid = double.TryParse(e.NewTextValue, out result);

            var myEntry = ((Entry)sender);

            if (IsValid || visitCounter == 1)
            {
                if (BackgroundImage != null && BackgroundImage.File != null)
                    myEntry.BackgroundImage = BackgroundImage;
            }
            else
            {
                if (ErrorBackgroundImage != null && ErrorBackgroundImage.File != null)
                    myEntry.BackgroundImage = ErrorBackgroundImage;
            }

            myEntry.Tag = IsValid.ToString();

            visitCounter++;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;
        }
    }
}
