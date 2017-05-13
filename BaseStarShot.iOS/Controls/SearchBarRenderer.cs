using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.ComponentModel;
using BaseStarShot.Controls;
using System.Drawing;
using CoreGraphics;

namespace BaseStarShot.Controls
{
	public class SearchBarRenderer : //ViewRenderer<BaseStarShot.Controls.SearchBar, UISearchBar>
		Xamarin.Forms.Platform.iOS.SearchBarRenderer
	{
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
            {
                if (e.OldElement != null)
                {
                    MessagingCenter.Unsubscribe<SearchBar>(this, SearchBar.FocusMessage);
                    MessagingCenter.Unsubscribe<SearchBar>(this, SearchBar.UnfocusMessage);
                }
                return;
            }

            var element = Element as Controls.SearchBar;
            if (element != null)
            {
                //SetShowCancelButton(element);
                SetSearchFieldBackground(element);
                SetShowsClearButton(element);
                SetShowsDoneButton(element);

                //element.FocusChanged += Element_FocusChanged;
            }

            Control.ShouldBeginEditing += Control_ShouldBeginEditing;
            Control.ShouldEndEditing += Control_ShouldEndEditing;
			Control.ClipsToBounds = true;
			Control.SetCornerRadius((nfloat)element.CornerRadius, UIRectCorner.AllCorners);

            MessagingCenter.Subscribe<SearchBar>(this, SearchBar.FocusMessage, Focus);
            MessagingCenter.Subscribe<SearchBar>(this, SearchBar.UnfocusMessage, Unfocus);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var element = Element as Controls.SearchBar;
            switch (e.PropertyName)
            {
                case "CancelButtonColor":
                case "SearchFieldBackground":
                    SetSearchFieldBackground(element);
                    break;
                case "Text":
                    SetShowsCancelButton(element);
                    break;
                case "ShowsClearButton":
                    SetShowsClearButton(element);
                    break;
                case "ShowsDoneButton":
                    SetShowsDoneButton(element);
                    break;
                default:
                    break;
            }
        }

        void SetShowsCancelButton(Controls.SearchBar element)
        {
            if (!element.ShowsCancelButton)
            {
                Control.ShowsCancelButton = element.ShowsCancelButton;
            }
        }

        void SetShowsClearButton(Controls.SearchBar element)
        {
            var textField = (UITextField)Control.ValueForKey(new Foundation.NSString("_searchField"));
            if (textField == null)
                textField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));

            textField.ClearButtonMode = element.ShowsClearButton ? UITextFieldViewMode.Always : UITextFieldViewMode.Never;
        }

        bool Control_ShouldBeginEditing(UISearchBar searchBar)
        {
            var element = Element as BaseStarShot.Controls.SearchBar;
            element.ChangeFocus(true);
            return true;
        }

        bool Control_ShouldEndEditing(UISearchBar searchBar)
        {
            var element = Element as BaseStarShot.Controls.SearchBar;
            element.ChangeFocus(false);
            return true;
        }

        void SetShowsDoneButton(Controls.SearchBar element)
        {
            if (element.ShowsDoneButton)
            {
                UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                {
                    this.Control.ResignFirstResponder();
                    if (element.DoneCommand != null &&
                        element.DoneCommand.CanExecute(null))
                    {
                        element.DoneCommand.Execute(null);
                    }
                });

                toolbar.Items = new UIBarButtonItem[]
                {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
                };
                this.Control.InputAccessoryView = toolbar;
            }
        }

        void Focus(SearchBar obj)
        {
            if (obj == Element)
            {
                if (Control != null)
                    Control.BecomeFirstResponder();
            }
        }

        void Unfocus(SearchBar obj)
        {
            if (obj == Element)
            {
                if (Control != null)
                    Control.ResignFirstResponder();
            }
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                this._disposed = true;
                SearchBar searchBar = Element as SearchBar;
                if (searchBar != null)
                {
                    MessagingCenter.Unsubscribe<SearchBar>(this, SearchBar.FocusMessage);
                    MessagingCenter.Unsubscribe<SearchBar>(this, SearchBar.UnfocusMessage);
                }
            }
            base.Dispose(disposing);
        }

//		protected bool DisableFocusOnFocusChanged { get; set; }
		
//		protected override void OnElementChanged(ElementChangedEventArgs<BaseStarShot.Controls.SearchBar> e)
//		{
//			if (e.NewElement != null)
//			{
//				if (Control == null)
//				{
//					SetNativeControl(new UISearchBar {
//                        ShowsCancelButton = false
//					});

//					var textField = (UITextField)Control.ValueForKey(new Foundation.NSString("_searchField"));
//					if (textField == null)
//						textField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));
//                    textField.ClearButtonMode = UITextFieldViewMode.Never;

//                    if (Element.SearchFieldBackground != Xamarin.Forms.Color.Default)
//                    {
//                        textField.BackgroundColor = Element.SearchFieldBackground.ToUIColor();
//                    }
//                    if (!string.IsNullOrWhiteSpace(Element.Placeholder))
//                    {
//                        textField.AttributedPlaceholder = new Foundation.NSAttributedString(
//                            Element.Placeholder, foregroundColor: Element.PlaceholderColor.ToUIColor());
//                    }

//                    if (Element.TextColor != Xamarin.Forms.Color.Default)
//                        textField.TextColor = Element.TextColor.ToUIColor();
//                    Control.SearchBarStyle = UISearchBarStyle.Minimal;


//					Control.SearchButtonClicked += OnSearchButtonClicked;
//					Control.TextChanged += OnTextChanged;
//					Control.CancelButtonClicked += OnCancelClicked;
//					Control.OnEditingStarted += OnEditingStarted;
//					Control.OnEditingStopped += OnEditingStopped;
//				}
//				UpdatePlaceholder();
//				UpdateText();
////				UpdateCancelButton();
//				Element.FocusChanged += FocusChanged;

//				SetTextInputTypes (Element);

//				Control.ClipsToBounds = true;
//				Control.SetCornerRadius((nfloat)Element.CornerRadius, UIRectCorner.AllCorners);
//			}

//			base.OnElementChanged(e);
//		}

//		protected void AddDoneButton()
//		{
//			UIToolbar toolbar = new UIToolbar (new RectangleF (0.0f, 0.0f, 50.0f, 44.0f));

//			var doneButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
//				this.Control.ResignFirstResponder ();
//				Element.SearchCommand.Execute(null);
//			});

//			toolbar.Items = new UIBarButtonItem[] {
//				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
//				doneButton
//			};
//			this.Control.InputAccessoryView = toolbar;
//		}

//		void FocusChanged (object sender, FocusEventArgs e)
//		{
//			if (DisableFocusOnFocusChanged)
//				return;
//			if (e.IsFocused) {
//				Control.BecomeFirstResponder ();
//			} else {
//				isUnfocusing = true;
//				Control.ResignFirstResponder ();
//				//Control.EndEditing (true);
//				isUnfocusing = false;
//			}
////			if (!Element.IsFocused) {
////				Control.ResignFirstResponder ();
////			}
//		}

//		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//		{
//			base.OnElementPropertyChanged(sender, e);

//			if (e.PropertyName == Xamarin.Forms.SearchBar.PlaceholderProperty.PropertyName)
//			{
//				UpdatePlaceholder();
//				return;
//			}

//			if (e.PropertyName == Xamarin.Forms.SearchBar.TextProperty.PropertyName)
//			{
//				UpdateText();
//				return;
//			}
//		}

//        void SetTextInputTypes(SearchBar element)
//        {
//			if (element == null)
//				return;
			
//            switch (element.TextInputType)
//            {
//                case TextInputType.Normal: Control.KeyboardType = UIKeyboardType.Default; break;
//                case TextInputType.EmailAddress: Control.KeyboardType = UIKeyboardType.EmailAddress; break;
//                case TextInputType.Password: Control.KeyboardType = UIKeyboardType.Default; break;
//                case TextInputType.Phone: Control.KeyboardType = UIKeyboardType.PhonePad; break;
//                case TextInputType.Number: 
//					Control.KeyboardType = UIKeyboardType.NumberPad; 
//					AddDoneButton ();
//					break;
//                case TextInputType.PersonName: Control.KeyboardType = UIKeyboardType.Default; break;
//            }
//        }

//		protected override void SetBackgroundColor(Color color)
//		{
//			base.SetBackgroundColor(color);

//			if (Control == null)
//				return;

        void SetSearchFieldBackground(Controls.SearchBar element)
        {
            var textField = (UITextField)Control.ValueForKey(new Foundation.NSString("_searchField"));
            if (textField == null)
                textField = (UITextField)Control.ValueForKey(new Foundation.NSString("searchField"));
            textField.ClearButtonMode = UITextFieldViewMode.Never;
            textField.BackgroundColor = element.SearchFieldBackground.ToUIColor();
        }

//		private void OnTextChanged(object sender, UISearchBarTextChangedEventArgs a)
//		{
//			((IElementController)Element).SetValueFromRenderer (Xamarin.Forms.SearchBar.TextProperty, Control.Text);
//		}

//		private void OnSearchButtonClicked(object sender, EventArgs e)
//		{
//			var searchCommand = Element.SearchCommand;
//			if (searchCommand != null)
//			{
//				searchCommand.Execute(Element.SearchCommandParameter);
//			}

//			Element.Raise("SearchButtonPressed", EventArgs.Empty);
//			Control.ResignFirstResponder();
//			Control.SetShowsCancelButton(false, true);
//		}

//		private void OnCancelClicked(object sender, EventArgs args)
//		{
//			((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.SearchBar.TextProperty, null);
//			Control.ResignFirstResponder();
//		}

//		private void OnEditingStarted(object sender, EventArgs args)
//		{
////			UpdateCancelButton();
//			Console.WriteLine("SearchBar OnEditingStarted");
//			Element.Focus ();
//		}

//		bool isUnfocusing;
//		void OnEditingStopped (object sender, EventArgs e)
//		{
//			if (isUnfocusing)
//				return;
//			Element.Unfocus ();
//		}

//		private void UpdatePlaceholder()
//		{
//			Control.Placeholder = (Element.Placeholder ?? string.Empty);
//		}

//		private void UpdateText()
//		{
//			Control.Text = Element.Text;
////			UpdateCancelButton();
//		}

////		private void UpdateCancelButton()
////		{
////			var shown = !string.IsNullOrEmpty(base.Control.Text);
////			Control.SetShowsCancelButton(shown, true);
////		}

//		protected override void Dispose(bool disposing)
//		{
//			if (base.Control != null)
//			{
//				Control.SearchButtonClicked -= OnSearchButtonClicked;
//				Control.TextChanged -= OnTextChanged;
//				Control.CancelButtonClicked -= OnCancelClicked;
//				Control.OnEditingStarted -= OnEditingStarted;
//				Element.FocusChanged -= FocusChanged;
//			}
//			base.Dispose(disposing);
//		}
	}
}

