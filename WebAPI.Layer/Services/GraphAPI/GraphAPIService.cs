using Common.Layer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebAPI.Layer.ExceptionLog;

namespace WebAPI.Layer.Services.GraphAPI
{
    public class GraphAPIService : IGraphAPIService
    {
        private readonly IConfiguration _configuration;

        private readonly string clientId = "";
        private readonly string clientSecret = "";
        private readonly string tenantId = "";

        public GraphAPIService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));

            clientId = _configuration.GetValue<string>("AppConfig:BOT_ID");
            clientSecret = _configuration.GetValue<string>("AppConfig:BOT_PASSWORD");
            tenantId = _configuration.GetValue<string>("AppConfig:BOT_TENANT_ID");
        }


        private async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                    var body = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                    var response = await client.PostAsync(tokenUrl, body);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(errorContent.Trim());

                        ExceptionLogging.WriteMessageToText($"Error at GraphAPIService -> GetAccessToken(); Response code:{(int)response.StatusCode}; Message:{jsonObject["errorMessage"]}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(responseBody))
                    {
                        if (doc.RootElement.TryGetProperty("access_token", out JsonElement tokenElement))
                        {
                            return tokenElement.GetString();
                        }
                    }
                }

                ExceptionLogging.WriteMessageToText("Error at GraphAPIService -> GetAccessToken()");
                return string.Empty;
            }
            catch (Exception ex) 
            {
                ExceptionLogging.WriteMessageToText("Error at GraphAPIService -> GetAccessToken()");
                ExceptionLogging.SendErrorToText(ex);
                return string.Empty;
            }
        }


        public async Task<bool> SendEmailAsync(SMTPConfig configModel, EmailModel emailModel)
        {
            try
            {
                if (string.IsNullOrEmpty(clientId)
                    || string.IsNullOrEmpty(clientSecret)
                    || string.IsNullOrEmpty(tenantId))
                {
                    throw new ArgumentNullException("Client Id / Secret / Tenant Id is missing.");
                }

                string token = await GetAccessToken(tenantId, clientId, clientSecret);
                if (string.IsNullOrEmpty(token))
                {
                    return true;
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var emailMessage = new
                    {
                        message = new
                        {
                            subject = emailModel.Subject,
                            body = new
                            {
                                contentType = "HTML",
                                content = emailModel.Body
                            },
                            from = new
                            {
                                emailAddress = new
                                {
                                    address = configModel.UserMail,
                                    name = configModel.DisplayName 
                                }
                            },
                            toRecipients = new[]
                            {
                                new { emailAddress = new { address = emailModel.To } }
                            },
                            ccRecipients = string.IsNullOrEmpty(emailModel.CC)
                            ? new object[] { }
                            : new[] { new { emailAddress = new { address = emailModel.CC } } }
                        }
                    };

                    var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(emailMessage), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://graph.microsoft.com/v1.0/users/" + configModel.UserMail + "/sendMail", jsonContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        var jsonObject = JObject.Parse(errorContent.Trim());

                        ExceptionLogging.WriteMessageToText($"Error at GraphAPIService -> SendEmailAsync(); Response code:{(int)response.StatusCode}; Message:{jsonObject["errorMessage"]}");
                    }
                    else if (response.IsSuccessStatusCode)
                    {
                        await Task.Delay(500);
                        return true;
                    }
                    else
                    {
                        ExceptionLogging.WriteMessageToText($"Error at GraphAPIService -> SendEmailAsync()");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at GraphAPIService -> SendEmailAsync()");
                ExceptionLogging.SendErrorToText(ex);
                return false;
            }
        }
    }
}
