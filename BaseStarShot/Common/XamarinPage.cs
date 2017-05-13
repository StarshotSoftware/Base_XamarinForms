using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot
{
    internal class XamarinPage<T> : IPage
        where T : BaseViewModel
    {
        private readonly WeakReference<Xamarin.Forms.Page> _page;
        protected Xamarin.Forms.Page Page
        {
            get
            {
                Xamarin.Forms.Page page;
                _page.TryGetTarget(out page);
                return page;
            }
        }

        private readonly WeakReference<T> _viewModel;
        protected T ViewModel
        {
            get
            {
                T viewModel;
                _viewModel.TryGetTarget(out viewModel);
                return viewModel;
            }
        }

        internal XamarinPage(Page page)
        {
            this._page = new WeakReference<Xamarin.Forms.Page>(page);
            this._viewModel = new WeakReference<T>(ContructViewModel());

            this.Page.BindingContext = this.ViewModel;
        }

        private T ContructViewModel()
        {
            Type type = typeof(T);
            return (T)Activator.CreateInstance(type, Page.Navigation, this);
        }

        public async Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            var page = Page;
            if (page != null)
                return await Page.DisplayActionSheet(title, cancel, destruction, buttons);
            return null;
        }

        public async Task DisplayAlert(string title, string message, string cancel)
        {
            var page = Page;
            if (page != null)
                await page.DisplayAlert(title, message, cancel);
        }

        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            var page = Page;
            if (page != null)
                return await page.DisplayAlert(title, message, accept, cancel);
            return false;
        }
    }
}
