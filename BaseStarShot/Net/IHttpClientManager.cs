using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    public interface IHttpClientManager : IDisposable
    {
        event EventHandler<TransferCompletedArgs> OnTransferCompletedEvent;
        event EventHandler<TransferProgressBytesArgs> OnTransferProgressEvent;
        event EventHandler<TransferFailedArgs> OnTransferFailedEvent;

        int BufferSize { get; set; }
        Dictionary<string, string> CustomHeaders { get; }
        bool DisableCaching { get; set; }
        bool EncodeJson { get; set; }
        GETRequestSerialization GETRequestSerialization { get; set; }
        TimeSpan Timeout { get; set; }

        Task<HttpResponse> GetResponseAsync(HttpRequest request, CancellationToken? token = null);
    }
}