using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseStarShot.IO;
using System.IO;
using System.Net;
using Base1902;
using Base1902.IO;

namespace BaseStarShot.Net
{
    public class HttpClientManager : IHttpClientManager
    {
        const int DefaultBufferSize = 8192;

        private TaskCompletionSource<bool> firstRequest;
        private readonly object requestLocker = new object();
        private readonly HttpClient httpClient;

        public event EventHandler<TransferCompletedArgs> OnTransferCompletedEvent;
        public event EventHandler<TransferProgressBytesArgs> OnTransferProgressEvent;
        public event EventHandler<TransferFailedArgs> OnTransferFailedEvent;

        public virtual int BufferSize { get; set; }
        public virtual GETRequestSerialization GETRequestSerialization { get; set; }
        public virtual bool DisableCaching { get; set; }
        public virtual Dictionary<string, string> CustomHeaders { get; private set; }
        public virtual bool EncodeJson { get; set; }
        public virtual TimeSpan Timeout { get { return httpClient.Timeout; } set { httpClient.Timeout = value; } }

        public HttpClientManager()
        {
            Init();
            httpClient = new HttpClient();
        }

        internal HttpClientManager(HttpMessageHandler handler)
        {
            Init();
            httpClient = new HttpClient(handler);
        }

        private void Init()
        {
            BufferSize = DefaultBufferSize;
            GETRequestSerialization = GETRequestSerialization.QueryString;
            this.CustomHeaders = new Dictionary<string, string>();
        }

        private async Task WaitForFirstRequest()
        {
            if (firstRequest == null)
            {
                lock(requestLocker)
                {
                    if (firstRequest == null)
                    {
                        firstRequest = new TaskCompletionSource<bool>();
                        return;
                    }
                }
            }
            await firstRequest.Task;
        }

        void OnTransferCompleted(TransferCompletedArgs args)
        {
            OnTransferCompletedEvent?.Invoke(this, args);
        }

        void OnTransferProgress(TransferProgressBytesArgs args)
        {
            OnTransferProgressEvent?.Invoke(this, args);
        }

        void OnTransferFailed(TransferFailedArgs args)
        {
            OnTransferFailedEvent?.Invoke(this, args);
        }

        protected virtual string BuildUrl(HttpRequest request)
        {
            var url = request.Url;
            if (request.Parameters != null)
            {
                bool append = request.Serialization.GetValueOrDefault(GETRequestSerialization) == GETRequestSerialization.QueryString;

                if (append)
                {
                    if (!url.Contains("?"))
                        url = url + "?";
                }
                else
                {
                    if (url.EndsWith("/"))
                        url = url.Substring(0, url.Length - 1);
                }

                Dictionary<string, object> properties = request.Parameters as Dictionary<string, object>;

                if (properties == null)
                {
                    if (request.UseJsonPropertyEncoding)
                        properties = ReflectionHelper.ToFlatDictionary(request.Parameters);
                    else
                    {
                        properties = new Dictionary<string, object>();
                        foreach (var prop in ReflectionHelper.GetPropertyValues(request.Parameters))
                        {
                            if (prop.CurrentValue != null)
                                properties.Add(prop.Name, prop.CurrentValue);
                        }
                    }
                }

                foreach (var prop in properties)
                {
                    url += string.Format(append ? "&{0}={1}" : "/{0}/{1}", System.Uri.EscapeDataString(prop.Key), System.Uri.EscapeDataString(EncodeUrlData(prop.Value)));
                }
            }

            if (!string.IsNullOrEmpty(request.AppendUrl))
                url += request.AppendUrl;

            // force download instead of getting cached result
            if (DisableCaching)
                url += (url.Contains("?") ? "" : "?") + "&_=" + Guid.NewGuid();

            return url;
        }

        protected virtual string EncodeUrlData(object value)
        {
            if (value == null) return string.Empty;
            DateTime? dtValue = null;
            if (value is DateTime)
                dtValue = (DateTime)value;
            if (!dtValue.HasValue)
            {
                var ntype = Nullable.GetUnderlyingType(value.GetType());
                if (ntype != null && ntype.FullName == "System.DateTime")
                    dtValue = (DateTime?)value;
            }
            if (dtValue.HasValue)
                return dtValue.Value.ToString(Globals.DateTimeFormat);
            return value.ToString();
        }

        protected virtual string EncodePostData(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (!EncodeJson &&
                    ((data.StartsWith("[") && data.EndsWith("]"))
                    || (data.StartsWith("{") && data.EndsWith("}"))))
                    return data;
                return Uri.EscapeDataString(data);
            }
            return data;
        }

        protected virtual HttpMethod Map(WebRequestMethod method)
        {
            switch (method)
            {
                case WebRequestMethod.POST: return HttpMethod.Post;
                case WebRequestMethod.PUT: return HttpMethod.Put;
                case WebRequestMethod.DELETE: return HttpMethod.Delete;
                case WebRequestMethod.OPTIONS: return HttpMethod.Options;
                case WebRequestMethod.HEAD: return HttpMethod.Head;
                case WebRequestMethod.GET:
                default: return HttpMethod.Get;
            }
        }

        public virtual async Task<HttpResponse> GetResponseAsync(HttpRequest request, CancellationToken? token = null)
        {
            var url = BuildUrl(request);
            Logger.DebugWrite("HttpClientManager", "Requesting: " + request.Method + " " + url);

            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = Map(request.Method)
            };

            foreach (var header in CustomHeaders)
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                    requestMessage.Headers.Add(header.Key, header.Value);
            }

            await WaitForFirstRequest();

            Dictionary<string, Base1902.IO.File> files = new Dictionary<string, Base1902.IO.File>();
            try
            {
                if (request.Data != null && (request.Method == WebRequestMethod.POST || request.Method == WebRequestMethod.PUT))
                {
                    var data = request.Data;
                    bool dataIsString = data is string;

                    Dictionary<string, object> postData = data as Dictionary<string, object>;
                    if (postData == null && !dataIsString)
                        postData = ReflectionHelper.ToFlatDictionary(data);

                    bool hasFiles = dataIsString ? false : postData.Values.Any(i => i is Base1902.IO.File);

                    if (!hasFiles)
                    {
                        string lcPostData = dataIsString ? data as string : string.Join("&", postData.Select(p => p.Key + "=" + EncodePostData(EncodeUrlData(p.Value))));

                        Logger.DebugWrite("HttpClientManager", "PostData: " + lcPostData);

                        requestMessage.Content = new StringContent(lcPostData, Encoding.UTF8, request.IsJsonContent ? "application/json" : "application/x-www-form-urlencoded");
                    }
                    else
                    {
                        string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                        var content = new MultipartFormDataContent(boundary);
                        requestMessage.Content = content;

                        // write all non file items first
                        foreach (var pd in postData)
                        {
                            if (pd.Value is Base1902.IO.File)
                            {
                                files.Add(pd.Key, (Base1902.IO.File)pd.Value);
                                continue;
                            }

                            var stringContent = new StringContent(EncodeUrlData(pd.Value), Encoding.UTF8, "text/plain");
                            stringContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                            {
                                Name = pd.Key
                            };
                            content.Add(stringContent);
                        }
                        foreach (var fileItem in files)
                        {
                            var file = fileItem.Value;

                            var stream = await file.GetStreamAsync();
                            //var streamContent = new ProgressStreamContent(stream, BufferSize, this, request.Data, file.Name, request.Tag);
                            var streamContent = new StreamContent(stream, BufferSize);
                            streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                Name = fileItem.Key,
                                FileName = file.Name,
                                Size = stream.Length
                            };
                            streamContent.Headers.Add("Content-Type", file.GetMimeType());
                            content.Add(streamContent);
                        }
                    }
                }

                var response = token.HasValue ? await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token.Value)
                    : await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                NotifyResult(files, request.Data, request.Tag, response.IsSuccessStatusCode);

                return new HttpResponse(response) { RequestMessage = request };
            }
            catch (Exception)
            {
                NotifyResult(files, request.Data, request.Tag, false);
                throw;
            }
            finally
            {
                if (!firstRequest.Task.IsCompleted)
                    firstRequest.SetResult(true);

                if (requestMessage.Content != null)
                    requestMessage.Content.Dispose();
            }
        }

        void NotifyResult(Dictionary<string, Base1902.IO.File> files, object data, object tag, bool success)
        {
            foreach (var fileItem in files)
            {
                var file = fileItem.Value;
                if (success)
                    OnTransferCompleted(new TransferCompletedArgs(data, file.Name, tag));
                else
                    OnTransferFailed(new TransferFailedArgs(data, file.Name, tag));
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        class ProgressStreamContent : HttpContent
        {
            private const int defaultBufferSize = 4096;

            HttpClientManager manager;
            Stream content;
            int bufferSize;
            private bool contentConsumed;
            private object data;
            private string fileName;
            private object tag;

            public ProgressStreamContent(Stream content, int bufferSize, HttpClientManager manager, object data, string fileName, object tag)
            {
                if (content == null)
                {
                    throw new ArgumentNullException(nameof(content));
                }
                if (bufferSize <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(bufferSize));
                }
                if (manager == null)
                {
                    throw new ArgumentNullException(nameof(manager));
                }

                this.content = content;
                this.bufferSize = bufferSize;
                this.manager = manager;
                this.data = data;
                this.fileName = fileName;
                this.tag = tag;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                PrepareContent();

                return Task.Run(() =>
                {
                    var buffer = new Byte[this.bufferSize];
                    var size = content.Length;
                    var uploaded = 0;

                    using (content) while (true)
                        {
                            var length = content.Read(buffer, 0, buffer.Length);
                            if (length <= 0) break;

                            stream.Write(buffer, 0, length);
                            uploaded += length;
                            manager.OnTransferProgress(new TransferProgressBytesArgs(uploaded, size, data, fileName, tag));
                        }
                    manager.OnTransferCompleted(new TransferCompletedArgs(data, fileName, tag));
                });
            }

            protected override bool TryComputeLength(out long length)
            {
                length = content.Length;
                return true;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    content.Dispose();
                }
                base.Dispose(disposing);
            }

            private void PrepareContent()
            {
                if (contentConsumed)
                {
                    // If the content needs to be written to a target stream a 2nd time, then the stream must support
                    // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                    // stream (e.g. a NetworkStream).
                    if (content.CanSeek)
                    {
                        content.Position = 0;
                    }
                    else
                    {
                        throw new InvalidOperationException("HttpContent stream already read.");
                    }
                }

                contentConsumed = true;
            }
        }
    }
}
