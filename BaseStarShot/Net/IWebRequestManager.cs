using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
namespace BaseStarShot.Net
{
    /// <summary>
    /// Performs low level HTTP functions.
    /// </summary>
    public interface IWebRequestManager
    {
        /// <summary>
        /// Max allowable concurrent requests.
        /// </summary>
        int MaxConcurrentConnections { get; set; }
        /// <summary>
        /// Buffer sice to be used when uploading files.
        /// </summary>
        int BufferSize { get; set; }
        /// <summary>
        /// Specifes which method to use when serializing GET request parameters.
        /// </summary>
        GETRequestSerialization GETRequestSerialization { get; set; }
        /// <summary>
        /// Disables GET request caching by appending a randomized value in the query string.
        /// </summary>
        bool DisableCaching { get; set; }
        /// <summary>
        /// Custom headers used for all requests.
        /// </summary>
        Dictionary<string, string> CustomHeaders { get; }

        /// <summary>
        /// Creates a HTTP request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="serialization"></param>
        /// <param name="appendUrl"></param>
        /// <param name="useJsonPropertyEncoding"></param>
        /// <returns></returns>
        HttpWebRequest Create(string url, WebRequestMethod method = WebRequestMethod.GET,
            object parameters = null, GETRequestSerialization? serialization = null, string appendUrl = null,
            bool useJsonPropertyEncoding = true);

        /// <summary>
        /// Sends the HTTP request to the server ang gets the response.
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<Response> GetResponseAsync(HttpWebRequest webRequest, object data = null);

        /// <summary>
        /// Sends a request with the given Url and gets the response.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<Response> GetResponseAsync(string url, WebRequestMethod method = WebRequestMethod.GET, object data = null);

        /// <summary>
        /// Sends if transfer completed or not
        /// </summary>
        event EventHandler<bool> OnTransferCompletedEvent;
        void OnTransferCompleted(bool success);

        /// <summary>
        /// Sends progress of transferred byte and total bytes of the request
        /// </summary>
        event EventHandler<TransferProgressBytesArgs> OnTransferProgressEvent;
        void OnTransferProgress(TransferProgressBytesArgs args);

    }

    /// <summary>
    /// Transfer args
    /// <param name="TransferredBytes">bytes currently transferred</param>
    /// <param name="TotalBytes">file total bytes</param>
    /// <param name="Data">class request</param>
    /// <param name="FileName">file name currently transferring</param>
    /// </summary>
    public class TransferProgressBytesArgs : EventArgs
    {
        public long TransferredBytes { get; protected set; }
        public long TotalBytes { get; protected set; }
        public object Data { get; protected set; }
        public string FileName { get; protected set; }
        public object Tag { get; protected set; }

        public TransferProgressBytesArgs(long transferredBytes, long totalBytes, object data, string fileName) : this(transferredBytes, totalBytes, data, fileName, null)
        {

        }

        public TransferProgressBytesArgs(long transferredBytes, long totalBytes, object data, string fileName, object tag)
        {
            this.TransferredBytes = transferredBytes;
            this.TotalBytes = totalBytes;
            this.Data = data;
            this.FileName = fileName;
            this.Tag = tag;
        }
    }

    public class TransferCompletedArgs : EventArgs
    {
        public object Data { get; protected set; }
        public string FileName { get; protected set; }
        public object Tag { get; protected set; }

        public TransferCompletedArgs(object data, string fileName) : this(data, fileName, null)
        {

        }

        public TransferCompletedArgs(object data, string fileName, object tag)
        {
            this.Data = data;
            this.FileName = fileName;
            this.Tag = tag;
        }
    }

    public class TransferFailedArgs : EventArgs
    {
        public object Data { get; protected set; }
        public string FileName { get; protected set; }
        public object Tag { get; protected set; }

        public TransferFailedArgs(object data, string fileName) : this(data, fileName, null)
        {

        }

        public TransferFailedArgs(object data, string fileName, object tag)
        {
            this.Data = data;
            this.FileName = fileName;
            this.Tag = tag;
        }
    }
}

