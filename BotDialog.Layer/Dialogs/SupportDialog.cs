using BotDialog.Layer.Dialogs;
using BotDialog.Layer.ExceptionLog;
using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SupportDialog : ComponentDialog
{
    private readonly ICommonData _commonData;
    private readonly ICustomerPortalData _customerPortalData;

    public SupportDialog(ICommonData commonData, ICustomerPortalData customerPortalData) : base(nameof(SupportDialog))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            IntroStepAsync,
            DetermineNextStepAsync
        };

        _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
        _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new CategoryDialog(_commonData, _customerPortalData));  // Add the CategoryDialog
        AddDialog(new SubCategoryDialog(_commonData, _customerPortalData));  // Add the SubCategoryDialog
        AddDialog(new OptionsDialog(_commonData, _customerPortalData));  // Add the OptionsDialog
        AddDialog(new ProcessQueryDialog(_commonData, _customerPortalData));  // Add the ProcessQueryDialog
        AddDialog(new ProcessOtherQueryDialog(_commonData, _customerPortalData));  // Add the ProcessOtherQueryDialog
        AddDialog(new FeedbackDialog(_commonData, _customerPortalData));  // Add the FeedbackDialog
        AddDialog(new SatisfactionDialog(_commonData, _customerPortalData));  // Add the FeedbackDialog
        AddDialog(new EndDialog(_commonData, _customerPortalData));  // Add the FeedbackDialog

        // Set the initial dialog to the waterfall dialog
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var conversationData = stepContext.Options as WebChatConversationModel;

        try
        {
            if (conversationData == null)
            {
                ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in IntroStepAsync.");

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }

            //await stepContext.Context.SendActivityAsync("Chat Started.", cancellationToken: cancellationToken);

            var userName = conversationData?.User?.UserName ?? "there";
            await stepContext.Context.SendActivityAsync($"Hi! {userName}, How can I assist you today?", cancellationToken: cancellationToken);

            return await stepContext.NextAsync(conversationData, cancellationToken);
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at IntroStepAsync() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
            return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        }
    }


    private async Task<DialogTurnResult> DetermineNextStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        var conversationData = stepContext.Options as WebChatConversationModel;

        try
        {
            if (conversationData == null)
            {
                ExceptionLogging.WriteMessageToText("Warning: Conversation data is null in DetermineNextStepAsync.");

                await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
            }

            if (conversationData.SelectedCategory != null && conversationData.SelectedSubCategory != null)
            {
                if (conversationData.WebChatOptions == null)
                {
                    var categoryId = conversationData.SelectedCategory.Id;
                    var subcategoryId = conversationData.SelectedSubCategory.Id;

                    var options = await this._customerPortalData.WebChatOptions_Get(categoryId, subcategoryId, 10);
                    if (options != null && options.Count > 0)
                    {
                        conversationData.WebChatOptions = options;
                    }
                }

                if (conversationData.WebChatOptions != null && conversationData.WebChatOptions.Count > 0)
                {
                    await stepContext.Context.SendActivityAsync("Please select your query.", cancellationToken: cancellationToken);
                    // Start the OptionsDialog
                    return await stepContext.BeginDialogAsync(nameof(OptionsDialog), conversationData, cancellationToken);
                }
                else
                {
                    // Start the ProcessOtherQueryDialog
                    return await stepContext.BeginDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                }
            }
            else
            {
                //await stepContext.Context.SendActivityAsync("Let's start with the Sub-Category Selection.", cancellationToken: cancellationToken);
                // Start SubCategoryDialog
                return await stepContext.BeginDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
            }

            //else if (conversationData.SelectedCategory != null && conversationData.SelectedSubCategory == null)
            //{
            //    await stepContext.Context.SendActivityAsync("Let's start with the Sub-Category Selection.", cancellationToken: cancellationToken);
            //    // Start SubCategoryDialog
            //    return await stepContext.BeginDialogAsync(nameof(SubCategoryDialog), conversationData, cancellationToken);
            //}
            //else
            //{
            //    await stepContext.Context.SendActivityAsync("Let's start with the Category Selection.", cancellationToken: cancellationToken);
            //    // Start the CategoryDialog
            //    return await stepContext.BeginDialogAsync(nameof(CategoryDialog), conversationData, cancellationToken);
            //}
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at DetermineNextStepAsync() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await stepContext.Context.SendActivityAsync($"Something went wrong. Please restart the conversation.", cancellationToken: cancellationToken);
            return await stepContext.ReplaceDialogAsync(nameof(EndDialog), conversationData, cancellationToken);
        }
    }
}