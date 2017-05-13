using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseStarShot;
using BaseStarShot.Net;

namespace BaseStarShot.Services
{
    public interface IBackgroundManager
    {
        /// <summary>
        /// Get All Active Uploads and Repost
        /// </summary>
        Task ReattachActiveUploadsAsync<T>();

        /// <summary>
        /// Setup Uploader
        /// <param name="headers">Headers needed for request</param>
        /// <param name="urlRequest">Full url for request</param>
        /// <param name="methodRequest">Method to be used in request</param>
        /// <param name="notificationParams"></param>
        /// </summary>
		void CreateRequest(List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, Guid? operationId = null);

		/// <summary>
		/// Setup Uploader
		/// <param name="headers">Headers needed for request</param>
		/// <param name="urlRequest">Full url for request</param>
		/// <param name="methodRequest">Method to be used in request</param>
		/// <param name="notificationParams"></param>
		/// </summary>
		void CreateRequestAndUpload(List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, object fileRequest = null, Guid? operationId = null);

        /// <summary>
        /// Create Success Notification Prompt
        /// </summary>
        void CreateSuccessNotification(string title, string message, string failedMessage, string imagePath, IDictionary<string, string> notificationParams, int additionalCount = 0);

        /// <summary>
        /// Upload in background
        /// <param name="data">request data to be posted</param>
        /// <param name="T">server response class</param>
        /// </summary>
        void UploadInBackground<T>(object data, CancellationTokenSource cts = null);

        /// <summary>
        /// Fire event every 2 sec for progress
        /// </summary>
        event EventHandler<TransferBackgroundProgressArgs> OnTransferProgressEvent;
        void OnTransferProgress(TransferBackgroundProgressArgs args);

        /// <summary>
        /// Fire event when upload is finished
        /// </summary>
        event EventHandler<TransferBackgroundCompleteArgs> OnTransferCompleteEvent;
        void OnTransferComplete(TransferBackgroundCompleteArgs args);

        /// <summary>
        /// Fire event when upload is failed
        /// </summary>
        event EventHandler<TransferBackgroundFailedArgs> OnTransferFailedEvent;
        void OnTransferFailed(TransferBackgroundFailedArgs args);
    }

    /// <summary>
    /// Args status for background process
    /// </summary>
    public class TransferBackgroundProgressArgs : EventArgs
    {
        public long TransferredBytes { get; protected set; }
        public long TotalBytes { get; protected set; }
        public Guid OperationGUID { get; protected set; }

        public TransferBackgroundProgressArgs(long transferredBytes, long totalBytes, Guid operationGUID)
        {
            this.TransferredBytes = transferredBytes;
            this.TotalBytes = totalBytes;
            this.OperationGUID = operationGUID;
        }
    }

    /// <summary>
    /// Args status for background process
    /// </summary>
    public class TransferBackgroundCompleteArgs : EventArgs
    {
        public Guid OperationGUID { get; protected set; }
        public object ResponseData { get; protected set; }
        public int TotalSuccessFileCount { get; protected set; }
        public int TotalFileCount { get; protected set; }
        public NotificationParamsDTO notificationParams { get; set; }

        public TransferBackgroundCompleteArgs(Guid operationGUID, object responseData, int totalSuccessFileCount = 0, int totalFileCount = 0, NotificationParamsDTO notificationParams = null)
        {
            ResponseData = responseData;
            OperationGUID = operationGUID;
            this.TotalSuccessFileCount = totalSuccessFileCount;
            this.TotalFileCount = totalFileCount;
        }
    }

    public class TransferBackgroundFailedArgs : EventArgs
    {
        public Guid OperationGUID { get; protected set; }
        public string ResponseData { get; protected set; }

        public TransferBackgroundFailedArgs(Guid operationGUID, string responseData)
        {
            ResponseData = responseData;
            OperationGUID = operationGUID;
        }
    }


    public class NotificationParamsDTO
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string FailedMessage { get; set; }
        public string ImagePath { get; set; }
		public int AdditionalCount { get; set; }
        public IDictionary<string, string> Params { get; set; }
    }
}
