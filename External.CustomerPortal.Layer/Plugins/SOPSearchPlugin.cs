using Azure.AI.OpenAI;
using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using External.CustomerPortal.Layer.ExceptionLog;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace External.CustomerPortal.Layer.Plugins
{
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
    public class SOPSearchPlugin
    {
        private const string SOP_DESCRIPTION = "Provides information based on sop data for support queries.";
        private const string SOP_TEMPLATE = @"How to back up the configuration of a FortiGate firewall. Include the document reference link";
        private const string GET_SOP_FUNC = "get_sop_data";
        internal const string SOP_PLUGIN_NAME = "SOPSearchPlugin";

        private readonly KernelFunction _sopSearch;

        private readonly IConfiguration _configuration;

        private readonly string serviceEndpoint;
        private readonly string indexName;
        private readonly string key;
        private readonly string openAIApiKey;
        private readonly string openAIEndpoint;
        private readonly string deploymentName;
        private readonly string deploymentTextEmbeddings;

        //private readonly AzureOpenAISettingsModel _azureOpenAISettings;
        //private readonly AISearchSettingsModel _aISearchSettings;

        public SOPSearchPlugin(
            //IOptions<AzureOpenAISettingsModel> azureOpenAISettings
            //, IOptions<AISearchSettingsModel> aISearchSettings
            IConfiguration configuration
            )
        {
            PromptExecutionSettings settings = new()
            {
                ExtensionData = new Dictionary<string, object>()
                {
                    { "Temperature", 0.7 },
                    { "MaxTokens", 250 }
                },
            };

            _sopSearch = KernelFunctionFactory.CreateFromPrompt(SOP_TEMPLATE,
            functionName: GET_SOP_FUNC,
            executionSettings: settings);

            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            // Azure AI Search Keys
            serviceEndpoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
            indexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
            key = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");

            // OpenAI Keys
            openAIApiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");
            openAIEndpoint = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
            deploymentName = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");
            deploymentTextEmbeddings = _configuration.GetValue<string>("AzureOpenAI:DeploymentIdTextEmbeddings");

            //_azureOpenAISettings = azureOpenAISettings.Value ?? throw new ArgumentNullException(nameof(azureOpenAISettings));
            //_aISearchSettings = aISearchSettings.Value ?? throw new ArgumentNullException(nameof(aISearchSettings));
        }

        [KernelFunction]
        [Description(SOP_DESCRIPTION)]
        public async Task<string> GetSOPData(Kernel kernel,
            KernelArguments arguments
            )
        {
            try
            {
                var userId = string.Empty;
                var userEmail = string.Empty;
                var sopIndexName = string.Empty;

                if (kernel.Data.TryGetValue("UserContext", out var userContextObj) && userContextObj is KernelUserContext userContext)
                {
                    userId = userContext.UserId;
                    userEmail = userContext.UserEmail;
                    sopIndexName = userContext.IndexName;
                }

                if (string.IsNullOrEmpty(sopIndexName))
                {
                    return "My apologies, I couldn't find a suitable response to your question. Could you please rephrase your query?";
                }

                var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
                {
                    SearchEndpoint = new Uri(serviceEndpoint),
                    Authentication = new OnYourDataApiKeyAuthenticationOptions(key),
                    IndexName = sopIndexName
                };

                var chatExtensionsOptions = new AzureChatExtensionsOptions { Extensions = { azureSearchExtensionConfiguration } };

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    AzureChatExtensionsOptions = chatExtensionsOptions,
                };

                var result = await _sopSearch.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings)).ConfigureAwait(false);

                var message = result.GetValue<string>() ?? "My apologies, I couldn't find a suitable response to your question. Could you please rephrase your query?";

                return message;

            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at CustomerPortal -> KernelFunction -> GetSOPData() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
                return "An error occurred while retrieving SOP data. Please try again later.";
            }
        }
    }
}
