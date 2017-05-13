using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using BaseStarShot.IO;
using System.Threading;
using Base1902;
using Base1902.IO;

namespace BaseStarShot.Net
{
    public class WebRequestManager : IWebRequestManager
    {
        const int DefaultMaxConcurrentConnections = 2;
        const int DefaultBufferSize = 8192;

        public virtual int MaxConcurrentConnections { get; set; }
        public virtual int BufferSize { get; set; }
        public virtual GETRequestSerialization GETRequestSerialization { get; set; }
        public virtual bool DisableCaching { get; set; }
        public virtual Dictionary<string, string> CustomHeaders { get; private set; }
        public virtual bool EncodeJson { get; set; }

        public WebRequestManager()
        {
            MaxConcurrentConnections = DefaultMaxConcurrentConnections;
            BufferSize = DefaultBufferSize;
            GETRequestSerialization = GETRequestSerialization.QueryString;
            this.CustomHeaders = new Dictionary<string, string>();
        }


        public virtual HttpWebRequest Create(string url,
            WebRequestMethod method = WebRequestMethod.GET,
            object parameters = null,
            GETRequestSerialization? serialization = null,
            string appendUrl = null,
            bool useJsonPropertyEncoding = true)
        {
            if (parameters != null)
            {
                bool append = serialization.GetValueOrDefault(GETRequestSerialization) == GETRequestSerialization.QueryString;

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

                Dictionary<string, object> properties = parameters as Dictionary<string, object>;

                if (properties == null)
                {
                    if (useJsonPropertyEncoding)
                        properties = ReflectionHelper.ToFlatDictionary(parameters);
                    else
                    {
                        properties = new Dictionary<string, object>();
                        foreach (var prop in ReflectionHelper.GetPropertyValues(parameters))
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

            if (!string.IsNullOrEmpty(appendUrl))
                url += appendUrl;

            // force download instead of getting cached result
            if (DisableCaching && method == WebRequestMethod.GET)
                url += (url.Contains("?") ? "" : "?") + "&_=" + Guid.NewGuid();

            HttpWebRequest wRequest = HttpWebRequest.CreateHttp(url);
            wRequest.Method = method.ToString();

            foreach (var header in CustomHeaders)
            {
                wRequest.Headers[header.Key] = header.Value;
            }

            if (method == WebRequestMethod.POST || method == WebRequestMethod.PUT)
                wRequest.ContentType = "application/x-www-form-urlencoded";

            Logger.DebugWrite("WebRequestManager", "Requesting: " + wRequest.Method + " " + url);

            OnCreate(wRequest);

            return wRequest;
        }

        /// <summary>
        /// Override to do modifications to the request upon creation.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void OnCreate(HttpWebRequest request)
        {

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

        public virtual async Task<Response> GetResponseAsync(HttpWebRequest webRequest, object data = null)
        {
            if (data != null && (webRequest.Method == "POST" || webRequest.Method == "PUT"))
            {
                bool dataIsString = data is string;

                Dictionary<string, object> postData = data as Dictionary<string, object>;
                if (postData == null && !dataIsString)
                    postData = ReflectionHelper.ToFlatDictionary(data);

                bool hasFiles = dataIsString ? false : postData.Values.Any(i => i is Base1902.IO.File);

                if (!hasFiles)
                {
                    string lcPostData = dataIsString ? data as string : string.Join("&", postData.Select(p => p.Key + "=" + EncodePostData(EncodeUrlData(p.Value))));

                    Logger.DebugWrite("WebRequestManager", "PostData: " + lcPostData);

                    byte[] lbPostBuffer = System.Text.Encoding.UTF8.GetBytes(lcPostData);

                    try
                    {
                        using (Stream loPostData = await GetRequestStreamAsync(webRequest))
                        {
                            if (loPostData == null)
                                return new Response(null, WebExceptionStatus.Timeout);
                            loPostData.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                        }
                    }
                    catch (WebException ex)
                    {
                        return new Response(null, (WebExceptionStatus)ex.Status);
                    }
                }
                else
                {
                    string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

                    webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                    byte[] boundarybytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                    string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\nContent-Type: text/plain; charset=UTF-8\r\n\r\n{1}";

                    try
                    {
                        using (Stream loPostData = await GetRequestStreamAsync(webRequest))
                        {
                            if (loPostData == null)
                                return new Response(null, WebExceptionStatus.Timeout);
                            Dictionary<string, object> files = new Dictionary<string, object>();

                            // write all non file items first
                            foreach (var pd in postData)
                            {
                                if (pd.Value is Base1902.IO.File)
                                {
                                    files.Add(pd.Key, pd.Value);
                                    continue;
                                }
                                string formitem = string.Format(formdataTemplate, pd.Key, EncodeUrlData(pd.Value));
                                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                                loPostData.Write(formitembytes, 0, formitembytes.Length);
                            }

                            string headerTemplate = "Content-Disposition: attachment; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                            foreach (var fileItem in files)
                            {
                                var file = (Base1902.IO.File)fileItem.Value;

                                loPostData.Write(boundarybytes, 0, boundarybytes.Length);

                                string header = string.Format(headerTemplate, fileItem.Key, file.Name, file.GetMimeType());

                                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                                loPostData.Write(headerbytes, 0, headerbytes.Length);

                                using (var stream = await file.GetStreamAsync())
                                {
                                    byte[] buffer = new byte[BufferSize];
                                    long size = (long)stream.Length;
                                    long totalSize = size;
                                    while (size > 0)
                                    {
                                        int length = await stream.ReadAsync(buffer, 0, BufferSize);
                                        size -= length;
                                        loPostData.Write(buffer, 0, length);
                                        OnTransferProgress(new TransferProgressBytesArgs(size, totalSize, data, file.Name));
                                    }
                                }
                            }
                            byte[] oef = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                            loPostData.Write(oef, 0, oef.Length);
                        }
                    }
                    catch (WebException ex)
                    {
                        return new Response(null, (WebExceptionStatus)ex.Status);
                    }
                }
            }

            var result = await GetResponseAsync(webRequest);
            if (result == null)
                return new Response(null, WebExceptionStatus.ConnectFailure);

            return result;
        }

        public virtual async Task<Response> GetResponseAsync(string url, WebRequestMethod method = WebRequestMethod.GET, object data = null)
        {
            HttpWebRequest request;
            if (method == WebRequestMethod.GET)
                request = Create(url, method: method, parameters: data);
            else
                request = Create(url, method: method);

            OnGetResponseAsync(request);

            var result = await GetResponseAsync(request);
            if (result == null)
                return new Response(null, WebExceptionStatus.ConnectFailure);

            return result;
        }

        protected virtual async Task<Stream> GetRequestStreamAsync(HttpWebRequest webRequest)
        {
            return await webRequest.GetRequestStreamAsync();
        }

        protected virtual async Task<Response> GetResponseAsync(HttpWebRequest webRequest)
        {
            HttpWebResponse loWebResponse = null;
            WebExceptionStatus status = WebExceptionStatus.Success;

            try
            {
                loWebResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
            }
            catch (WebException ex)
            {
                status = (WebExceptionStatus)ex.Status;
                loWebResponse = (HttpWebResponse)ex.Response;
            }

            return new Response(loWebResponse, status);
        }

        /// <summary>
        /// Override to do additional modifications to the request before sending it to the server.
        /// </summary>
        /// <param name="request"></param>
        protected virtual void OnGetResponseAsync(HttpWebRequest request)
        {

        }

        public event EventHandler<bool> OnTransferCompletedEvent;
        
        public void OnTransferCompleted(bool success) {
            if (OnTransferCompletedEvent != null)
                OnTransferCompletedEvent(this, success);
        }

        public event EventHandler<TransferProgressBytesArgs> OnTransferProgressEvent;
        
        public void OnTransferProgress(TransferProgressBytesArgs args) {
            if (OnTransferProgressEvent != null)
                OnTransferProgressEvent(this, args);
        }
    }
}
