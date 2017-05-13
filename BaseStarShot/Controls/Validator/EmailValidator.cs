using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class EmailValidatorBehavior : Behavior<Entry>
    {
        private int visitCounter = 0;

        public EmailValidatorBehavior()
        {
            visitCounter++;
        }

        static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(NumberValidatorBehavior), false);
        public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;
        public bool IsValid
        {
            get { return (bool)base.GetValue(IsValidProperty); }
            private set { base.SetValue(IsValidPropertyKey, value); }
        }

        public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create("BackgroundImage", typeof(FileImageSource), typeof(EmailValidatorBehavior), null);

        public static readonly BindableProperty ErrorBackgroundImageProperty = BindableProperty.Create("ErrorBackgroundImage", typeof(FileImageSource), typeof(EmailValidatorBehavior), null);

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

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
        }
        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            IsValid = (Regex.IsMatch(e.NewTextValue.Trim(), Base1902.DataAnnotations.Validation.EmailRegEx, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

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

            visitCounter++;
            myEntry.Tag = IsValid.ToString();
        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
        }
    }

}
