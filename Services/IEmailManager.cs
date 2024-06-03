using CalendarInviteICS.Dtos;
using CalendarInviteICS.Helpers;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CalendarInviteICS.Services
{
    public interface IEmailManager
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendEmailAsync(string email, string subject, string htmlMessage, Attachment attachment);
        Task SendEmailMultiSenderAsync(string emails, string ccemails, string bccemails, string subject, string message);
        Task SendEmailMultiSenderAsync(string emails, string ccemails, string bccemails, string subject, string htmlMessage, Attachment attachment);

    }

    public class EmailManager : IEmailManager
    {
        private readonly EmailSettingsDto _emailSettings;

        public EmailManager(IOptions<EmailSettingsDto> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SenderName, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = _emailSettings.EnableSSL,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            return Task.CompletedTask;
        }


        public Task SendEmailAsync(string email, string subject, string message, Attachment attachment)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.SenderName, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                Attachment data = new Attachment(attachment.ContentStream, attachment.Name);
                mail.Attachments.Add(data);
                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = _emailSettings.EnableSSL,//false,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            return Task.CompletedTask;
        }


        #region "Multiple Emails Sender"

        public Task SendEmailMultiSenderAsync(string emails, string ccemails, string bccemails, string subject, string message)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                if (!emails.Contains(GlobalEnumFunctions.stringSplitterPipe))
                {
                    mail.To.Add(new MailAddress(emails));
                }
                else
                {
                    string[] _mails = emails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string email in _mails)
                    {
                        mail.To.Add(new MailAddress(email)); //adding multiple To Email Id
                    }
                }

                if (!string.IsNullOrEmpty(ccemails))
                {
                    string[] CCId = ccemails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string CCEmail in CCId)
                    {
                        mail.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }

                if (!string.IsNullOrEmpty(bccemails))
                {
                    string[] bccid = bccemails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string bccEmailId in bccid)
                    {
                        mail.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                    }
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = _emailSettings.EnableSSL,//false,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            return Task.CompletedTask;
        }


        public Task SendEmailMultiSenderAsync(string emails, string ccemails, string bccemails, string subject, string message, Attachment attachment)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                Attachment data = new Attachment(attachment.ContentStream, attachment.Name);
                mail.Attachments.Add(data);

                //if single email then send email to one 
                if (!emails.Contains(GlobalEnumFunctions.stringSplitterPipe))
                {
                    mail.To.Add(new MailAddress(emails));
                }
                else
                {
                    string[] _mails = emails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string email in _mails)
                    {
                        mail.To.Add(new MailAddress(email)); //adding multiple To Email Id
                    }
                }

                if (!string.IsNullOrEmpty(ccemails))
                {
                    string[] CCId = ccemails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string CCEmail in CCId)
                    {
                        mail.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }

                if (!string.IsNullOrEmpty(bccemails))
                {
                    string[] bccid = bccemails.TrimEnd(GlobalEnumFunctions.stringSplitterPipe).Split(GlobalEnumFunctions.stringSplitterPipe);
                    foreach (string bccEmailId in bccid)
                    {
                        mail.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                    }
                }

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = _emailSettings.EnableSSL,//false,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
