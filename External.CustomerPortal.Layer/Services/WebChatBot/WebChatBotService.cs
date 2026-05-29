using Azure.Core;
using Common.Layer.Models.WebChatBot;
using External.CustomerPortal.Layer.ExceptionLog;
using System.Text;
using System.Text.Json;

namespace External.CustomerPortal.Layer.Services.WebChatBot
{
    public class WebChatBotService : IWebChatBotService
    {
        public async Task<WebChatLogModel> GenerateDirectLineToken(WebChatSettings settings, WebChatLogModel dataModel)
        {
            try
            {
                ExceptionLogging.WriteMessageToText("Info -> GenerateDirectLineToken() -> DirectLine Token (Old): " + dataModel.DirectLineToken);

                if (dataModel != null && dataModel.NeedsRefresh && !string.IsNullOrEmpty(dataModel.DirectLineToken))
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", dataModel.DirectLineToken);

                        var payload = new
                        {
                            //user = new
                            //{
                            //    id = dataModel.UserId,
                            //    email = dataModel.UserEmail,
                            //    name = dataModel.UserName,
                            //    sessionId = dataModel.SessionId
                            //},
                            //trustedOrigins = settings.TrustedOrigins
                        };

                        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync(
                            "https://directline.botframework.com/v3/directline/tokens/refresh",
                            content
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();

                            using (JsonDocument jsonDoc = JsonDocument.Parse(responseBody))
                            {
                                JsonElement root = jsonDoc.RootElement;

                                //dataModel.ConversationId = root.GetProperty("conversationId").GetString();
                                //dataModel.DirectLineToken = root.GetProperty("token").GetString();
                                //dataModel.Expires_InSecs = root.GetProperty("expires_in").GetInt32();

                                if (root.TryGetProperty("conversationId", out JsonElement conversationIdElement))
                                {
                                    dataModel.ConversationId = conversationIdElement.GetString();
                                }

                                if (root.TryGetProperty("token", out JsonElement tokenElement))
                                {
                                    dataModel.DirectLineToken = tokenElement.GetString();
                                }

                                if (root.TryGetProperty("expires_in", out JsonElement expiresInElement))
                                {
                                    dataModel.Expires_InSecs = expiresInElement.GetInt32();
                                }
                            }
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            ExceptionLogging.WriteMessageToText(errorContent);
                        }
                    }
                }
                else
                {
                    /*
                     * /conversations vs /tokens/generate:
                     * The start conversation operation (POST /v3/directline/conversations) is similar to the generate token operation (POST /v3/directline/tokens/generate) in that both operations return a token that can be used to access a single conversation.
                     * However, the start conversation operation also starts the conversation, contacts the bot, and creates a WebSocket stream URL, whereas the generate token operation does none of these things.
                     * If you intend to start the conversation immediately with your client, use the start conversation operation. If you plan to distribute the token to clients and want them to initiate the conversation, use the generate token operation instead.
                     */

                    ExceptionLogging.WriteMessageToText("Info -> GenerateDirectLineToken() -> DirectLine Secret: " + settings.DirectLineSecret);

                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.DirectLineSecret);

                        var payload = new
                        {
                            user = new
                            {
                                id = dataModel.UserId,
                                email = dataModel.UserEmail,
                                name = dataModel.UserName,
                                sessionId = dataModel.SessionId
                            },
                            trustedOrigins = settings.TrustedOrigins
                        };

                        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                        // tokens/generate
                        // conversations - Not working with backend token generation

                        var response = await httpClient.PostAsync(
                            "https://directline.botframework.com/v3/directline/tokens/generate",
                            content
                        );

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();

                            using (JsonDocument jsonDoc = JsonDocument.Parse(responseBody))
                            {
                                JsonElement root = jsonDoc.RootElement;

                                //dataModel.ConversationId = root.GetProperty("conversationId").GetString();
                                //dataModel.DirectLineToken = root.GetProperty("token").GetString();
                                //dataModel.Expires_InSecs = root.GetProperty("expires_in").GetInt32();

                                if (root.TryGetProperty("conversationId", out JsonElement conversationIdElement))
                                {
                                    dataModel.ConversationId = conversationIdElement.GetString();
                                }

                                if (root.TryGetProperty("streamUrl", out JsonElement streamUrlElement))
                                {
                                    dataModel.StreamUrl = streamUrlElement.GetString();
                                }

                                if (root.TryGetProperty("token", out JsonElement tokenElement))
                                {
                                    dataModel.DirectLineToken = tokenElement.GetString();
                                }

                                if (root.TryGetProperty("expires_in", out JsonElement expiresInElement))
                                {
                                    dataModel.Expires_InSecs = expiresInElement.GetInt32();
                                }
                            }
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            ExceptionLogging.WriteMessageToText(errorContent);
                        }
                    }
                }                

                return dataModel;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return dataModel;
            }
        }
    }
}