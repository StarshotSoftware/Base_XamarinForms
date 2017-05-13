using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETFX_CORE 
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Authentication.Web;
#else
using System.Security.Cryptography;
#endif

namespace BaseStarShot.Services
{
    public class SocialMediaService : ISocialMediaService
    {
        const string SettingsBaseKey = "BaseStarShot.Services.SocialMediaService.";
        //const string CallbackUrl = "http://google.com";
        const string CallbackUrl = "en-posedus://response";

        #region Twitter
        const string TwitterBaseUrl = "https://api.twitter.com/";
        const string TwitterRequestTokenUrl = "oauth/request_token";
        const string TwitterAuthorizeUrl = "oauth/authorize";
        const string TwitterAccessTokenUrl = "oauth/access_token";
        #endregion

        #region Evernote
        const string EvernoteBaseUrl = "https://www.evernote.com/";
        const string EvernoteDevelopmentUrl = "https://sandbox.evernote.com/";
        const string EvernoteRequestTokenUrl = "oauth";
        const string EvernoteAuthorizeUrl = "OAuth.action";
        const string EvernoteAccessTokenUrl = "oauth";
        #endregion

        Dictionary<SocialMedia, SocialMediaOptions> options = new Dictionary<SocialMedia, SocialMediaOptions>();

        private SocialMediaOptions GetOptions(SocialMedia socialMedia)
        {
            if (options.ContainsKey(socialMedia))
                return options[socialMedia];
            return null;
        }

        public void SetConsumerKeyAndSecret(SocialMedia socialMedia, string consumerKey, string consumerSecret, bool isDevelopment)
        {
            SocialMediaOptions socialOptions = null;
            switch (socialMedia)
            {
                case SocialMedia.Twitter:
                    socialOptions = new SocialMediaOptions
                    {
                        BaseUrl = TwitterBaseUrl,
                        RequestTokenUrl = TwitterRequestTokenUrl,
                        AuthorizeUrl = TwitterAuthorizeUrl,
                        AccessTokenUrl = TwitterAccessTokenUrl,
                        Type = SocialMedia.Twitter
                    };
                    break;
                case SocialMedia.Evernote:
                    socialOptions = new SocialMediaOptions
                    {
                        BaseUrl = isDevelopment ? EvernoteDevelopmentUrl : EvernoteBaseUrl,
                        RequestTokenUrl = EvernoteRequestTokenUrl,
                        AuthorizeUrl = EvernoteAuthorizeUrl,
                        AccessTokenUrl = EvernoteAccessTokenUrl,
                        IsDevelopment = isDevelopment,
                        Type = SocialMedia.Evernote
                    };
                    break;
            }
            if (socialOptions != null)
            {
                socialOptions.ConsumerKey = consumerKey;
                socialOptions.ConsumerSecret = consumerSecret;
                options[socialMedia] = socialOptions;
            }
        }

        public string GetAccessToken(SocialMedia socialMedia)
        {
            return SettingsHelper.GetSettings<string>(SettingsBaseKey + "AccessToken");
        }

        public string GetAccessTokenSecret(SocialMedia socialMedia)
        {
            return SettingsHelper.GetSettings<string>(SettingsBaseKey + "AccessTokenSecret");
        }

        protected void SaveAccessToken(SocialMedia socialMedia, string accessToken)
        {
            SettingsHelper.SaveSettings(SettingsBaseKey + "AccessToken", accessToken);
        }

        protected void SaveAccessTokenSecret(SocialMedia socialMedia, string accessTokenSecret)
        {
            SettingsHelper.SaveSettings(SettingsBaseKey + "AccessTokenSecret", accessTokenSecret);
        }

        #region OAuth methods
        protected RequestParams GetRequestParams(SocialMediaOptions options, string url, string method, string authenticationToken = null, string verifier = null)
        {
            string nonce = GetNonce();
            string timeStamp = GetTimeStamp();
            string sigBaseStringParams = null;
            if (string.IsNullOrEmpty(authenticationToken))
                sigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(CallbackUrl) + "&";
            sigBaseStringParams += "oauth_consumer_key=" + options.ConsumerKey;
            sigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            sigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            sigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            if (!string.IsNullOrEmpty(authenticationToken))
                sigBaseStringParams += "&" + "oauth_token=" + authenticationToken;
            if (!string.IsNullOrEmpty(verifier))
                sigBaseStringParams += "&" + "oauth_verifier=" + verifier;
            sigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = method + "&";
            SigBaseString += Uri.EscapeDataString(url) + "&" + Uri.EscapeDataString(sigBaseStringParams);
            string signature = GetSignature(SigBaseString, options.ConsumerSecret);
            return new RequestParams { Signature = signature, SigBaseString = sigBaseStringParams, Nonce = nonce, Timestamp = timeStamp };
        }

        string GetNonce()
        {
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        string GetTimeStamp()
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(SinceEpoch.TotalSeconds).ToString();
        }

        string GetSignature(string sigBaseString, string consumerSecretKey)
        {
#if NETFX_CORE
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha1);
            CryptographicKey macKey = hmacSha1Provider.CreateKey(keyMaterial);
            IBuffer dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            string signature = CryptographicBuffer.EncodeToBase64String(signatureBuffer);
#else
            var key = System.Text.Encoding.UTF8.GetBytes(consumerSecretKey + "&");
            var hmacAlgo = new HMACSHA1(key);
            string signature = Convert.ToBase64String(hmacAlgo.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sigBaseString)));
#endif
            return signature;
        }

        #region Request Tokens
        private async Task<string> GetRequestTokenAsync(SocialMediaOptions options)
        {
            if (options == null) return null;

            var url = options.BaseUrl + options.RequestTokenUrl;
            var requestParams = GetRequestParams(options, url, "GET");

            url += "?" + requestParams.SigBaseString + "&oauth_signature=" + Uri.EscapeDataString(requestParams.Signature);
            var httpClient = new System.Net.Http.HttpClient();
            string getResponse = await httpClient.GetStringAsync(new Uri(url));

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = getResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            return request_token;
        }

        private async Task<AuthorizationResult> GetAccessToken(SocialMediaOptions options, string webAuthResultResponseData)
        {
            if (string.IsNullOrEmpty(webAuthResultResponseData)) return AuthorizationResult.ConnectionFailed;
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"));
            string request_token = null;
            string oauth_verifier = null;
            String[] keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }

            if (string.IsNullOrEmpty(oauth_verifier))
                return AuthorizationResult.Cancelled;

            var url = options.BaseUrl + options.AccessTokenUrl;
            System.Net.Http.HttpResponseMessage httpResponseMessage = null;
            if (options.Type == SocialMedia.Twitter)
            {
                var requestParams = GetRequestParams(options, url, "POST", authenticationToken: request_token);
                var httpClient = new System.Net.Http.HttpClient();

                var httpContent = new System.Net.Http.StringContent("oauth_verifier=" + oauth_verifier);
                httpContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                string authorizationHeaderParams = "oauth_consumer_key=\"" + options.ConsumerKey + "\", oauth_nonce=\"" + requestParams.Nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(requestParams.Signature) + "\", oauth_timestamp=\"" + requestParams.Timestamp + "\", oauth_token=\"" + Uri.EscapeDataString(request_token) + "\", oauth_version=\"1.0\"";

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authorizationHeaderParams);
                httpResponseMessage = await httpClient.PostAsync(new Uri(url), httpContent);
            }
            else
            {
                var requestParams = GetRequestParams(options, url, "GET", authenticationToken: request_token, verifier: oauth_verifier);
                url += "?"// + requestParams.SigBaseString
                    //+ "&"
                    + "oauth_verifier=" + Uri.EscapeDataString(oauth_verifier);
                    //+ "&" + "oauth_signature=" + Uri.EscapeDataString(requestParams.Signature);
                var httpClient = new System.Net.Http.HttpClient();

                string authorizationHeaderParams =
                    "oauth_signature=\"" + Uri.EscapeDataString(requestParams.Signature) + "\""
                    + ",oauth_nonce=\"" + requestParams.Nonce + "\""
                    + ",oauth_timestamp=\"" + requestParams.Timestamp + "\""
                    + ",oauth_consumer_key=\"" + options.ConsumerKey + "\""
                    + ",oauth_token=\"" + Uri.EscapeDataString(request_token) + "\""
                    + ",oauth_version=\"1.0\""
                    + ",oauth_signature_method=\"HMAC-SHA1\"";

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", authorizationHeaderParams);

                httpResponseMessage = await httpClient.SendAsync(new System.Net.Http.HttpRequestMessage { RequestUri = new Uri(url), Method = System.Net.Http.HttpMethod.Get });
            }
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string response = await httpResponseMessage.Content.ReadAsStringAsync();

                String[] tokens = response.Split('&');
                string oauth_token_secret = null;
                string access_token = null;

                for (int i = 0; i < tokens.Length; i++)
                {
                    String[] splits = tokens[i].Split('=');
                    switch (splits[0])
                    {
                        case "oauth_token":
                            access_token = splits[1];
                            break;
                        case "oauth_token_secret":
                            oauth_token_secret = splits[1];
                            break;
                    }
                }

                if (access_token != null)
                {
                    SaveAccessToken(options.Type, access_token);
                    SaveAccessTokenSecret(options.Type, oauth_token_secret);
                    return AuthorizationResult.Authorized;
                }
                return AuthorizationResult.FailedToRetrieveAccessToken;
            }
            return AuthorizationResult.ConnectionFailed;
        }
        #endregion

        public async Task<AuthorizationResult> AuthorizeAsync(SocialMedia socialMedia)
        {
            var options = GetOptions(socialMedia);
            if (options == null) return AuthorizationResult.NoConsumerKeyAndSecret;

            var requestToken = await GetRequestTokenAsync(options);

            var url = options.BaseUrl + options.AuthorizeUrl + "?oauth_token=" + requestToken;
            Uri startUri = new Uri(url);
            Uri endUri = new Uri(CallbackUrl);

            string webAuthResultResponseData = null;

#if NETFX_CORE
            WebAuthenticationResult result = null;
#if WINDOWS_PHONE_APP
            WebAuthenticationBroker.AuthenticateAndContinue(startUri, endUri, null, WebAuthenticationOptions.None);
            var authEvent = new System.Threading.ManualResetEvent(false);
            var view = global::Windows.ApplicationModel.Core.CoreApplication.GetCurrentView();
            view.Activated += (s, e) =>
                {
                    if (e.Kind == global::Windows.ApplicationModel.Activation.ActivationKind.WebAuthenticationBrokerContinuation)
                    {
                        var args = e as global::Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs;
                        result = args.WebAuthenticationResult;
                    }
                    authEvent.Set();
                };
            await Task.Run(() => authEvent.WaitOne());
#elif WINDOWS_APP
            result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
#endif
            if (result == null)
            {
                return AuthorizationResult.ConnectionFailed;
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                webAuthResultResponseData = result.ResponseData.ToString();
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                return AuthorizationResult.Cancelled;
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                return AuthorizationResult.ConnectionFailed;
            }
            else
            {
                return AuthorizationResult.ConnectionFailed;
            }
#elif WINDOWS_PHONE
            // TODO: Windows Phone Silverlight WebAuthenticationBroker implementation
#elif __IOS__
            // TODO: iOS WebAuthenticationBroker implementation
#else // Droid
            // TODO: Android WebAuthenticationBroker implementation
#endif

            return await GetAccessToken(options, webAuthResultResponseData);
        }
        #endregion

        protected class RequestParams
        {
            public string Signature { get; set; }
            public string SigBaseString { get; set; }
            public string Nonce { get; set; }
            public string Timestamp { get; set; }
        }
    }
}
