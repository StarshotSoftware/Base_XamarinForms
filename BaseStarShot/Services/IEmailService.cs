using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// From XLabs.Platform.Services.EmailService
namespace BaseStarShot.Services
{
    /// <summary>
    /// Interface IEmailService
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gets a value indicating whether this instance can send.
        /// </summary>
        /// <value><c>true</c> if this instance can send; otherwise, <c>false</c>.</value>
        bool CanSend { get; }

        /// <summary>
        /// Shows the draft.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="html">if set to <c>true</c> [HTML].</param>
        /// <param name="to">To.</param>
        /// <param name="attachments">The attachments.</param>
        void ShowDraft(string subject, string body, bool html, string to, IEnumerable<string> attachments = null);

        /// <summary>
        /// Shows the draft.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="html">if set to <c>true</c> [HTML].</param>
        /// <param name="to">To.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="attachments">The attachments.</param>
        void ShowDraft(string subject, string body, bool html, string[] to, string[] cc, string[] bcc, IEnumerable<string> attachments = null);

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="textBody"></param>
        /// <param name="htmlBody"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="senderName"></param>
        /// <param name="bcc"></param>
        /// <param name="attachments"></param>
        bool Send(string subject, string textBody, string htmlBody, string from, string to, string senderName = null, string bcc = null, IEnumerable<string> attachments = null);

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="textBody"></param>
        /// <param name="htmlBody"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="senderName"></param>
        /// <param name="bcc"></param>
        /// <param name="attachments"></param>
        Task<bool> SendAsync(string subject, string textBody, string htmlBody, string from, string to, string senderName = null, string bcc = null, IEnumerable<string> attachments = null);
    }
}
