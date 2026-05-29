using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using DataAccess.Layer.Data.Common;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System.Threading;
using Internal.TeamsBot.Layer.ExceptionLog;
using Common.Layer.Models.Enum;
using Internal.TeamsBot.Layer.Bots;
using Common.Layer.Models.AdaptiveCard;
using Internal.TeamsBot.Layer.Services.AdaptiveCards;
using Common.Layer.Models.AppSettings;
using Microsoft.Extensions.Options;
using Common.Layer.Models.Bot;

namespace Internal.TeamsBot.Layer
{
    /// <summary>
    /// An empty bot handler.
    /// You can add your customization code here to extend your bot logic if needed.
    /// </summary>
    public class TeamsBot : TeamsActivityHandler
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IAppLifecycleHandler _appLifecycleHandler;
        private readonly IBotConversationHandler _botConversationHandler;
        private readonly BotState _conversationState;
        private readonly BotState _userState;
        //private readonly DialogSet _dialogs;

        private readonly AppSettingsModel _appSettings;
        private readonly ICommonData _commonData;
        private readonly IAdaptiveCardService _adaptiveCardService;

        public TeamsBot(IBotFrameworkHttpAdapter adapter
            , ConversationState conversationState
            , UserState userState
            , ICommonData commonData
            , IAppLifecycleHandler appLifecycleHandler
            , IAdaptiveCardService adaptiveCardService
            , IBotConversationHandler botConversationHandler
            , IOptions<AppSettingsModel> appSettings
            )
        {
            _adapter = adapter;
            _conversationState = conversationState;
            _userState = userState;
            _commonData = commonData ?? throw new ArgumentNullException(nameof(ICommonData));
            _appLifecycleHandler = appLifecycleHandler ?? throw new ArgumentNullException(nameof(appLifecycleHandler));
            _adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));
            _botConversationHandler = botConversationHandler ?? throw new ArgumentNullException(nameof(botConversationHandler));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
            //_dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>(nameof(DialogState)));
            //_dialogs.Add(new SupportDialog(_commonData));
        }

        /// <summary>
        /// This method is triggered when a message is sent to the bot. While it doesn't specifically indicate that a user has landed on the chat box, it does indicate that the user has interacted with the bot.
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                ExceptionLogging.WriteMessageToText($"OnMessageActivityAsync() Started at {DateTime.Now}");

                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

                // Start sending typing indicator in a separate task
                var typingTask = Task.Run(() => SendTypingIndicatorAsync(turnContext, cancellationToken));

                if (_appSettings.InternalBot.SendDefaultReply
                && !string.IsNullOrEmpty(_appSettings.InternalBot?.DefaultReplyMessage))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(_appSettings.InternalBot.DefaultReplyMessage), cancellationToken);
                }
                else
                {
                    await _botConversationHandler.OnMessageActivityHandler(turnContext, cancellationToken);
                }

                ExceptionLogging.WriteMessageToText($"OnMessageActivityAsync() Ended at {DateTime.Now}");
            }
            catch(Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at OnMessageActivityAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }
        }



        /// <summary>
        /// This method is specifically triggered when one or more members are added to the conversation. It allows you to handle the addition of new members directly, making it ideal for sending welcome messages or performing actions specific to new users joining the chat.
        /// </summary>
        /// <param name="membersAdded"></param>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                ExceptionLogging.WriteMessageToText($"OnMembersAddedAsync() Started at {DateTime.Now}");

                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                var activity = turnContext.Activity;

                switch (activity.Conversation.ConversationType)
                {
                    case ConversationTypes.Personal:

                        //User Install the app
                        if (activity.MembersAdded != null && activity.MembersAdded.Any(member => member.Id == activity.Recipient.Id))
                        {
                            try
                            {
                                await this._appLifecycleHandler.OnBotInstalledInPersonalAsync(turnContext, BotModel.InternalBotName);

                                // Send Adaptive Card
                                try
                                {
                                    //var welcomeText = "Hello and welcome!";
                                    //foreach (var member in membersAdded)
                                    //{
                                    //    if (member.Id != turnContext.Activity.Recipient.Id)
                                    //    {
                                    //        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                                    //    }
                                    //}
                                    if (_appSettings.InternalBot.SendWelcomeCard
                                        && !string.IsNullOrEmpty(_appSettings.WelcomeCard?.ShortDesc)
                                        && !string.IsNullOrEmpty(_appSettings.WelcomeCard?.LongDesc)
                                        && !string.IsNullOrEmpty(_appSettings.WelcomeCard?.ImageUrl)
                                        && !string.IsNullOrEmpty(_appSettings.AppDomainUrl))
                                    {
                                        var welcomeCard_Obj = new WelcomeCardModel
                                        {
                                            ShortDesc = _appSettings.WelcomeCard.ShortDesc,
                                            ImageUrl = $"{_appSettings.AppDomainUrl}/{_appSettings.WelcomeCard?.ImageUrl}",
                                            LongDesc = _appSettings.WelcomeCard.LongDesc
                                        };

                                        if (!string.IsNullOrEmpty(welcomeCard_Obj?.ShortDesc)
                                            && !string.IsNullOrEmpty(welcomeCard_Obj?.LongDesc))
                                        {
                                            var cardAttachment = this._adaptiveCardService.CreateCard_WelcomeMessage_PersonalScope(welcomeCard_Obj);
                                            if (cardAttachment != null)
                                            {
                                                await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ExceptionLogging.WriteMessageToText($"Unable to send welcome card to user - {ex.Message}");
                                    ExceptionLogging.SendErrorToText(ex);
                                }
                            }
                            catch (Exception ex)
                            {
                                await turnContext.SendActivityAsync(MessageFactory.Text($"Something went wrong. Please re-install the application."), cancellationToken);
                                ExceptionLogging.SendErrorToText(ex);
                            }
                        }

                        ExceptionLogging.WriteMessageToText($"OnMembersAddedAsync() Ended at {DateTime.Now}");
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at OnMembersAddedAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            try
            {
                ExceptionLogging.WriteMessageToText($"OnTurnAsync() Started at {DateTime.Now}");

                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                switch (turnContext.Activity.Conversation.ConversationType)
                {
                    case ConversationTypes.Personal:
                        if (turnContext.Activity.Action == "remove"
                            && turnContext.Activity.Type == "installationUpdate"
                            && turnContext.Activity.From.AadObjectId != null)
                        {
                            await this._appLifecycleHandler.OnBotRemovedInPersonalAsync(turnContext, BotModel.InternalBotName);
                            return;
                        }

                        ExceptionLogging.WriteMessageToText($"OnTurnAsync() Ended at {DateTime.Now}");
                        break;

                    default: break;
                }

                ITypingActivity replyActivity = Activity.CreateTypingActivity();
                await turnContext.SendActivityAsync((Activity)replyActivity);

                await base.OnTurnAsync(turnContext, cancellationToken);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at OnTurnAsync() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }
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
        /// This method is triggered when there are changes in the conversation, such as when a new user joins or when the bot is added to a chat. You can override this method to handle such events.
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    foreach (var member in turnContext.Activity.MembersAdded)
        //    {
        //        if (member.Id != turnContext.Activity.Recipient.Id)
        //        {
        //            await turnContext.SendActivityAsync(MessageFactory.Text("Welcome! How can I assist you today?"), cancellationToken);
        //        }
        //    }
        //}
    }
}