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
using Microsoft.Bot.Builder.Dialogs.Choices;
using External.CustomerPortal.Layer.ExceptionLog;
using External.CustomerPortal.Layer.Bot;

namespace BotDialog.Layer.Dialogs
{
    public class ProcessOtherQueryDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;
        private readonly IBotConversationHandler _botConversationHandler;

        private readonly ConversationState _conversationState;

        public ProcessOtherQueryDialog(
            ICommonData commonData, 
            ICustomerPortalData customerPortalData, 
            IBotConversationHandler botConversationHandler,
            ConversationState conversationState) : base(nameof(ProcessOtherQueryDialog))
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
            _botConversationHandler = botConversationHandler ?? throw new ArgumentNullException(nameof(botConversationHandler));
            _conversationState = conversationState;
        }

        private async Task<DialogTurnResult> GetUserInputAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your query in a single line.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> SendResponseCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

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

                conversationData.UserMessage.Text = userInput?.Trim().ToString();



                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);



                var result = await _botConversationHandler.OnMessageActivityHandlerForDialogs(stepContext, cancellationToken, conversationData);
                if (result)
                {
                    return await stepContext.NextAsync(conversationData, cancellationToken);
                }
                else
                {
                    return await stepContext.ReplaceDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
                }
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
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                conversationData.WaitingForQueryResponse = true;

                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


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
            //var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                if (!conversationData.WaitingForQueryResponse)
                {
                    return Dialog.EndOfTurn;
                }

                conversationData.WaitingForQueryResponse = false;


                var choice = (stepContext.Result as FoundChoice)?.Value;

                if (choice == "Yes")
                {

                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    return await stepContext.ReplaceDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                }
                else
                {
                    await stepContext.Context.SendActivityAsync(
                                "Thank you for your inquiry! If you need further assistance, feel free to ask.",
                                cancellationToken: cancellationToken);



                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


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
