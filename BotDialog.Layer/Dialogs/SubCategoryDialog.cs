using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;
using System.Linq;
using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using BotDialog.Layer.Services;
using BotDialog.Layer.ExceptionLog;
using DataAccess.Layer.Data.CustomerPortal;

namespace BotDialog.Layer.Dialogs
{
    public class SubCategoryDialog : ComponentDialog
    {
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;

        public SubCategoryDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(SubCategoryDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            ShowSubCategoriesAsync,
            HandleSubCategorySelectionAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        private async Task<DialogTurnResult> ShowSubCategoriesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in ShowSubCategoriesAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                //if (conversationData.SelectedCategory == null)
                //{
                //    await stepContext.Context.SendActivityAsync("Please select a category.", cancellationToken: cancellationToken);

                //    // Replace the current SubCategoryDialog with the CategoryDialog
                //    return await stepContext.ReplaceDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
                //}

                //var subCategoryMasterList = await this._commonData.GetSubCategoryMaster(selectedCategoryCode);
                var subCategoryList = conversationData.SubCategoryList?
                    //.Where(sc => sc.CategoryId == conversationData.SelectedCategory.Id)
                    .Select(sc => new AdaptiveCardModel(sc.Id, sc.SubCategoryName, ""))
                    .ToList();

                int subCategoryCount = subCategoryList?.Count ?? 0;
                if (subCategoryCount == 0)
                {
                    await stepContext.Context.SendActivityAsync("You are not associated with any active contract. Please contact the administrator.", cancellationToken: cancellationToken);
                    // Replace the current SubCategoryDialog with the CategoryDialog
                    // return await stepContext.ReplaceDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                // Add an go-back option
                // subCategoryList?.Add(new AdaptiveCardModel(-1, "Go Back", ""));

                // Add an exit option
                subCategoryList?.Add(new AdaptiveCardModel(0, "End Chat", ""));

                // Generate the adaptive card for subcategories
                var adaptiveCardService = new AdaptiveCardService();
                var adaptiveCardAttachment = adaptiveCardService.GenerateSubCategoryAdaptiveCard(subCategoryList);

                // Send the adaptive card
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);

                // Wait for user response
                return Dialog.EndOfTurn;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at ShowSubCategoriesAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleSubCategorySelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var conversationData = stepContext.Options as WebChatConversationModel;

            try
            {
                if (conversationData == null)
                {
                    ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in HandleSubCategorySelectionAsync.");

                    await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
                }

                var activity = stepContext.Context.Activity;
                if (activity.Value != null)
                {
                    // Extract the user's selection
                    var userResponse = activity.Value as JObject;
                    var selectedSubCategoryId = userResponse?["subCategoryId"]?.ToString();

                    if (selectedSubCategoryId == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    if (!int.TryParse(selectedSubCategoryId, out int subcategoryId))
                    {
                        await stepContext.Context.SendActivityAsync(
                            "I didn't receive a valid selection. Please try again.",
                            cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    // Handle the "Exit" case
                    if (subcategoryId == 0)
                    {
                        await stepContext.Context.SendActivityAsync(
                            "Thank you for your inquiry! If you need further assistance, feel free to ask.",
                            cancellationToken: cancellationToken);

                        // Begin the FeedbackDialog
                        return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), conversationData, cancellationToken);
                    }

                    //if (subcategoryId == -1)
                    //{
                    //    await stepContext.Context.SendActivityAsync(
                    //        "Please select a category.",
                    //        cancellationToken: cancellationToken);

                    //    // Begin the CategoryDialog
                    //    return await stepContext.ReplaceDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
                    //}


                    var selectedSubCategoryObj = conversationData.SubCategoryList?.FirstOrDefault(c => c.Id == subcategoryId);
                    if (selectedSubCategoryObj == null)
                    {
                        await stepContext.Context.SendActivityAsync(
                                                   "I didn't receive a valid selection. Please try again.",
                                                                          cancellationToken: cancellationToken);
                        return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
                    }

                    conversationData.SelectedSubCategory = selectedSubCategoryObj;
                    conversationData.SelectedCategory = conversationData.CategoryList?.FirstOrDefault(c => c.Id == selectedSubCategoryObj.CategoryId);

                    if (conversationData.WebChatOptions == null)
                    {
                        var categoryId = conversationData.SelectedCategory.Id;
                        var options = await this._customerPortalData.WebChatOptions_Get(categoryId, subcategoryId, 10);
                        if (options != null && options.Count > 0)
                        {
                            conversationData.WebChatOptions = options;
                        }
                    }

                    if (conversationData.WebChatOptions != null && conversationData.WebChatOptions.Count > 0)
                    {
                        //await stepContext.Context.SendActivityAsync(
                        //$"You have selected: {selectedSubCategoryObj.SubCategoryName}. Please select your query.",
                        //cancellationToken: cancellationToken);

                        return await stepContext.BeginDialogAsync(nameof(OptionsDialog), conversationData, cancellationToken);
                    }
                    else
                    {
                        return await stepContext.BeginDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                    }
                }

                // If no response received, prompt the user again
                // await stepContext.Context.SendActivityAsync("Please select a sub-category.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(InitialDialogId, conversationData, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at HandleSubCategorySelectionAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }
        }
    }
}
