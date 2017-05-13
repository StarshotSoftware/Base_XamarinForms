using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BaseStarShot.Controls;
using Android.Graphics.Drawables;
using Android.Text;
using System.Reflection;


[assembly: ExportRenderer(typeof(BaseStarShot.Controls.RoundedComboBox), typeof(RoundedComboBoxRenderer))]
namespace BaseStarShot.Controls
{
    using System.Collections.Specialized;
    using BaseStarShot.Util;

    public class RoundedComboBoxRenderer : ViewRenderer<RoundedComboBox, GridLayout>, IDialogInterfaceOnClickListener
    {
        private GridLayout.LayoutParams textViewLayoutParam;
        private INotifyCollectionChanged collectionChanged = null;

        private TextView textView = null;
        AlertDialog.Builder spinnerAlert = null;
        AlertDialog alert = null;

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedComboBox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            if (Control == null)
            {
                var grid = new GridLayout(this.Context);
				grid.SetPadding(0, 0, 0, 0);
                grid.Clickable = false;
                grid.SetBackgroundColor(Xamarin.Forms.Color.Transparent.ToAndroid());

                spinnerAlert = new AlertDialog.Builder(this.Context);
                textView = new TextView(this.Context);
                //alert = spinnerAlert.Create();

                textViewLayoutParam = new GridLayout.LayoutParams();
                textViewLayoutParam.RowSpec = GridLayout.InvokeSpec(0, GridLayout.Fill);
                textViewLayoutParam.ColumnSpec = GridLayout.InvokeSpec(0, GridLayout.Fill);

                grid.AddView(textView, textViewLayoutParam);


                SetNativeControl(grid);

                SetTextAlignment();
                SetText();
				SetTextColor();
                CreateShapeDrawable();

                SetSpinnerAdapter(new List<string>());
                SetItemsSource();
                SetTypeface();

                textView.Click += async (s, ev) =>
                {
                    if (alert != null)
                    {

                        if (Element.IsEnabled)
                        {
                            alert.Show();
                            SetSelectedIndex();
                        }
                        

                    }
                   
                    //spinnerAlert.Show();
                };

                this.Background = new ColorDrawable(Xamarin.Forms.Color.Transparent.ToAndroid());
            }

        }

        void SetTypeface()
        {
            if (!string.IsNullOrEmpty(Element.FontFamily))
                textView.Typeface = FontCache.GetTypeFace(Element.FontFamily);

            textView.TextSize = (int)Math.Round(Element.FontSize);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            var width = MeasureSpec.GetSize(widthMeasureSpec);
            var height = MeasureSpec.GetSize(heightMeasureSpec);

            SetImage(height);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            var property = e.PropertyName;
            switch (property)
            {
                case "ItemsSource":
                    if (Adapter.Count > 0)
                        Adapter.Clear();

                    SetItemsSource();
                    break;
                case "SelectedIndex":
                    SetSelectedIndex();
                    break;
                case "FontSize":
                case "Font":
                    SetTypeface();
                    break;
				case "TextColor":
					SetTextColor();
					break;
                case "ImageLeft":
                    this.RequestLayout();
                    break;
            }
        }

        void SetSelectedIndex()
        {
            var hasPlaceholder = !string.IsNullOrWhiteSpace(Element.Placeholder);
            int position = Element.SelectedIndex;

            if (hasPlaceholder)
                position = Element.SelectedIndex + 1;

            if (alert != null && alert.ListView != null)
            {
                if (position >= 0 && position < Adapter.Count)
                {
                    alert.ListView.SetItemChecked(position, true);
                    alert.ListView.SmoothScrollToPositionFromTop(position, 0, 0);
                    textView.Text = Adapter.GetItem(position);
                }
            }
        }

        void SetItemsSource()
        {
            if (Element.ItemsSource == null)
                return;

            if (collectionChanged == null)
            {
                #region set collectionchanged
                collectionChanged = (INotifyCollectionChanged)Element.ItemsSource;
                collectionChanged.CollectionChanged += (s, ev) =>
                {
                    if (ev.Action == NotifyCollectionChangedAction.Add)
                    {
                        PropertyInfo prop = null;

                        var item = Element.ItemsSource.Cast<object>().ToList().ElementAt(ev.NewStartingIndex);

                        if (!string.IsNullOrWhiteSpace(Element.DisplayTextField))
                        {
                            if (prop == null)
                                prop = item.GetType().GetRuntimeProperty(Element.DisplayTextField);

                            if (prop != null && prop.GetValue(item) != null)
                            {
                                Adapter.Add(prop.GetValue(item).ToString());
                            }
                        }
                        else
                        {
                            Adapter.Add(item + "");
                        }

                        Adapter.NotifyDataSetChanged();

                        if (!string.IsNullOrWhiteSpace(Element.Placeholder))
                        {
                            if ((Adapter.GetItem(0) + "") != Element.Placeholder)
                            {
                                Adapter.Insert(Element.Placeholder, 0);
                                Adapter.NotifyDataSetChanged();
                            }
                        }

                        if (GetItemsSourceCount() == ev.NewStartingIndex)
                        {
                            var hasPlaceholder = !string.IsNullOrWhiteSpace(Element.Placeholder);
                            int position = Element.SelectedIndex;

                            if (hasPlaceholder)
                                position = Element.SelectedIndex + 1;

                            if (alert == null)
                            {
                                spinnerAlert.SetSingleChoiceItems(Adapter, position, this);
                                CreateAlert();
                            }
                            else
                            {
                                Adapter.NotifyDataSetChanged();
                                alert.ListView.SetSelection(position);
                            }
                            textView.Text = Adapter.GetItem(position);
                        }
                    }
                    else if (ev.Action == NotifyCollectionChangedAction.Reset)
                    {
                        if (Adapter != null)
                        {
                            Adapter.Clear();
                            Adapter.NotifyDataSetChanged();
                        }
                    }
                };
                #endregion
            }

            if (Adapter.Count == 0)
            {
                int index = 0, selectedIndex = -1;
                foreach (var item in Element.ItemsSource)
                {
                    PropertyInfo prop = null;

                    if (!string.IsNullOrWhiteSpace(Element.DisplayTextField))
                    {
                        if (prop == null)
                            prop = item.GetType().GetRuntimeProperty(Element.DisplayTextField);

                        if (prop != null && prop.GetValue(item) != null)
                        {
                            Adapter.Add(prop.GetValue(item).ToString());
                        }
                    }
                    else
                    {
                        Adapter.Add(item + "");
                    }

                    Adapter.NotifyDataSetChanged();

                    if (!string.IsNullOrWhiteSpace(Element.Placeholder))
                    {
                        if ((Adapter.GetItem(0) + "") != Element.Placeholder)
                        {
                            Adapter.Insert(Element.Placeholder, 0);
                            Adapter.NotifyDataSetChanged();
                        }
                    }

                    var hasPlaceholder = !string.IsNullOrWhiteSpace(Element.Placeholder);
                    int position = Element.SelectedIndex;

                    if (hasPlaceholder)
                    {
                        position = Element.SelectedIndex + 1;
                    }

                    if (index == position)
                    {
                        selectedIndex = position;
                        textView.Text = Adapter.GetItem(selectedIndex);
                    }
                    index += 1;
                }

                if (alert == null)
                {
                    spinnerAlert.SetSingleChoiceItems(Adapter, selectedIndex, this);
                    CreateAlert();
                }

            }

        }

        void CreateAlert()
        {
            alert = spinnerAlert.Create();
            alert.SetCanceledOnTouchOutside(true);
            alert.DismissEvent += Alert_DismissEvent;
            alert.ShowEvent += Alert_ShowEvent;
        }

        private void Alert_ShowEvent(object sender, EventArgs e)
        {
			if (Element != null)
            	Element.IsDialogVisible = true;
        }

        private void Alert_DismissEvent(object sender, EventArgs e)
        {
			if(Element != null)
            	Element.IsDialogVisible = false;
        }

        private ArrayAdapter<String> Adapter = null;
        void SetSpinnerAdapter(List<string> list)
        {
            if (Adapter == null)
            {
                Adapter = new ArrayAdapter<String>(this.Context, Android.Resource.Layout.SelectDialogSingleChoice);
            }
        }


        int GetItemsSourceCount()
        {
            if (Element.ItemCount.HasValue)
                return Element.ItemCount.Value - 1;

            int ctr = 0;
            foreach (var item in Element.ItemsSource)
            {
                ctr += 1;
            }
            return ctr;
        }

        void SetTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center: textView.Gravity = GravityFlags.Center; break;
                case Xamarin.Forms.TextAlignment.End: textView.Gravity = GravityFlags.End | GravityFlags.CenterVertical; break;
                case Xamarin.Forms.TextAlignment.Start: textView.Gravity = GravityFlags.Start | GravityFlags.CenterVertical; break;
            }
        }

        void SetText()
        {
            textView.Hint = Element.Placeholder;
            textView.SetMaxLines(1);
            textView.Ellipsize = TextUtils.TruncateAt.End;
            textView.LayoutParameters = textViewLayoutParam;
            textView.CompoundDrawablePadding = 10;
            textView.SetSingleLine(true);

            textView.SetBackgroundColor(Xamarin.Forms.Color.Transparent.ToAndroid());

            if (Element.PlaceholderColor != Xamarin.Forms.Color.Default)
                textView.SetHintTextColor(Element.PlaceholderColor.ToAndroid());

            textView.TextChanged += (s, ev) =>
            {
                Element.Text = textView.Text;
            };
        }

		void SetTextColor()
		{
			if (Element.TextColor != Xamarin.Forms.Color.Default)
				textView.SetTextColor(Element.TextColor.ToAndroid());
		}

        void CreateShapeDrawable()
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetCornerRadius(UIHelper.ConvertDPToPixels(Element.CornerRadius));

            if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
                shape.SetColor(Element.BackgroundColor.ToAndroid());

            Control.Background = shape;
        }

        void SetImage(int height)
        {
            Drawable drawableRight = null;
            Drawable drawableLeft = null;

            if (Element.ImageRight != null && Element.ImageRight.File != null)
            {
                var element = Element.ImageRight;
                drawableRight = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.ImageRight));
                drawableRight.SetBounds(0, 0, UIHelper.ConvertDPToPixels(Element.ImageRightWidth), height);

            }

            if (Element.ImageLeft != null && Element.ImageLeft.File != null)
            {
                drawableLeft = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.ImageLeft));
                drawableLeft.SetBounds(0, 0, UIHelper.ConvertDPToPixels(Element.ImageLeftWidth), UIHelper.ConvertDPToPixels(Element.ImageLeftHeight));
            }

            textView.SetCompoundDrawablesRelative(drawableLeft, null, drawableRight, null);
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }

        public void OnClick(IDialogInterface dialog, int which)
        {
            textView.Text = Adapter.GetItem(which) + "";

            var hasPlaceholder = !string.IsNullOrWhiteSpace(Element.Placeholder);
            if (hasPlaceholder)
            {
                var position = which - 1;
                Element.SelectedIndex = position;
                if (position < 0)
                    Element.SelectedItem = null;
                else
                    Element.SelectedItem = Element.ItemsSource.Cast<object>().ToList().ElementAt(position);
            }
            else
            {
                Element.SelectedIndex = which;
                Element.SelectedItem = Element.ItemsSource.Cast<object>().ToList().ElementAt(which);
            }

            alert.Dismiss();
        }
    }

}