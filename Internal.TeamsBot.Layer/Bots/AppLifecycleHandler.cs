using Common.Layer.Models.Bot;
using Common.Layer.Models.Enum;
using DataAccess.Layer.Data.Common;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Internal.TeamsBot.Layer.Bots
{
    public class AppLifecycleHandler : IAppLifecycleHandler
    {
        private readonly ILogger<AppLifecycleHandler> logger;

        private readonly ICommonData _commonData;

        public AppLifecycleHandler(
           ILogger<AppLifecycleHandler> logger,
           ICommonData commonData)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
        }

        #region Personal Conversation

        public async Task OnBotInstalledInPersonalAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "turnContext cannot be null");

            this.logger.LogInformation($"Bot added in personal scope for user {turnContext.Activity.From.AadObjectId}");

            var activity = turnContext.Activity;

            await InsertUpdateConversation(turnContext, activity, appName, true);

            this.logger.LogInformation($"Successfully installed app for user {activity.From.AadObjectId}.");
        }

        public async Task OnBotRemovedInPersonalAsync(ITurnContext turnContext, string appName)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "turnContext cannot be null");

            this.logger.LogInformation($"Removed added in personal scope for user {turnContext.Activity.From.AadObjectId} for app {appName}");

            var activity = turnContext.Activity;

            await InsertUpdateConversation(turnContext, activity, appName, false);

            this.logger.LogInformation($"Successfully installed app for user {turnContext.Activity.From.AadObjectId}.");
        }

        #endregion


        #region Helper Methods

        private async Task InsertUpdateConversation(ITurnContext<IConversationUpdateActivity> turnContext, IConversationUpdateActivity activity, string appName, bool active)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "turnContext cannot be null");

            var conversation = new ConversationModel
            {
                ActivityId = activity.Id,
                ConversationId = activity.Conversation?.Id ?? null,
                RecipientId = activity.Recipient?.Id,
                RecipientName = activity.Recipient?.Name,
                ServiceUrl = activity.ServiceUrl,
                TenantId = Guid.TryParse(activity.Conversation?.TenantId, out var TenantId) ? TenantId : Guid.Empty,
                UserId = Guid.TryParse(activity.From?.AadObjectId, out var UserId) ? UserId : Guid.Empty,
                UserName = activity.From?.Name,
                Active = active,
                AppName =appName,
            };

            try
            {
                var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext?.Activity?.From?.Id, cancellationToken: CancellationToken.None);
                conversation.UserName = member?.Name ?? null;
                conversation.UserEmail = member?.Email ?? null;
                conversation.UserPrincipalName = member?.UserPrincipalName ?? null;
            }
            catch (Exception ex)
            {
                throw;
            }

            await this._commonData.BotInstallUninstall_InsertUpdate(conversation);
        }

        private async Task InsertUpdateConversation(ITurnContext turnContext, IConversationUpdateActivity activity, string appName, bool active)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "turnContext cannot be null");

            var conversation = new ConversationModel
            {
                ActivityId = activity.Id,
                ConversationId = activity.Conversation?.Id ?? null,
                RecipientId = activity.Recipient?.Id,
                RecipientName = activity.Recipient?.Name,
                ServiceUrl = activity.ServiceUrl,
                TenantId = Guid.TryParse(activity.Conversation?.TenantId, out var TenantId) ? TenantId : Guid.Empty,
                UserId = Guid.TryParse(activity.From?.AadObjectId, out var UserId) ? UserId : Guid.Empty,
                UserName = activity.From?.Name,
                Active = active,
                AppName = appName,
            };

            try
            {
                var member = await TeamsInfo.GetMemberAsync(turnContext, turnContext?.Activity?.From?.Id, cancellationToken: CancellationToken.None);
                conversation.UserName = member?.Name ?? null;
                conversation.UserEmail = member?.Email ?? null;
                conversation.UserPrincipalName = member?.UserPrincipalName ?? null;
            }
            catch (Exception ex)
            {
                throw;
            }

            await this._commonData.BotInstallUninstall_InsertUpdate(conversation);
        }

        #endregion 
    }
}
