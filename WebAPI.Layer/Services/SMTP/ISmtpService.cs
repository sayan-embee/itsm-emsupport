using Common.Layer.Models;

namespace WebAPI.Layer.Services.SMTP
{
    public interface ISmtpService
    {
        Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel);
    }
}