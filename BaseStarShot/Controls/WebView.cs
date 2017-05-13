using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class WebView : Xamarin.Forms.WebView
    {
		public static readonly BindableProperty IsScrollableProperty =
		   BindableProperty.Create<WebView, bool> (p => p.IsScrollable, false);

		public bool IsScrollable {
			get { return (bool)GetValue (IsScrollableProperty); }
			set { SetValue (IsScrollableProperty, value); }
		}


		//<!------------------ For winPhone use only
		//public static BindableProperty HTMLProperty =
		//    BindableProperty.Create<BaseWebView, string>(p => p.HTMLString, "");

		//public static BindableProperty BaseProperty =
		//    BindableProperty.Create<BaseWebView, string>(p => p.Base, "");

		//public string HTMLString
		//{
		//    get { return (string)GetValue(HTMLProperty); }
		//    set { SetValue(HTMLProperty, value); }
		//}

		//public string Base
		//{
		//    get { return (string)GetValue(BaseProperty); }
		//    set { SetValue(BaseProperty, value); }
		//}

		//------------------->
	}
}
