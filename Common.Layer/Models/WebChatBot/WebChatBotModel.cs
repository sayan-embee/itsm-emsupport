using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.WebChatBot
{
    public class WebChatBotModel{}

    public class WebChatSettings
    {
        [JsonProperty("directLineSecret")]
        public string DirectLineSecret { get; set; }

        [JsonProperty("trustedOrigins")]
        public string[] TrustedOrigins { get; set; }
    }

    public class WebChatLogModel
    {
        [JsonProperty("webChatLogId")]
        public long? WebChatLogId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("logId")]
        public long? LogId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("directLineToken")]
        public string DirectLineToken { get; set; }

        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("streamUrl")]
        public string StreamUrl { get; set; }

        [JsonProperty("expires_InSecs")]
        public int? Expires_InSecs { get; set; }

        [JsonProperty("expiredOn")]
        public DateTime? ExpiredOn { get; set; }

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("needsRefresh")]
        public bool NeedsRefresh { get; set; }

        [JsonProperty("startedOn")]
        public DateTime? StartedOn { get; set; }

        [JsonProperty("endedOn")]
        public DateTime? EndedOn { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("feedbackRatingId")]
        public int? FeedbackRatingId { get; set; }

        [JsonProperty("additionalFeedback")]
        public string AdditionalFeedback { get; set; }

        [JsonProperty("satisfiedWithResolution")]
        public bool? SatisfiedWithResolution { get; set; }

        [JsonProperty("conversationType")]
        public string ConversationType { get; set; }

        [JsonProperty("sessionCloseRemarks")]
        public string SessionCloseRemarks { get; set; }        
    }

    public class WebChatUserMessageModel
    {
        [JsonProperty("messageId")]
        public long MessageId { get; set; }

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

        [JsonProperty("fileList")]
        public List<FileDetailsModel?> FileList { get; set; }

        public long? WebChatLogId { get; set; } // ID of the web chat log
        public string MessageActivityId { get; set; } // ID of the message sent to the user
        public DateTime? MessageSentUTC { get; set; } // Timestamp when message was logged
        public string FeedbackCardActivityId { get; set; } // ID of the adaptive card activity
        public DateTime? FeedbackCardSentUTC { get; set; } // When the adaptive card was sent
        public bool? LikeDislike { get; set; } // User's feedback (null = no feedback, true = like, false = dislike)
        public DateTime? FeedbackReceivedUTC { get; set; } // When the feedback was provided
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
    }

    public class WebChatOptionsModel
    {
        public int OptionId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Option { get; set; }
        public bool Active { get; set; }
    }

    public class WebChatSOPIndexModel
    {
        public int AutoId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string IndexName { get; set; }
        public bool Active { get; set; }
    }

    public class WebChatFeedbackOptionsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
