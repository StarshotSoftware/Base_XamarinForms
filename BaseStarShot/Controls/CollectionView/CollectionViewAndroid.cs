using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class CollectionViewAndroid : View
    {
        public static new readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create<CollectionViewAndroid, IEnumerable>(p => p.ItemsSource, null);

        public static new readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create<CollectionViewAndroid, DataTemplate>(p => p.ItemTemplate, null);

        public static readonly BindableProperty ItemsInRowProperty =
            BindableProperty.Create<CollectionViewAndroid, int>(p => p.ItemsInRow, 1);

        public static readonly BindableProperty RowPaddingProperty =
            BindableProperty.Create<CollectionViewAndroid, Thickness>(p => p.RowPadding, 0);

        public static readonly BindableProperty ColumnSpacingProperty =
            BindableProperty.Create<CollectionViewAndroid, double>(p => p.ColumnSpacing, 0);

        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public new DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public int ItemsInRow
        {
            get { return (int)GetValue(ItemsInRowProperty); }
            set { SetValue(ItemsInRowProperty, value); }
        }

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        public Thickness RowPadding
        {
            get { return (Thickness)GetValue(RowPaddingProperty); }
            set { SetValue(RowPaddingProperty, value); }
        }


        public CollectionViewAndroid()
        {

        }



    }
}
