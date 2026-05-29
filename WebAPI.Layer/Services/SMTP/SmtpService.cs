using Common.Layer.Models;
using System.Net;
using System.Net.Mail;
using WebAPI.Layer.ExceptionLog;
using WebAPI.Layer.Services.GraphAPI;

namespace WebAPI.Layer.Services.SMTP
{
    public class SmtpService : ISmtpService
    {
        public async Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel)
        {
            try
            {
                if (configModel.IsCredRequired)
                {
                    var smtpClient = new SmtpClient(configModel.Server)
                    {
                        Port = int.Parse(configModel.Port),
                        Credentials = new NetworkCredential(configModel.UserId, configModel.Pass),
                        EnableSsl = configModel.EnableSSL,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(configModel.UserMail, configModel.DisplayName),
                        Subject = emailModel.Subject,
                        Body = emailModel.Body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(emailModel.To);
                    if (!string.IsNullOrEmpty(emailModel.CC))
                    {
                        mailMessage.CC.Add(emailModel.CC);
                    }

                    //await smtpClient.SendMailAsync(mailMessage);
                    smtpClient.Send(mailMessage);
                    await Task.Delay(500);

                    return true;
                }
                else
                {
                    var result = false;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(configModel.UserMail, configModel.DisplayName),
                        Subject = emailModel.Subject,
                        Body = emailModel.Body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(emailModel.To);
                    if (!string.IsNullOrEmpty(emailModel.CC))
                    {
                        mailMessage.CC.Add(emailModel.CC);
                    }

                    using (var smtpClient = new SmtpClient
                    {
                        Host = configModel.Server,
                        Port = int.Parse(configModel.Port),
                        EnableSsl = configModel.EnableSSL,
                    })
                    {
                        smtpClient.UseDefaultCredentials = true;
                        smtpClient.Send(mailMessage);
                        await Task.Delay(500);

                        result = true;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return false;
            }
        }
    }
}
