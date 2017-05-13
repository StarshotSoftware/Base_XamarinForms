using Base1902;
using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Collections
{
    public class IncrementalCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        readonly Func<int, Task<IPagedResult<T>>> getPageAsyncCallback;

        readonly object pageLocker = new object();

        public int VirtualCount { get; private set; }
        public int CurrentPage { get; set; }
        private int StartingPage { get; set; }
        private int PageSize { get; set; }

        public event PageLoadingEventHandler PageLoading;
        public event PageLoadedEventHandler PageLoaded;

        private Dictionary<int, Task<LoadMoreItemsResult>> pageTasks;
        private HashSet<int> loadingPages;
        private List<PageLoadedEventArgs> loadedPages;

        private bool isLoadingFirst;
        public bool IsLoadingFirst
        {
            get { return isLoadingFirst; }
            set { SetProperty(ref isLoadingFirst, value); }
        }

        private bool isLoadingNextPage;
        public bool IsLoadingNextPage
        {
            get { return isLoadingNextPage; }
            set { SetProperty(ref isLoadingNextPage, value); }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { SetProperty(ref isLoading, value); }
        }

        private bool isEmpty;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { SetProperty(ref isEmpty, value); }
        }

        private Guid TaskId { get; set; }

        public object this[int index] { get { return base[index]; } }

        public IncrementalCollection()
        {
            this.VirtualCount = int.MaxValue;
            this.PageSize = 40;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStarShot.Collections.IncrementalCollection`1"/> class.
        /// </summary>
        /// <param name="getPageAsyncCallback">First parameter of callback is page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="
        /// 
        /// 
        /// 
        /// ">Starting page.</param>
        public IncrementalCollection(Func<int, Task<IPagedResult<T>>> getPageAsyncCallback, int pageSize = 40, int startingPage = 0)
        {
            this.getPageAsyncCallback = getPageAsyncCallback;
            this.VirtualCount = int.MaxValue;
            this.CurrentPage = startingPage;
            this.StartingPage = startingPage;
            this.PageSize = pageSize;

            TaskId = Guid.NewGuid();

            pageTasks = new Dictionary<int, Task<LoadMoreItemsResult>>();
            loadingPages = new HashSet<int>();
            loadedPages = new List<PageLoadedEventArgs>();
        }

        public async Task Refresh()
        {
            await Task.WhenAll(pageTasks.Select(t => t.Value));

            TaskId = Guid.NewGuid();
            pageTasks.Clear();
            loadedPages.Clear();
            loadingPages.Clear();
            IsLoading = true;
            IsLoadingFirst = true;
            IsLoadingNextPage = false;
            IsEmpty = false;

            this.VirtualCount = int.MaxValue;
            CurrentPage = StartingPage;
            //this.Clear(); <-- needs to test in android when this line is removed. In iOS, this line should be removed.
            await this.LoadMoreItemsAsync(0);
        }

		public void Cancel()
		{
			TaskId = Guid.NewGuid();
			IsLoading = false;
			IsLoadingFirst = false;
			IsLoadingNextPage = false;
			IsEmpty = false;

			this.VirtualCount = this.Count;
		}

        public bool HasMoreItems
        {
            get { return this.VirtualCount > CurrentPage * PageSize; }
        }


        public Task<LoadMoreItemsResult> LoadMoreItemsAsync(int count)
        {
            //if (this.Count >= this.VirtualCount)
            //    return Task.FromResult(new LoadMoreItemsResult { Count = 0 });

            Task<LoadMoreItemsResult> pageTask = null;
            pageTask = new Task<LoadMoreItemsResult>(
                () =>
                {
                    Guid taskId = TaskId;
                    int page;
                    Task<IPagedResult<T>> task = null;
                    lock (pageLocker)
                    {
                        page = CurrentPage++;

                        while(pageTasks.ContainsKey(page))
                        {
                            Task.Delay(10).Wait();
                        }

                        pageTasks.Add(page, pageTask);
                        task = getPageAsyncCallback(page);
                    }

                    var dispatcher = Resolver.Get<IDispatcherService>();
                    dispatcher.BeginInvokeOnMainThread(
                        () =>
                        {
                            NotifyPropertyChanged("CurrentPage");
                            OnPageLoading(taskId, new PageLoadingEventArgs(page));
                        });

                    // if list was refreshed, discard result
                    if (taskId != TaskId) return new LoadMoreItemsResult();

                    uint resultCount = 0;

                    var pagedResult = task.Result;
                    object result = null;

                    if (pagedResult != null)
                    {
                        if (page == 0)
                        {
                            dispatcher.BeginInvokeOnMainThread(() => this.Clear());
                        }

                        this.VirtualCount = pagedResult.VirtualCount;

                        result = pagedResult.Result;

                        resultCount = (uint)(pagedResult.Result != null ? pagedResult.Result.Count : 0);
                    }

                    // delay loading of messages into list if previous page has not loaded yet
                    //while (loadingPages.ToList().Any(p => p < page))
                    //    await Task.Delay(50);
                    Task.WhenAll(pageTasks.Where(t => t.Key < page).Select(t => t.Value)).Wait();

                    dispatcher.BeginInvokeOnMainThread(
                        () =>
                        {
                            pageTasks.Remove(page);
                            OnPageLoaded(taskId, new PageLoadedEventArgs(page, (int)resultCount, result, pagedResult.Failed));
                        });
                    if (resultCount == 0)
                    {
                        lock (pageLocker)
                        {
                            CurrentPage--;
                        }
                    }
                    return new LoadMoreItemsResult() { Count = resultCount };
                });
            pageTask.Start();
            return pageTask;
        }

        void OnPageLoading(Guid taskId, PageLoadingEventArgs e)
        {
            if (this.TaskId != taskId) return;

            IsEmpty = false;
            IsLoading = true;

            if (loadedPages.Count == 0 || loadedPages.Sum(p => p.Count) == 0) IsLoadingFirst = true;
            else IsLoadingNextPage = true;

            loadingPages.Add(e.Page);

            if (PageLoading != null)
                PageLoading(this, e);
        }

        void OnPageLoaded(Guid taskId, PageLoadedEventArgs e)
        {
            if (this.TaskId != taskId) return;

            loadingPages.Remove(e.Page);

            if (IsLoadingFirst) IsLoadingFirst = false;
            else if (loadingPages.Count == 0) IsLoadingNextPage = false;

            if (loadingPages.Count == 0) IsLoading = false;

            loadedPages.Add(e);

            IsEmpty = !isLoading && loadedPages.Count(p => p.Count > 0) == 0;

            if (e.Result != null)
            {
                foreach (var item in (IList<T>)e.Result)
                    this.Add(item);
            }

            if (PageLoaded != null)
                PageLoaded(this, e);
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

    public delegate void PageLoadingEventHandler(object sender, PageLoadingEventArgs e);
    public delegate void PageLoadedEventHandler(object sender, PageLoadedEventArgs e);

    public class PageLoadingEventArgs : EventArgs
    {
        public int Page { get; private set; }

        public PageLoadingEventArgs(int page) { this.Page = page; }
    }

    public class PageLoadedEventArgs : PageLoadingEventArgs
    {
        public int Count { get; private set; }

        public object Result { get; private set; }

        public bool Failed { get; private set; }

        public PageLoadedEventArgs(int page, int count, object result, bool failed) : base(page) { this.Count = count; this.Result = result; this.Failed = failed; }
    }

    public struct LoadMoreItemsResult
    {
        public uint Count;
    }

    /// <summary>
    /// Specifies a calling contract for collection views that support incremental loading.
    /// </summary>
    public interface ISupportIncrementalLoading : System.Collections.ICollection
    {
        object this[int index] { get; }
        bool IsLoading { get; }

        /// <summary>
        /// Gets a sentinel value that supports incremental loading implementations.
        /// </summary>
        bool HasMoreItems { get; }

        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">The number of items to load.</param>
        /// <returns>The wrapped results of the load operation.</returns>
        Task<LoadMoreItemsResult> LoadMoreItemsAsync(int count);
    }
}
