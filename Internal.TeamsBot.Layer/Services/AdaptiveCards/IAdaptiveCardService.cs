using Common.Layer.Models;
using Common.Layer.Models.AdaptiveCard;
using Microsoft.Bot.Schema;

namespace Internal.TeamsBot.Layer.Services.AdaptiveCards
{
    public interface IAdaptiveCardService
    {
        Attachment GetCard_WelcomeMessage_PersonalScope(WelcomeCardModel data);
        Attachment CreateCard_WelcomeMessage_PersonalScope(WelcomeCardModel data);
        Attachment? CreateCard_UserMessage_PersonalScope(UserMessageModel data);
    }
}