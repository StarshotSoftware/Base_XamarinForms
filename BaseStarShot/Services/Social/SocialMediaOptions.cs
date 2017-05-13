using System;
using System.Collections.Generic;
using System.Text;

namespace BaseStarShot.Services
{
    public class SocialMediaOptions
    {
        public SocialMedia Type { get; set; }

        public string BaseUrl { get; set; }

        public string RequestTokenUrl { get; set; }

        public string AuthorizeUrl { get; set; }

        public string AccessTokenUrl { get; set; }

        public bool IsDevelopment { get; set; }

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }
    }
}
