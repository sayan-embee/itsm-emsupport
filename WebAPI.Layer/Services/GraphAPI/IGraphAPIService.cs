using Common.Layer.Models;

namespace WebAPI.Layer.Services.GraphAPI
{
    public interface IGraphAPIService
    {
        Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel);
    }
}