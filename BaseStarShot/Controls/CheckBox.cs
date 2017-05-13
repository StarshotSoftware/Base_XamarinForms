using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    /// <summary>
    /// The check box.
    /// </summary>
    public class CheckBox : View
    {
        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create<CheckBox, double>(p => p.ImageWidth, 0);

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create<CheckBox, double>(p => p.ImageHeight, 0);

        public static readonly BindableProperty SpacingProperty =
            BindableProperty.Create<CheckBox, double?>(p => p.Spacing, null);

        public static readonly BindableProperty CheckedImageProperty =
            BindableProperty.Create<CheckBox, FileImageSource>(p => p.CheckedImage, null);

        public static readonly BindableProperty UnCheckedImageProperty =
            BindableProperty.Create<CheckBox, FileImageSource>(p => p.UnCheckedImage, null);

        /// <summary>
        /// The inline text only for windows
        /// </summary>
        public static readonly BindableProperty InlineTextProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.InlineText, string.Empty);
        /// <summary>
        /// The checked state property.
        /// </summary>
        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create<CheckBox, bool>(
                p => p.Checked, false, BindingMode.TwoWay, propertyChanged: OnCheckedPropertyChanged);

        /// <summary>
        /// The checked text property.
        /// </summary>
        public static readonly BindableProperty CheckedTextProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.CheckedText, string.Empty, BindingMode.TwoWay);

        /// <summary>
        /// The unchecked text property.
        /// </summary>
        public static readonly BindableProperty UncheckedTextProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.UncheckedText, string.Empty);

        public static readonly BindableProperty EnableLinkProperty =
           BindableProperty.Create<CheckBox, bool>(p => p.EnableLink, false);

        public static readonly BindableProperty LinkStringProperty =
            BindableProperty.Create<CheckBox, string>(p => p.LinkString, null);

        /// <summary>
        /// The default text property.
        /// </summary>
        public static readonly BindableProperty DefaultTextProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.Text, string.Empty);

        /// <summary>
        /// Identifies the TextColor bindable property.
        /// </summary>
        /// 
        /// <remarks/>
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<CheckBox, Color>(
                p => p.TextColor, Color.Black);

        /// <summary>
        /// The font size property
        /// </summary>
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<CheckBox, double>(
                p => p.FontSize, -1);

        /// <summary>
        /// The font name property.
        /// </summary>
        public static readonly BindableProperty FontNameProperty =
            BindableProperty.Create<CheckBox, string>(
                p => p.FontName, string.Empty);


        /// <summary>
        /// The checked changed event.
        /// </summary>
        public event EventHandler<EventArgs<bool>> CheckedChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get
            {
                return (bool)this.GetValue(CheckedProperty);
            }

            set
            {
                if (this.Checked != value)
                {
                    this.SetValue(CheckedProperty, value);
                    this.CheckedChanged.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the inline text
        /// </summary>
        public string InlineText
        {
            get
            {
                return (string)this.GetValue(InlineTextProperty);
            }

            set
            {
                this.SetValue(InlineTextProperty, value);
            }
        }

        public double? Spacing
        {
            get { return (double?)this.GetValue(SpacingProperty); }
            set { this.SetValue(SpacingProperty, value); }
        }

        public double ImageWidth
        {
            get { return (double)this.GetValue(ImageWidthProperty); }
            set { this.SetValue(ImageWidthProperty, value); }
        }

        public double ImageHeight
        {
            get { return (double)this.GetValue(ImageHeightProperty); }
            set { this.SetValue(ImageHeightProperty, value); }
        }

        public FileImageSource CheckedImage
        {
            get
            {
                return (FileImageSource)this.GetValue(CheckedImageProperty);
            }

            set
            {
                this.SetValue(CheckedImageProperty, value);
            }
        }

        public FileImageSource UnCheckedImage
        {
            get
            {
                return (FileImageSource)this.GetValue(UnCheckedImageProperty);
            }

            set
            {
                this.SetValue(UnCheckedImageProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating the checked text.
        /// </summary>
        /// <value>The checked state.</value>
        /// <remarks>
        /// Overwrites the default text property if set when checkbox is checked.
        /// </remarks>
        public string CheckedText
        {
            get
            {
                return (string)this.GetValue(CheckedTextProperty);
            }

            set
            {
                this.SetValue(CheckedTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        /// <remarks>
        /// Overwrites the default text property if set when checkbox is checked.
        /// </remarks>
        public string UncheckedText
        {
            get
            {
                return (string)this.GetValue(UncheckedTextProperty);
            }

            set
            {
                this.SetValue(UncheckedTextProperty, value);
            }
        }

        public bool EnableLink
        {
            get
            {
                return (bool)this.GetValue(EnableLinkProperty);
            }

            set
            {
                this.SetValue(EnableLinkProperty, value);
            }
        }

        public string LinkString
        {
            get
            {
                return (string)this.GetValue(LinkStringProperty);
            }

            set
            {
                this.SetValue(LinkStringProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string DefaultText
        {
            get
            {
                return (string)this.GetValue(DefaultTextProperty);
            }

            set
            {
                this.SetValue(DefaultTextProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return (Color)this.GetValue(TextColorProperty);
            }

            set
            {
                this.SetValue(TextColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
            set
            {
                SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get
            {
                return (string)GetValue(FontNameProperty);
            }
            set
            {
                SetValue(FontNameProperty, value);
            }
        }
        public string Text
        {
            get
            {
                return this.Checked
                    ? (string.IsNullOrEmpty(this.CheckedText) ? this.DefaultText : this.CheckedText)
                        : (string.IsNullOrEmpty(this.UncheckedText) ? this.DefaultText : this.UncheckedText);
            }
        }

        /// <summary>
        /// Called when [checked property changed].
        /// </summary>
        /// <param name="bindable">The bindable.</param>
        /// <param name="oldvalue">if set to <c>true</c> [oldvalue].</param>
        /// <param name="newvalue">if set to <c>true</c> [newvalue].</param>
        private static void OnCheckedPropertyChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var checkBox = (CheckBox)bindable;
            checkBox.Checked = newvalue;
        }
    }
}
