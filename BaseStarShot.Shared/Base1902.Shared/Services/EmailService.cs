using Base1902;
using Base1902.IO;
using BaseStarShot;
using BaseStarShot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if !WINDOWS_PHONE && !NETFX_CORE
using System.Net.Mail;
using System.Net.Mime;
#endif
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public partial class EmailService : IEmailService
    {
        #if !WINDOWS_PHONE && !NETFX_CORE
                protected SmtpClient CreateSmtpClient()
                {
                    SmtpClient smtp;

                    if (Globals.Smtp.Port.HasValue)
                        smtp = new SmtpClient(Globals.Smtp.Server, Globals.Smtp.Port.Value);
                    else
                        smtp = new SmtpClient(Globals.Smtp.Server);

                    smtp.UseDefaultCredentials = false;

                    switch (Globals.Smtp.SecurityType)
                    {
                        case SecurityType.None:
                            // Do nothing
                            break;
                        case SecurityType.StartTls:
                            smtp.EnableSsl = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (Globals.Smtp.User != null && Globals.Smtp.User.Length > 0)
                    {
                        // Use authentication
                        smtp.Credentials = new NetworkCredential(Globals.Smtp.User, Globals.Smtp.Password);
                    }
                    return smtp;
                }

                public bool Send(string subject, string textBody, string htmlBody, string from, string to, string senderName = null, string bcc = null, IEnumerable<string> attachments = null)
                {
                    try
                    {
                        to = to ?? Globals.Smtp.DefaultEmailRecepient;
                        if (string.IsNullOrEmpty(to)) return false;

                        from = from ?? Globals.Smtp.DefaultEmailSender ?? Globals.Smtp.User;

                        StringBuilder mailsb = new StringBuilder();
                        mailsb.AppendLine(textBody);

                        List<string> recipients = to.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        MailMessage mail = new MailMessage(from, recipients.First(), subject, mailsb.ToString());
                        if (recipients.Count > 1)
                        {
                            foreach (var recipient in recipients.Skip(1))
                                mail.To.Add(recipient);
                        }
                        if (string.IsNullOrEmpty(senderName))
                            senderName = Globals.Smtp.DefaultEmailSenderName;
                        if (!string.IsNullOrEmpty(senderName))
                            mail.From = new MailAddress(from, senderName);
                        if (!string.IsNullOrEmpty(bcc))
                            mail.Bcc.Add(bcc);

                        if (!string.IsNullOrEmpty(htmlBody))
                        {
                            htmlBody = htmlBody.ToHtml();

                            ContentType mimeType = new ContentType("text/html");
                            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, mimeType);
                            mail.AlternateViews.Add(htmlView);
                            mail.IsBodyHtml = true;
                        }

                        if (attachments != null)
                        {
                            var fileService = Resolver.Get<IFileService>();
                            foreach (var item in attachments)
                            {
                                var file = fileService.LoadAsync(item).Result;
                                if (file != null)
                                    mail.Attachments.Add(new Attachment(file.GetStreamAsync().Result, new ContentType(file.GetMimeType())) { Name = file.Name });
                            }
                        }

                        var smtpClient = CreateSmtpClient();
                        smtpClient.Send(mail);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteError("EmailService", ex);
                    }
                    return false;
                }
        #endif

        #if !NETFX_CORE
                public async Task<bool> SendAsync(string subject, string textBody, string htmlBody, string from, string to, string senderName = null, string bcc = null, IEnumerable<string> attachments = null)
                {
                    ManualResetEvent sendEvent = new ManualResetEvent(false);
                    bool result = false;
                    Thread sendTaskThread = null;
            
                    var sendTask = Task.Run(() =>
                    {
                        sendTaskThread = Thread.CurrentThread;
                        result = Send(subject, textBody, htmlBody, from, to, senderName: senderName, bcc: bcc, attachments: attachments);
                        sendEvent.Set();
                    });

                    await Task.Run(() =>
                    {
                        sendEvent.WaitOne(60000);
                        if (!sendTask.IsCompleted)
                            sendTaskThread.Abort();
                    });

                    return result;
                }
        #endif
    }
}
