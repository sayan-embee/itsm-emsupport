using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using BotDialog.Layer.Services;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using Common.Layer.Models;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Common.Layer.Models.WebChatBot;
using External.CustomerPortal.Layer.ExceptionLog;

namespace BotDialog.Layer.Dialogs
{
    public class FeedbackDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        private readonly ConversationState _conversationState;

        public FeedbackDialog(
            ICommonData commonData, 
            ICustomerPortalData customerPortalData,
            ConversationState conversationState) : base(nameof(FeedbackDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            AskFeedbackStepAsync,
            HandleFeedbackResponseAsync,
            //AskAdditionalFeedbackStepAsync,
            //ProcessAdditionalFeedbackQueryResponseAsync,
            //HandleAdditionalFeedbackResponseAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
            _conversationState = conversationState;
        }

        private async Task<DialogTurnResult> AskFeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                var feedbackOptions = await this._customerPortalData.WebChatFeedbackOptions_Get();
                if (feedbackOptions != null && feedbackOptions.Count > 0)
                {
                    var adaptiveCardService = new AdaptiveCardService();
                    var adaptiveCardAttachment = adaptiveCardService.GenerateRatingCard(feedbackOptions);

                    conversationData.WaitingForFeedbackResponse = true;

                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                    return Dialog.EndOfTurn;
                }
                else
                {
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at AskFeedbackStepAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleFeedbackResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleFeedbackResponseAsync.");

                    await stepContext.Context.SendActivityAsync("Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var activity = stepContext.Context.Activity;

                // Check if the activity contains a value (indicating Adaptive Card submission)
                if (activity.Value != null)
                {
                    // Extract the user's feedback responses
                    var userResponse = activity.Value as JObject;
                    var selectedFeedbackId = userResponse?["rating"]?.ToString();
                    var comments = userResponse?["comments"]?.ToString();
                    var satisfaction = userResponse?["satisfaction"]?.ToString();

                    if (!int.TryParse(selectedFeedbackId, out int feedbackId))
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid rating. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    conversationData.WaitingForFeedbackResponse = false;

                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

                    // Save the feedback rating, comments, and satisfaction
                    var webchatModel = new WebChatLogModel
                    {
                        UserEmail = conversationData.User?.UserEmail,
                        FeedbackRatingId = feedbackId,
                        AdditionalFeedback = comments,
                        SatisfiedWithResolution = satisfaction?.ToLower() == "yes" ? true : false
                    };

                    _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);

                    var adaptiveCardService = new AdaptiveCardService();
                    var adaptiveCardAttachment = adaptiveCardService.CreateCard_LikeDislike_Response_PersonalScope("Thank you for your feedback.");
                    if (adaptiveCardAttachment != null)
                    {
                        await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                    }

                    // Begin the FeedbackDialog
                    return await stepContext.BeginDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                    //return await stepContext.NextAsync(conversationData, cancellationToken);
                }

                return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleFeedbackResponseAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync("Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AskAdditionalFeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                conversationData.WaitingForFeedbackResponse = true;

                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you want to provide additional feedback?"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Yes", "No" })
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at AskAdditionalFeedbackStepAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ProcessAdditionalFeedbackQueryResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                if (!conversationData.WaitingForFeedbackResponse)
                {
                    return Dialog.EndOfTurn;
                }

                conversationData.WaitingForFeedbackResponse = false;


                var choice = (stepContext.Result as FoundChoice)?.Value;

                if (choice == "Yes")
                {
                    var adaptiveCardService = new AdaptiveCardService();
                    var adaptiveCardAttachment = adaptiveCardService.GenerateAdditionalFeedbackCard();


                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                    // Wait for the user to select an option (the dialog remains open until user selection)
                    return Dialog.EndOfTurn;
                }
                else
                {
                    await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(SatisfactionDialog), conversationData, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at ProcessAdditionalFeedbackQueryResponseAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleAdditionalFeedbackResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleCategorySelectionAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var activity = stepContext.Context.Activity;
                if (activity.Value != null)
                {
                    // Extract the user's selection
                    var userResponse = activity.Value as JObject;
                    var selectedAddFeedback = userResponse?["additionalFeedback"]?.ToString();

                    // Save the feedback
                    var webchatModel = new WebChatLogModel
                    {
                        UserEmail = conversationData.User?.UserEmail,
                        AdditionalFeedback = selectedAddFeedback
                    };
                    _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);


                    conversationData.WaitingForFeedbackResponse = false;

                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    await stepContext.Context.SendActivityAsync($"Thank you for your feedback!", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(SatisfactionDialog), conversationData, cancellationToken);
                }

                return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleAdditionalFeedbackResponseAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }
    }
}
