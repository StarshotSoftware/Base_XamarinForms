using System;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using Xamarin.Forms;
using BaseStarShot.Controls;
using UIKit;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using BaseStarShot.Services;
using System.Reflection;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(RoundedComboBox), typeof(RoundedComboBoxRenderer))]
namespace BaseStarShot.Controls
{
    public class RoundedComboBoxRenderer : ViewRenderer<RoundedComboBox, UITextField>
    {
        private UIPickerView picker;
        UIView overlayView;
        private INotifyCollectionChanged collectionChanged = null;
        PickerModel pickerModel = null;

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedComboBox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            var element = Element as RoundedComboBox;

            var textField = new CustomTextField
            {
                 ShowCursor = false    
                //EdgeInsets = new UIEdgeInsets(5, 10, 5, 10)
            };
                    
            textField.BackgroundColor = element.BackgroundColor.ToUIColor();
            textField.EditingDidBegin += (sender, args) =>
            {
                var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (rootViewController.PresentedViewController != null)
                    rootViewController = rootViewController.PresentedViewController;
                var rootView = rootViewController.View;

                if (overlayView == null)
                {
                    overlayView = new UIView(rootView.Bounds);
                    overlayView.UserInteractionEnabled = true;
                    overlayView.AddGestureRecognizer(new UITapGestureRecognizer(() => Control.ResignFirstResponder()));
                }
                Element.IsDialogVisible = true;
                rootView.Add(overlayView);
            };
            textField.EditingDidEnd += (sender, args) =>
            {
                if (overlayView != null)
                {
                    overlayView.RemoveFromSuperview();
                }
                Element.IsDialogVisible = false;
            };

            SetNativeControl(textField);

            Control.ShouldChangeCharacters += (field, range, replacementString) => false;

            //Control.ShouldBeginEditing = t => { return false; };
            Control.SetCornerRadius((nfloat)element.CornerRadius, UIRectCorner.AllCorners);

            picker = new UIPickerView();

            Control.InputView = picker;

            SetModel(element);
            SetItemsSource();

            SetSelectedIndex(element);
			//SetText(element);
			SetTextColor(element);
            SetTextAlignment(element);
            SetBorder(element);
            SetFont(element);

            AddDoneButton();
        }

        protected void AddDoneButton()
        {
            UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                this.Control.ResignFirstResponder();
            });

            toolbar.Items = new UIBarButtonItem[] {
				new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
				doneButton
			};
            this.Control.InputAccessoryView = toolbar;
        }
            
        void SetItemsSource()
        {
            if (Element.ItemsSource == null)
                return;

            var itemCount = Element.ItemsSource.Cast<object>().ToList().Count;
            if (itemCount > 0)
            {
                for (var i = 0; i < itemCount; i++)
                {
                    AddObjectToSource(i);
                }
            }

            if (collectionChanged == null)
            {
                collectionChanged = (INotifyCollectionChanged)Element.ItemsSource;
                collectionChanged.CollectionChanged += (s, ev) =>
                {
                    if (Element == null || Control == null)
                        return;

                    if (ev.Action == NotifyCollectionChangedAction.Add)
                    {
                        AddObjectToSource(ev.NewStartingIndex);
                    }
                    else if (ev.Action == NotifyCollectionChangedAction.Reset)
                    {
                        if (pickerModel != null)
                        {
                            pickerModel.ResetData();
                            picker.ReloadAllComponents();
                            //Adapter.NotifyDataSetChanged();
                            //isReset = true;
                        }
                    }
                };

            }
        }

        void AddObjectToSource(int newIndex)
        {
            PropertyInfo prop = null;

            var item = Element.ItemsSource.Cast<object>().ToList().ElementAt(newIndex);
            string text = "";

            if (!string.IsNullOrWhiteSpace(Element.DisplayTextField))
            {
                if (prop == null)
                    prop = item.GetType().GetRuntimeProperty(Element.DisplayTextField);

                if (prop != null && prop.GetValue(item) != null)
                {
                    text = prop.GetValue(item).ToString();
                    pickerModel.AddData(text);
                }
            }
            else
            {
                text = item + "";
                pickerModel.AddData(text);
            }

            if (!string.IsNullOrWhiteSpace(Element.Placeholder))
            {
                pickerModel.InsertData(0, Element.Placeholder);
            }

            var index = (!string.IsNullOrWhiteSpace(Element.Placeholder)) ?
                Element.SelectedIndex + 1 : Element.SelectedIndex;
            if (newIndex == index)
            {

                //Control.Text = text;
                //                                var selectedIndex = element.SelectedIndex;
                //                                if (!string.IsNullOrEmpty(element.Placeholder))
                //                                    selectedIndex = selectedIndex < 0 ? 0 : selectedIndex + 1;
                //
                if (pickerModel.Count() > 0)
                    picker.Select(index, 0, false);

                Control.Text = pickerModel.GetText(index);
            }
            picker.ReloadAllComponents();
        }

        void SetBorder(RoundedComboBox element)
        {
            if (element.BorderColor != Xamarin.Forms.Color.Default)
            {
                Control.Layer.BorderColor = element.BorderColor.ToUIColor().CGColor;
                Control.Layer.BorderWidth = 1f;
            }
        }

        void SetTextAlignment(RoundedComboBox element)
        {
            switch (element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.TextAlignment = UITextAlignment.Center;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.TextAlignment = UITextAlignment.Right;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.TextAlignment = UITextAlignment.Left;
                    break;
            }
        }
            
        void SetModel(RoundedComboBox element)
        {
            //            if (element.ItemsSource != null)
            //            {
            //                PropertyInfo prop = null;
            //
            //                foreach (var item in Element.ItemsSource)
            //                {
            //                    if (!string.IsNullOrWhiteSpace(Element.DisplayTextField))
            //                    {
            //                        if (prop == null)
            //                            prop = item.GetType().GetRuntimeProperty(Element.DisplayTextField);
            //
            //                        if (prop != null && prop.GetValue(item) != null)
            //                        {
            //                            list.Add(prop.GetValue(item).ToString());
            //                        }
            //                    }
            //                    else
            //                    {
            //                        list.Add(item + "");
            //                    }
            //                }
            //
            //                if (!string.IsNullOrEmpty(element.Placeholder))
            //                    list.Insert(0, element.Placeholder);
            //
            //
            //            }
            if (pickerModel == null)
            {
                pickerModel = new PickerModel(new List<string>(), Control,
                    element, !string.IsNullOrEmpty(Element.Placeholder));
                picker.Model = pickerModel; //new PickerModel(list.ToArray(), Control, element);
            }
        }

        void SetSelectedIndex(RoundedComboBox element)
        {
            var selectedIndex = element.SelectedIndex;
            if (!string.IsNullOrEmpty(element.Placeholder))
                selectedIndex = selectedIndex < 0 ? 0 : selectedIndex + 1;

            if (pickerModel.Count() > 0)
                picker.Select(selectedIndex, 0, false);
        }

        void SetText(RoundedComboBox element)
        {
			if (element.ItemsSource != null && element.SelectedIndex >= 0)
            {
                PropertyInfo prop = null;
				var list = Element.ItemsSource.Cast<object> ().ToList ();
				if (list.Count > element.SelectedIndex) {
					var item = list.ElementAt (element.SelectedIndex);
					string text = "";

					if (!string.IsNullOrWhiteSpace (Element.DisplayTextField)) {
						if (prop == null)
							prop = item.GetType ().GetRuntimeProperty (Element.DisplayTextField);

						if (prop != null && prop.GetValue (item) != null) {
							text = prop.GetValue (item).ToString ();
						}
					} else {
						text = item + "";
					}

					Control.Text = text;
				}
            }
            else
                Control.Text = element.Placeholder;
        }

		void SetTextColor(RoundedComboBox element)
		{
			Control.TextColor = element.TextColor.ToUIColor();
		}

        void SetFont(RoundedComboBox element)
        {
            if (!string.IsNullOrWhiteSpace(element.FontFamily))
            {
                Control.Font = UIFont.FromName(element.FontFamily, element.FontSize < 0 ? Control.Font.PointSize : (nfloat)element.FontSize);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var element = Element as BaseStarShot.Controls.RoundedComboBox;

            switch (e.PropertyName)
            {
                case "ItemsSource":
                    SetItemsSource();
                    //SetModel(element); 
                    break;
                case "SelectedIndex":
                    SetSelectedIndex(element);
                    SetText(element);
                    break;
                case "ImageLeft":
                    SetImages((double)this.Control.Frame.Height);
                    break;
				case "TextColor":
					SetTextColor(element);
					break;
					
            }
        }

        //public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        //{
        //    SetImages(heightConstraint);

        //    return base.GetDesiredSize(widthConstraint, heightConstraint);
        //}

        private void SetImages(double heightConstraint)
        {
            var element = Element;

            if (element.ImageRight != null && element.ImageRight.File != null)
            {
                Control.RightViewMode = UITextFieldViewMode.Always;
                var imageView = new UIImageView(new UIImage(element.ImageRight));
                imageView.Frame = new CoreGraphics.CGRect(0, 0, element.ImageRightWidth, heightConstraint);
                imageView.ContentMode = UIViewContentMode.ScaleToFill;
                imageView.UserInteractionEnabled = true;
                imageView.AddGestureRecognizer(new UITapGestureRecognizer(() => Control.BecomeFirstResponder()));

                var paddingView = new UIView();
                paddingView.Frame = new CoreGraphics.CGRect(0, 0, element.ImageRightWidth + element.TextPadding.Right, heightConstraint);
                paddingView.AddSubview(imageView);

                imageView.UserInteractionEnabled = true;
                paddingView.UserInteractionEnabled = true;

                Control.RightView = paddingView;
            }

            Control.LeftViewMode = UITextFieldViewMode.Always;
            if (element.ImageLeft != null && element.ImageLeft.File != null)
            {
                var imageView = new UIImageView(new UIImage(element.ImageLeft));
                imageView.Frame = new CoreGraphics.CGRect(element.TextPadding.Left,
                    (heightConstraint / 2) - (element.ImageLeftHeight / 2),
                    element.ImageLeftWidth,
                    element.ImageLeftHeight);
                imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

                var paddingView = new UIView();
                paddingView.Frame = new CoreGraphics.CGRect(0, 0, element.ImageLeftWidth + 10, heightConstraint);
                paddingView.AddSubview(imageView);

                Control.LeftView = paddingView;
            }
            else
            {
                var paddingView = new UIView();
                paddingView.Frame = new CoreGraphics.CGRect(0, 0, element.TextPadding.Left, heightConstraint);

                Control.LeftView = paddingView;
            }
        }
    }

    public class PickerModel : UIPickerViewModel
    {
        private List<string> source;
        private UITextField textField;
        private RoundedComboBox element;
        private bool hasPlaceholder;

        public PickerModel(List<string> source, UITextField textField,
                           RoundedComboBox element, bool hasPlaceholder)
        {
            this.source = source;
            this.textField = textField;
            this.element = element;
            this.hasPlaceholder = hasPlaceholder;
        }

        public int Count()
        {
            return (source == null) ? 0 : source.Count;
        }

        public void ResetData()
        {
            if (source != null)
            {
                source.Clear();
                //this.NotifyDataSetChanged();
            }
        }

        public void InsertData(int index, string item)
        {
            if (source != null && !source.Contains(item))
            {
                source.Insert(index, item);
                //this.NotifyDataSetChanged();
            }
        }

        public void AddData(string item)
        {
            if (source != null && !source.Contains(item))
            {
                source.Add(item);
                //this.NotifyDataSetChanged();
            }
        }

        public string GetText(int index)
        {
            if (source != null && source.Count > index && index >= 0)
            {
                return source.ElementAt(index);
            }
            return "";
        }

        public override nint GetComponentCount(UIPickerView v)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return source != null ? source.Count : 0;
        }

        public override string GetTitle(UIPickerView picker, nint row, nint component)
        {
            if (source == null || row > source.Count() || row < 0)
                return null;

            if (component == 0)
                return source.ElementAt((int)row);
            else
                return row.ToString();
        }

        public override void Selected(UIPickerView picker, nint row, nint component)
        {
            textField.Text = source.ElementAt((int)picker.SelectedRowInComponent(0)) + "";
            element.SelectedItem = source.ElementAt((int)picker.SelectedRowInComponent(0)) + "";
            element.SelectedIndex = (this.hasPlaceholder) ? ((int)row - 1) : (int)row;
            element.Text = source.ElementAt((int)picker.SelectedRowInComponent(0)) + "";
        }

        public override nfloat GetRowHeight(UIPickerView picker, nint component)
        {
            return 40f;
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            var tView = (UILabel)view;
            if (tView == null)
            {
                tView = new UILabel();
                tView.TextAlignment = UITextAlignment.Center;

                if (!string.IsNullOrWhiteSpace(element.FontFamily))
                    tView.Font = UIFont.FromName(element.FontFamily, tView.Font.PointSize);
            }

            tView.Text = source.ElementAt((int)row);

            return tView;
        }
    }

}

