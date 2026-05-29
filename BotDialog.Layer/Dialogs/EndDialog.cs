using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using BotDialog.Layer.Services;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using Common.Layer.Models;
using BotDialog.Layer.ExceptionLog;
using Microsoft.Bot.Schema;
using Common.Layer.Models.WebChatBot;

namespace BotDialog.Layer.Dialogs
{
    public class EndDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        public EndDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(EndDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            EndChatAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        private async Task<DialogTurnResult> EndChatAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;
            if (conversationData == null)
            {
                ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in EndChatAsync.");
                await stepContext.Context.SendActivityAsync($"Something went wrong. Chat has been terminated unexpectedly.", cancellationToken: cancellationToken);

                var completionEvent = new Activity
                {
                    Type = ActivityTypes.Event,
                    Name = "endChat",
                    Value = new { message = "Chat Session Ended" }
                };

                await stepContext.Context.SendActivityAsync(completionEvent, cancellationToken);
            }
            else
            {
                try
                {
                    conversationData.EndDateTime = DateTime.Now;

                    var webchatModel = new WebChatLogModel
                    {
                        EndedOn = conversationData.EndDateTime,
                        UserEmail = conversationData.User?.UserEmail,
                        Active = false
                    };
                    _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);
                }
                catch (Exception ex)
                {
                    //
                }

                var completionEvent = new Activity
                {
                    Type = ActivityTypes.Event,
                    Name = "endChat",
                    Value = new { message = "Chat Session Ended" }
                };
                await stepContext.Context.SendActivityAsync(completionEvent, cancellationToken);                
            }

            await stepContext.Context.SendActivityAsync("Chat Ended.", cancellationToken: cancellationToken);

            await Task.Delay(2000);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
