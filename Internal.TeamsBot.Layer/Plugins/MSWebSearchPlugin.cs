using Common.Layer.Models.AppSettings;
using Internal.TeamsBot.Layer.ExceptionLog;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace Internal.TeamsBot.Layer.Plugins
{
    #pragma warning disable SKEXP0010
    #pragma warning disable SKEXP0001
    public class MSWebSearchPlugin
    {
        private const string WEBSEARCH_DESCRIPTION = "Access and retrieve relevant information exclusively from the official Microsoft Learn website, https://learn.microsoft.com/, and its subpages, for the specified topic or query.";
        private const string WEBSEARCH_TEMPLATE = @"What can I run on an Azure VM? Search data from only Microsoft sites and be sure to mention reference website link.";
        private const string GET_WEBSEARCH_FUNC = "get_web_site_data";

        private readonly KernelFunction _webSearch;

        private readonly IConfiguration _configuration;

        private readonly int maxTokens;
        private readonly float temperature;
        private readonly string apologyMessage;

        public MSWebSearchPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
            temperature = _configuration.GetValue<float>("AzureOpenAI:Temperature");
            apologyMessage = _configuration.GetValue<string>("AzureOpenAI:ApologyMessage");

            PromptExecutionSettings settings = new()
            {
                ExtensionData = new Dictionary<string, object>()
                {
                    { "Temperature", temperature },
                    { "MaxTokens", maxTokens }
                },
            };

            _webSearch = KernelFunctionFactory.CreateFromPrompt(
                WEBSEARCH_TEMPLATE,
                functionName: GET_WEBSEARCH_FUNC,
                executionSettings: settings);
        }

        [KernelFunction]
        [Description(WEBSEARCH_DESCRIPTION)]
        public async Task<string> GetAnswersFromPublicWebSite(Kernel kernel)
        {
            try
            {
                string systemMessage = GetSystemMessage();

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ChatSystemPrompt = systemMessage,
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    MaxTokens = maxTokens
                };

                var result = await _webSearch.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings)).ConfigureAwait(false);

                return result.GetValue<string>() ?? apologyMessage;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at TeamsBOT -> KernelFunction -> GetAnswersFromPublicWebSite(): {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                return apologyMessage;
            }
        }




        private string GetSystemMessage()
        {
            try
            {
                // wwwroot/Templates/HTML folder
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "Text");
                var filePath = Path.Combine(rootPath, "WebSearchPlugin_SystemMessage.txt");

                if (!File.Exists(filePath))
                {
                    ExceptionLogging.WriteMessageToText($"Template file 'WebSearchPlugin_SystemMessage.txt' not found at path: {filePath}");
                    return "";
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error in GetAnswersFromPublicWebSite -> GetSystemMessage(): {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                return "";
            }
        }

    }
}
