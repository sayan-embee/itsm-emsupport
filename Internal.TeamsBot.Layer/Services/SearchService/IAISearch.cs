using Azure.Search.Documents;
using Common.Layer.Models.AzureOpenAI;

namespace Internal.TeamsBot.Layer.Services.SearchService
{
    public interface IAISearch
    {
        Task<SearchClient?> CreateSearchClientForQueries();
        Task<Temperatures?> ChatCompletionResult(string userQuery, string userEmail);
    }
}




