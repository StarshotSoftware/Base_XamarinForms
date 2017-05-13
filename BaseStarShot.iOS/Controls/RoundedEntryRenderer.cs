using Base1902;
using BaseStarShot.Controls;
using BaseStarShot.Services;
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundedEntry), typeof(RoundedEntryRenderer))]

namespace BaseStarShot.Controls
{
    public class RoundedEntryRenderer : ViewRenderer
    {
        private CustomTextField textField = null;
        private CustomTextView textView = null;
        private bool isBeginEditingSet = false;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            var element = Element as RoundedEntry;

            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));
            toolbar.BackgroundColor = element.InputAccessoryBackgroundColor.ToUIColor();
            toolbar.TintColor = UIColor.Black;

         
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
				if (!element.ShouldEndEditing) {
					element.ShouldEndEditing = true;
					this.Control.ResignFirstResponder ();
					element.ShouldEndEditing = false;
				} else {
					if(this.Control != null)
						this.Control.ResignFirstResponder ();
				}

            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

            if (element.IsSingleLine)
            {
                textField = new CustomTextField()
                {
                    EdgeInsets = new UIEdgeInsets(5, 10, 5, 10),
                    ShowCursor = true
                };
                //textField.InputAccessoryView = toolbar;
                SetNativeControl(textField);

                textField.ShouldReturn = new UITextFieldCondition(OnShouldReturn);
                textField.ShouldChangeCharacters = OnShouldChangeCharacters;
                SetReturnKeyType(element);

                textField.EditingChanged += (sender, ev) =>
                {
                    ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Entry.TextProperty, textField.Text);
                };

                textField.SecureTextEntry = element.IsPassword;
                textField.ShouldBeginEditing = t => { return !element.IsReadOnly; };
                textField.AutocorrectionType = (element.EnableAutoCorrect) ? UITextAutocorrectionType.Default : UITextAutocorrectionType.No;

                ((IKeyPressHandler)element).SetPerformKeyPress(key =>
                {
                    var code = key.ToChar();
                    if (code.HasValue)
                        PerformKeystroke(code.Value);
                });
            }
            else
            {
                textView = new CustomTextView
                {
                    ShowCursor = true
                };
                textView.AlwaysBounceHorizontal = false;
                textView.AlwaysBounceVertical = false;
                textView.ShowsHorizontalScrollIndicator = false;
                textView.ShowsVerticalScrollIndicator = false;
                textView.BouncesZoom = false;
                textView.Bounces = false;

                textView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
                textView.ContentOffset = new CoreGraphics.CGPoint(0, 0);

                textView.ReturnKeyType = UIReturnKeyType.Default;

                textView.Changed += (sender, ev) =>
                {
                    if (textView.Text.Length == 0)
                    {
                        textView.SelectedRange = new NSRange(0, 0);

                        textView.Text = element.Placeholder;
                        textView.TextColor = element.PlaceholderColor.ToUIColor();
                        textView.Tag = 0;

                        //textView.TintColor = UIColor.Clear;
                        //if (textView.IsFirstResponder)
                        //{
                        //    //textView.ResignFirstResponder();
                        //    //textView.BecomeFirstResponder();
                        //}
                        ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Entry.TextProperty, "");

                        textView.SelectedRange = new NSRange(0, 0);
                    }
                    else
                    {
                        if (textView.Tag == 0)
                        {
                            var placeholder = element.Placeholder.Substring(1, element.Placeholder.Length - 1);
                            //bool isPlaceholderVisible = !string.IsNullOrWhiteSpace(element.Placeholder)
                            //    && textView.Text.StartsWith(placeholder);

                            //if (isPlaceholderVisible)
                            {
                                string text = textView.Text.Replace(element.Placeholder, "");
                                text = text.Replace(placeholder, "");

                                textView.TextColor = element.TextColor.ToUIColor();
                                textView.Tag = 1;
                                textView.Text = text;

                                //textView.TintColor = defaultTintColor;
                                //if (textView.IsFirstResponder)
                                //{
                                //    textView.ResignFirstResponder();
                                //    textView.BecomeFirstResponder();
                                //}
                            }
                        }

                        ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Entry.TextProperty, textView.Text);
                    }
                    //element.Text = textView.Text;
                };

                textView.SecureTextEntry = element.IsPassword;
                //textView.ShouldBeginEditing = t => { return !element.IsReadOnly; };
                textView.AutocorrectionType = UITextAutocorrectionType.No;
                textView.AutocorrectionType = UITextAutocorrectionType.No; //(element.EnableAutoCorrect) ? UITextAutocorrectionType.Default : UITextAutocorrectionType.No;

                if (element.Text != null && element.Text.Length > 0)
                {
                    textView.Tag = 1;
                }

                textView.ShowCursor = element.ShowCursor;

                if (!element.AllowCutCopyPaste)
                {
//TO DO disable ui text field
                }

                textView.InputAccessoryView = toolbar;

                SetNativeControl(textView);
            }

            SetText(element);
            SetTextAlignment(element);
            SetCornerRadius(element);
            SetBorder(element);
            SetFont(element);
            SetTextInputTypes(element);
        }



        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            var element = Element as RoundedEntry;

            switch (e.PropertyName)
            {
                case "Text":
                    if (textView != null && textView.Text.Length > 0)
                    {
                        textView.Tag = 1;
                    }

                    SetText(element);
                    break;
				case "TriggerResignResponder":
					if (!element.ShouldEndEditing) {
						element.ShouldEndEditing = true;
						this.Control.ResignFirstResponder ();
						element.ShouldEndEditing = false;
					} else
						this.Control.ResignFirstResponder ();
				break;

                case "Placeholder":
                    if (textField != null && element.Placeholder != null)
                        textField.AttributedPlaceholder = new Foundation.NSAttributedString(
                            element.Placeholder, foregroundColor: element.PlaceholderColor.ToUIColor());
                    break;

                case "ReturnKeyType":
                    if (element.IsSingleLine)
                        SetReturnKeyType(element);
                    else
                        textView.ReturnKeyType = UIReturnKeyType.Default;

                    break;
            }
        }

        private string GetTextAfterCursor()
        {
            var cursorPositionEnd = (int)textField.GetOffsetFromPosition(textField.BeginningOfDocument, textField.SelectedTextRange.End);

            var textAfterCursor = string.Empty;
            if (textField.Text.Length > cursorPositionEnd)
                textAfterCursor = textField.Text.Substring(cursorPositionEnd, textField.Text.Length - cursorPositionEnd);

            return textAfterCursor;
        }

        private string GetTextBeforeCursor()
        {
            var cursorPositionStart = (int)textField.GetOffsetFromPosition(textField.BeginningOfDocument, textField.SelectedTextRange.Start);

            var textBeforeCursor = string.Empty;
            if (cursorPositionStart > 0)
                textBeforeCursor = textField.Text.Substring(0, cursorPositionStart);

            return textBeforeCursor;
        }

        private bool OnShouldChangeCharacters(UITextField textField, NSRange range, string replacementString)
        {
            var baseElement = Element as RoundedEntry;
            if (baseElement != null && baseElement.MaxCharacter.HasValue && baseElement.MaxCharacter.Value > 0)
            {
                var newLength = (int)(textField.Text.Length + replacementString.Length - range.Length);
                return newLength <= baseElement.MaxCharacter.Value;
            }

            return true;
        }

        private bool OnShouldReturn(UITextField view)
        {
            var shouldReturn = true;

            var element = Element as RoundedEntry;
            if (view.ReturnKeyType == UIReturnKeyType.Next)
            {
                if (element.NextElement != null)
                {
                    element.NextElement.Focus();
                    shouldReturn = false;
                }
            }
            else if (view.ReturnKeyType == UIReturnKeyType.Done || view.ReturnKeyType == UIReturnKeyType.Send
              || view.ReturnKeyType == UIReturnKeyType.Go)
            {
                textField.ResignFirstResponder();
                if (element.Command != null && element.Command.CanExecute(element.CommandParameter))
                    element.Command.Execute(element.CommandParameter);
            }

            Element.Raise("Completed", EventArgs.Empty);
            return shouldReturn;
        }

        private void PerformKeystroke(char c)
        {
            var element = Element as RoundedEntry;

            if (!textField.IsFirstResponder)
                textField.BecomeFirstResponder();

            if (c != '\b')
            { // if number key has been pressed
                if (textField.Text == string.Empty)
                    textField.Text += c;
                else
                {
                    var textRangeStartPosition = textField.SelectedTextRange.Start;
                    element.Text = textField.Text = GetTextBeforeCursor() + c + GetTextAfterCursor();

                    var newCursorPosition = textField.GetPosition(textRangeStartPosition, 1);
                    textField.SelectedTextRange = textField.GetTextRange(newCursorPosition, newCursorPosition);
                }
            }
            else
            { // if delete key has been pressed
                if (textField.Text != string.Empty)
                {
                    if (!textField.SelectedTextRange.Start.IsEqual(textField.SelectedTextRange.End))
                    { // Has selected text
                        var textRangeStartPosition = textField.SelectedTextRange.Start;
                        element.Text = textField.Text = GetTextBeforeCursor() + GetTextAfterCursor();
                        textField.SelectedTextRange = textField.GetTextRange(textRangeStartPosition, textRangeStartPosition);
                    }
                    else
                    {
                        if (textField.BeginningOfDocument != textField.SelectedTextRange.Start)
                        {
                            var textRangeStartPosition = textField.SelectedTextRange.Start;
                            var cursorPositionStart = (int)textField.GetOffsetFromPosition(textField.BeginningOfDocument, textRangeStartPosition);
                            element.Text = textField.Text = GetTextBeforeCursor().Substring(0, cursorPositionStart - 1) + GetTextAfterCursor();

                            var newCursorPosition = textField.GetPosition(textRangeStartPosition, -1);
                            textField.SelectedTextRange = textField.GetTextRange(newCursorPosition, newCursorPosition);
                        }
                    }
                }
            }
        }

        private void SetBorder(RoundedEntry element)
        {
            if (element.BorderColor != Color.Default)
            {
                if (element.IsSingleLine)
                {
                    textField.Layer.BorderColor = element.BorderColor.ToUIColor().CGColor;
                    textField.Layer.BorderWidth = 1f;
                }
                else
                {
                    textView.Layer.BorderColor = element.BorderColor.ToUIColor().CGColor;
                    textView.Layer.BorderWidth = 1f;
                }
            }
        }

        private void SetCornerRadius(RoundedEntry element)
        {
            if (element.CornerRadius > 0)
            {
                if (element.IsSingleLine)
                {
                    textField.ClipsToBounds = true;
                    textField.Layer.CornerRadius = (nfloat)element.CornerRadius;
                }
                else
                {
                    textView.ClipsToBounds = true;
                    textView.Layer.CornerRadius = (nfloat)element.CornerRadius;
                }
            }
        }

        private void SetFont(RoundedEntry element)
        {
            var fontService = Resolver.Get<IFontService>();
            if (fontService == null) throw new InvalidOperationException("IFontService is not implemented.");
            if (element.IsSingleLine)
                textField.Font = UIFont.FromName(fontService.GetFontName(element.FontStyle), (nfloat)element.FontSize);
            else
                textView.Font = UIFont.FromName(fontService.GetFontName(element.FontStyle), (nfloat)element.FontSize);
        }

        private void SetReturnKeyType(RoundedEntry element)
        {
            if (element.IsSingleLine)
            {
                if (element.ReturnKeyType == ReturnKeyType.Automatic)
                {
                    if (element.NextElement != null)
                        textField.ReturnKeyType = UIReturnKeyType.Next;
                    else if (element.Command != null)
                        textField.ReturnKeyType = UIReturnKeyType.Done;
                }
                else if (element.ReturnKeyType == ReturnKeyType.Default)
                    textField.ReturnKeyType = UIReturnKeyType.Default;
                else if (element.ReturnKeyType == ReturnKeyType.Go)
                    textField.ReturnKeyType = UIReturnKeyType.Go;
                else if (element.ReturnKeyType == ReturnKeyType.Search)
                    textField.ReturnKeyType = UIReturnKeyType.Search;
                else if (element.ReturnKeyType == ReturnKeyType.Next)
                    textField.ReturnKeyType = UIReturnKeyType.Next;
                else if (element.ReturnKeyType == ReturnKeyType.Done)
                    textField.ReturnKeyType = UIReturnKeyType.Done;
                else if (element.ReturnKeyType == ReturnKeyType.Search)
                    textField.ReturnKeyType = UIReturnKeyType.Search;
                else if (element.ReturnKeyType == ReturnKeyType.Send)
                    textField.ReturnKeyType = UIReturnKeyType.Send;

                if (this.textField.InputAccessoryView != null)
                    this.textField.InputAccessoryView.BackgroundColor = element.InputAccessoryBackgroundColor.ToUIColor();
            }
            else
            {
                if (element.ReturnKeyType == ReturnKeyType.Automatic)
                {
                    if (element.NextElement != null)
                        textView.ReturnKeyType = UIReturnKeyType.Next;
                    else if (element.Command != null)
                        textView.ReturnKeyType = UIReturnKeyType.Done;
                }
                else if (element.ReturnKeyType == ReturnKeyType.Default)
                    textView.ReturnKeyType = UIReturnKeyType.Default;
                else if (element.ReturnKeyType == ReturnKeyType.Go)
                    textView.ReturnKeyType = UIReturnKeyType.Go;
                else if (element.ReturnKeyType == ReturnKeyType.Search)
                    textView.ReturnKeyType = UIReturnKeyType.Search;
                else if (element.ReturnKeyType == ReturnKeyType.Next)
                    textView.ReturnKeyType = UIReturnKeyType.Next;
                else if (element.ReturnKeyType == ReturnKeyType.Done)
                    textView.ReturnKeyType = UIReturnKeyType.Done;
                else if (element.ReturnKeyType == ReturnKeyType.Search)
                    textView.ReturnKeyType = UIReturnKeyType.Search;
                else if (element.ReturnKeyType == ReturnKeyType.Send)
                    textView.ReturnKeyType = UIReturnKeyType.Send;

                if (this.textView.InputAccessoryView != null)
                    this.textView.InputAccessoryView.BackgroundColor = element.InputAccessoryBackgroundColor.ToUIColor();
            }
        }

        private void SetText(RoundedEntry element)
        {
            if (textField != null)
            {
                if (!string.IsNullOrWhiteSpace(element.Placeholder))
                {
                    textField.AttributedPlaceholder = new Foundation.NSAttributedString(
                        element.Placeholder, foregroundColor: element.PlaceholderColor.ToUIColor());
                }

                if ((textField.Text + "").Trim() == (element.Text + "").Trim())
                    return;

                textField.Text = element.Text;
                textField.TextColor = element.TextColor.ToUIColor();
            }
            else
            {
                if ((textView.Text + "").Trim().Length > 0 && (textView.Text + "").Trim() == (element.Text + "").Trim())
                    return;

                textView.Text = element.Text;
                textView.TextColor = element.TextColor.ToUIColor();

                if (!isBeginEditingSet)
                {
                    textView.ShouldBeginEditing = ((tv) =>
                    {
                        if (element.IsReadOnly)
                            return false;

                        if (textView.Tag == 0)
                        {
                            textView.Text = string.Empty;
                            textView.TextColor = element.TextColor.ToUIColor();
                            textView.Tag = 1;
                            textView.SelectedRange = new NSRange(0, 0);
                        }
                        return true;
                    });

                    textView.ShouldEndEditing = ((tv) =>
                    {
                        if (textView.Text.Length == 0)
                        {
                            textView.Text = element.Placeholder;
                            textView.TextColor = element.PlaceholderColor.ToUIColor();
                            textView.Tag = 0;
                        }
						return element.ShouldEndEditing;
                    });
                }

                if (textView.Text.Length == 0)
                {
                    textView.Text = element.Placeholder;
                    textView.TextColor = element.PlaceholderColor.ToUIColor();
                    textView.Tag = 0;

                    textView.SelectedRange = new NSRange(0, 0);
                }

            }
        }

        private void SetTextAlignment(RoundedEntry element)
        {
            if (element.IsSingleLine)
            {
                switch (element.HorizontalTextAlignment)
                {
                    case Xamarin.Forms.TextAlignment.Center:
                        textField.TextAlignment = UITextAlignment.Center;
                        break;

                    case Xamarin.Forms.TextAlignment.End:
                        textField.TextAlignment = UITextAlignment.Right;
                        break;

                    case Xamarin.Forms.TextAlignment.Start:
                        textField.TextAlignment = UITextAlignment.Left;
                        break;
                }
            }
            else
            {
                switch (element.HorizontalTextAlignment)
                {
                    case Xamarin.Forms.TextAlignment.Center:
                        textView.TextAlignment = UITextAlignment.Center;
                        break;

                    case Xamarin.Forms.TextAlignment.End:
                        textView.TextAlignment = UITextAlignment.Right;
                        break;

                    case Xamarin.Forms.TextAlignment.Start:
                        textView.TextAlignment = UITextAlignment.Left;
                        break;
                }
            }
        }

        private void SetTextInputTypes(RoundedEntry element)
        {
            if (element.IsSingleLine)
            {
                switch (element.TextInputType)
                {
                    case TextInputType.Normal:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textField.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.EmailAddress:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                        textField.KeyboardType = UIKeyboardType.EmailAddress;
                        break;

                    case TextInputType.Password:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                        textField.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.Phone:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textField.KeyboardType = UIKeyboardType.PhonePad;
                        break;

                    case TextInputType.Number:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textField.KeyboardType = UIKeyboardType.NumberPad;
                        break;

                    case TextInputType.PersonName:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.Words;
                        textField.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.None:
                        textField.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        //textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                        break;
                }
            }
            else
            {
                switch (element.TextInputType)
                {
                    case TextInputType.Normal:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textView.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.EmailAddress:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.None;
                        textView.KeyboardType = UIKeyboardType.EmailAddress;
                        break;

                    case TextInputType.Password:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.None;
                        textView.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.Phone:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textView.KeyboardType = UIKeyboardType.PhonePad;
                        break;

                    case TextInputType.Number:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textView.KeyboardType = UIKeyboardType.NumberPad;
                        break;

                    case TextInputType.PersonName:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        textView.KeyboardType = UIKeyboardType.Default;
                        break;

                    case TextInputType.None:
                        textView.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                        //textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                        break;
                }
            }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (textField != null)
            {
                var element = Element as RoundedEntry;

                if (element.ImageRight != null && element.ImageRight.File != null)
                {
                    textField.LeftView = new UIImageView(new CoreGraphics.CGRect(0, 0, 7, heightConstraint));
                    textField.LeftViewMode = UITextFieldViewMode.Always;

                    textField.RightViewMode = UITextFieldViewMode.Always;
                    var imageView = new UIImageView(new UIImage(element.ImageRight));
                    imageView.Frame = new CoreGraphics.CGRect(0, (heightConstraint / 2) - (element.ImageRightHeight / 2), element.ImageRightWidth + element.TextPadding.Right, element.ImageRightHeight);
                    imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
                    textField.RightView = imageView;
                }
            }
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }
    }
}