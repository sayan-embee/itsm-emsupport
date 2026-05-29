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

namespace BotDialog.Layer.Dialogs
{
    public class OptionsDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        public OptionsDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(OptionsDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            ShowFinalOptionsAsync,
            HandleFinalOptionsAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        private async Task<DialogTurnResult> ShowFinalOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in ShowFinalOptionsAsync.");
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

                var options = conversationData.WebChatOptions?.Select(c => new AdaptiveCardModel(c.OptionId, c.Option)).ToList();

                options?.Add(new AdaptiveCardModel(-3, "Others", ""));
                options?.Add(new AdaptiveCardModel(-2, "Go Back", ""));
                //options?.Add(new AdaptiveCardModel(-1, "Return to Category Menu", ""));
                options?.Add(new AdaptiveCardModel(0, "End Chat", ""));

                // Generate the Adaptive Card for final options
                var adaptiveCardService = new AdaptiveCardService();
                var adaptiveCardAttachment = adaptiveCardService.GenerateFinalOptionsCard(options);

                // Send the adaptive card with options to the user
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                // Wait for the user to select an option (the dialog remains open until user selection)
                return Dialog.EndOfTurn;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at ShowFinalOptionsAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> HandleFinalOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleFinalOptionsAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var activity = stepContext.Context.Activity;
                if (activity.Value != null)
                {
                    // Extract the user's selection
                    var userResponse = activity.Value as JObject;
                    var action = userResponse?["action"]?.ToString(); // Capture button action
                    var selectedOptionId = userResponse?["optionId"]?.ToString();

                    // Handle button actions
                    if (!string.IsNullOrEmpty(action))
                    {
                        if (action == "endChat")
                        {
                            await stepContext.Context.SendActivityAsync("Thank you for your inquiry! If you need further assistance, feel free to ask.", cancellationToken: cancellationToken);
                            return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                        }
                        if (action == "goBack")
                        {
                            return await stepContext.ReplaceDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
                        }
                        if (action == "others")
                        {
                            return await stepContext.BeginDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                        }
                    }

                    if (selectedOptionId == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    if (!int.TryParse(selectedOptionId, out int optionId))
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    // Handle the "Exit" case
                    if (optionId == 0)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "Thank you for your inquiry! If you need further assistance, feel free to ask.",
                            cancellationToken: cancellationToken);

                        // Begin the FeedbackDialog
                        return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), conversationData, cancellationToken);
                    }

                    if (optionId == -1)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "Please select a category.",
                            cancellationToken: cancellationToken);

                        // Begin the CategoryDialog
                        return await stepContext.ReplaceDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
                    }

                    if (optionId == -2)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "Please select a sub-category.",
                            cancellationToken: cancellationToken);

                        // Begin the CategoryDialog
                        return await stepContext.ReplaceDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
                    }

                    if (optionId == -3)
                    {
                        return await stepContext.BeginDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                    }

                    var selectedOptionObj = conversationData.WebChatOptions?.FirstOrDefault(c => c.OptionId == optionId);
                    
                    if (selectedOptionObj == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                                                   "I didn't receive a valid selection. Please try again.",
                                                                          cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    conversationData.SelectedOption = selectedOptionObj;

                    return await stepContext.BeginDialogAsync(nameof(ProcessQueryDialog), conversationData, cancellationToken);
                }

                // If no response received, prompt the user again
                await stepContext.Context.SendActivityAsync("Please select your query.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleFinalOptionsAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }
    }
}
