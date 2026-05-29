using Common.Layer.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace External.CustomerPortal.Layer.Bot
{
    public interface IBotConversationHandler
    {
        Task<bool> OnMessageActivityHandlerForDialogs(WaterfallStepContext stepContext, CancellationToken cancellationToken, WebChatConversationModel conversationData);
    }
}