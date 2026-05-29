using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using BotDialog.Layer.Services;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;

namespace BotDialog.Layer.Dialogs
{
    public class CategoryDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        private readonly ConversationState _conversationState;

        public CategoryDialog(
            ICommonData commonData, 
            ICustomerPortalData customerPortalData,
            ConversationState conversationState) : base(nameof(CategoryDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            ShowCategoriesAsync,
            HandleCategorySelectionAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            // Set the initial dialog
            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
            _conversationState = conversationState;
        }

        private async Task<DialogTurnResult> ShowCategoriesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // var conversationData = stepContext.Options as WebChatConversationModel;

            var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
            var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in ShowCategoriesAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var categoryList = conversationData.CategoryList;
                int categoryCount = categoryList?.Count ?? 0;

                if (categoryCount == 0)
                {
                    await stepContext.Context.SendActivityAsync("Could not find any tagged services. Please contact your administrator.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var categories = categoryList?.Select(c => new AdaptiveCardModel(c.Id, c.CategoryName)).ToList();
                categories?.Add(new AdaptiveCardModel(0, "End Chat"));

                var adaptiveCardService = new AdaptiveCardService();
                var adaptiveCardAttachment = adaptiveCardService.GenerateCategoryAdaptiveCard(categories);


                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                // Wait for user response
                return Dialog.EndOfTurn;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at ShowCategoriesAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> HandleCategorySelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
                    var userResponse = activity.Value as JObject;
                    var selectedCategoryId = userResponse?["categoryId"]?.ToString();

                    if (selectedCategoryId == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    if (!int.TryParse(selectedCategoryId, out int categoryId))
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    if (categoryId == 0)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "Thank you for your inquiry! If you need further assistance, feel free to ask.",
                            cancellationToken: cancellationToken);

                        // Begin the FeedbackDialog
                        return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), conversationData, cancellationToken);
                        //return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                    }

                    var selectedCategoryObj = conversationData.CategoryList?.FirstOrDefault(c => c.Id == categoryId);
                    if (selectedCategoryObj == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                                                   "I didn't receive a valid selection. Please try again.",
                                                                          cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    conversationData.SelectedCategory = selectedCategoryObj;

                    await stepContext.Context.SendActivityAsync(
                        $"You have selected: {selectedCategoryObj.CategoryName}. Please select a sub-category.",
                        cancellationToken: cancellationToken);



                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    // Start SubCategoryDialog
                    return await stepContext.BeginDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
                }



                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                // If no response received, prompt the user again
                await stepContext.Context.SendActivityAsync("Please select a category.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleCategorySelectionAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }
    }
}
