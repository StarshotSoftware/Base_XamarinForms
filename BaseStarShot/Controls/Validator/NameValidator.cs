using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class NameValidator : Behavior<Entry>
    {
        private int visitCounter = 0;

        public NameValidator()
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

        public static readonly BindableProperty IsLengthValidProperty = BindableProperty.Create("IsLengthValid", typeof(bool), typeof(NameValidator), true);

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create("MaxLength", typeof(int), typeof(NameValidator), 20);

        public static readonly BindableProperty MinLengthProperty = BindableProperty.Create("MinLength", typeof(int), typeof(NameValidator), 2);

        public static readonly BindableProperty BackgroundImageProperty = BindableProperty.Create("BackgroundImage", typeof(FileImageSource), typeof(NameValidator), null);

        public static readonly BindableProperty ErrorBackgroundImageProperty = BindableProperty.Create("ErrorBackgroundImage", typeof(FileImageSource), typeof(NameValidator), null);

        public bool IsLengthValid
        {
            get { return (bool)GetValue(IsLengthValidProperty); }
            set { SetValue(IsLengthValidProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

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
            bindable.TextInputType = TextInputType.PersonName;
            bindable.TextChanged += HandleTextChanged;
        }
        void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            IsValid = (Regex.IsMatch(e.NewTextValue, Base1902.DataAnnotations.Validation.NameRegEx, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)));

            IsLengthValid = e.NewTextValue.Trim().Length >= MinLength && e.NewTextValue.Trim().Length <= MaxLength;

            var myEntry = ((Entry)sender);

            if (e.NewTextValue.Length >= MaxLength)
            {
                myEntry.Text = e.NewTextValue.Substring(0, MaxLength);
                IsLengthValid = true;
            }

            if ((IsValid && IsLengthValid) || visitCounter == 1)
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

            myEntry.Tag = (IsValid && IsLengthValid).ToString();


        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
        }
    }

}
