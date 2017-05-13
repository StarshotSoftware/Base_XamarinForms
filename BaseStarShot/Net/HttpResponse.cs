using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    public class HttpResponse
    {
        private readonly HttpResponseMessage response;

        public HttpResponse(HttpResponseMessage response)
        {
            this.response = response;
        }

        /// <summary>
        /// Serialize the HTTP content to a byte array as an asynchronous operation.
        /// </summary>
        /// <returns></returns>
        public Task<byte[]> ReadAsByteArrayAsync()
        {
            return response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Serialize the HTTP content and return a stream that represents the content as
        //     an asynchronous operation.
        /// </summary>
        /// <returns></returns>
        public Task<Stream> ReadAsStreamAsync()
        {
            return response.Content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Serialize the HTTP content to a string as an asynchronous operation.
        /// </summary>
        /// <returns></returns>
        public Task<string> ReadAsStringAsync()
        {
            return response.Content.ReadAsStringAsync();
        }

        private Dictionary<string, List<string>> _headers;
        /// <summary>
        /// Gets the collection of HTTP response headers.
        /// </summary>
        public IDictionary<string, List<string>> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new Dictionary<string, List<string>>();
                    foreach (var header in response.Headers)
                    {
                        _headers.Add(header.Key, header.Value.ToList());
                    }
                }
                return _headers;
            }
        }

        /// <summary>
        /// Gets a value that indicates if the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode { get { return response.IsSuccessStatusCode; } }

        /// <summary>
        /// Gets or sets the reason phrase which typically is sent by servers together with
        //     the status code.
        /// </summary>
        public string ReasonPhrase { get { return response.ReasonPhrase; } set { response.ReasonPhrase = value; } }

        /// <summary>
        /// Gets or sets the request message which led to this response message.
        /// </summary>
        public HttpRequest RequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get { return response.StatusCode; } set { response.StatusCode = value; } }

        /// <summary>
        /// Releases the unmanaged resources and disposes of unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            response.Dispose();
        }
    }
}
