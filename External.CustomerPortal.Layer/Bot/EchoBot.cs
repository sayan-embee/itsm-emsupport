using Azure.AI.OpenAI;
using BotDialog.Layer.Dialogs;
using Common.Layer.Models;
using Common.Layer.Models.AdaptiveCard;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.Bot;
using Common.Layer.Models.WebChatBot;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;
using External.CustomerPortal.Layer.Services.AdaptiveCards;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace External.CustomerPortal.Layer.Bot;

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0010
#pragma warning disable CS8604
#pragma warning disable SKEXP0001

public class EchoBot : TeamsActivityHandler
{
    private readonly IMemoryCache _memoryCache;

    private readonly IBotFrameworkHttpAdapter _adapter;
    private readonly IBotConversationHandler _botConversationHandler;
    private readonly ConversationState _conversationState;
    private readonly BotState _userState;
    private readonly DialogSet _dialogs;
    private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;

    private readonly AppSettingsModel _appSettings;
    private readonly ICommonData _commonData;
    private readonly ICustomerPortalData _customerPortalData;
    private readonly IAdaptiveCardService _adaptiveCardService;

    private readonly IChatCompletionService _chatCompletionService;
    private readonly Kernel _kernel;

    private readonly IConfiguration _configuration;

    private readonly string serviceEndpoint;
    private readonly string indexName;
    private readonly string key;
    private readonly string openAIApiKey;
    private readonly string openAIEndpoint;
    private readonly string deploymentName;
    private readonly int maxTokens;
    private readonly string instruction;
    private readonly string apologyMessage;
    private readonly int promptMaxLength;
    private readonly string maxLengthExceedsMessage;

    public EchoBot(IBotFrameworkHttpAdapter adapter,
                   ConversationState conversationState,
                   UserState userState,
                   ICommonData commonData,
                   ICustomerPortalData customerPortalData,
                   IBotConversationHandler botConversationHandler,
                   IAdaptiveCardService adaptiveCardService,
                   IOptions<AppSettingsModel> appSettings,
                   Kernel kernel,
                   IMemoryCache memoryCache,
                   IConfiguration configuration)
    {
        _adapter = adapter;
        _conversationState = conversationState;
        _userState = userState;
        _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
        _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        _botConversationHandler = botConversationHandler ?? throw new ArgumentNullException(nameof(botConversationHandler));
        _adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));
        _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

        _chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
        _kernel = kernel;

        // Ensure dialog state is persisted
        _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
        _dialogs = new DialogSet(_dialogStateAccessor);
        _dialogs.Add(new SupportDialog(_commonData, _customerPortalData, _botConversationHandler, _conversationState));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _configuration = configuration;

        // Azure AI Search Keys
        serviceEndpoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
        indexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
        key = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");

        // OpenAI Keys
        openAIApiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");
        openAIEndpoint = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
        deploymentName = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");
        maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
        instruction = _configuration.GetValue<string>("AzureOpenAI:Instruction");
        apologyMessage = _configuration.GetValue<string>("AzureOpenAI:ApologyMessage");
        promptMaxLength = _configuration.GetValue<int>("AzureOpenAI:PromptMaxLength");
        maxLengthExceedsMessage = _configuration.GetValue<string>("AzureOpenAI:MaxLengthExceedsMessage");
    }


    protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            ExceptionLogging.WriteMessageToText($"OnMembersAddedAsync() Started at {DateTime.Now}");


            if (_appSettings.ExternalBot.SendDefaultReply && !string.IsNullOrEmpty(_appSettings.ExternalBot?.DefaultReplyMessage))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(_appSettings.ExternalBot.DefaultReplyMessage), cancellationToken);
            }
            else
            {
                var activity = turnContext.Activity;

                if (_appSettings.ExternalBot.SendWelcomeCard &&
                    !string.IsNullOrEmpty(_appSettings.WelcomeCard?.ShortDesc) &&
                    !string.IsNullOrEmpty(_appSettings.WelcomeCard?.LongDesc) &&
                    !string.IsNullOrEmpty(_appSettings.WelcomeCard?.ImageUrl) &&
                    !string.IsNullOrEmpty(_appSettings.AppDomainUrl))
                {
                    var welcomeCard_Obj = new WelcomeCardModel
                    {
                        ShortDesc = _appSettings.WelcomeCard.ShortDesc,
                        ImageUrl = $"{_appSettings.AppDomainUrl}/{_appSettings.WelcomeCard.ImageUrl}",
                        LongDesc = _appSettings.WelcomeCard.LongDesc
                    };

                    var cardAttachment = _adaptiveCardService.CreateCard_WelcomeMessage_PersonalScope(welcomeCard_Obj);
                    if (cardAttachment != null)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                    }
                }

                var conversationReference = turnContext.Activity.GetConversationReference();

                var userId = turnContext.Activity.From.Id;
                var userName = turnContext.Activity.From?.Name;

                var newConversation = new WebChatConversationModel
                {
                    StartDateTime = DateTime.UtcNow,
                    User = new UserModel
                    {
                        UserId = userId,
                        UserName = userName
                    },
                    ConversationId = conversationReference.Conversation.Id,
                    ServiceUrl = conversationReference.ServiceUrl,
                    LastActivityTime = DateTime.UtcNow,
                    ConversationReference = conversationReference
                };

                var userMessageModel = new WebChatUserMessageModel();

                userMessageModel.Name = userName;
                userMessageModel.Email = "";

                userMessageModel.ChannelId = turnContext.Activity.ChannelId;
                userMessageModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                userMessageModel.ConversationId = turnContext.Activity.Conversation.Id;
                userMessageModel.TenantId = turnContext.Activity.Conversation.TenantId;
                userMessageModel.ChatId = turnContext.Activity.Id;
                userMessageModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                userMessageModel.Text = "";
                userMessageModel.TextFormat = "";

                userMessageModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                userMessageModel.Timestamp = turnContext.Activity.Timestamp;
                userMessageModel.Locale = "";

                newConversation.UserMessage = userMessageModel;


                // Store in memory cache
                StoreConversationInCache(userId, newConversation);

                var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{userId}");
                await conversationStateAccessor.SetAsync(turnContext, newConversation, cancellationToken);
                await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);

                var completionEvent = new Activity
                {
                    Type = ActivityTypes.Event,
                    Name = "onboardingComplete",
                    Value = new { message = "OnMembersAddedAsync completed" }
                };
                await turnContext.SendActivityAsync(completionEvent, cancellationToken);
            }

            ExceptionLogging.WriteMessageToText($"OnMembersAddedAsync() Ended at {DateTime.Now}");
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at OnMembersAddedAsync() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }

    protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            ExceptionLogging.WriteMessageToText($"OnEventActivityAsync() Started at {DateTime.Now}");

            if (turnContext.Activity.Name == "webchat/join" && turnContext.Activity.Value is JObject eventData)
            {
                var conversationType = eventData["conversationType"]?.ToString();

                var webChatLogId = eventData["webChatLogId"]?.ToString();

                var userId = eventData["userId"]?.ToString();
                var userName = eventData["userName"]?.ToString();
                var userEmail = eventData["userEmail"]?.ToString();
                var selectedCategory = eventData["selectedCategory"]?.ToObject<CategoryModel>();
                var selectedSubCategory = eventData["selectedSubCategory"]?.ToObject<SubCategoryModel>();
                var categoryList = eventData["categoryList"]?.ToObject<List<CategoryModel>>();
                var subCategoryList = eventData["subCategoryList"]?.ToObject<List<SubCategoryModel>>();

                var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{userId}");
                var conversationData = await conversationStateAccessor.GetAsync(turnContext, () => null, cancellationToken);

                var conversationReference = turnContext.Activity.GetConversationReference();

                if (conversationData != null && conversationData.User.UserId == userId)
                {
                    if (int.TryParse(webChatLogId, out int logId))
                    {
                        conversationData.WebChatLogId = logId;
                    }

                    conversationData.ConversationType = conversationType;

                    conversationData.User.UserEmail = userEmail;

                    conversationData.SelectedCategory = selectedCategory;
                    conversationData.SelectedSubCategory = selectedSubCategory;

                    conversationData.CategoryList = categoryList;
                    conversationData.SubCategoryList = subCategoryList;

                    // Update existing conversation
                    conversationData.LastActivityTime = DateTime.UtcNow;
                    conversationData.ConversationReference = conversationReference;


                    var userMessageModel = new WebChatUserMessageModel();

                    userMessageModel.Name = userName;
                    userMessageModel.Email = userEmail;

                    userMessageModel.ChannelId = turnContext.Activity.ChannelId;
                    userMessageModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                    userMessageModel.ConversationId = turnContext.Activity.Conversation.Id;
                    userMessageModel.TenantId = turnContext.Activity.Conversation.TenantId;
                    userMessageModel.ChatId = turnContext.Activity.Id;
                    userMessageModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                    userMessageModel.Text = "";
                    userMessageModel.TextFormat = "";

                    userMessageModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                    userMessageModel.Timestamp = turnContext.Activity.Timestamp;
                    userMessageModel.Locale = "";

                    conversationData.UserMessage = userMessageModel;


                    // Store in memory cache
                    StoreConversationInCache(userId, conversationData);

                    await conversationStateAccessor.SetAsync(turnContext, conversationData, cancellationToken);
                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                }
                else if (conversationData == null)
                {
                    var newConversation = new WebChatConversationModel
                    {
                        WebChatLogId = int.Parse(webChatLogId),
                        StartDateTime = DateTime.UtcNow,
                        User = new UserModel
                        {
                            UserId = userId,
                            UserName = userName,
                            UserEmail = userEmail
                        },
                        ConversationType = conversationType,
                        SelectedCategory = selectedCategory,
                        SelectedSubCategory = selectedSubCategory,
                        CategoryList = categoryList,
                        SubCategoryList = subCategoryList,

                        ConversationId = conversationReference.Conversation.Id,
                        ServiceUrl = conversationReference.ServiceUrl,
                        LastActivityTime = DateTime.UtcNow,
                        ConversationReference = conversationReference
                    };


                    var userMessageModel = new WebChatUserMessageModel();

                    userMessageModel.Name = userName;
                    userMessageModel.Email = "";

                    userMessageModel.ChannelId = turnContext.Activity.ChannelId;
                    userMessageModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                    userMessageModel.ConversationId = turnContext.Activity.Conversation.Id;
                    userMessageModel.TenantId = turnContext.Activity.Conversation.TenantId;
                    userMessageModel.ChatId = turnContext.Activity.Id;
                    userMessageModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                    userMessageModel.Text = "";
                    userMessageModel.TextFormat = "";

                    userMessageModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                    userMessageModel.Timestamp = turnContext.Activity.Timestamp;
                    userMessageModel.Locale = "";

                    newConversation.UserMessage = userMessageModel;


                    // Store in memory cache
                    StoreConversationInCache(userId, conversationData);

                    await conversationStateAccessor.SetAsync(turnContext, newConversation, cancellationToken);
                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);

                    conversationData = await conversationStateAccessor.GetAsync(turnContext, () => null, cancellationToken);
                }

                try
                {
                    var webchatModel = new WebChatLogModel
                    {
                        StartedOn = conversationData.StartDateTime,
                        UserEmail = userEmail,
                    };
                    _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);
                }
                catch (Exception ex)
                {
                    ExceptionLogging.WriteMessageToText($"Error in DirectLineToken_InsertUpdate: {ex.Message}");
                }

                if (conversationType == "SOP")
                {
                    // Start the dialog
                    var dialogStateAccessor = _conversationState.CreateProperty<DialogState>("DialogState");
                    var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

                    if (dialogContext.ActiveDialog != null)
                    {
                        try
                        {
                            await dialogContext.ContinueDialogAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await dialogContext.BeginDialogAsync(nameof(SupportDialog), conversationData, cancellationToken);
                        }
                    }
                    else
                    {
                        await dialogContext.BeginDialogAsync(nameof(SupportDialog), conversationData, cancellationToken);
                        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);


                        ExceptionLogging.WriteMessageToText($"OnEventActivityAsync() Ended at {DateTime.Now}");
                        return;
                    }
                }
                else if (conversationType == "FreshService")
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Please continue with your FreshService ticket query."), cancellationToken);
                }
                else if (conversationType == "PublicSite")
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Please continue with your query."), cancellationToken);
                }
                else if (conversationType == "Miscellaneous")
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hi! {userName}, How can I assist you today?"), cancellationToken);

                    if (conversationData.WebChatOptions == null)
                    {
                        var categoryIdList = new List<string>();
                        var subcategoryIdList = new List<string>();

                        if (conversationData.SelectedCategory != null 
                            && conversationData.SelectedSubCategory != null)
                        {
                            categoryIdList.Add(conversationData.SelectedCategory.Id.ToString());
                            subcategoryIdList.Add(conversationData.SelectedSubCategory.Id.ToString());
                        }
                        else if(conversationData.CategoryList != null
                            && conversationData.CategoryList.Count > 0
                            && conversationData.SubCategoryList != null
                            && conversationData.SubCategoryList.Count > 0)
                        {
                            foreach (var item in conversationData.CategoryList)
                            {
                                categoryIdList.Add(item.Id.ToString());
                            }

                            foreach (var item in conversationData.SubCategoryList)
                            {
                                subcategoryIdList.Add(item.Id.ToString());
                            }
                        }

                        if (categoryIdList.Count > 0 && subcategoryIdList.Count > 0)
                        {
                            string categoryIds = string.Join(",", categoryIdList);
                            string subcategoryIds = string.Join(",", subcategoryIdList);

                            var options = await this._customerPortalData.WebChatOptions_Get(categoryIds, subcategoryIds, 5);
                            if (options != null && options.Count > 0)
                            {
                                conversationData.WebChatOptions = options;
                            }
                        }

                        // Store in memory cache
                        StoreConversationInCache(userId, conversationData);

                        await conversationStateAccessor.SetAsync(turnContext, conversationData, cancellationToken);
                        await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                    }

                    if (conversationData.WebChatOptions != null
                        && conversationData.WebChatOptions.Count > 0)
                    {
                        var options = conversationData.WebChatOptions?.Select(c => new AdaptiveCardModel(c.OptionId, c.Option)).ToList();

                        var adaptiveCardAttachment = this._adaptiveCardService.CreateCard_WebChatOptions_PersonalScope(options);
                        if (adaptiveCardAttachment != null)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                        }
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
                }
            }
            else if (turnContext.Activity.Name == "webchat/end" && turnContext.Activity.Value is JObject eventData2)
            {
                var conversationType = eventData2["conversationType"]?.ToString();

                var webChatLogId = eventData2["webChatLogId"]?.ToString();

                var userId = eventData2["userId"]?.ToString();
                var userName = eventData2["userName"]?.ToString();
                var userEmail = eventData2["userEmail"]?.ToString();

                var webchatModel = new WebChatLogModel
                {
                    EndedOn = DateTime.UtcNow,
                    UserEmail = userEmail,
                    Active = false,
                    SessionCloseRemarks = "Manual-End"
                };

                _ = this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "U", dataModel: webchatModel);

                // Remove from memory cache
                RemoveConversationFromCache(userId);

                var completionEvent = new Activity
                {
                    Type = ActivityTypes.Event,
                    Name = "endChat",
                    Value = new { message = "Chat Session Ended" }
                };

                await turnContext.SendActivityAsync(completionEvent, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at OnEventActivityAsync() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            ExceptionLogging.WriteMessageToText($"OnMessageActivityAsync() Started at {DateTime.Now}");

            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext.Activity.Value is JObject value && value.ContainsKey("feedback"))
            {
                await UserFeedbackHandler(turnContext, cancellationToken);

                //if (dialogContext.ActiveDialog != null)
                //{
                //    await dialogContext.ContinueDialogAsync(cancellationToken);
                //}
            }
            else if (_appSettings.ExternalBot.SendDefaultReply && !string.IsNullOrEmpty(_appSettings.ExternalBot?.DefaultReplyMessage))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(_appSettings.ExternalBot.DefaultReplyMessage), cancellationToken);
            }
            else
            {
                await this.OnMessageActivityHandler(turnContext, cancellationToken);
            }


            ExceptionLogging.WriteMessageToText($"OnMessageActivityAsync() Ended at {DateTime.Now}");
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at OnMessageActivityAsync() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }


    #region Personal Conversation

    public async Task OnMessageActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Typing indicator
                await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                await Task.Delay(500, cancellationToken);

                var userId = turnContext.Activity.From.Id;

                var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{userId}");
                var conversationData = await conversationStateAccessor.GetAsync(turnContext, () => null, cancellationToken);

                if (conversationData != null)
                {
                    // Update activity
                    conversationData.ConversationId = turnContext.Activity.Conversation.Id;
                    conversationData.ServiceUrl = turnContext.Activity.ServiceUrl;
                    conversationData.LastActivityTime = DateTime.UtcNow;


                    var userMessageModel = new WebChatUserMessageModel();

                    userMessageModel.Name = conversationData.User.UserName;
                    userMessageModel.Email = conversationData.User.UserEmail;

                    userMessageModel.ChannelId = turnContext.Activity.ChannelId;
                    userMessageModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                    userMessageModel.ConversationId = turnContext.Activity.Conversation.Id;
                    userMessageModel.TenantId = turnContext.Activity.Conversation.TenantId;
                    userMessageModel.ChatId = turnContext.Activity.Id;
                    userMessageModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                    userMessageModel.Text = turnContext.Activity.Text;
                    userMessageModel.TextFormat = turnContext.Activity.TextFormat;

                    userMessageModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                    userMessageModel.Timestamp = turnContext.Activity.Timestamp;
                    userMessageModel.Locale = turnContext.Activity.Locale;

                    conversationData.UserMessage = userMessageModel;

                    // Store in memory cache
                    StoreConversationInCache(userId, conversationData);

                    await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                    await _userState.SaveChangesAsync(turnContext, false, cancellationToken);


                    if (conversationData.ConversationType == "SOP")
                    {
                        var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

                        if (dialogContext.ActiveDialog != null)
                        {
                            try
                            {
                                await dialogContext.ContinueDialogAsync(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                await dialogContext.BeginDialogAsync(nameof(SupportDialog), conversationData, cancellationToken);
                            }
                        }
                        else
                        {
                            await dialogContext.BeginDialogAsync(nameof(SupportDialog), conversationData, cancellationToken);
                        }
                    }
                    else if (conversationData.ConversationType == "FreshService")
                    {
                        await FreshServiceActivityHandler(turnContext, cancellationToken);
                    }
                    else if (conversationData.ConversationType == "PublicSite")
                    {
                        await PublicSiteActivityHandler(turnContext, cancellationToken);
                    }
                    else if (conversationData.ConversationType == "Miscellaneous")
                    {
                        await MiscellaneousActivityHandler(turnContext, cancellationToken);
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
                    }

                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at OnMessageActivityHandler() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }

    public async Task FreshServiceActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {

                // Typing indicator
                await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                await Task.Delay(500, cancellationToken);

                await turnContext.SendActivityAsync(MessageFactory.Text("Handling FreshService conversation..."), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at FreshServiceActivityHandler() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }

    public async Task PublicSiteActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {

                // Typing indicator
                await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                await Task.Delay(500, cancellationToken);

                await turnContext.SendActivityAsync(MessageFactory.Text("Handling PublicSite conversation..."), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at PublicSiteActivityHandler() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }

    #region Bakup
    /*
    public async Task MiscellaneousActivityHandler_Bak(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {

                // Typing indicator
                // await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                _ = this.SendTypingIndicatorAsync(turnContext, cancellationToken);
                await Task.Delay(500, cancellationToken);

                var userId = turnContext.Activity.From.Id;

                var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{userId}");
                var conversationData = await conversationStateAccessor.GetAsync(turnContext, () => null, cancellationToken);

                var userContext = new KernelUserContext
                {
                    UserId = conversationData.User?.UserId,
                    UserEmail = conversationData.User?.UserEmail,
                    UserName = conversationData.User?.UserName,
                };

                string messageText = turnContext.Activity.RemoveRecipientMention()?.Trim();

                if (!string.IsNullOrEmpty(messageText) && !await HandleUserGreetingAsync(turnContext, cancellationToken))
                {
                    return;
                }

                //// Enable planning
                //OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                //{
                //    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                //    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                //    // AzureChatExtensionsOptions = chatExtensionsOptions,
                //    ExtensionData = new Dictionary<string, object>
                //    {
                //        { "UserContext", userContext }
                //    }
                //};

                //// Create a history store the conversation
                //var history = new ChatHistory();
                //history.AddUserMessage(messageText);

                //// Get the response from the AI
                ////var result = await _kernel.InvokePromptAsync(messageText, openAIPromptExecutionSettings);
                //var result = await _chatCompletionService.GetChatMessageContentAsync(
                //   history,
                //   executionSettings: openAIPromptExecutionSettings,
                //   kernel: _kernel
                //   );

                //var outPutResultContent = result.Content;

                //// Add the message from the agent to the chat history
                //history.AddAssistantMessage(outPutResultContent);

                #region For Testing

                var outPutResultContent = @"To set up FortiClient VPN, you can follow these general steps which might vary slightly depending on your operating system:

                ### For Windows:

                1. **Download FortiClient**:
                   - Go to the [Fortinet support site](https://www.fortiguard.com/) or [FortiClient download page](https://www.fortiguard.com/products/forticlient).
                   - Select the appropriate version for your operating system and download the installer.

                2. **Install FortiClient**:
                   - Locate the downloaded file and run the installation.
                   - Follow the on-screen instructions to finish the installation.

                3. **Configure VPN**:
                   - Open FortiClient.
                   - Switch to the ""VPN"" section.
                   - Click on ""+ Add a new connection.""
                   - Fill in the necessary fields:
                     - **Connection Name**: Choose a name for the connection.
                     - **Remote Gateway**: Enter the IP address or hostname of the VPN server.
                     - **Customize other settings** (if required): Authentication method, port, etc.

                4. **Connect to VPN**:
                   - Select the configured VPN connection.
                   - Enter your username and password.
                   - Click ""Connect.""

                5. **Verify Connection**:
                   - Once connected, verify your VPN status within FortiClient or check your IP address to confirm you are using the VPN.

                ### For macOS:

                1. **Download FortiClient**:
                   - Visit the [FortiClient download page](https://www.fortiguard.com/products/forticlient) and download the macOS version.

                2. **Install FortiClient**:
                   - Open the downloaded `.dmg` file and drag FortiClient to the Applications folder.

                3. **Configure VPN**:
                   - Open FortiClient from your Applications folder.
                   - Go to the ""VPN"" section.
                   - Click on ""+ Add a new connection.""
                   - Fill in the connection details as per your VPN server settings.

                4. **Connect to VPN**:
                   - Choose the configured connection.
                   - Enter your credentials and click on ""Connect.""

                ### For Mobile Devices (iOS/Android):

                1. **Download FortiClient App**:
                   - Search for ""FortiClient"" in your device's app store (App Store for iOS or Google Play Store for Android).
                   - Download and install the app.

                2. **Configure VPN**:
                   - Open the app and go to the VPN section.
                   - Add a new VPN connection with the necessary details such as the server address and your credentials.

                3. **Connect to VPN**:
                   - Select the VPN you configured and connect.

                Always refer to your organization's IT support for specific configuration details unique to your VPN setup.";

                #endregion

                try
                {
                    var returnModel = new WebChatUserMessageModel();

                    returnModel.Name = conversationData.User.UserName;
                    returnModel.Email = conversationData.User.UserEmail;

                    returnModel.ChannelId = turnContext.Activity.ChannelId;
                    returnModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                    returnModel.ConversationId = turnContext.Activity.Conversation.Id;
                    returnModel.TenantId = turnContext.Activity.Conversation.TenantId;
                    returnModel.ChatId = turnContext.Activity.Id;
                    returnModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                    returnModel.Text = turnContext.Activity.Text;
                    returnModel.TextFormat = turnContext.Activity.TextFormat;

                    returnModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                    returnModel.Timestamp = turnContext.Activity.Timestamp;
                    returnModel.Locale = turnContext.Activity.Locale;

                    returnModel.Response = outPutResultContent;
                    //returnModel.Intent = result.Intent;

                    //if (result.Citations != null && result.Citations.Any())
                    //{
                    //    var fileList = new List<FileDetailsModel>();
                    //    foreach (var file in result.Citations)
                    //    {
                    //        fileList.Add(new FileDetailsModel()
                    //        {
                    //            FileName = file.FilePath ?? null,
                    //            FileURL = file.Url?.AbsoluteUri.ToString() ?? null,
                    //            FileContent = file.Content
                    //        });
                    //    }

                    //    returnModel.FileList = fileList;
                    //}

                    ResourceResponse botResponseActivity = null;

                    var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(returnModel);
                    if (cardAttachment != null)
                    {
                        botResponseActivity = await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                    }
                    else
                    {
                        botResponseActivity = await turnContext.SendActivityAsync(MessageFactory.Text(outPutResultContent), cancellationToken);
                    }

                    returnModel.WebChatLogId = conversationData.WebChatLogId;
                    returnModel.MessageActivityId = botResponseActivity?.Id ?? null;
                    returnModel.MessageSentUTC = DateTime.UtcNow;

                    var adaptiveCardAttachment = this._adaptiveCardService.CreateCard_LikeDislike_PersonalScope(returnModel);
                    if (adaptiveCardAttachment != null)
                    {
                        var adaptiveCardActivity = await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                        returnModel.FeedbackCardActivityId = adaptiveCardActivity?.Id ?? null;
                        returnModel.FeedbackCardSentUTC = DateTime.UtcNow;
                    }

                    await _customerPortalData.UserConversationLog_InsertUpdate(transactionType: "I", data: returnModel);
                }
                catch (Exception ex)
                {
                    ExceptionLogging.WriteMessageToText($"UserConversationLog_InsertUpdate failed: {ex.Message}");
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at MiscellaneousActivityHandler() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please restart the conversation."), cancellationToken);
        }
    }
    */
    #endregion

    public async Task MiscellaneousActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var userId = turnContext.Activity.From.Id;

                var conversationStateAccessor = _conversationState.CreateProperty<WebChatConversationModel>($"conversation_{userId}");
                var conversationData = await conversationStateAccessor.GetAsync(turnContext, () => null, cancellationToken);

                string messageText = turnContext.Activity.RemoveRecipientMention()?.Trim();

                List<Task> tskList = new List<Task>();
                List<string> tskResponse = new List<string>();
                List<string> indexNameList = new List<string>();

                int categoryId = 0;
                int subCategoryId = 0;

                if (turnContext.Activity.Value is JObject value && value.ContainsKey("action"))
                {
                    string actionType = value["action"]?.ToString();
                    if (actionType == "optionSubmit" && value.ContainsKey("optionId"))
                    {
                        // Typing indicator
                        // await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                        _ = this.SendTypingIndicatorAsync(turnContext, cancellationToken);

                        string selectedOption = value["optionId"]?.ToString();
                        if (!string.IsNullOrEmpty(selectedOption))
                        {
                            var selectedWebChatOption = conversationData.WebChatOptions?.FirstOrDefault(c => c.OptionId == int.Parse(selectedOption));
                            messageText = selectedWebChatOption.Option;

                            var selectedSubCategory = conversationData.SubCategoryList?.FirstOrDefault(c => c.Id == selectedWebChatOption.SubCategoryId);
                            indexNameList.Add(selectedSubCategory.IndexName);

                            categoryId = selectedWebChatOption.CategoryId;
                            subCategoryId = selectedWebChatOption.SubCategoryId;

                            await turnContext.SendActivityAsync(MessageFactory.Text(messageText), cancellationToken);
                        }
                    }
                    else if (actionType == "endChatAction" && value.ContainsKey("userId"))
                    {
                        string response_UserId = value["userId"]?.ToString();

                        if (userId == response_UserId
                            && conversationData != null)
                        {

                            try
                            {
                                //var endChatEvent = new Activity
                                //{
                                //    Type = ActivityTypes.Event,
                                //    Name = "webchat/end",
                                //    Value = new
                                //    {
                                //        conversationType = conversationData.ConversationType,
                                //        webChatLogId = conversationData.WebChatLogId.ToString(),
                                //        userId = userId,
                                //        userName = conversationData.User?.UserName,
                                //        userEmail = conversationData.User?.UserEmail
                                //    }
                                //};

                                //await turnContext.SendActivityAsync(endChatEvent, cancellationToken);

                                var completionEvent = new Activity
                                {
                                    Type = ActivityTypes.Event,
                                    Name = "endChat",
                                    Value = new { message = "Chat Session Ended" }
                                };

                                await turnContext.SendActivityAsync(completionEvent, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogging.WriteMessageToText($"Error Sending Event Activity webchat/end: {ex.Message}");
                            }

                            return;
                        }
                    }


                    else
                    {
                        // Typing indicator
                        // await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                        _ = this.SendTypingIndicatorAsync(turnContext, cancellationToken);

                        if (string.IsNullOrEmpty(messageText))
                        {
                            return;
                        }
                        else if (await this.GetTextLength(messageText) > promptMaxLength)
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text(maxLengthExceedsMessage.Replace("{promptMaxLength}", promptMaxLength.ToString())), cancellationToken);
                            return;
                        }

                        if (!await HandleUserGreetingAsync(turnContext, cancellationToken))
                        {
                            return;
                        }
                    }

                    if (indexNameList.Count == 0
                        && conversationData.SubCategoryList != null
                        && conversationData.SubCategoryList.Count > 0)
                    {
                        foreach (var item in conversationData.SubCategoryList)
                        {
                            indexNameList.Add(item.IndexName);
                        }
                    }

                    foreach (var item in indexNameList)
                    {
                        tskList.Add(Task.Run(async () => tskResponse.Add(await Call_Agent(messageText, item, item, ""))));
                    }

                    var response = string.Empty;

                    if (tskList.Any())
                    {
                        // Typing indicator
                        // await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);
                        _ = this.SendTypingIndicatorAsync(turnContext, cancellationToken);

                        await Task.Delay(100);

                        Task.WaitAll(tskList.ToArray());

                        string pattern = @"\[doc\d+\]";
                        string replacement = "";

                        string pattern2 = @"^#{1,6}";
                        string replacement2 = "######";

                        for (var i = 0; i < tskResponse.Count; i++)
                        {
                            if (!tskResponse[i].ToLower().Trim().Contains("Please try another query or topic.".ToLower().Trim()))
                            {
                                response = Regex.Replace(tskResponse[i], pattern, replacement);

                                try
                                {
                                    response = Regex.Replace(response, pattern2, replacement2, RegexOptions.Multiline);
                                }
                                catch (Exception ex)
                                {

                                }

                                break;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(response))
                    {
                        try
                        {
                            var returnModel = new WebChatUserMessageModel();

                            returnModel.Name = conversationData.User.UserName;
                            returnModel.Email = conversationData.User.UserEmail;

                            returnModel.ChannelId = turnContext.Activity.ChannelId;
                            returnModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                            returnModel.ConversationId = turnContext.Activity.Conversation.Id;
                            returnModel.TenantId = turnContext.Activity.Conversation.TenantId;
                            returnModel.ChatId = turnContext.Activity.Id;
                            returnModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                            returnModel.Text = messageText;
                            returnModel.TextFormat = turnContext.Activity.TextFormat ?? "plain";

                            returnModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
                            returnModel.Timestamp = turnContext.Activity.Timestamp;
                            returnModel.Locale = turnContext.Activity.Locale;

                            returnModel.Response = response;
                            //returnModel.Intent = result.Intent;

                            //if (result.Citations != null && result.Citations.Any())
                            //{
                            //    var fileList = new List<FileDetailsModel>();
                            //    foreach (var file in result.Citations)
                            //    {
                            //        fileList.Add(new FileDetailsModel()
                            //        {
                            //            FileName = file.FilePath ?? null,
                            //            FileURL = file.Url?.AbsoluteUri.ToString() ?? null,
                            //            FileContent = file.Content
                            //        });
                            //    }

                            //    returnModel.FileList = fileList;
                            //}

                            if (categoryId > 0)
                            {
                                returnModel.CategoryId = categoryId;
                            }
                            else if (conversationData.SelectedCategory != null)
                            {
                                returnModel.CategoryId = conversationData.SelectedCategory.Id;
                            }

                            if (subCategoryId > 0)
                            {
                                returnModel.SubCategoryId = subCategoryId;
                            }
                            else if (conversationData.SelectedSubCategory != null)
                            {
                                returnModel.SubCategoryId = conversationData.SelectedSubCategory.Id;
                            }

                            ResourceResponse botResponseActivity = null;

                            //var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(returnModel);
                            //if (cardAttachment != null)
                            //{
                            //    botResponseActivity = await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                            //}
                            //else
                            //{
                            //    botResponseActivity = await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);
                            //}

                            botResponseActivity = await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);

                            returnModel.WebChatLogId = conversationData.WebChatLogId;
                            returnModel.MessageActivityId = botResponseActivity?.Id ?? null;
                            returnModel.MessageSentUTC = DateTime.UtcNow;

                            var adaptiveCardAttachment = this._adaptiveCardService.CreateCard_LikeDislike_PersonalScope(returnModel);
                            if (adaptiveCardAttachment != null)
                            {
                                var adaptiveCardActivity = await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                                returnModel.FeedbackCardActivityId = adaptiveCardActivity?.Id ?? null;
                                returnModel.FeedbackCardSentUTC = DateTime.UtcNow;
                            }

                            await _customerPortalData.UserConversationLog_InsertUpdate(transactionType: "I", data: returnModel);
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging.WriteMessageToText($"UserConversationLog_InsertUpdate failed: {ex.Message}");
                            ExceptionLogging.SendErrorToText(ex);

                            await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
                        }
                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text(apologyMessage), cancellationToken);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error at MiscellaneousActivityHandler() - {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
        }
    }

    public async Task UserFeedbackHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        try
        {
            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);

            if (turnContext == null) throw new ArgumentNullException(nameof(turnContext));

            // Extract feedback data from the submitted Adaptive Card
            var value = turnContext.Activity.Value as JObject;
            if (value == null)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Invalid feedback submission."), cancellationToken);
                return;
            }

            string feedback = value["feedback"]?.ToString();
            string webChatLogId = value["webChatLogId"]?.ToString();
            string messageId = value["messageId"]?.ToString();

            if (string.IsNullOrEmpty(feedback) || string.IsNullOrEmpty(webChatLogId) || string.IsNullOrEmpty(messageId))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Feedback submission is missing required fields."), cancellationToken);
                return;
            }

            var conversationLog = new WebChatUserMessageModel
            {
                WebChatLogId = int.Parse(webChatLogId),
                MessageActivityId = messageId,
                LikeDislike = feedback.ToLower() == "like" ? true : false,
                FeedbackReceivedUTC = DateTime.UtcNow
            };

            try
            {
                var result = await _customerPortalData.UserConversationLog_InsertUpdate(transactionType: "U", data: conversationLog);
                if (result != null && int.TryParse(result.Id, out int outputId) && outputId > 0) 
                {
                    var adaptiveCardAttachment = this._adaptiveCardService.CreateCard_LikeDislike_Response_PersonalScope(result.Message,feedback);
                    if (adaptiveCardAttachment != null)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"CreateCard_LikeDislike_Response_PersonalScope failed: {ex.Message}");
                await turnContext.SendActivityAsync(MessageFactory.Text("Thank you for your feedback."), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in HandleFeedback(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            await turnContext.SendActivityAsync(MessageFactory.Text("Something went wrong while processing feedback."), cancellationToken);
        }
    }


    #endregion






    #region Private Methods

    private async Task<int> GetTextLength(string input)
    {
        await Task.Delay(0);
        return input.Length;
    }

    /// <summary>
    /// Sends a typing indicator periodically while the bot processes a long-running task.
    /// </summary>
    private async Task SendTypingIndicatorAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        try
        {
            // Send typing indicators every 3 seconds while the task is running
            while (!cancellationToken.IsCancellationRequested)
            {
                // Ensure the turn context is still valid
                if (turnContext.Activity != null)
                {
                    await turnContext.SendActivityAsync(new Activity
                    {
                        Type = ActivityTypes.Typing
                    }, cancellationToken);
                }

                // Wait for 3 seconds before sending the next typing indicator
                await Task.Delay(3000, cancellationToken);
            }
        }
        catch (TaskCanceledException)
        {
            // Task was canceled, safe to ignore
        }
        catch (ObjectDisposedException)
        {
            // The context was disposed, stop typing
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in SendTypingIndicatorAsync() - {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if the user input is a greeting or not
    /// </summary>
    /// <param name="turnContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns true if it is not a greeting else false</returns>
    private async Task<bool> HandleUserGreetingAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        string userName = turnContext.Activity.From?.Name ?? "there"; // Fallback if username is not available
        string userMessage = turnContext.Activity.Text.Trim().ToLower();

        // Define a regular expression for common greetings
        var greetingPattern = @"\b(hi+|hello+|hey+|yo+|hlw+|good (morning|afternoon|evening|night)|namaste|what's up|howdy|greetings)\b";
        var okayPattern = @"\b(o+k+|okay+|alright|fine|sure)\b";
        var thankYouPattern = @"\b(thanks?|thank\s?you+|thx|ty|many\s?thanks|much\s?appreciated|cheers)\b";
        var casualPositivePattern = @"\b(good+|very\s?good+|nice+|hm+|h\s?m+|hmm+|awesome|great|cool|fine|amazing|lovely)\b";
        var howAreYouPattern = @"\b(how\s?(are|r)\s?(you|u)\??|how's\s?(it|everything|life)\s?(going|been)?\??)\b";

        // Get the current time to personalize greetings
        string botResponse;

        if (Regex.IsMatch(userMessage, greetingPattern, RegexOptions.IgnoreCase))
        {
            var match = Regex.Match(userMessage, greetingPattern, RegexOptions.IgnoreCase);
            string matchedGreeting = match.Value.ToLower();

            if (matchedGreeting.Contains("good morning"))
            {
                botResponse = $"🌞 Good morning, {userName}! Hope your day is off to a great start!";
            }
            else if (matchedGreeting.Contains("good afternoon"))
            {
                botResponse = $"☀️ Good afternoon, {userName}! How's your day going?";
            }
            else if (matchedGreeting.Contains("good evening"))
            {
                botResponse = $"🌆 Good evening, {userName}! How was your day?";
            }
            else if (matchedGreeting.Contains("good night"))
            {
                botResponse = $"🌙 Good night, {userName}! have a restful night!";
            }
            else if (matchedGreeting.Contains("what's up") || matchedGreeting.Contains("hey"))
            {
                botResponse = $"👋 Hey, {userName}! What's on your mind?";
            }
            else
            {
                // Respond with the user's greeting style
                botResponse = $"{char.ToUpper(matchedGreeting[0]) + matchedGreeting.Substring(1)}, {userName}! 🙂";
            }

            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
            return false;
        }
        else if (Regex.IsMatch(userMessage, howAreYouPattern, RegexOptions.IgnoreCase))
        {
            botResponse = $"😊 I'm doing great, {userName}! Thanks for asking. How about you?";
            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
            return false;
        }
        else if (Regex.IsMatch(userMessage, okayPattern, RegexOptions.IgnoreCase))
        {
            botResponse = "👍 Alright, let me know if there's anything else I can assist you with!";
            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
            return false;
        }
        else if (Regex.IsMatch(userMessage, thankYouPattern, RegexOptions.IgnoreCase))
        {
            botResponse = "🤝 You're very welcome! Let me know if there's anything else I can assist you with.";
            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
            return false;
        }
        else if (Regex.IsMatch(userMessage, casualPositivePattern, RegexOptions.IgnoreCase))
        {
            botResponse = "Happy to help! Let me know if there's anything else I can assist you with.";
            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
            return false;
        }
        else
        {
            // Fallback response for unrecognized messages
            //string fallbackResponse = $"🤔 I'm not sure how to respond to that, {userName}. Can you tell me more?";
            //await turnContext.SendActivityAsync(MessageFactory.Text(fallbackResponse), cancellationToken);
            return true;
        }
    }


    #endregion


    #region Cache Methods

    public void StoreConversationInCache(string userId, WebChatConversationModel conversation)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30));

        _memoryCache.Set(userId, conversation, cacheOptions);

        // Track keys separately (store all userIds)
        if (!_memoryCache.TryGetValue("CacheKeys", out HashSet<string> keys))
        {
            keys = new HashSet<string>();
        }

        keys.Add(userId);

        _memoryCache.Set("CacheKeys", keys);
    }

    public WebChatConversationModel GetConversationFromCache(string userId)
    {
        _memoryCache.TryGetValue(userId, out WebChatConversationModel conversation);
        return conversation;
    }

    public void RemoveConversationFromCache(string userId)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            _memoryCache.Remove(userId);
        }
    }

    #endregion


    #region AI Methods

    private ChatCompletionAgent Get_Agent(string IndexName, string AgentName, string instruction)
    {
        try
        {
            var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
            {
                SearchEndpoint = new Uri(serviceEndpoint),

                Authentication = new OnYourDataApiKeyAuthenticationOptions(key),

                IndexName = IndexName
            };

            var chatExtensionsOptions = new AzureChatExtensionsOptions { Extensions = { azureSearchExtensionConfiguration } };

            var executionSettings = new OpenAIPromptExecutionSettings { MaxTokens = maxTokens, AzureChatExtensionsOptions = chatExtensionsOptions };

            var agentSOP = new ChatCompletionAgent
            {
                Name = AgentName,

                Kernel = _kernel,

                Instructions = instruction,

                Arguments = new KernelArguments(executionSettings),
            };

            return agentSOP;
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in Call_Agent(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);

            throw ex;
        }
    }

    private async Task<string> Call_Agent(string messageText, string IndexName, string AgentName, string instruction)
    {
        StringBuilder response = new StringBuilder();

        try
        {
            var history = new ChatHistory();

            history.AddUserMessage(messageText);

            var MSHelpAgent = Get_Agent(IndexName, AgentName, instruction);

            await foreach (ChatMessageContent msgResponse in MSHelpAgent.InvokeAsync(history))
            {
                foreach (var content in msgResponse.Content ?? "")
                {
                    response.Append(content);
                }
            }
        }
        catch (Exception ex)
        {
            response.Clear();
            //response.Append(ex.ToString());

            ExceptionLogging.WriteMessageToText($"Error in Call_Agent(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
        }

        return response.ToString();
    }

    #endregion
}