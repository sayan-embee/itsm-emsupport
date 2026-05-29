using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;
using External.CustomerPortal.Layer.Services.AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;
using Common.Layer.Models.WebChatBot;
using Common.Layer.Models.AppSettings;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace External.CustomerPortal.Layer.Bot
{
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0010
#pragma warning disable CS8604
#pragma warning disable SKEXP0001

    public class BotConversationHandler : IBotConversationHandler
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly BotState _conversationState;

        private readonly AppSettingsModel _appSettings;
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;
        private readonly IAdaptiveCardService _adaptiveCardService;

        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;

        private readonly IConfiguration _configuration;
        private readonly int maxTokens;

        public BotConversationHandler(
                    IBotFrameworkHttpAdapter adapter,
                   ConversationState conversationState,
                   UserState userState,
                   ICommonData commonData,
                   ICustomerPortalData customerPortalData,
                   IAdaptiveCardService adaptiveCardService,
                   IOptions<AppSettingsModel> appSettings,
                   Kernel kernel,
                   IMemoryCache memoryCache,
                   IConfiguration configuration)
        {
            _adapter = adapter;
            _conversationState = conversationState;
            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
            _adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

            _chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
            _kernel = kernel;

            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
        }

        #region Personal Conversation_Backup

        // Azure.AI.OpenAI 1.0.0-beta.9

        //public async Task OnMessageActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

        //        if (turnContext.Activity.Type == ActivityTypes.Message
        //                    && turnContext.Activity.From.AadObjectId != null
        //                    && turnContext.Activity.Text != null
        //                    && turnContext.Activity.Text != "")
        //        {
        //            if (!await HandleUserGreetingAsync(turnContext, cancellationToken))
        //            {
        //                return;
        //            }

        //            var result = await _aISearch.ChatCompletionResult(turnContext.Activity.Text);

        //            if (result != null && result.Content != "")
        //            {
        //                var returnModel = new UserMessageModel();

        //                returnModel.ADID = turnContext.Activity.From.AadObjectId;
        //                if (!string.IsNullOrEmpty(returnModel.ADID))
        //                {
        //                    try
        //                    {
        //                        var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext?.Activity?.From?.Id, cancellationToken: CancellationToken.None);
        //                        returnModel.Name = member?.Name ?? null;
        //                        returnModel.Email = member?.Email ?? null;
        //                        returnModel.UPN = member?.UserPrincipalName ?? null;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ExceptionLogging.SendErrorToText(ex);
        //                    }
        //                }

        //                returnModel.ChannelId = turnContext.Activity.ChannelId;
        //                returnModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
        //                returnModel.ConversationId = turnContext.Activity.Conversation.Id;
        //                returnModel.TenantId = turnContext.Activity.Conversation.TenantId;
        //                returnModel.ChatId = turnContext.Activity.Id;
        //                returnModel.ServiceUrl = turnContext.Activity.ServiceUrl;
        //                returnModel.Text = turnContext.Activity.Text;
        //                returnModel.TextFormat = turnContext.Activity.TextFormat;

        //                returnModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
        //                returnModel.Timestamp = turnContext.Activity.Timestamp;
        //                returnModel.Locale = turnContext.Activity.Locale;

        //                returnModel.Response = result.Content;
        //                returnModel.Intent = result.Intent;

        //                if (result.Citations != null && result.Citations.Any())
        //                {
        //                    var fileList = new List<FileDetailsModel>();
        //                    foreach (var file in result.Citations)
        //                    {
        //                        fileList.Add(new FileDetailsModel()
        //                        {
        //                            FileName = file.FilePath ?? null,
        //                            FileURL = file.Url?.AbsoluteUri.ToString() ?? null,
        //                            FileContent = file.Content
        //                        });
        //                    }

        //                    returnModel.FileList = fileList;
        //                }

        //                var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(returnModel);
        //                if (cardAttachment != null)
        //                {
        //                    // Send card using turnContext
        //                    // await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);

        //                    // Send card using BOT client
        //                    if (!string.IsNullOrEmpty(returnModel.ADID))
        //                    {
        //                        var sentResponse = await _notificationService.SendCard_PersonalScope(returnModel.ADID, cardAttachment, returnModel.MessageId);
        //                    }
        //                }
        //                else
        //                {
        //                    await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
        //                }

        //                // save response in db
        //                try
        //                {
        //                    _ = this._commonData.TeamsBot_UserSearch_InsertUpdate(returnModel);
        //                    //if (dbResult != null && dbResult.Status == 1)
        //                    //{
        //                    //    returnModel.MessageId = int.Parse(dbResult.Id);
        //                    //}
        //                }
        //                catch (Exception ex)
        //                {
        //                    ExceptionLogging.SendErrorToText(ex);
        //                }

        //            }
        //            else
        //            {
        //                await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
        //            }

        //            return;
        //        }
        //        else if (turnContext.Activity.Type == ActivityTypes.Message)
        //        {
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //        return;
        //    }
        //}

        #endregion


        #region Personal Conversation

        public async Task<bool> OnMessageActivityHandlerForDialogs(WaterfallStepContext stepContext, CancellationToken cancellationToken, WebChatConversationModel conversationData)
        {
            try
            {
                if (conversationData == null || conversationData?.UserMessage == null)
                {
                    return false;
                }

                string messageText = conversationData.UserMessage.Text;

                if (!string.IsNullOrEmpty(messageText))
                {
                    var message = await HandleUserGreetingAsync(conversationData);
                    if (message != "")
                    {
                        return true;
                    }
                }

                var indexName = await GetIndexNameBasedOnSelection(conversationData.SelectedCategory, conversationData.SelectedSubCategory);
                if (string.IsNullOrWhiteSpace(indexName))
                {
                    return false;
                }

                var userContext = new KernelUserContext
                {
                    UserId = conversationData.User?.UserId,
                    UserEmail = conversationData.User?.UserEmail,
                    UserName = conversationData.User?.UserName,
                    IndexName = indexName
                };

                var arguments = new Dictionary<string, object>
                {
                    { "UserContext", System.Text.Json.JsonSerializer.Serialize(userContext) }
                };


                // Enable planning
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    // AzureChatExtensionsOptions = chatExtensionsOptions,
                    ExtensionData = new Dictionary<string, object>
                    {
                        { "UserContext", userContext },
                        { "KernelArguments", arguments }
                    }
                };

                var requestKernel = _kernel.Clone();

                requestKernel.Data.Add("UserContext", userContext);

                // Create a history store the conversation
                var history = new ChatHistory();
                history.AddUserMessage(messageText);

                // Get the response from the AI
                var result = await _chatCompletionService.GetChatMessageContentAsync(
                   history,
                   executionSettings: openAIPromptExecutionSettings,
                   kernel: requestKernel
                   );

                requestKernel.Data.Remove("UserContext");

                var outPutResultContent = result.Content;

                // Add the message from the agent to the chat history
                history.AddAssistantMessage(outPutResultContent);


                // Manual Garbage collection
                requestKernel.Data.Clear();
                requestKernel.Plugins.Clear();
                requestKernel = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Manual Garbage collection Checking
                //WeakReference<Kernel> weakRef = new WeakReference<Kernel>(requestKernel);
                //requestKernel = null;
                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                //if (!weakRef.TryGetTarget(out _))
                //{
                //    Console.WriteLine("✅ requestKernel has been garbage collected!");
                //}
                //else
                //{
                //    Console.WriteLine("⚠️ requestKernel is still in memory. Possible leak!");
                //}


                #region For Testing

                //var outPutResultContent = @"To set up FortiClient VPN, you can follow these general steps which might vary slightly depending on your operating system:

                //### For Windows:

                //1. **Download FortiClient**:
                //   - Go to the [Fortinet support site](https://www.fortiguard.com/) or [FortiClient download page](https://www.fortiguard.com/products/forticlient).
                //   - Select the appropriate version for your operating system and download the installer.

                //2. **Install FortiClient**:
                //   - Locate the downloaded file and run the installation.
                //   - Follow the on-screen instructions to finish the installation.

                //3. **Configure VPN**:
                //   - Open FortiClient.
                //   - Switch to the ""VPN"" section.
                //   - Click on ""+ Add a new connection.""
                //   - Fill in the necessary fields:
                //     - **Connection Name**: Choose a name for the connection.
                //     - **Remote Gateway**: Enter the IP address or hostname of the VPN server.
                //     - **Customize other settings** (if required): Authentication method, port, etc.

                //4. **Connect to VPN**:
                //   - Select the configured VPN connection.
                //   - Enter your username and password.
                //   - Click ""Connect.""

                //5. **Verify Connection**:
                //   - Once connected, verify your VPN status within FortiClient or check your IP address to confirm you are using the VPN.

                //### For macOS:

                //1. **Download FortiClient**:
                //   - Visit the [FortiClient download page](https://www.fortiguard.com/products/forticlient) and download the macOS version.

                //2. **Install FortiClient**:
                //   - Open the downloaded `.dmg` file and drag FortiClient to the Applications folder.

                //3. **Configure VPN**:
                //   - Open FortiClient from your Applications folder.
                //   - Go to the ""VPN"" section.
                //   - Click on ""+ Add a new connection.""
                //   - Fill in the connection details as per your VPN server settings.

                //4. **Connect to VPN**:
                //   - Choose the configured connection.
                //   - Enter your credentials and click on ""Connect.""

                //### For Mobile Devices (iOS/Android):

                //1. **Download FortiClient App**:
                //   - Search for ""FortiClient"" in your device's app store (App Store for iOS or Google Play Store for Android).
                //   - Download and install the app.

                //2. **Configure VPN**:
                //   - Open the app and go to the VPN section.
                //   - Add a new VPN connection with the necessary details such as the server address and your credentials.

                //3. **Connect to VPN**:
                //   - Select the VPN you configured and connect.

                //Always refer to your organization's IT support for specific configuration details unique to your VPN setup.";

                #endregion

                try
                {
                    conversationData.UserMessage.Response = outPutResultContent;
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

                    var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(conversationData.UserMessage);
                    if (cardAttachment != null)
                    {
                        botResponseActivity = await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                    }
                    else
                    {
                        botResponseActivity = await stepContext.Context.SendActivityAsync(outPutResultContent, cancellationToken: cancellationToken);
                    }

                    conversationData.UserMessage.WebChatLogId = conversationData.WebChatLogId;
                    conversationData.UserMessage.MessageActivityId = botResponseActivity?.Id ?? null;
                    conversationData.UserMessage.MessageSentUTC = DateTime.UtcNow;

                    var adaptiveCardAttachment = this._adaptiveCardService.CreateCard_LikeDislike_PersonalScope(conversationData.UserMessage);
                    if (adaptiveCardAttachment != null)
                    {
                        var adaptiveCardActivity = await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(adaptiveCardAttachment), cancellationToken);
                        conversationData.UserMessage.FeedbackCardActivityId = adaptiveCardActivity?.Id ?? null;
                        conversationData.UserMessage.FeedbackCardSentUTC = DateTime.UtcNow;
                    }

                    conversationData.UserMessage.CategoryId = conversationData.SelectedCategory.Id;
                    conversationData.UserMessage.SubCategoryId = conversationData.SelectedSubCategory.Id;

                    await _customerPortalData.UserConversationLog_InsertUpdate(transactionType: "I", data: conversationData.UserMessage);
                }
                catch (Exception ex)
                {
                    ExceptionLogging.WriteMessageToText($"UserConversationLog_InsertUpdate failed: {ex.Message}");
                    ExceptionLogging.SendErrorToText(ex);

                    await stepContext.Context.SendActivityAsync("⚠️ Something went wrong. Please try again.", cancellationToken: cancellationToken);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"OnMessageActivityHandlerForDialogs failed: {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                return false;
            }
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// Checks if the user input is a greeting or not
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if it is not a greeting else false</returns>
        private async Task<string> HandleUserGreetingAsync(WebChatConversationModel conversationData)
        {
            string userName = conversationData.User.UserName;
            string userMessage = conversationData.UserMessage.Text.Trim().ToLower();

            // Define a regular expression for common greetings
            var greetingPattern = @"\b(hi+|hello+|hey+|yo+|good (morning|afternoon|evening|night)|namaste|what's up|howdy|greetings)\b";
            var okayPattern = @"\b(o+k+|okay+|alright|fine|sure)\b";
            var thankYouPattern = @"\b(thanks?|thank\s?you+|thx|ty|many\s?thanks|much\s?appreciated|cheers)\b";
            var casualPositivePattern = @"\b(good+|very\s?good+|nice+|hm+|h\s?m+|hmm+|awesome|great|cool|fine|amazing|lovely)\b";
            var howAreYouPattern = @"\b(how\s?(are|r)\s?(you|u)\??|how's\s?(it|everything|life)\s?(going|been)?\??)\b";


            // Get the current time to personalize greetings
            string botResponse;

            await Task.Delay(500);

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

                return botResponse;
            }
            else if (Regex.IsMatch(userMessage, howAreYouPattern, RegexOptions.IgnoreCase))
            {
                botResponse = $"😊 I'm doing great, {userName}! Thanks for asking. How about you?";
                return botResponse;
            }
            else if (Regex.IsMatch(userMessage, okayPattern, RegexOptions.IgnoreCase))
            {
                botResponse = "👍 Alright, let me know if there's anything else I can assist you with!";
                return botResponse;
            }
            else if (Regex.IsMatch(userMessage, thankYouPattern, RegexOptions.IgnoreCase))
            {
                botResponse = "🤝 You're very welcome! Let me know if there's anything else I can assist you with.";
                return botResponse;
            }
            else if (Regex.IsMatch(userMessage, casualPositivePattern, RegexOptions.IgnoreCase))
            {
                botResponse = "Happy to help! Let me know if there's anything else I can assist you with.";
                return botResponse;
            }
            else
            {
                return "";
            }
        }


        private async Task<string> GetIndexNameBasedOnSelection(CategoryModel category, SubCategoryModel subCategory)
        {
            string indexName = "";

            try
            {
                var result = await _customerPortalData.WebChatSOPIndex_Get(category.Id, subCategory.Id);
                if (result != null)
                {
                    indexName = result.IndexName;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"GetIndexNameBasedOnSelection failed: {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }

            return indexName;
        }

        #endregion
    }
}
