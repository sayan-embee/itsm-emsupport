using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Common.Layer.Models.AppSettings;
using Internal.TeamsBot.Layer.ExceptionLog;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Linq;
using Azure.AI.OpenAI.Chat;
using OpenAI.Chat;
using static Internal.TeamsBot.Layer.Plugins.SOPSearchPlugin;

namespace Internal.TeamsBot.Layer.Plugins
{
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0010
#pragma warning disable CS8604

    public class SOPSearchPlugin
    {
        private const string SOP_DESCRIPTION = "Provide response based on Standard Operating Procedure (SOP) documents ensuring efficient and accurate responses to user queries.";
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
        private readonly double temperature;
        private readonly double topP;
        private readonly double frequencyPenalty;
        private readonly double presencePenalty;
        private readonly int maxTokens;
        private readonly string apologyMessage;
        private readonly string systemMessage;
        private readonly int documentCount;

        public SOPSearchPlugin(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            // Azure AI Search Keys
            serviceEndpoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
            indexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
            key = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");
            documentCount = _configuration.GetValue<int>("AISearch:DocumentCount");

            // OpenAI Keys
            openAIApiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");
            openAIEndpoint = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
            deploymentName = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");
            deploymentTextEmbeddings = _configuration.GetValue<string>("AzureOpenAI:DeploymentIdTextEmbeddings");
            temperature = _configuration.GetValue<float>("AzureOpenAI:Temperature");
            topP = _configuration.GetValue<float>("AzureOpenAI:NucleusSamplingFactor");
            frequencyPenalty = _configuration.GetValue<float>("AzureOpenAI:FrequencyPenalty");
            presencePenalty = _configuration.GetValue<float>("AzureOpenAI:PresencePenalty");
            maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
            apologyMessage = _configuration.GetValue<string>("AzureOpenAI:ApologyMessage");
            systemMessage = _configuration.GetValue<string>("AzureOpenAI:SystemMessage");

            PromptExecutionSettings settings = new()
            {
                ExtensionData = new Dictionary<string, object>()
                {
                    { "Temperature", temperature},
                    { "MaxTokens", maxTokens }
                },
            };

            _sopSearch = KernelFunctionFactory.CreateFromPrompt(
                SOP_TEMPLATE,
                functionName: GET_SOP_FUNC,
                executionSettings: settings);
        }

        //[KernelFunction]
        //[Description(SOP_DESCRIPTION)]
        //public async Task<string> GetSOPData(Kernel kernel, [Description("User query for any search")] string userQuery)
        //{
        //    try
        //    {
        //        StringBuilder response = new StringBuilder();

        //        string systemMessage = GetSystemMessage();

        //        var history = new ChatHistory();

        //        history.AddUserMessage(userQuery);

        //        var MSHelpAgent = Get_Agent(kernel, indexName, indexName, systemMessage);

        //        await foreach (ChatMessageContent msgResponse in MSHelpAgent.InvokeAsync(history))
        //        {
        //            foreach (var content in msgResponse.Content ?? "")
        //            {
        //                response.Append(content);
        //            }
        //        }

        //        if (response.Length == 0)
        //        {
        //            response.Append(apologyMessage);
        //        }

        //        return response.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.WriteMessageToText($"Error at TeamsBOT -> KernelFunction -> GetSOPData() - {ex.Message}");
        //        ExceptionLogging.SendErrorToText(ex);

        //        return apologyMessage;
        //    }
        //}

        /*
        public async Task<string> GetSOPData_Bak(Kernel kernel, [Description("User query for SOP search")] string userQuery)
        {
            try
            {
                //string systemMessage = GetSystemMessage();

                var searchClient = new SearchClient(new Uri(serviceEndpoint), indexName, new AzureKeyCredential(key));
                var searchResponse = await searchClient.SearchAsync<SearchDocument>(userQuery);
                var searchResults = searchResponse.Value.GetResults().ToList();

                var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
                {
                    SearchEndpoint = new Uri(serviceEndpoint),
                    Authentication = new OnYourDataApiKeyAuthenticationOptions(key),
                    IndexName = indexName
                };

                var chatExtensionsOptions = new AzureChatExtensionsOptions { Extensions = { azureSearchExtensionConfiguration } };

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    //ChatSystemPrompt = systemMessage,
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    AzureChatExtensionsOptions = chatExtensionsOptions,
                    MaxTokens = maxTokens
                };

                ExceptionLogging.WriteMessageToText($"Retrieved {searchResults.Count} documents from Azure Search.");
                foreach (var doc in searchResults)
                {
                    ExceptionLogging.WriteMessageToText($"Document: {JsonConvert.SerializeObject(doc)}");
                }

                var result = await _sopSearch.InvokeAsync(kernel, new KernelArguments(openAIPromptExecutionSettings)).ConfigureAwait(false);

                return result.GetValue<string>() ?? apologyMessage;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at TeamsBOT -> KernelFunction -> GetSOPData() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                return apologyMessage;
            }
        }
        */
        public class SopResult
        {
            public string Content { get; set; }
            public List<SopCitation> Citations { get; set; } = new();
        }

        public class SopCitation
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
        /*
        [KernelFunction]
        [Description(SOP_DESCRIPTION)]
        public async Task<List<SopResult>> GetSOPData(
            Kernel kernel,
            [Description("User query for any search")] string userQuery)
        {
            var results = new List<SopResult>();

            try
            {
                var searchClient = new SearchClient(new Uri(serviceEndpoint), indexName, new AzureKeyCredential(key));
                var options = new SearchOptions { Size = documentCount };

                var searchResponse = await searchClient.SearchAsync<SearchDocument>(userQuery, options);
                var searchResults = searchResponse.Value.GetResults().Take(options.Size ?? 2).ToList();

                if (!searchResults.Any())
                {
                    return results;
                }

                foreach (var doc in searchResults)
                {
                    string content = doc.Document.ContainsKey("content")
                        ? doc.Document["content"]?.ToString()
                        : "";

                    string docUrl = doc.Document.ContainsKey("url")
                        ? doc.Document["url"]?.ToString()
                        : doc.Document.ContainsKey("metadata_storage_path")
                            ? doc.Document["metadata_storage_path"]?.ToString()
                            : "URL not available";

                    string docName = doc.Document.ContainsKey("metadata_storage_name")
                        ? doc.Document["metadata_storage_name"]?.ToString()
                        : (doc.Document.ContainsKey("title")
                            ? doc.Document["title"]?.ToString()
                            : Path.GetFileName(docUrl));

                    if (string.IsNullOrWhiteSpace(content)) continue;

                    // Clean up
                    content = System.Text.RegularExpressions.Regex.Replace(content, @"image\d+\.png", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    results.Add(new SopResult
                    {
                        Content = content,
                        DocUrl = docUrl,
                        DocName = docName
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at TeamsBOT -> KernelFunction -> GetSOPData() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }

            return results;
        }



        private ChatCompletionAgent Get_Agent(Kernel kernel, string IndexName, string AgentName, string instruction)
        {
            try
            {
                var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
                {
                    SearchEndpoint = new Uri(serviceEndpoint),

                    Authentication = new OnYourDataApiKeyAuthenticationOptions(key),

                    IndexName = IndexName,

                    ShouldRestrictResultScope = true,
                };

                var chatExtensionsOptions = new AzureChatExtensionsOptions { Extensions = { azureSearchExtensionConfiguration } };

                var executionSettings = new OpenAIPromptExecutionSettings { MaxTokens = maxTokens, AzureChatExtensionsOptions = chatExtensionsOptions };

                var agentSOP = new ChatCompletionAgent
                {
                    Name = AgentName,

                    Kernel = kernel,

                    Instructions = instruction,

                    Arguments = new KernelArguments(executionSettings),
                };

                return agentSOP;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error in Call_Agent(): {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                throw ex;
            }
        }

        */

        private string GetSystemMessage()
        {
            try
            {
                // wwwroot/Templates/HTML folder
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "Text");
                var filePath = Path.Combine(rootPath, "SOPSearchPlugin_SystemMessage.txt");

                if (!File.Exists(filePath))
                {
                    ExceptionLogging.WriteMessageToText($"Template file 'SOPSearchPlugin_SystemMessage.txt' not found at path: {filePath}");
                    return "";
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error in GetSOPData -> GetSystemMessage(): {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);

                return "";
            }
        }

        [KernelFunction]
        [Description(SOP_DESCRIPTION)]
        public async Task<List<SopResult>> SearchFromSOP(Kernel kernel, string userQuery)
        {
            var results = new List<SopResult>();
            try
            {

                var result = await CallAzureOpenAI(userQuery);
                if (result != null)
                {
                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
            }
            return results;
        }
        public async Task<SopResult> CallAzureOpenAI(string userQuery)
        {
            SopResult _SopResult = null;
            try
            {
                _SopResult = new SopResult();

                #pragma warning disable AOAI001 // Suppress the diagnostic warning

                AzureKeyCredential credential = new(openAIApiKey); // Add your OpenAI API key here
                AzureOpenAIClient azureClient = new(
                    new Uri(openAIEndpoint),
                    credential
                );
                ChatClient chatClient = azureClient.GetChatClient(deploymentName);


                // Setup chat completion options with Azure Search data source
               // ChatCompletionOptions options = new ChatCompletionOptions();


                ChatCompletionOptions options = new ChatCompletionOptions
                {
                    Temperature = (float)temperature,
                    TopP = (float)topP,
                    FrequencyPenalty = (float)frequencyPenalty,
                    PresencePenalty = (float)presencePenalty,
                    MaxOutputTokenCount = maxTokens
                };

                //options.Temperature = (float)0.7;
                //options.TopP = (float)0.95;
                //options.FrequencyPenalty = (float)0;
                //options.PresencePenalty = (float)0;
                //options.MaxOutputTokenCount = 6553;
                options.AddDataSource(new AzureSearchChatDataSource()
                {
                    Endpoint = new Uri(serviceEndpoint),
                    IndexName = indexName,
                    Authentication = DataSourceAuthentication.FromApiKey(key), // Add your Azure AI Search admin key here
                    TopNDocuments = documentCount,
                });


                // Replace the incorrect property 'MaxTokens' with the correct property 'MaxTokenLimit'.
                // 'MaxTokens' is not a valid property in 'ChatCompletionOptions'.

                ChatCompletion completion = await chatClient.CompleteChatAsync(
                    new List<ChatMessage>()
                    {
                        new SystemChatMessage(systemMessage),
                    new UserChatMessage(userQuery)
                    
                    },
                    options


                //new ChatCompletionOptions
                //{
                //    //PastMessages = 10,
                //    Temperature = (float)0.7,
                //    TopP = (float)0.95,
                //    FrequencyPenalty = (float)0,
                //    PresencePenalty = (float)0,
                //    MaxOutputTokenCount= 6553,


                //    //MaxTokenLimit = 6553, // Corrected property name
                //    //StopSequences = new List<string>(),
                //}


                );

                // Process and print the response
                AzureChatMessageContext onYourDataContext = completion.GetAzureMessageContext();

                _SopResult.Content = completion.Content[0].Text;
                string message = "The requested information is not available in the retrieved data. Please try another query or topic.";

                if (string.IsNullOrWhiteSpace(_SopResult.Content))
                {
                    _SopResult.Content = apologyMessage;
                }
                else if (_SopResult.Content.Equals(message, StringComparison.Ordinal))
                {
                    _SopResult.Content = apologyMessage;
                    return _SopResult;
                }

                if (onYourDataContext?.Citations != null && onYourDataContext.Citations.Any())
                {
                    foreach (var c in onYourDataContext.Citations)
                    {
                        _SopResult.Citations.Add(new SopCitation
                        {
                            Title = c.Title,
                            Url = c.Url
                        });
                    }
                }
               

#pragma warning restore AOAI001 // Restore the diagnostic warning
            }

            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error in CallAzureOpenAI: {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
            }
            return _SopResult;
        }

    }
}
