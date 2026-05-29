using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Common.Layer.Models;
using BotDialog.Layer.Services;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using BotDialog.Layer.ExceptionLog;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace BotDialog.Layer.Dialogs
{
    public class ProcessOtherQueryDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        public ProcessOtherQueryDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(ProcessOtherQueryDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            GetUserInputAsync,
            SendResponseCardAsync,
            AskForAnotherQueryAsync,
            ProcessAnotherQueryResponseAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        private async Task<DialogTurnResult> GetUserInputAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your query.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SendResponseCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                var userInput = stepContext.Result as string;

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    await stepContext.Context.SendActivityAsync("I didn't receive a valid input. Please try again.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                }

                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in SendResponseCardAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                if (conversationData.SelectedCategory == null)
                {
                    await stepContext.Context.SendActivityAsync("Please select a category.", cancellationToken: cancellationToken);

                    // Replace the current with the CategoryDialog
                    return await stepContext.ReplaceDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
                }

                if (conversationData.SelectedSubCategory == null)
                {
                    await stepContext.Context.SendActivityAsync("Please select a sub-category.", cancellationToken: cancellationToken);

                    // Replace the current with the CategoryDialog
                    return await stepContext.ReplaceDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
                }

                await stepContext.Context.SendActivityAsync("Processing your query...", cancellationToken: cancellationToken);

                return await stepContext.NextAsync(conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at SendResponseCardAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> AskForAnotherQueryAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you have another query?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" })
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at AskForAnotherQueryAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ProcessAnotherQueryResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                var choice = (stepContext.Result as FoundChoice)?.Value;

                if (choice == "Yes")
                {
                    return await stepContext.ReplaceDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(
                                "Thank you for your inquiry! If you need further assistance, feel free to ask.",
                                cancellationToken: cancellationToken);

                    // Begin the FeedbackDialog
                    return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), conversationData, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at ProcessAnotherQueryResponseAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

    }
}
