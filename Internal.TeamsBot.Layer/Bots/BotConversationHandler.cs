using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
using Internal.TeamsBot.Layer.ExceptionLog;
using Internal.TeamsBot.Layer.Services.AdaptiveCards;
using Internal.TeamsBot.Layer.Services.Notification;
using Internal.TeamsBot.Layer.Services.SearchService;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Azure.AI.OpenAI;
using Microsoft.Graph.Models.CallRecords;
using Microsoft.SemanticKernel.Agents;
using System.Text;
using YamlDotNet.Core.Tokens;
using System.Globalization;
using Internal.TeamsBot.Layer.Plugins;
using static Internal.TeamsBot.Layer.Plugins.SOPSearchPlugin;

namespace Internal.TeamsBot.Layer.Bots
{

#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0010
#pragma warning disable CS8604
#pragma warning disable SKEXP0001

    public class BotConversationHandler : IBotConversationHandler
    {
        private readonly ILogger<BotConversationHandler> _logger;
        private readonly IAdaptiveCardService _adaptiveCardService;
        private readonly INotificationService _notificationService;
        private readonly IAISearch _aISearch;
        private readonly ICommonData _commonData;
        private readonly IConfiguration _configuration;

        private readonly SOPSearchPlugin _sopSearchPlugin;

        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;

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

        public BotConversationHandler(
           ILogger<BotConversationHandler> logger
            , IAdaptiveCardService adaptiveCardService
            , INotificationService notificationService
            , IAISearch aISearch
            , ICommonData commonData
            , Kernel kernel
            , IConfiguration configuration
            , SOPSearchPlugin sopSearchPlugin)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));
            this._notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            this._aISearch = aISearch ?? throw new ArgumentNullException(nameof(logger));
            this._commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            this._sopSearchPlugin = sopSearchPlugin ?? throw new ArgumentNullException(nameof(sopSearchPlugin));

           // _chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
            _kernel = kernel;

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Azure AI Search Keys
            serviceEndpoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
            indexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
            key = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");

            // OpenAI Keys
            openAIApiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");
            openAIEndpoint = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
            deploymentName = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");
            maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
            //instruction = _configuration.GetValue<string>("AzureOpenAI:Instruction");
            apologyMessage = _configuration.GetValue<string>("AzureOpenAI:ApologyMessage");
            promptMaxLength = _configuration.GetValue<int>("AzureOpenAI:PromptMaxLength");
            maxLengthExceedsMessage = _configuration.GetValue<string>("AzureOpenAI:MaxLengthExceedsMessage");
            instruction = _configuration.GetValue<string>("AzureOpenAI:InstructionMessage");
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


        #region Personal Conversation - Kernel Plugins

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

        //            var userInput = turnContext.Activity.RemoveRecipientMention()?.Trim();

        //            string messageText = await this.NormalizeText(userInput);

        //            if (string.IsNullOrEmpty(messageText))
        //            {
        //                return;
        //            }
        //            else if(await this.GetTextLength(messageText) > promptMaxLength)
        //            {
        //                await turnContext.SendActivityAsync(MessageFactory.Text(maxLengthExceedsMessage.Replace("{promptMaxLength}", promptMaxLength.ToString())), cancellationToken);
        //                return;
        //            }

        //            // Enable planning
        //            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        //            {
        //                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        //                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        //                // AzureChatExtensionsOptions = chatExtensionsOptions,
        //                MaxTokens = maxTokens,
        //                ChatSystemPrompt = $"Avoid topics related to politics, celebrities, sports & religion."
        //            };

        //            // Create a history store the conversation
        //            var history = new ChatHistory();

        //            history.AddUserMessage(messageText);


        //            // Get the response from the AI
        //            var result = await _chatCompletionService.GetChatMessageContentAsync(
        //               history,
        //               executionSettings: openAIPromptExecutionSettings,
        //               kernel: _kernel
        //               );

        //            var response = string.Empty;
        //            var returnModel = new UserMessageModel();

        //            if (result == null
        //                || result.Content == null
        //                || result.Content.ToLower().Trim().Contains("please try another query or topic.".ToLower().Trim())
        //                || result.Content.ToLower().Trim().Contains("couldn't find specific information".ToLower().Trim())
        //                || result.Content.ToLower().Trim().Contains("wasn't able to find specific information".ToLower().Trim())
        //                || result.Content.ToLower().Trim().Contains("cannot provide information".ToLower().Trim()))
        //            {
        //                returnModel.QuerySucceed = false;
        //                response = apologyMessage;
        //            }
        //            else
        //            {
        //                returnModel.QuerySucceed = true;
        //                response = result.Content;
        //            }

        //            // Add the message from the agent to the chat history
        //            history.AddAssistantMessage(response);

        //            if (response != null)
        //            {

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
        //                returnModel.Text = messageText;
        //                returnModel.TextFormat = turnContext.Activity.TextFormat;

        //                returnModel.LocalTimestamp = turnContext.Activity.LocalTimestamp;
        //                returnModel.Timestamp = turnContext.Activity.Timestamp;
        //                returnModel.Locale = turnContext.Activity.Locale;

        //                returnModel.Response = response;
        //                //returnModel.Intent = result.Intent;

        //                //if (result.Citations != null && result.Citations.Any())
        //                //{
        //                //    var fileList = new List<FileDetailsModel>();
        //                //    foreach (var file in result.Citations)
        //                //    {
        //                //        fileList.Add(new FileDetailsModel()
        //                //        {
        //                //            FileName = file.FilePath ?? null,
        //                //            FileURL = file.Url?.AbsoluteUri.ToString() ?? null,
        //                //            FileContent = file.Content
        //                //        });
        //                //    }

        //                //    returnModel.FileList = fileList;
        //                //}

        //                // Send card using turnContext
        //                await turnContext.SendActivityAsync(MessageFactory.Text(response), cancellationToken);

        //                //var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(returnModel);
        //                //if (cardAttachment != null)
        //                //{
        //                //    // Send card using BOT client
        //                //    if (!string.IsNullOrEmpty(returnModel.ADID))
        //                //    {
        //                //        var sentResponse = await _notificationService.SendCard_PersonalScope(returnModel.ADID, cardAttachment, returnModel.MessageId);
        //                //    }
        //                //}
        //                //else
        //                //{
        //                //    await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
        //                //}

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
        //        await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
        //        return;
        //    }
        //}
        //public async Task OnMessageActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        if (turnContext.Activity.Type == ActivityTypes.Message
        //            && turnContext.Activity.From.AadObjectId != null
        //            && !string.IsNullOrEmpty(turnContext.Activity.Text))
        //        {
        //            if (!await HandleUserGreetingAsync(turnContext, cancellationToken))
        //                return;

        //            var userInput = turnContext.Activity.RemoveRecipientMention()?.Trim();
        //            string messageText = await this.NormalizeText(userInput);

        //            if (string.IsNullOrEmpty(messageText)) return;

        //            if (await this.GetTextLength(messageText) > promptMaxLength)
        //            {
        //                await turnContext.SendActivityAsync(
        //                    MessageFactory.Text(maxLengthExceedsMessage.Replace("{promptMaxLength}", promptMaxLength.ToString())),
        //                    cancellationToken
        //                );
        //                return;
        //            }


        //            //var sopResults = await _sopSearchPlugin.GetSOPData(_kernel, messageText);
        //            var sopResults = await _sopSearchPlugin.SearchFromSOP(_kernel, messageText);


        //            if (!sopResults.Any())
        //            {
        //                await turnContext.SendActivityAsync(MessageFactory.Text(apologyMessage), cancellationToken);
        //                return;
        //            }


        //            StringBuilder rawContext = new StringBuilder();
        //            var sopResult = sopResults.First();
        //            string responseText = sopResult.Content;


        //            // Normalize strings for loose comparison
        //            // Remove [doc1][doc2] placeholders
        //            // Remove [doc1] [doc2] placeholders
        //            var docPattern = new Regex(@"\[doc\s*\d+\]", RegexOptions.IgnoreCase);
        //            responseText = docPattern.Replace(responseText, string.Empty);

        //            // Utility function to clean filenames
        //            string CleanFileName(string name)
        //            {
        //                if (string.IsNullOrWhiteSpace(name)) return string.Empty;

        //                return name.Replace("[", "")
        //                           .Replace("]", "")
        //                           .Replace("sandbox:/", "")
        //                           .Replace("sandbox\\", "")
        //                           .Trim();
        //            }

        //            string FormatSopResponse(SopResult sopResult)
        //            {
        //                if (sopResult == null || string.IsNullOrWhiteSpace(sopResult.Content))
        //                    return "No response from SOP search.";

        //                string content = sopResult.Content;

        //                // Regex to find "Reference Document" section
        //                var refDocPattern = @"Reference\s*Document\s*(?:\r?\n|\r)([\s\S]*)";
        //                var refDocMatch = Regex.Match(content, refDocPattern, RegexOptions.IgnoreCase);

        //                if (!refDocMatch.Success)
        //                {
        //                    // If no reference section, just return content
        //                    return content.Trim();
        //                }

        //                // Extract the reference document block
        //                string refDocBlock = refDocMatch.Groups[1].Value.Trim();

        //                // Capture each line as a filename
        //                var matches = Regex.Matches(refDocBlock, @"([^\r\n]+)");

        //                var refSection = new StringBuilder();
        //                refSection.AppendLine("**Reference Document**");

        //                var addedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //                foreach (Match match in matches)
        //                {
        //                    string rawFileName = match.Groups[1].Value.Trim();
        //                    string cleanedFileName = CleanFileName(rawFileName);

        //                    if (string.IsNullOrWhiteSpace(cleanedFileName))
        //                        continue;

        //                    // Skip duplicates
        //                    if (!addedFiles.Add(cleanedFileName))
        //                        continue;

        //                    // Try to match with Citations list
        //                    var citation = sopResult.Citations?.FirstOrDefault(c =>
        //                        string.Equals(CleanFileName(c.Title), cleanedFileName, StringComparison.OrdinalIgnoreCase));

        //                    if (citation != null && !string.IsNullOrWhiteSpace(citation.Url))
        //                    {
        //                        // Show clickable filename with URL
        //                        refSection.AppendLine($"- [{citation.Title}]({citation.Url})");
        //                    }
        //                    else
        //                    {
        //                        // Fallback: show plain filename
        //                        refSection.AppendLine($"- {cleanedFileName}");
        //                    }
        //                }

        //                // Replace original reference section with formatted one
        //                string formattedResponse = Regex.Replace(
        //                    content,
        //                    refDocPattern,
        //                    refSection.ToString().Trim(),
        //                    RegexOptions.IgnoreCase
        //                );

        //                return formattedResponse.Trim();
        //            }





        //            if (responseText.Length > 4000)
        //            {
        //                responseText = responseText.Substring(0, 3900) +
        //                               "...\n\n⚠️ Response truncated. Please open the reference document(s) for full details.";
        //            }


        //           // string instructionMessage = _configuration.GetValue<string>("AzureOpenAI:InstructionMessage");

        //            //OpenAIPromptExecutionSettings settings = new()
        //            //{
        //            //    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        //            //    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        //            //    MaxTokens = maxTokens,
        //            //    ChatSystemPrompt = instructionMessage
        //            //};

        //            //var history = new ChatHistory();
        //            //history.AddUserMessage(messageText);
        //            //history.AddSystemMessage("Use the following SOP content strictly to create the answer:");
        //            //history.AddSystemMessage(rawContext.ToString());

        //            //var result = await _chatCompletionService.GetChatMessageContentAsync(
        //            //    history,
        //            //    executionSettings: settings,
        //            //    kernel: _kernel
        //            //);

        //            //string response = result?.Content ?? apologyMessage;


        //            //if (response.Length > 4000)
        //            //{
        //            //    response = response.Substring(0, 3900) +
        //            //               "...\n\n⚠️ Response truncated. Please open the reference document(s) for full details.";
        //            //}

        //            //await turnContext.SendActivityAsync(MessageFactory.Text(sopResults[0].Content), cancellationToken);
        //            await turnContext.SendActivityAsync(MessageFactory.Text(responseText), cancellationToken);

        //            var returnModel = new UserMessageModel
        //            {
        //                ADID = turnContext.Activity.From.AadObjectId,
        //                ChannelId = turnContext.Activity.ChannelId,
        //                ConversationType = turnContext.Activity.Conversation.ConversationType,
        //                ConversationId = turnContext.Activity.Conversation.Id,
        //                TenantId = turnContext.Activity.Conversation.TenantId,
        //                ChatId = turnContext.Activity.Id,
        //                ServiceUrl = turnContext.Activity.ServiceUrl,
        //                Text = messageText,
        //                TextFormat = turnContext.Activity.TextFormat,
        //                LocalTimestamp = turnContext.Activity.LocalTimestamp,
        //                Timestamp = turnContext.Activity.Timestamp,
        //                Locale = turnContext.Activity.Locale,
        //                Response = responseText,
        //                QuerySucceed = true
        //            };

        //            try { 
        //                _ = this._commonData.TeamsBot_UserSearch_InsertUpdate(returnModel);
        //            }
        //            catch (Exception ex) 
        //            { 
        //                ExceptionLogging.SendErrorToText(ex); 
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //        await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Opps!!!! Something went wrong. Please try again."), cancellationToken);
        //    }
        //}

        public async Task OnMessageActivityHandler(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                if (turnContext.Activity.Type == ActivityTypes.Message
                    && turnContext.Activity.From.AadObjectId != null
                    && !string.IsNullOrEmpty(turnContext.Activity.Text))
                {
                    if (!await HandleUserGreetingAsync(turnContext, cancellationToken))
                        return;

                    var userInput = turnContext.Activity.RemoveRecipientMention()?.Trim();
                    string messageText = await this.NormalizeText(userInput);

                    if (string.IsNullOrEmpty(messageText)) return;

                    if (await this.GetTextLength(messageText) > promptMaxLength)
                    {
                        await turnContext.SendActivityAsync(
                            MessageFactory.Text(maxLengthExceedsMessage.Replace("{promptMaxLength}", promptMaxLength.ToString())),
                            cancellationToken
                        );
                        return;
                    }

                    var sopResults = await _sopSearchPlugin.SearchFromSOP(_kernel, messageText);

                    if (!sopResults.Any())
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text(apologyMessage), cancellationToken);
                        return;
                    }

                    var sopResult = sopResults.First();
                    string responseText = sopResult.Content;

                    // 🔹 Step 1: Remove [doc1][doc2] placeholders
                    var docPattern = new Regex(@"\[doc\s*\d+\]", RegexOptions.IgnoreCase);
                    responseText = docPattern.Replace(responseText, string.Empty);

                    // 🔹 Step 2: Remove Reference Document section completely
                    var refDocPattern = new Regex(@"Reference\s*Document[s]?\s*([\s\S]*)", RegexOptions.IgnoreCase);
                    responseText = refDocPattern.Replace(responseText, string.Empty).Trim();

                    // 🔹 Step 3: Append only the first citation (if available)
                    if (sopResult.Citations != null && sopResult.Citations.Any())
                    {
                        var firstCitation = sopResult.Citations.First();
                        if (!string.IsNullOrWhiteSpace(firstCitation.Url))
                        {
                            responseText += $"\n\n**Reference Document**\n- [{firstCitation.Title}]({firstCitation.Url})";
                        }
                        else
                        {
                            responseText += $"\n\n**Reference Document**\n- {firstCitation.Title}";
                        }
                    }

                    // Truncate if too long
                    if (responseText.Length > 4000)
                    {
                        responseText = responseText.Substring(0, 3900) +
                                       "...\n\n⚠️ Response truncated. Please open the reference document(s) for full details.";
                    }

                    await turnContext.SendActivityAsync(MessageFactory.Text(responseText), cancellationToken);

                    var returnModel = new UserMessageModel
                    {
                        ADID = turnContext.Activity.From.AadObjectId,
                        ChannelId = turnContext.Activity.ChannelId,
                        ConversationType = turnContext.Activity.Conversation.ConversationType,
                        ConversationId = turnContext.Activity.Conversation.Id,
                        TenantId = turnContext.Activity.Conversation.TenantId,
                        ChatId = turnContext.Activity.Id,
                        ServiceUrl = turnContext.Activity.ServiceUrl,
                        Text = messageText,
                        TextFormat = turnContext.Activity.TextFormat,
                        LocalTimestamp = turnContext.Activity.LocalTimestamp,
                        Timestamp = turnContext.Activity.Timestamp,
                        Locale = turnContext.Activity.Locale,
                        Response = responseText,
                        QuerySucceed = true
                    };

                    try
                    {
                        _ = this._commonData.TeamsBot_UserSearch_InsertUpdate(returnModel);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Opps!!!! Something went wrong. Please try again."), cancellationToken);
            }
        }






        #endregion


        #region Personal Conversation - AI Agent
        /*
        public async Task OnMessageActivityHandler_Bak(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

                if (turnContext.Activity.Type == ActivityTypes.Message
                            && turnContext.Activity.From.AadObjectId != null
                            && turnContext.Activity.Text != null
                            && turnContext.Activity.Text != "")
                {
                    if (!await HandleUserGreetingAsync(turnContext, cancellationToken))
                    {
                        return;
                    }

                    string messageText = turnContext.Activity.RemoveRecipientMention()?.Trim();

                    List<Task> tskList = new List<Task>();
                    List<string> tskResponse = new List<string>();
                    List<string> indexNameList = new List<string>();

                    if (!string.IsNullOrEmpty(indexName))
                    {
                        indexNameList.Add(indexName);
                    }

                    foreach (var item in indexNameList)
                    {
                        tskList.Add(Task.Run(async () => tskResponse.Add(await Call_Agent(messageText, item, item, ""))));
                    }

                    var response = string.Empty;

                    if (tskList.Any())
                    {
                        await Task.Delay(100);

                        Task.WaitAll(tskList.ToArray());

                        string pattern = @"\[doc\d+\]";
                        string replacement = "";

                        string pattern2 = @"^(#+)\s(Step \d+:)";
                        string replacement2 = "###### $2"; // Replace heading with H6 while keeping "Step X:"

                        for (var i = 0; i < tskResponse.Count; i++)
                        {
                            if (!tskResponse[i].ToLower().Trim().Contains("Please try another query or topic.".ToLower().Trim()))
                            {
                                response = Regex.Replace(tskResponse[i], pattern, replacement);

                                try
                                {
                                    response = Regex.Replace(response, pattern, replacement, RegexOptions.Multiline);
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
                        var returnModel = new UserMessageModel();

                        returnModel.ADID = turnContext.Activity.From.AadObjectId;
                        if (!string.IsNullOrEmpty(returnModel.ADID))
                        {
                            try
                            {
                                var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext?.Activity?.From?.Id, cancellationToken: CancellationToken.None);
                                returnModel.Name = member?.Name ?? null;
                                returnModel.Email = member?.Email ?? null;
                                returnModel.UPN = member?.UserPrincipalName ?? null;
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogging.SendErrorToText(ex);
                            }
                        }

                        returnModel.ChannelId = turnContext.Activity.ChannelId;
                        returnModel.ConversationType = turnContext.Activity.Conversation.ConversationType;
                        returnModel.ConversationId = turnContext.Activity.Conversation.Id;
                        returnModel.TenantId = turnContext.Activity.Conversation.TenantId;
                        returnModel.ChatId = turnContext.Activity.Id;
                        returnModel.ServiceUrl = turnContext.Activity.ServiceUrl;
                        returnModel.Text = messageText;
                        returnModel.TextFormat = turnContext.Activity.TextFormat;

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

                        var cardAttachment = this._adaptiveCardService.CreateCard_UserMessage_PersonalScope(returnModel);
                        if (cardAttachment != null)
                        {
                            // Send card using turnContext
                            // await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);

                            // Send card using BOT client
                            if (!string.IsNullOrEmpty(returnModel.ADID))
                            {
                                var sentResponse = await _notificationService.SendCard_PersonalScope(returnModel.ADID, cardAttachment, returnModel.MessageId);
                            }
                        }
                        else
                        {
                            await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
                        }

                        // save response in db
                        try
                        {
                            _ = this._commonData.TeamsBot_UserSearch_InsertUpdate(returnModel);
                            //if (dbResult != null && dbResult.Status == 1)
                            //{
                            //    returnModel.MessageId = int.Parse(dbResult.Id);
                            //}
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogging.SendErrorToText(ex);
                        }

                    }
                    else
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Text("Sorry I am not able to find the answer. Please try a different query or topic."), cancellationToken);
                    }

                    return;
                }
                else if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                await turnContext.SendActivityAsync(MessageFactory.Text("⚠️ Something went wrong. Please try again."), cancellationToken);
                return;
            }
        }
        */
        #endregion


        #region Private Methods

        private async Task<string> NormalizeText(string input)
        {
            var cleanedInput = string.Concat(input.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

            // Remove newlines, tabs, and extra spaces
            cleanedInput = Regex.Replace(cleanedInput, @"\s+", " ").Trim();

            await Task.Delay(0);
            return cleanedInput;
        }

        private async Task<int> GetTextLength(string input)
        {
            await Task.Delay(0);
            return input.Length;
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
            var greetingPattern = @"\b(hi+|hello+|hey+|yo+|good (morning|afternoon|evening|night)|namaste|what's up|howdy|greetings)\b";
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

        #region AI Methods
        /*
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
        */
        #endregion
    }
}
