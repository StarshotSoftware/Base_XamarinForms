using System;
using System.Threading.Tasks;
namespace BaseStarShot.Services
{
    public interface ISocialMediaService
    {
        /// <summary>
        /// Gets saved access token for the social media.
        /// </summary>
        /// <param name="socialMedia"></param>
        /// <returns></returns>
        string GetAccessToken(SocialMedia socialMedia);

        /// <summary>
        /// Gets saved access token secret for the social media.
        /// </summary>
        /// <param name="socialMedia"></param>
        /// <returns></returns>
        string GetAccessTokenSecret(SocialMedia socialMedia);

        /// <summary>
        /// Sets the consumer key and consumer secret to be used for auhtorization.
        /// </summary>
        /// <param name="socialMedia"></param>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="isDevelopment"></param>
        void SetConsumerKeyAndSecret(SocialMedia socialMedia, string consumerKey, string consumerSecret, bool isDevelopment);

        /// <summary>
        /// Uses a web authentication broker to let user authorize the app to access social media API.
        /// When result is authorized, the access token can be retrieved using the GetAccessToken method.
        /// </summary>
        /// <param name="socialMedia"></param>
        /// <returns></returns>
        Task<AuthorizationResult> AuthorizeAsync(SocialMedia socialMedia);
    }
}
