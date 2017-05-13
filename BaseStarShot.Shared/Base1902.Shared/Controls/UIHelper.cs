using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public static partial class UIHelper
    {
        public static byte ConvertDoubleToByteColor(double value)
        {
            return (byte)(value * 255);
        }

        public static int ConvertDPToPixels(double value)
        {
            double scale = (double)Globals.ResolutionScale / 100d;
            return (int)(value * scale + 0.5d);
        }

#if NETFX_CORE
        private static TypeInfo GetType<T>()
        {
            return typeof(T).GetTypeInfo();
        }

        private static TypeInfo GetType(object obj)
        {
            return obj.GetType().GetTypeInfo();
        }
#else
        private static Type GetType<T>()
        {
            return typeof(T);
        }

        private static Type GetType(object obj)
        {
            return obj.GetType();
        }
#endif

        public static Xamarin.Forms.ListView GetContainingListView(Xamarin.Forms.Element element)
        {
            Element parentElement = element.ParentView;

            if (parentElement == null)
                return null;

            if (GetType<Xamarin.Forms.ListView>().IsAssignableFrom(GetType(parentElement)))
                return (Xamarin.Forms.ListView)parentElement;
            else
                return GetContainingListView(parentElement);
        }

        public static Page GetContainingPage(Xamarin.Forms.Element element)
        {
            Element parentElement = element.ParentView;

            if (parentElement == null)
                return null;

            if (GetType<Page>().IsAssignableFrom(GetType(parentElement)))
                return (Page)parentElement;
            else
                return GetContainingPage(parentElement);
        }

        public static VisualElement GetRootElement(Xamarin.Forms.Element element)
        {
            VisualElement parentElement = element.ParentView;

            while (parentElement.ParentView != null)
                parentElement = parentElement.ParentView;

            return parentElement;
        }

        public static ViewCell GetContainingViewCell(Xamarin.Forms.Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (GetType<ViewCell>().IsAssignableFrom(GetType(parentElement)))
                return (ViewCell)parentElement;
            else
                return GetContainingViewCell(parentElement);
        }

        public static TabbedPage GetContainingTabbedPage(Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (GetType<TabbedPage>().IsAssignableFrom(GetType(parentElement)))
                return (TabbedPage)parentElement;
            else
                return GetContainingTabbedPage(parentElement);
        }

        public static NavigationPage GetContainingNavigationPage(Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (GetType<NavigationPage>().IsAssignableFrom(GetType(parentElement)))
                return (NavigationPage)parentElement;
            else
                return GetContainingNavigationPage(parentElement);
        }
    }
}
