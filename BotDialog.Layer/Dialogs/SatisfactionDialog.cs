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
using Microsoft.Bot.Builder.Dialogs.Choices;
using Common.Layer.Models.WebChatBot;

namespace BotDialog.Layer.Dialogs
{
    public class SatisfactionDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        public SatisfactionDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(FeedbackDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            AskSatisfactionStepAsync,
            HandleSatisfactionResponseAsync,
            //AskAdditionalFeedbackStepAsync,
            //ProcessAdditionalFeedbackQueryResponseAsync,
            //HandleAdditionalFeedbackResponseAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        private async Task<DialogTurnResult> AskSatisfactionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Are you satisfied with the resolution?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" })
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at AskSatisfactionStepAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleSatisfactionResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleSatisfactionResponseAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var choice = (stepContext.Result as FoundChoice)?.Value;

                // Save the feedback
                var webchatModel = new WebChatLogModel
                {
                    UserEmail = conversationData.User?.UserEmail,
                    SatisfiedWithResolution = choice == "Yes" ? true : false
                };
                _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);

                if (choice == "Yes")
                {
                    await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleSatisfactionResponseAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        //private async Task<DialogTurnResult> AskAdditionalFeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var conversationData = stepContext.Options as WebChatConversationModel;

        //    try
        //    {
        //        return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
        //        {
        //            Prompt = MessageFactory.Text("Do you want to provide additional feedback?"),
        //            Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" })
        //        }, cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.WriteMessageToText($"Error at AskAdditionalFeedbackStepAsync() - {ex.Message}");
        //        ExceptionLogging.SendErrorToText(ex);

        //        await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
        //        return await stepContext.EndDialogAsync(conversationData, cancellationToken);
        //    }
        //}

        //private async Task<DialogTurnResult> ProcessAdditionalFeedbackQueryResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var conversationData = stepContext.Options as WebChatConversationModel;

        //    try
        //    {
        //        var choice = (stepContext.Result as FoundChoice)?.Value;

        //        if (choice == "Yes")
        //        {
        //            var adaptiveCardService = new AdaptiveCardService();
        //            var adaptiveCardAttachment = adaptiveCardService.GenerateAdditionalFeedbackCard();

        //            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

        //            // Wait for the user to select an option (the dialog remains open until user selection)
        //            return Dialog.EndOfTurn;
        //        }
        //        else
        //        {
        //            await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
        //            return await stepContext.NextAsync(conversationData, cancellationToken);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.WriteMessageToText($"Error at ProcessAdditionalFeedbackQueryResponseAsync() - {ex.Message}");
        //        ExceptionLogging.SendErrorToText(ex);

        //        await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
        //        return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        //    }
        //}

        //private async Task<DialogTurnResult> HandleAdditionalFeedbackResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var conversationData = stepContext.Options as WebChatConversationModel;

        //    try
        //    {
        //        if (conversationData == null)
        //        {
        //            ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleCategorySelectionAsync.");

        //            await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
        //            return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        //        }

        //        var activity = stepContext.Context.Activity;
        //        if (activity.Value != null)
        //        {
        //            // Extract the user's selection
        //            var userResponse = activity.Value as JObject;
        //            var selectedAddFeedback = userResponse?["additionalFeedback"]?.ToString();

        //            await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
        //            return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        //        }

        //        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.WriteMessageToText($"Error at HandleAdditionalFeedbackResponseAsync() - {ex.Message}");
        //        ExceptionLogging.SendErrorToText(ex);

        //        await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
        //        return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        //    }
        //}
    }
}
