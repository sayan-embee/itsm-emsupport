using Common.Layer.Models;
using Common.Layer.Models.AdaptiveCard;
using Common.Layer.Models.WebChatBot;
using Microsoft.Bot.Schema;

namespace External.CustomerPortal.Layer.Services.AdaptiveCards
{
    public interface IAdaptiveCardService
    {
        Attachment GetCard_WelcomeMessage_PersonalScope(WelcomeCardModel data);
        Attachment CreateCard_WelcomeMessage_PersonalScope(WelcomeCardModel data);
        Attachment CreateCard_UserMessage_PersonalScope(WebChatUserMessageModel data);
        Attachment CreateCard_LikeDislike_PersonalScope(WebChatUserMessageModel data);
        Attachment CreateCard_LikeDislike_Response_PersonalScope(string message, string feedback);
        Attachment CreateCard_WebChatOptions_PersonalScope(List<AdaptiveCardModel> optionList);
        Attachment CreateCard_IdleChatMessage_PersonalScope(string message, WebChatConversationModel data);
    }
}