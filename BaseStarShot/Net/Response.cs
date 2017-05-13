using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    /// <summary>
    /// A response from a HTTP web request.
    /// </summary>
    public class Response : IDisposable
    {
        private HttpWebResponse innerResponse;

        /// <summary>
        /// Gets the length of the content returned by the request.
        /// </summary>
        public long ContentLength { get { return innerResponse != null ? innerResponse.ContentLength : 0; } }

        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        public string ContentType { get { return innerResponse != null ? innerResponse.ContentType : null; } }

        /// <summary>
        /// Gets or sets the cookies that are associated with this response.
        /// </summary>
        public CookieCollection Cookies { get { return innerResponse != null ? innerResponse.Cookies : null; } }

        /// <summary>
        /// A System.Net.WebHeaderCollection that contains the header information returned with the response.
        /// </summary>
        public WebHeaderCollection Headers { get { return innerResponse != null ? innerResponse.Headers : null; } }

        /// <summary>
        /// Gets the method that is used to return the response.
        /// </summary>
        public string Method { get { return innerResponse != null ? innerResponse.Method : null; } }

        /// <summary>
        /// Gets the URI of the Internet resource that responded to the request.
        /// </summary>
        public Uri ResponseUri { get { return innerResponse != null ? innerResponse.ResponseUri : null; } }

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get { return innerResponse != null ? innerResponse.StatusCode : (HttpStatusCode)0; } }

        /// <summary>
        /// Gets the status description returned with the response.
        /// </summary>
        public string StatusDescription { get { return innerResponse != null ? innerResponse.StatusDescription : null; } }

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public WebExceptionStatus Status { get; private set; }

        public Response(HttpWebResponse innerResponse, WebExceptionStatus status)
        {
            this.innerResponse = innerResponse;
            this.Status = status;
        }

        /// <summary>
        /// Gets the stream that is used to read the body of the response from the server.
        /// </summary>
        /// <returns></returns>
        public Stream GetResponseStream()
        {
            if (this.innerResponse == null)
                return null;
            return this.innerResponse.GetResponseStream();
        }

        /// <summary>
        /// Gets the response body as string.
        /// </summary>
        /// <returns></returns>
        public string GetResponseAsString()
        {
            if (this.innerResponse == null)
                return null;
            using (StreamReader sr = new StreamReader(this.GetResponseStream()))
                return sr.ReadToEnd();
        }

        public void Dispose()
        {
            if (this.innerResponse != null)
                this.innerResponse.Dispose();
        }
    }
}
