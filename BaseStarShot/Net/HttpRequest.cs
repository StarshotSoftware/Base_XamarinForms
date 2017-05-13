using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Net
{
    public class HttpRequest
    {
        public string Url { get; set; }

        public object Parameters { get; set; }

        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public GETRequestSerialization? Serialization { get; set; }

        public string AppendUrl { get; set; }

        public bool UseJsonPropertyEncoding { get; set; }

        public bool IsJsonContent { get; set; }

        public WebRequestMethod Method { get; set; } = WebRequestMethod.GET;

        public object Data { get; set; }

        public object Tag { get; set; }
    }
}
