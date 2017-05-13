using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public class SmtpInfo
    {
        /// <summary>
        /// Smtp user.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Smtp password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Smtp server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Smtp port. Uses default mail port if set to null.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Default email recipient.
        /// </summary>
        public string DefaultEmailRecepient { get; set; }

        /// <summary>
        /// Default email sender.
        /// </summary>
        public string DefaultEmailSender { get; set; }

        /// <summary>
        /// Default email sender name.
        /// </summary>
        public string DefaultEmailSenderName { get; set; }

        /// <summary>
        /// Smtp security setting.
        /// </summary>
        public SecurityType SecurityType { get; set; }
    }
}
