using Common.Layer.Models.WebChatBot;

namespace External.CustomerPortal.Layer.Services.WebChatBot
{
    public interface IWebChatBotService
    {
        Task<WebChatLogModel> GenerateDirectLineToken(WebChatSettings settings, WebChatLogModel dataModel);
    }
}