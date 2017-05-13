using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    public static class WebRequestExtensions
    {
        public static Task<Stream> GetRequestStreamAsync(this WebRequest webRequest)
        {
            return Task.Factory.FromAsync<Stream>(webRequest.BeginGetRequestStream, webRequest.EndGetRequestStream, null);
        }

        public static Task<WebResponse> GetResponseAsync(this WebRequest webRequest)
        {
            return Task.Factory.FromAsync<WebResponse>(webRequest.BeginGetResponse, webRequest.EndGetResponse, null);
        }
    }
}
