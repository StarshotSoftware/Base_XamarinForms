using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot.Services;
using Base1902;

namespace BaseStarShot.Collections
{
    public class ReverseIncrementalCollection<T> : IncrementalCollection<T>, ISupportReverseIncrementalLoading
    {
        readonly Func<int, Task<IPagedResult<T>>> getTopPageAsyncCallback;
        private Dictionary<int, Task<LoadTopItemsResult>> topPageTasks;
        private HashSet<int> topLoadingPages;
        private List<TopPageLoadedEventArgs> topLoadedPages;

        readonly object topPageLocker = new object();
        private int TopStartingPage { get; set; }
        private int TopPageSize { get; set; }

        public event TopPageLoadingEventHandler TopPageLoading;
        public event TopPageLoadedEventHandler TopPageLoaded;

        public int CurrentTopPage { get; set; }

        private Guid TaskId { get; set; }

        private bool isTopLoadingFirst;
        public bool IsTopLoadingFirst
        {
            get { return isTopLoadingFirst; }
            set { SetProperty(ref isTopLoadingFirst, value); }
        }

        private bool isTopLoadingNextPage;
        public bool IsTopLoadingNextPage
        {
            get { return isTopLoadingNextPage; }
            set { SetProperty(ref isTopLoadingNextPage, value); }
        }

        private bool isTopLoading;
        public bool IsTopLoading
        {
            get { return isTopLoading; }
            set { SetProperty(ref isTopLoading, value); }
        }

        private bool isTopEmpty;
        public bool IsTopEmpty
        {
            get { return isTopEmpty; }
            set { SetProperty(ref isTopEmpty, value); }
        }

        private bool OnFirstLoad = false;

        public ReverseIncrementalCollection()
        {

        }

        public ReverseIncrementalCollection(Func<int, Task<IPagedResult<T>>> getPageAsyncCallback, Func<int, Task<IPagedResult<T>>> getTopPageAsyncCallback, int pageSize = 40, int startingPage = 0)
            : base(getPageAsyncCallback, pageSize, startingPage)
        {
            this.getTopPageAsyncCallback = getTopPageAsyncCallback;
            topPageTasks = new Dictionary<int, Task<LoadTopItemsResult>>();

            this.CurrentTopPage = startingPage;
            this.TopStartingPage = startingPage;
            this.TopPageSize = pageSize;

            topPageTasks = new Dictionary<int, Task<LoadTopItemsResult>>();
            topLoadingPages = new HashSet<int>();
            topLoadedPages = new List<TopPageLoadedEventArgs>();
        }

        public async Task RefreshTop()
        {
            await Task.WhenAll(topPageTasks.Select(t => t.Value));

            TaskId = Guid.NewGuid();
            topPageTasks.Clear();
            topLoadedPages.Clear();
            topLoadingPages.Clear();
            IsTopLoading = true;
            IsTopLoadingFirst = true;
            IsTopLoadingNextPage = false;
            IsTopEmpty = false;

            //this.VirtualCount = int.MaxValue;
            CurrentPage = TopStartingPage;
            this.Clear(); //<-- needs to test in android when this line is removed. In iOS, this line should be removed.
            await this.LoadMoreTopItemsAsync(0);
        }

        public Task<LoadTopItemsResult> LoadMoreTopItemsAsync(int count)
        {
            if (this.Count >= this.VirtualCount)
                return Task.FromResult(new LoadTopItemsResult { Count = 0 });

            if (CurrentTopPage == 0)
            {
                lock(topPageLocker)
                    CurrentTopPage--;
            }

            //if (OnFirstLoad)
            {
                Task<LoadTopItemsResult> pageTask = null;
                pageTask = new Task<LoadTopItemsResult>(
                    () =>
                    {
                        Guid taskId = TaskId;
                        int page;
                        Task<IPagedResult<T>> task = null;
                        lock (topPageLocker)
                        {
                            page = CurrentTopPage--;

                            while (topPageTasks.ContainsKey(page))
                            {
                                Task.Delay(10).Wait();
                            }

                            topPageTasks.Add(page, pageTask);
                            task = getTopPageAsyncCallback(page);
                        }
                        // if list was refreshed, discard result
                        if (taskId != TaskId) return new LoadTopItemsResult();

                        var dispatcher = Resolver.Get<IDispatcherService>();
                        dispatcher.BeginInvokeOnMainThread(
                            () =>
                            {
                                NotifyPropertyChanged("CurrentTopPage");
                                OnTopPageLoading(taskId, new TopPageLoadingEventArgs(page));
                            });

                        uint resultCount = 0;

                        var pagedResult = task.Result;
                        object result = null;

                        if (pagedResult != null)
                        {
                            if (page == 0)
                            {
                                // dispatcher.BeginInvokeOnMainThread(() => this.Clear());
                            }

                            result = pagedResult.Result;

                            resultCount = (uint)(pagedResult.Result != null ? pagedResult.Result.Count : 0);
                        }

                        // delay loading of messages into list if previous page has not loaded yet
                        // while (loadingPages.ToList().Any(p => p < page))
                        //    await Task.Delay(50);
                        Task.WhenAll(topPageTasks.Where(t => t.Key < page).Select(t => t.Value)).Wait();

                        dispatcher.BeginInvokeOnMainThread(
                            () =>
                            {
                                topPageTasks.Remove(page);
                                OnTopPageLoaded(taskId, new TopPageLoadedEventArgs(page, (int)resultCount, result));
                            });
                        return new LoadTopItemsResult() { Count = resultCount };
                    });
                pageTask.Start();
                return pageTask;
            }
            OnFirstLoad = true;
            return null;
        }

        void OnTopPageLoading(Guid taskId, TopPageLoadingEventArgs e)
        {
            if (this.TaskId != taskId) return;

            IsTopEmpty = false;
            IsTopLoading = true;

            if (topLoadedPages.Count == 0 || topLoadedPages.Sum(p => p.Count) == 0) IsTopLoadingFirst = true;
            else IsTopLoadingNextPage = true;

            topLoadingPages.Add(e.Page);

            if (TopPageLoading != null)
                TopPageLoading(this, e);
        }

        void OnTopPageLoaded(Guid taskId, TopPageLoadedEventArgs e)
        {
            if (this.TaskId != taskId) return;

            topLoadingPages.Remove(e.Page);

            if (IsTopLoadingFirst) IsTopLoadingFirst = false;
            else if (topLoadingPages.Count == 0) IsTopLoadingNextPage = false;

            if (topLoadingPages.Count == 0) IsTopLoading = false;

            topLoadedPages.Add(e);

            IsEmpty = !IsTopLoading && topLoadedPages.Count(p => p.Count > 0) == 0;

            if (e.Result != null)
            {
                foreach (var item in ((IEnumerable<T>)e.Result).Reverse())
                    this.Insert(0, item);
            }

            if (TopPageLoaded != null)
                TopPageLoaded(this, e);
        }
        #region Notify helper methods
        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.NotifyPropertyChanged(propertyName);
            return true;
        }

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }

    public struct LoadTopItemsResult
    {
        public uint Count;
    }

    public delegate void TopPageLoadingEventHandler(object sender, TopPageLoadingEventArgs e);
    public delegate void TopPageLoadedEventHandler(object sender, TopPageLoadedEventArgs e);

    public class TopPageLoadingEventArgs : EventArgs
    {
        public int Page { get; private set; }

        public TopPageLoadingEventArgs(int page) { this.Page = page; }
    }

    public class TopPageLoadedEventArgs : PageLoadingEventArgs
    {
        public int Count { get; private set; }

        public object Result { get; private set; }

        public TopPageLoadedEventArgs(int page, int count, object result) : base(page) { this.Count = count; this.Result = result; }
    }

    /// <summary>
    /// Specifies a calling contract for collection views that support incremental loading.
    /// </summary>
    public interface ISupportReverseIncrementalLoading : System.Collections.ICollection
    {
        bool IsTopLoading { get; }

        /// <summary>
        /// Top loaded event on reverse collection
        /// </summary>
        event TopPageLoadedEventHandler TopPageLoaded;

        /// <summary>
        /// Initializes top list loading from the view.
        /// </summary>
        /// <param name="count">The number of items to load.</param>
        /// <returns>The wrapped results of the load operation.</returns>
        Task<LoadTopItemsResult> LoadMoreTopItemsAsync(int count);
    }
}
