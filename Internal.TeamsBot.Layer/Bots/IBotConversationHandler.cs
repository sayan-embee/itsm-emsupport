using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Internal.TeamsBot.Layer.Bots
{
    public interface IBotConversationHandler
    {
        Task OnMessageActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken);
    }
}