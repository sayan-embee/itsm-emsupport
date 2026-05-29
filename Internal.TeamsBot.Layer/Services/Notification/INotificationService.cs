using Common.Layer.Models;
using Microsoft.Bot.Schema;

namespace Internal.TeamsBot.Layer.Services.Notification
{
    public interface INotificationService
    {
        Task<NotificationResponseModel> SendCard_PersonalScope(string userADID, Attachment cardAttachment, int referenceId);
    }
}