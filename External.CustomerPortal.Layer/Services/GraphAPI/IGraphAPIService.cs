using Common.Layer.Models;

namespace External.CustomerPortal.Layer.Services.GraphAPI
{
    public interface IGraphAPIService
    {
        Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel);
    }
}