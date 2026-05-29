using BotDialog.Layer.Dialogs;
using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.Bot;
using External.CustomerPortal.Layer.ExceptionLog;
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
    private readonly IBotConversationHandler _botConversationHandler;

    private readonly ConversationState _conversationState;

    public SupportDialog(
        ICommonData commonData, 
        ICustomerPortalData customerPortalData, 
        IBotConversationHandler botConversationHandler,
        ConversationState conversationState) : base(nameof(SupportDialog))
    {
        var waterfallSteps = new WaterfallStep[]
        {
            IntroStepAsync,
            DetermineNextStepAsync
        };

        _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
        _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        _botConversationHandler = botConversationHandler ?? throw new ArgumentNullException(nameof(botConversationHandler));
        _conversationState = conversationState;

        AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
        AddDialog(new CategoryDialog(_commonData, _customerPortalData, _conversationState));  // Add the CategoryDialog
        AddDialog(new SubCategoryDialog(_commonData, _customerPortalData, _conversationState));  // Add the SubCategoryDialog
        AddDialog(new OptionsDialog(_commonData, _customerPortalData, _conversationState));  // Add the OptionsDialog
        AddDialog(new ProcessQueryDialog(_commonData, _customerPortalData, _botConversationHandler, _conversationState));  // Add the ProcessQueryDialog
        AddDialog(new ProcessOtherQueryDialog(_commonData, _customerPortalData, _botConversationHandler, _conversationState));  // Add the ProcessOtherQueryDialog
        AddDialog(new FeedbackDialog(_commonData, _customerPortalData, _conversationState));  // Add the FeedbackDialog
        AddDialog(new SatisfactionDialog(_commonData, _customerPortalData, _conversationState));  // Add the FeedbackDialog
        AddDialog(new EndDialog(_commonData, _customerPortalData, _conversationState));  // Add the FeedbackDialog

        // Set the initial dialog to the waterfall dialog
        InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
        //var conversationData = stepContext.Options as WebChatConversationModel;

        var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
        var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

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


            await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
            await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);

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
        // var conversationData = stepContext.Options as WebChatConversationModel;

        var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{stepContext.Context.Activity.From.Id}");
        var conversationData = await conversationStateAccessor.GetAsync(stepContext.Context, () => new WebChatConversationModel(), cancellationToken);

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
                    //var categoryId = conversationData.SelectedCategory.Id;
                    //var subcategoryId = conversationData.SelectedSubCategory.Id;

                    //var options = await this._customerPortalData.WebChatOptions_Get(categoryId, subcategoryId, 10);
                    //if (options != null && options.Count > 0)
                    //{
                    //    conversationData.WebChatOptions = options;
                    //}
                }

                if (conversationData.WebChatOptions != null && conversationData.WebChatOptions.Count > 0)
                {
                    await stepContext.Context.SendActivityAsync("Please select your query.", cancellationToken: cancellationToken);


                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    // Start the OptionsDialog
                    return await stepContext.BeginDialogAsync(nameof(OptionsDialog), conversationData, cancellationToken);
                }
                else
                {
                    await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


                    // Start the ProcessOtherQueryDialog
                    return await stepContext.BeginDialogAsync(nameof(ProcessOtherQueryDialog), conversationData, cancellationToken);
                }
            }
            else
            {
                //await stepContext.Context.SendActivityAsync("Let's start with the Sub-Category Selection.", cancellationToken: cancellationToken);

                await conversationStateAccessor.SetAsync(stepContext.Context, conversationData, cancellationToken);
                await _conversationState.SaveChangesAsync(stepContext.Context, false, cancellationToken);


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