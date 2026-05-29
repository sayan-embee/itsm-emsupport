using Azure;
using Azure.Search.Documents;
using Common.Layer.Models.AzureOpenAI;
using Internal.TeamsBot.Layer.ExceptionLog;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Azure.AI.OpenAI;

namespace Internal.TeamsBot.Layer.Services.SearchService
{
    public class AISearch : IAISearch
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private readonly string serviceEndpoint;
        private readonly string indexName;
        private readonly string key;
        private readonly string openAIApiKey;
        private readonly string openAIEndpoint;
        private readonly string deploymentName;

        public AISearch(
           ILogger<AISearch> logger,
           IConfiguration configuration
           )
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            // Azure AI Search Keys
            serviceEndpoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
            indexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
            key = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");

            // OpenAI Keys
            openAIApiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");
            openAIEndpoint = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
            deploymentName = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");

        }

        public async Task<SearchClient?> CreateSearchClientForQueries()
        {
            try
            {
                string SearchIndexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
                string SearchServiceEndPoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
                string SearchServiceQueryApiKey = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");

                SearchClient searchClient = new SearchClient(new Uri(SearchServiceEndPoint), SearchIndexName, new AzureKeyCredential(SearchServiceQueryApiKey));
                await Task.Delay(0);
                return searchClient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AISearch --> CreateSearchClientForQueries() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }



        //public async Task<SearchResults<SearchDocument>> SemanticHybridSearch(string query)
        //{


        //    // Initialize Azure AI Search client  
        //    var searchCredential = new AzureKeyCredential(key);
        //    var indexClient = new SearchIndexClient(new Uri(serviceEndpoint), searchCredential);
        //    var searchClient = indexClient.GetSearchClient(indexName);

        //    // Generate the embedding for the query  
        //    var queryEmbeddings = await GenerateEmbeddings(query);

        //    // Perform the vector similarity search  
        //    var searchOptions = new SearchOptions
        //    {
        //        VectorSearch = new()
        //        {
        //            Queries = { new VectorizedQuery(queryEmbeddings.ToArray()) { KNearestNeighborsCount = 3, Fields = { "contentVector" } } }
        //        },
        //        SemanticSearch = new()
        //        {
        //            SemanticConfigurationName = "default",
        //            //QueryCaption = new(QueryCaptionType.Extractive),
        //            //QueryAnswer = new(QueryAnswerType.Extractive),
        //        },
        //        QueryType = SearchQueryType.Simple,
        //        Size = 3,
        //        Select = { "title", "content", "url", "filepath" },

        //    };

        //    SearchResults<SearchDocument> response = await searchClient.SearchAsync<SearchDocument>(query, searchOptions);
        //    return response;

        //}

        //private async Task<ReadOnlyMemory<float>> GenerateEmbeddings(string text)
        //{
        //    // Initialize OpenAI client  
        //    var credential = new AzureKeyCredential(openaiApiKey);
        //    var openAIClient = new OpenAIClient(new Uri(openaiEndpoint), credential);
        //    var response = await openAIClient.GetEmbeddingsAsync(new EmbeddingsOptions(deploymentName, new List<string> { text }));
        //    return response.Value.Data[0].Embedding;
        //}



        public async Task<Temperatures?> ChatCompletionResult(string prompt, string userEmail)
        {
            try
            {
                //string SearchIndexName = _configuration.GetValue<string>("AISearch:SearchIndexName");
                //string SearchServiceEndPoint = _configuration.GetValue<string>("AISearch:SearchServiceEndPoint");
                //string SearchServiceQueryApiKey = _configuration.GetValue<string>("AISearch:SearchServiceQueryApiKey");
                //int SearchServiceDocumentCount = _configuration.GetValue<int>("AISearch:DocumentCount");

                //var apiBase = _configuration.GetValue<string>("AzureOpenAI:EndPoint");
                //var apiKey = _configuration.GetValue<string>("AzureOpenAI:ApiKey");

                //var deploymentId = _configuration.GetValue<string>("AzureOpenAI:DeploymentId");
                //float temperature = _configuration.GetValue<float>("AzureOpenAI:Temperature");
                //int maxTokens = _configuration.GetValue<int>("AzureOpenAI:MaxTokens");
                //float nucleusSamplingFactor = _configuration.GetValue<float>("AzureOpenAI:NucleusSamplingFactor");
                //float frequencyPenalty = _configuration.GetValue<float>("AzureOpenAI:FrequencyPenalty");
                //float presencePenalty = _configuration.GetValue<float>("AzureOpenAI:PresencePenalty");
                //string systemMessage = _configuration.GetValue<string>("AzureOpenAI:SystemMessage");

                //var client = new OpenAIClient(new Uri(apiBase), new AzureKeyCredential(apiKey!));

                //if (string.IsNullOrEmpty(SearchIndexName)
                //    || string.IsNullOrEmpty(SearchServiceEndPoint)
                //    || string.IsNullOrEmpty(SearchServiceQueryApiKey)
                //    || string.IsNullOrEmpty(apiBase)
                //    || string.IsNullOrEmpty(apiKey)
                //    || string.IsNullOrEmpty(deploymentId)
                //    )
                //{
                //    _logger.LogError("AISearch --> ChatCompletionResult() execution failed -> Unable to get values from App-Settings");
                //    ExceptionLogging.WriteMessageToText("AISearch --> ChatCompletionResult() execution failed -> Unable to get values from App-Settings");
                //    return null;
                //}

                //if (temperature == 0
                //    || maxTokens == 0
                //    )
                //{
                //    _logger.LogError("AISearch --> ChatCompletionResult() execution failed -> Temperature/MaxTokens cannot be 0");
                //    ExceptionLogging.WriteMessageToText("AISearch --> ChatCompletionResult() execution failed -> Temperature/MaxTokens cannot be 0");
                //    return null;
                //}

                //AzureCognitiveSearchChatExtensionConfiguration _azureCognitiveSearchChatExtensionConfiguration =
                //    new AzureCognitiveSearchChatExtensionConfiguration()
                //    {
                //        SearchEndpoint = new Uri(SearchServiceEndPoint),
                //        IndexName = SearchIndexName,
                //        Type = AzureChatExtensionType.AzureCognitiveSearch,
                //        DocumentCount = SearchServiceDocumentCount
                //    };
                //_azureCognitiveSearchChatExtensionConfiguration.SetSearchKey(SearchServiceQueryApiKey);



                //var chatCompletionsOptions = new ChatCompletionsOptions()
                //{
                //    DeploymentName = deploymentId,
                //    Temperature = temperature,
                //    MaxTokens = maxTokens,
                //    NucleusSamplingFactor = nucleusSamplingFactor,
                //    FrequencyPenalty = frequencyPenalty,
                //    PresencePenalty = presencePenalty,
                //    //  StopSequences = null,
                //    Messages =
                //    {
                //        new ChatMessage(ChatRole.System, $"{systemMessage}"),
                //        new ChatMessage(ChatRole.User, prompt)
                //    },

                //    AzureExtensionsOptions = new AzureChatExtensionsOptions()
                //    {
                //        Extensions =
                //        {
                //            _azureCognitiveSearchChatExtensionConfiguration
                //        }
                //    }
                //};

                ////FOR REAL VALUES -->

                //var response = await client.GetChatCompletionsAsync(chatCompletionsOptions);

                //var messageResponse = response.Value.Choices[0].Message.Content;
                //var rawResponse = response.GetRawResponse();
                //var rawContentBinary = rawResponse.Content;
                //string rawContent = Encoding.UTF8.GetString(rawContentBinary);


                ////FOR TESTING -->

                ////var messageResponse = @"The support contact details are as follows:
                ////- For process related queries, please contact your respective HRs.
                ////- For any technical queries, please write to:
                ////- Niraj Kumar: n.kumar@noventiq.com
                ////- Yogendra Yadav: yogendra.yadav@noventiq.com [doc1][doc2][doc3]";
                ////var rawContent = @"{""id"": ""11a147bd-dad9-44ce-9579-6dba62673a03"", ""model"": ""gpt-35-turbo"", ""created"": 1701760719, ""object"": ""extensions.chat.completion"", ""choices"": [{""index"": 0, ""finish_reason"": ""stop"", ""message"": {""role"": ""assistant"", ""content"": ""The support contact details are as follows:\n- For process related queries, please contact your respective HRs.\n- For any technical queries, please write to:\n  - Niraj Kumar: n.kumar@noventiq.com\n  - Yogendra Yadav: yogendra.yadav@noventiq.com [doc1][doc2][doc3]"", ""end_turn"": true, ""context"": {""messages"": [{""role"": ""tool"", ""content"": ""{\""citations\"": [{\""content\"": \""PwC\\nOne Noventiq One HR \u2013\\nDarwinbox Implementation\\nUser Manual \u2013 Core\\nNovember 2023\\nH2O and People Insights\\nContents\\n01 Employee Log In\\n02 Employee Task Box\\n2\\n03 Employee Directory\\nH2O and People Insights\\n04 Employee Profile View\\nLogin Instructions \u2013 Web and Mobile App\\n3\\n\u2022 Step 1: Visit https://noventiq.darwinbox.com/on \\nyour Desktop/Laptop\\n\u2022 Step 2: Click on Sign-in with SSO (For Noventiq \\nIndia) and login to the platform with your \\nNoventiq Credentials\\nClick on Sign-in with Darwinbox credentials and \\nenter your username and password\\n\u2022 Step 1: Download and install Darwinbox App \\nfrom Playstore/App Store or scan the following \\nQR code to download app:\\n\u2022 Step 2: Click on Sign-in with SSO (For Noventiq \\nIndia) and login to the platform with your \\nNoventiq Credentials\\nClick on Sign-in with Darwinbox credentials and \\nenter your username and password\\nLogin via Web/Desktop Login via Mobile App\\nEmployee Login\\nSeptember 2022\\n4\\n4\\nEmployee Journey\\n- Improved candidate engagement\\n- Improved Employer branding\\nImpact\\nA Darwinbox homepage can answer queries for employees like:\\n- Employee attendance, leaves, HR Documents, reports, organisational view, etc.\\n- Check status of internal job openings, leave requests, resignation status, etc.\\n- Filling joining forms, details of the reporting manager and team, organisation\u2019s policy related FAQs, etc. \\nDarwinbox\\nEmployee\\nTaskbox\\nAttendance\\n5\\nOnboarding\\nEmployee \\nProfile\\nEmployee Dashboard\\nDarwinbox \\nLog-in Page\\nhttps://noventiq.stage.darwinbox.io/user/login\\n1\\nEmployee Login Page\\nLeave \\nRequests\\n2\\n3\\nEmployee Task Box\\nSeptember 2022\\n6\\n6\\nPwC\\nEmployee Task Box\\nRaised By MeAssigned To Me\\nProbation \\nConfirmation\\nSeparation Flows\\nTransfers\\nBenefits to be Achieved\\nLeave Requests\\nAttendance \\nRegularisation\\nProbation \\nConfirmation\\nSeparation Flows\\nTransfers\\nLeave Requests\\nAttendance \\nRegularisation\\n360 view of every \\nemployee in database\\nElevates candidate \\nexperience\\nEmployee Task-box allows the employee as well as manager to \\nkeep a track of all workflow requests like:\\n- Probation Confirmation status\\n- Leave Requests\\n- Transfers\\n- Attendance Regularisation\\nDarwinbox\\nEmployee Directory\\nSeptember 2022\\n8\\n8\\nPwC\\nEmployee Directory\\n1\\nReportees Screen\\nEmployees\u2019 Directory\\nDarwinbox Employees\\nEmployees/ Managers can \\nview their reportees\u2019 details (if \\nany) / or find any other \\nemployees\u2019 details\\n2\\nImpact\\nBetter candidate \\nengagement\\nImproved employee\u2019s \\nexperience\\nBetter Employer \\nBranding\\nLess time lost in non-\\nvalue adding activities\\n3\\nEmployee Profile View\\nSeptember 2022\\n10\\n10\\nPwC\\nOrganisational View: The view profile\\nbutton will enable the view of organizational\\nstructure, designation and employee ID\\ndirectly through standard portlets\\nImpact\\n1\\nEmployee Profile View\\nReduced manual \\nintervention\\nCorrect details flowing \\ninto Darwinbox \\n2\\n1\\nPwC\\nPersonal Details: Under personal details, cards are\\ndivided into Biographical, contact, address, work\\nexperience, personal identity, personal documents\\nImpact\\n1\\nEmployee Profile View\\nReduced manual \\nintervention\\nCorrect details flowing \\ninto Darwinbox EC\\nEmployment Details: Employment details will have\\ncards such as Work Role, Current office location,\\nManager, Employee type, Personal Documents and HR\\nletters.\\n2\\n1\\n3\\nEmployee can view their personal details and employment details where card-wise information is distributed in sections.Darwinbox\\n2\\nSupport Overview\\nSeptember 2022\\n13\\n13\\nSupport Contact Details\\nFor process related queries, please contact your respective HRs.\\nFor any technical queries, please write to:\\n\u2022 Niraj Kumar: n.kumar@noventiq.com\\n\u2022 Yogendra Yadav: yogendra.yadav@noventiq.com\\nmailto:yogendra.yadav@noventiq.com\\nThank you\\n\u00a9 2023 PwC. All rights reserved\"", \""id\"": null, \""title\"": \""PwC\"", \""filepath\"": \""One Noventiq One HR_Core Manual.pdf\"", \""url\"": \""https://esplspdamlwork9717773856.blob.core.windows.net/fileupload-cog-search-demo-2/One%20Noventiq%20One%20HR_Core%20Manual.pdf\"", \""metadata\"": {\""chunking\"": \""orignal document size=1022. Scores=0.84076715Org Highlight count=12.\""}, \""chunk_id\"": \""0\""}, {\""content\"": \""Darwinbox\\n27\\nManager\u2019s Dashboard in DB\\n1\\n2\\nL1 Manager\u2019s Persona for Transfer Process (1/4)\\nBenefits to be Achieved\\n360 view of every \\nworkflow in database\\nElevates employee \\nexperience\\nBetter Employer \\nBranding\\n2\\n3\\n4\\n5\\nL1 Manager\u2019s Persona for Transfer Process (2/4)\\n29\\nManager\u2019s Persona for Transfer Process (3/4)\\n6\\n7\\n8\\n9\\n10\\n30\\nManager\u2019s Persona for Transfer Process (4/4)\\nTransfer Workflow: After the initiation of the transfer\\nworkflow by the manager, the manager can see the\\nworkflow requests and the different stages at which it is\\npending\\n11\\nReduced number of \\ntouchpoints, leading to \\nimproved TAT\\nImproved employee\u2019s \\nexperience\\nOne platform leveraged \\nfor all changes\\nIMPACT\\nApprover\u2019s Persona\\nSeptember 2022\\n31\\n31\\n32\\n1\\nApprover\u2019s Persona for Transfer Process (3/3)\\n2\\n3\\nIMPACT Less time invested \\nin NVAs\\nImproved employee\u2019s \\nexperience\\nNOTE: Approval flow will be same for all \\napprovers (L1 and L2 Manager/ HOD/ \\nAdmin)\\nSupport Overview\\nSeptember 2022\\n33\\n33\\nSupport Contact Details\\nFor process related queries, please contact your respective HRs.\\nFor any technical queries, please write to:\\n\u2022 Niraj Kumar: n.kumar@noventiq.com\\n\u2022 Yogendra Yadav: yogendra.yadav@noventiq.com\\nmailto:yogendra.yadav@noventiq.com\\nThank you\\n\u00a9 2023 PwC. All rights reserved. Not for further distribution without the permission of PwC. \u201cPwC\u201d refers to the network of member firms of PricewaterhouseCoopers \\nInternational Limited (PwCIL), or, as the context requires, individual member firms of the PwC network. Each member firm is a separate legal entity and does not act as \\nagent of PwCIL or any other member firm. PwCIL does not provide any services to clients. PwCIL is not responsible or liable for the acts or omissions of any of its \\nmember firms nor can it control the exercise of their professional judgment or bind them in any way. No member firm is responsible or liable for the acts or omissions of \\nany other member firm nor can it control the exercise of another member firm\u2019s professional judgment or bind another member f irm or PwCIL in any way.\\n\\tDefault Section\\n\\tSlide 1\\n\\tSlide 2: Contents\\n\\tSlide 3\\n\\tSlide 4\\n\\tProbation Completion\\n\\tSlide 5\\n\\tSlide 6\\n\\tSlide 7\\n\\tSlide 8\\n\\tSlide 9\\n\\tSlide 10\\n\\tSlide 11\\n\\tSlide 12\\n\\tSlide 13\\n\\tSlide 14\\n\\tSeparation Workflow\\n\\tSlide 15\\n\\tSlide 16\\n\\tSlide 17\\n\\tSlide 18\\n\\tSlide 19\\n\\tSlide 20\\n\\tSlide 21\\n\\tSlide 22\\n\\tSlide 23\\n\\tTransfers\\n\\tSlide 24\\n\\tSlide 25\\n\\tSlide 26\\n\\tSlide 27\\n\\tSlide 28\\n\\tSlide 29\\n\\tSlide 30\\n\\tSlide 31\\n\\tSlide 32\\n\\tSlide 33\\n\\tSlide 34\\n\\tSlide 35: Thank you\"", \""id\"": null, \""title\"": \""One Noventiq One HR \u2013\"", \""filepath\"": \""One Noventiq One HR_Workflows Manual.pdf\"", \""url\"": \""https://esplspdamlwork9717773856.blob.core.windows.net/fileupload-cog-search-demo-2/One%20Noventiq%20One%20HR_Workflows%20Manual.pdf\"", \""metadata\"": {\""chunking\"": \""orignal document size=804. Scores=2.1498592Org Highlight count=9.\""}, \""chunk_id\"": \""0\""}, {\""content\"": \""PwC\\nOne Noventiq One HR \u2013\\nDarwinbox Implementation\\nUser Manual \u2013 Leave\\nNovember 2023\\nPwC\\nContents\\n01 Employee Login\\n02 Employee Persona\\n2\\n03 Manager Persona\\nH2O and People Insights\\n04 HOD Persona\\nEmployee Journey\\n- Improved candidate engagement\\n- Improved Employer branding\\nImpact\\nA Darwinbox homepage can answer queries for employees like:\\n- Employee attendance, leaves, HR Documents, reports, organisational view, etc.\\n- Check status of internal job openings, leave requests, resignation status, etc.\\n- Filling joining forms, details of the reporting manager and team, organisation\u2019s policy related FAQs, etc. \\nDarwinbox\\nEmployee\\nTaskbox\\nAttendance\\n3\\nOnboarding\\nEmployee \\nProfile\\nEmployee Dashboard\\nDarwinbox \\nLog-in Page\\nhttps://noventiq.darwinbox.com/user/login\\n1\\nEmployee Login Page\\nLeave \\nRequests\\n2\\n3\\nEmployee Persona\\nSeptember 2022\\n4\\n4\\n- The leave card shows the number of leaves balance of the employee as well as the allotment statusDarwinbox\\n5\\nEmployee\u2019s Dashboard in DB\\n1\\nEmployee\u2019s Persona for applying Leave\\nBenefits to be Achieved\\n360 view of every \\nworkflow in database\\nElevates employee \\nexperience\\n2\\n3\\n4\\nManager Persona\\nSeptember 2022\\n6\\n6\\n7\\nManager\u2019s Dashboard in DB\\n1\\nManager\u2019s Persona for applying Leave on Behalf of Reportee\\nBenefits to be Achieved\\n360 view of every \\nworkflow in database\\nElevates employee \\nexperience\\n2\\n3\\n3\\n4\\nManager\u2019s Dashboard in DB\\n1\\n2\\nManager\u2019s Persona for Leave Application Process (1/2)\\nBenefits to be Achieved\\n360 view of every \\nworkflow in database\\nElevates employee \\nexperience\\n3\\nSupport Overview\\nSeptember 2022\\n9\\n9\\nSupport Contact Details\\nFor process related queries, please contact your respective HRs.\\nFor any technical queries, please write to:\\n\u2022 Niraj Kumar: n.kumar@noventiq.com\\n\u2022 Yogendra Yadav: yogendra.yadav@noventiq.com\\nmailto:yogendra.yadav@noventiq.com\\nThank you\\n\u00a9 2023 PwC. All rights reserved. Not for further distribution without the permission of PwC. \u201cPwC\u201d refers to the network of member firms of PricewaterhouseCoopers \\nInternational Limited (PwCIL), or, as the context requires, individual member firms of the PwC network. Each member firm is a separate legal entity and does not act as \\nagent of PwCIL or any other member firm. PwCIL does not provide any services to clients. PwCIL is not responsible or liable for the acts or omissions of any of its \\nmember firms nor can it control the exercise of their professional judgment or bind them in any way. No member firm is responsible or liable for the acts or omissions of \\nany other member firm nor can it control the exercise of another member firm\u2019s professional judgment or bind another member f irm or PwCIL in any way.\\n\\tSlide 1\\n\\tSlide 2: Contents\\n\\tSlide 3\\n\\tSlide 4\\n\\tSlide 5\\n\\tSlide 6\\n\\tSlide 7\\n\\tSlide 8\\n\\tSlide 9\\n\\tSlide 10\\n\\tSlide 11: Thank you\"", \""id\"": null, \""title\"": \""PwC\"", \""filepath\"": \""One Noventiq One HR_Leave Manual.pdf\"", \""url\"": \""https://esplspdamlwork9717773856.blob.core.windows.net/fileupload-cog-search-demo-2/One%20Noventiq%20One%20HR_Leave%20Manual.pdf\"", \""metadata\"": {\""chunking\"": \""orignal document size=801. Scores=1.980371Org Highlight count=10.\""}, \""chunk_id\"": \""0\""}], \""intent\"": \""[\\\""How to contact customer support\\\"", \\\""customer service contact information\\\""]\""}"", ""end_turn"": false}]}}}], ""usage"": {""prompt_tokens"": 5914, ""completion_tokens"": 86, ""total_tokens"": 6000}}";


                //List<string> citationLinks = new List<string>();
                //MatchCollection matches = Regex.Matches(messageResponse, @"\[(doc\d\d?\d?)]");
                //foreach (Match match in matches)
                //{
                //    citationLinks.Add(match.Value);
                //}
                //int lengthDocN = "[doc".Length;
                //foreach (string link in citationLinks)
                //{
                //    messageResponse = messageResponse.Replace(link, "");
                //}

                //RootResponse? myDeserializedClass = JsonConvert.DeserializeObject<RootResponse>(rawContent);
                //var d = myDeserializedClass.choices[0].message.context.messages[0].content;
                //Temperatures? myTemperatures = JsonConvert.DeserializeObject<Temperatures>(d);
                //if (myTemperatures != null)
                //{
                //    myTemperatures.Content = messageResponse;
                //}

                //await Task.Delay(0);
                //return myTemperatures;



                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AISearch --> ChatCompletionResult() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }

        }
    }
}
