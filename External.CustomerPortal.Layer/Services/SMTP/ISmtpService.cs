using Common.Layer.Models;

namespace External.CustomerPortal.Layer.Services.SMTP
{
    public interface ISmtpService
    {
        Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel);
    }
}