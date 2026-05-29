using Common.Layer.Models.AppSettings;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace External.CustomerPortal.Layer.Plugins
{
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
    public class MSWebSearchPlugin
    {
        private const string WEBSEARCH_DESCRIPTION = "Search the public Microsoft website and provides the answers to user queries.";
        private const string WEBSEARCH_TEMPLATE = @"What can I run on an Azure VM?.Search data from only Microsoft sites and be sure to mention reference website link.";
        private const string GET_WEBSEARCH_FUNC = "get_web_site_data";

        private readonly KernelFunction _webSearch;

        private readonly IConfiguration _configuration;

        //private readonly AzureOpenAISettingsModel _azureOpenAISettings;
        //private readonly AISearchSettingsModel _aISearchSettings;

        public MSWebSearchPlugin(
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

            _webSearch = KernelFunctionFactory.CreateFromPrompt(WEBSEARCH_TEMPLATE,
            functionName: GET_WEBSEARCH_FUNC,
            executionSettings: settings);

            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            //_azureOpenAISettings = azureOpenAISettings.Value ?? throw new ArgumentNullException(nameof(azureOpenAISettings));
            //_aISearchSettings = aISearchSettings.Value ?? throw new ArgumentNullException(nameof(aISearchSettings));
        }

        [KernelFunction]
        [Description(WEBSEARCH_DESCRIPTION)]
        public async Task<string> GetAnswersFromPublicWebSite(Kernel kernel)
        {
            try
            {
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ChatSystemPrompt = $"You name is MS Helper and you are expert in retrieving information exclusively from the specified sites in response to the query provided. "
                    + "Please find information exclusively from Microsoft public sites, such as https://learn.microsoft.com/en-us/azure/virtual-machines/windows/faq,https://learn.microsoft.com/en-us/copilot/faq,https://learn.microsoft.com/en-us/azure/search/search-faq-frequently-asked-questions and provide detailed answers based on the content available there. Please response with no data found is answer is not found.",
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                };

                var result = await _webSearch.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings)).ConfigureAwait(false);

                return result.GetValue<string>();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
