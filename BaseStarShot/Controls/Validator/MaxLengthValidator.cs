using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class MaxLengthValidator : Behavior<Entry>
    {
        private int visitCounter = 0;

        public MaxLengthValidator()
        {
            visitCounter++;
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(MaxLengthValidator), 20);

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public static readonly BindableProperty MinLengthProperty = BindableProperty.Create("MinLength", typeof(int), typeof(MaxLengthValidator), 0);

        public static readonly BindableProperty BackgroundImageProperty =
            BindableProperty.Create("BackgroundImage", typeof(FileImageSource), typeof(MaxLengthValidator), null);

        public static readonly BindableProperty ErrorBackgroundImageProperty =
            BindableProperty.Create("ErrorBackgroundImage", typeof(FileImageSource), typeof(MaxLengthValidator), null);

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

        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += bindable_TextChanged;
        }

        private void bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (MaxLength != null && MaxLength.HasValue)
            var IsValid = (e.NewTextValue.Trim().Length >= MinLength && e.NewTextValue.Trim().Length <= MaxLength);

            var myEntry = ((Entry)sender);

            if (IsValid || visitCounter == 1)
            {
                //if (BackgroundImage != null && BackgroundImage.File != null)
                //    myEntry.BackgroundImage = BackgroundImage;
            }
            else
            {
                //if (ErrorBackgroundImage != null && ErrorBackgroundImage.File != null)
                //    myEntry.BackgroundImage = ErrorBackgroundImage;
                myEntry.Text = e.NewTextValue.Substring(0, MaxLength);
            }

            visitCounter++;
            myEntry.Tag = IsValid.ToString();
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= bindable_TextChanged;

        }
    }
}
