using System;
using Xamarin.Forms;
using BaseStarShot.Services;

namespace BaseStarShot.Controls
{
    public class LikeUsControl : View
    {
        public static BindableProperty PaddingProperty =
            BindableProperty.Create<LikeUsControl, Thickness>(l => l.Padding, new PointThickness(0));

        public static BindableProperty CommandProperty =
            BindableProperty.Create<LikeUsControl, System.Windows.Input.ICommand>(l => l.Command, null);

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create<LikeUsControl, object>(l => l.CommandParameter, null);

        public event EventHandler<double> ValueChangedEvent = delegate { };

        public static BindableProperty AllowSelectingZeroProperty =
            BindableProperty.Create<LikeUsControl, bool>(p => p.AllowSelectingZero, false);

        public static BindableProperty IsReadOnlyProperty =
            BindableProperty.Create<LikeUsControl, bool>(p => p.IsReadOnly, false);

        public static BindableProperty RatingItemCountProperty =
            BindableProperty.Create<LikeUsControl, int>(p => p.RatingItemCount, 0);

        public static BindableProperty ValueProperty =
            BindableProperty.Create<LikeUsControl, double>(p => p.Value, 0);

        public static BindableProperty ShowSelectionHelperProperty =
            BindableProperty.Create<LikeUsControl, bool>(p => p.ShowSelectionHelper, false);

        public static BindableProperty FilledItemBackgroundProperty =
            BindableProperty.Create<LikeUsControl, Color>(p => p.FilledItemBackground, Color.Default);

        public static BindableProperty UnFilledItemBackgroundProperty =
            BindableProperty.Create<LikeUsControl, Color>(p => p.UnFilledItemBackground, Color.Default);

        public static BindableProperty ItemWidthProperty =
            BindableProperty.Create<LikeUsControl, PointSize>(p => p.ItemWidth, new PointSize(20));

        public static BindableProperty ItemHeightProperty =
            BindableProperty.Create<LikeUsControl, PointSize>(p => p.ItemHeight, new PointSize(20));

        public void OnValueChangedEvent(object sender, double value)
        {
            if (ValueChangedEvent != null)
                ValueChangedEvent.Invoke(sender, value);
        }

        public System.Windows.Input.ICommand Command
        {
            get { return (System.Windows.Input.ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool AllowSelectingZero
        {
            get { return (bool)GetValue(AllowSelectingZeroProperty); }
            set { SetValue(AllowSelectingZeroProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public int RatingItemCount
        {
            get { return (int)GetValue(RatingItemCountProperty); }
            set { SetValue(RatingItemCountProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool ShowSelectionHelper
        {
            get { return (bool)GetValue(ShowSelectionHelperProperty); }
            set { SetValue(ShowSelectionHelperProperty, value); }
        }

        public Color FilledItemBackground
        {
            get { return (Color)GetValue(FilledItemBackgroundProperty); }
            set { SetValue(FilledItemBackgroundProperty, value); }
        }

        public Color UnFilledItemBackground
        {
            get { return (Color)GetValue(UnFilledItemBackgroundProperty); }
            set { SetValue(UnFilledItemBackgroundProperty, value); }
        }

        public PointSize ItemWidth
        {
            get { return (PointSize)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public PointSize ItemHeight
        {
            get { return (PointSize)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }
    }
}

