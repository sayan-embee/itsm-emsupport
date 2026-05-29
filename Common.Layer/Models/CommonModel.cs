using Common.Layer.Models.WebChatBot;
using Microsoft.Bot.Schema;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Layer.Models
{
    public class CommonModel{}
    public class TransactionModel
    {
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }
    }

    #region Master-Data

    public class CategoryModel
    {
        public int Id { get; set; }
        public string? CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string? CategoryDesc { get; set; }
        public bool Active { get; set; }
    }

    public class SubCategoryModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? SubCategoryCode { get; set; }
        public string SubCategoryName { get; set; }
        public string? SubCategoryDesc { get; set; }
        public bool Active { get; set; }
        public string? IndexName { get; set; }
    }

    public class TenantModel
    {
        public int Id { get; set; }
        public string? TenantCode { get; set; }
        public string TenantName { get; set; }
        public bool Active { get; set; }
    }

    public class RegionModel
    {
        public int Id { get; set; }
        public string? RegionCode { get; set; }
        public string RegionName { get; set; }
        public bool Active { get; set; }
    }

    public class SAP_CustomerModel
    {
        public string? sap_customer_name { get; set; }
        public string? embee_crm_id { get; set; }
        public string? tenant { get; set; }
    }

    public class SAP_DepartmentModel
    {
        public long? departmentId { get; set; }
        public string? departmentName { get; set; }
        public string? sap_customer_name { get; set; }
        public string? embee_crm_id { get; set; }
        public string? tenant { get; set; }
    }

    #endregion

    #region Adaptive-Card-Response

    public class NotificationResponseModel
    {
        [JsonProperty("notificationId")]
        public long NotificationId { get; set; }

        [JsonProperty("reqNotificationId")]
        public long ReqNotificationId { get; set; }

        [JsonProperty("messageId")]
        public int MessageId { get; set; }

        [JsonProperty("activityId")]
        public string ActivityId { get; set; }

        [JsonProperty("userADID")]
        public string UserADID { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("replyToId")]
        public string ReplyToId { get; set; }

        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }

        [JsonProperty("notificationDateTimeIST")]
        public DateTime? NotificationDateTimeIST { get; set; }

        [JsonProperty("notificationDateTimeUTC")]
        public DateTime? NotificationDateTimeUTC { get; set; }
    }

    #endregion

    #region User-Search-Bot

    public class UserMessageModel
    {
        [JsonProperty("messageId")]
        public int MessageId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("upn")]
        public string UPN { get; set; }

        [JsonProperty("adid")]
        public string ADID { get; set; }

        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("conversationType")]
        public string ConversationType { get; set; }

        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        [JsonProperty("chatId")]
        public string ChatId { get; set; }

        [JsonProperty("localTimestamp")]
        public DateTimeOffset? LocalTimestamp { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("serviceUrl")]
        public string ServiceUrl { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("textFormat")]
        public string TextFormat { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("querySucceed")]
        public bool? QuerySucceed { get; set; }

        [JsonProperty("fileList")]
        public List<FileDetailsModel?> FileList { get; set; }
    }

    public class FileDetailsModel
    {
        [JsonProperty("messageId")]
        public int MessageId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileURL")]
        public string FileURL { get; set; }

        [JsonProperty("fileContent")]
        public string FileContent { get; set; }
    }

    #endregion

    #region SMTP

    public class SMTPConfig
    {
        public bool SendOTP { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string UserId { get; set; }
        public string UserMail { get; set; }
        public string? DisplayName { get; set; }
        public string Pass { get; set; }
        public bool IsCredRequired { get; set; }
        public bool EnableSSL { get; set; }
        public bool UseGraphAPI { get; set; }
    }

    #endregion

    #region Email

    public class EmailSubjectConfig
    {
        public string? ContractMasterCreated { get; set; }
        public string? SignInOTPVerification { get; set; }
    }

    public class EmailModel
    {
        public string? ReferenceNo { get; set; }
        public int Id { get; set; } = 0;
        public string Type { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string? CC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Status { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? OTP_Id { get; set; }
        public string SessionId { get; set; }
    }

    #endregion

    #region OTP

    public class OTPConfig
    {
        public string? ValidityInSec { get; set; }
    }

    public class OTPModel
    {
        [JsonProperty("referenceNo")]
        public string? ReferenceNo { get; set; }

        [JsonProperty("otp_Id")]
        public int OTP_Id { get; set; } = 0;

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("validity")]
        public string? Validity { get; set; }

        [JsonProperty("validityInSec")]
        public int ValidityInSec { get; set; } = 0;

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("expiredOn")]
        public DateTime? ExpiredOn { get; set; }

        [JsonProperty("expiredOn_Formatted")]
        public string? ExpiredOn_Formatted { get; set; }

        [JsonProperty("recipient")]
        public string? Recipient { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; } = false;

        [JsonProperty("verifiedOn")]
        public DateTime? VerifiedOn { get; set; }

        [JsonProperty("invalidCount")]
        public int InvalidCount { get; set; } = 0;

        [JsonProperty("resendCount")]
        public int ResendCount { get; set; } = 0;

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }

    #endregion





    public class KernelUserContext
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string IndexName { get; set; }
    }
    public class KernelTicketDetails
    {
        public long SlNo { get; set; }
        public long TicketId { get; set; }
        public string department_name { get; set; }
        public long department_id { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public DateTime created_at { get; set; }
        public string created_at_display { get; set; }
        public string type { get; set; }
        public string subject { get; set; }
        public int status { get; set; }
        public string StatusName { get; set; }
        public string RequesterEmail { get; set; }
        public string RequesterName { get; set; }
        public string RequesterMobile { get; set; }
        public string location { get; set; }
        public string tenant { get; set; }
        public string nsd_member_name { get; set; }
        public string on_roaster_engineer { get; set; }
        public string resolution_remarks { get; set; }
        public string resource_name { get; set; }
        public string oem_case_idif_any { get; set; }
        public int priority { get; set; }
        public string priorityname { get; set; }
        public string ResolutionStatus { get; set; }
        public string ResponseStatus { get; set; }
        public string resolved_at_display { get; set; }
        public string closed_at_display { get; set; }
        public string status_updated_at_display { get; set; }
        public int first_resp_time_in_secs { get; set; }
        public int resolution_time_in_secs { get; set; }
        public string first_assigned_at_display { get; set; }
        public string first_responded_at_display { get; set; }
        public string assigned_at_display { get; set; }
    }



    /// <summary>
    /// Direct line token request model
    /// </summary>
    public class DLTokenRequestModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        // [JsonProperty("trustedOrigins")]
        // public string[]? TrustedOrigins { get; set; }
    }

    public class AdaptiveCardModel
    {
        public int Id { get; set; } // This will hold the id
        public string Name { get; set; } // This will hold the name
        public string ImageUrl { get; set; } // Optional: For images in the card

        public AdaptiveCardModel(int id, string name, string imageUrl = null)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
        }
    }

    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }

    public class QuestionAnswerModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class UserResponseModel
    {
        public CategoryModel SelectedCategory { get; set; }
        public SubCategoryModel SelectedSubCategory { get; set; }
        public List<QuestionAnswerModel> questionAnswerList { get; set; }
    }
    public class WebChatConversationModel
    {
        public long WebChatLogId { get; set; }
        public string ConversationType { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public UserModel? User { get; set; }

        public string ConversationId { get; set; }
        public string ServiceUrl { get; set; }
        public DateTime LastActivityTime { get; set; }
        public ConversationReference ConversationReference { get; set; }

        public bool WaitingForQueryResponse { get; set; }
        public bool WaitingForFeedbackResponse { get; set; }

        public CategoryModel SelectedCategory { get; set; }
        public SubCategoryModel SelectedSubCategory { get; set; }
        public WebChatOptionsModel SelectedOption { get; set; }

        public List<CategoryModel> CategoryList { get; set; }
        public List<SubCategoryModel> SubCategoryList { get; set; }
        public List<WebChatOptionsModel> WebChatOptions { get; set; }

        public WebChatUserMessageModel UserMessage { get; set; }
    }
}
