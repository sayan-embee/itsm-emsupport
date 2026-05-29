using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Internal.TeamsBot.Layer.Bots
{
    public interface IAppLifecycleHandler
    {
        Task OnBotInstalledInPersonalAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName);
        Task OnBotRemovedInPersonalAsync(ITurnContext turnContext, string appName);
    }
}
