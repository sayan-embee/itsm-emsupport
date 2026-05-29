using Common.Layer.Models.AdaptiveCard;
using Common.Layer.Models.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.AppSettings
{
    public class AppSettingsModel
    {
        public string API_Key { get; set; }
        public string? AppDomainUrl { get; set; }
        public string? UtcOffset { get; set; }
        public FreshServiceModel FreshService { get; set; }        
        public Site24x7Model Site24x7 { get; set; }        
        public WelcomeCardModel? WelcomeCard { get; set; }
        public InternalBot? InternalBot { get; set; }
        public ExternalBot? ExternalBot { get; set; }
        public OTPConfig? OTPConfig { get; set; }
        public SMTPConfig? SMTPConfig { get; set; }
        public EmailSubjectConfig? EmailSubject { get; set; }
    }

    public class FreshServiceModel
    {
        [JsonProperty("domainUrl")]
        public string DomainUrl { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }
    }
   
    public class Site24x7Model
    {
        [JsonProperty("accessTokenDomainUrl")]
        public string AccessTokenDomainUrl { get; set; }

        [JsonProperty("apiRootUrl")]
        public string ApiRootUrl { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
    }

    public class AISearchSettingsModel
    {
        [JsonProperty("searchIndexName")]
        public string SearchIndexName { get; set; }

        [JsonProperty("searchServiceEndPoint")]
        public string SearchServiceEndPoint { get; set; }

        [JsonProperty("searchServiceQueryApiKey")]
        public string SearchServiceQueryApiKey { get; set; }

        [JsonProperty("documentCount")]
        public int DocumentCount { get; set; }

    }

    public class AzureOpenAISettingsModel
    {
        [JsonProperty("endPoint")]
        public string EndPoint { get; set; }

        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty("deploymentId")]
        public string DeploymentId { get; set; }

        [JsonProperty("deploymentIdTextEmbeddings")]
        public string DeploymentIdTextEmbeddings { get; set; }

        [JsonProperty("temperature")]
        public Double Temperature { get; set; }

        [JsonProperty("maxTokens")]
        public int MaxTokens { get; set; }

        [JsonProperty("nucleusSamplingFactor")]
        public int NucleusSamplingFactor { get; set; }

        [JsonProperty("frequencyPenalty")]
        public int FrequencyPenalty { get; set; }

        [JsonProperty("presencePenalty")]
        public int PresencePenalty { get; set; }

        [JsonProperty("systemMessage")]
        public string SystemMessage { get; set; }
    }
}