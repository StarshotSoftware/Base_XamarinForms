using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;
using Android.Views.InputMethods;
using BaseStarShot.Helpers;

[assembly: ExportRenderer(typeof(Xamarin.Forms.TextCell), typeof(BaseStarShot.Controls.TextCellViewControlRenderer))]
namespace BaseStarShot.Controls
{
    public class TextCellViewControlRenderer : Xamarin.Forms.Platform.Android.TextCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            var view = base.GetCellCore(item, convertView, parent, context) as ViewGroup;
            if (String.IsNullOrEmpty((item as TextCell).Text))
            {
                view.Visibility = ViewStates.Gone;
                while (view.ChildCount > 0)
                    view.RemoveViewAt(0);
                view.SetMinimumHeight(0);
                view.SetPadding(0, 0, 0, 0);
            }
            return view;
        }

    }
}