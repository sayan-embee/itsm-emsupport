using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using DataAccess.Layer.Data.Common;
using Internal.TeamsBot.Layer.Bots;
using Internal.TeamsBot.Layer.ExceptionLog;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;

namespace Internal.TeamsBot.Layer.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly ConfigOptions _configOptions;
        private readonly ICommonData _commonData;

        public NotificationService(
            ILogger<NotificationService> logger
            , ICommonData commonData
            , IOptions<ConfigOptions> configOptions
            )
        {
            _logger = logger;
            _commonData = commonData ?? throw new ArgumentNullException(nameof(ICommonData));
            _configOptions = configOptions.Value ?? throw new ArgumentNullException(nameof(configOptions));
        }

        public async Task<NotificationResponseModel> SendCard_PersonalScope(string userADID, Attachment cardAttachment, int referenceId)
        {
            try
            {
                if (!string.IsNullOrEmpty(userADID) && cardAttachment != null)
                {
                    Guid Id;
                    if (Guid.TryParse(userADID, out Id))
                    {
                        var conversationDetails = await _commonData.Get_M_ConversationByUserId(Id);

                        if (conversationDetails != null
                            && !string.IsNullOrEmpty(conversationDetails.ServiceUrl)
                            && !string.IsNullOrEmpty(conversationDetails.ConversationId)
                            )
                        {
                            Uri url = new Uri(conversationDetails.ServiceUrl);
                            ConnectorClient connectorClient = new ConnectorClient(url, this._configOptions.BOT_ID, this._configOptions.BOT_PASSWORD);

                            var activity = new Activity()
                            {
                                Type = ActivityTypes.Message,
                                Conversation = new ConversationAccount()
                                {
                                    Id = conversationDetails.ConversationId
                                },
                                Attachments = new List<Attachment>()
                                {
                                    cardAttachment
                                }
                            };
                            var result = await connectorClient.Conversations.SendToConversationAsync(activity);
                            if (result != null)
                            {
                                var returnObj = new NotificationResponseModel();
                                returnObj.ReplyToId = result.Id;
                                //returnObj.ActivityId = conversationDetails.ActivityId;
                                returnObj.ActivityId = result.Id;
                                returnObj.ConversationId = conversationDetails.ConversationId;
                                returnObj.ServiceUrl = conversationDetails.ServiceUrl;
                                returnObj.UserName = conversationDetails.UserName;
                                returnObj.UserADID = conversationDetails.UserId.ToString();
                                returnObj.Status = true;
                                returnObj.MessageId = referenceId;
                                return returnObj;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationService --> SendCard_PersonalScope() execution failed for UserADID: {userADID}");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

    }
}
