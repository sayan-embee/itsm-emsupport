using Common.Layer.Models;
using Common.Layer.Models.WebChatBot;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;
using External.CustomerPortal.Layer.Services.AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class IdleUserMonitorService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _memoryCache;
    private readonly BotAdapter _adapter;
    private Timer _timer;

    private readonly IConfiguration _configuration;

    private readonly string botId;
    private readonly int idleInMinutes = 4;
    private readonly string idleMessage = "It appears you have left the session or are away from your desk. As you are unavailable to continue this chat, I will end the session in one minute.";
    private readonly int endChatInMinutes = 5;

    public IdleUserMonitorService(
            IServiceProvider serviceProvider
            , IMemoryCache memoryCache
            , BotAdapter adapter
            , IConfiguration configuration
        )
    {
        _serviceProvider = serviceProvider;
        _memoryCache = memoryCache;
        _adapter = adapter;
        _configuration = configuration;

        botId = _configuration.GetValue<string>("BOT_ID");
        idleInMinutes = _configuration.GetValue<int>("WebChatSettings:IdleInMinutes");
        idleMessage = _configuration.GetValue<string>("WebChatSettings:IdleMessage");
        endChatInMinutes = _configuration.GetValue<int>("WebChatSettings:EndChatInMinutes");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _timer = new Timer(async _ => await CheckIdleUsersAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in StartAsync(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return Task.CompletedTask;
        }
    }

    private async Task CheckIdleUsersAsync()
    {
        try
        {
            var activeConversations = GetActiveConversations();
            if (activeConversations == null || !activeConversations.Any()) return;

            foreach (var conversation in activeConversations)
            {
                if (conversation == null) continue;

                var idleTime = DateTime.UtcNow - conversation.LastActivityTime;

                //if (idleTime.TotalMinutes >= 3 && idleTime.TotalMinutes < 5)
                //{
                //    await SendIdleReminderAsync(_adapter, conversation, "It looks like our chat has been idle for a while, Would you like to continue chatting?");
                //}
                //else if (idleTime.TotalMinutes >= 4 && idleTime.TotalMinutes < 5)
                //{
                //    await SendIdleReminderAsync(_adapter, conversation, "It appears you have left the session or are away from your desk. As you are unavailable to continue this chat, I will end the session in one minute.");
                //    await Task.Delay(1000);
                //    await SendIdleReminderAsync(_adapter, conversation, "Please do not hesitate to contact us again at any time. We will be glad to assist you.");
                //}
                //else if (idleTime.TotalMinutes >= 5)
                //{
                //    await EndChatSessionAsync(_adapter, conversation);
                //}

                if (idleTime.TotalMinutes >= idleInMinutes && idleTime.TotalMinutes < endChatInMinutes)
                {
                    await SendIdleReminderAsync(_adapter, conversation, idleMessage);
                }
                else if (idleTime.TotalMinutes >= endChatInMinutes)
                {
                    await EndChatSessionAsync(_adapter, conversation);
                }
            }
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in CheckIdleUsersAsync(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return;
        }
    }

    private List<WebChatConversationModel> GetActiveConversations()
    {
        try
        {
            var activeConversations = new List<WebChatConversationModel>();

            // Retrieve all keys from memory cache
            foreach (var key in GetMemoryCacheKeys())
            {
                if (_memoryCache.TryGetValue(key, out WebChatConversationModel conversation))
                {
                    activeConversations.Add(conversation);
                }
            }

            return activeConversations;
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in GetActiveConversations(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return null;
        }
    }

    private IEnumerable<string> GetMemoryCacheKeys()
    {
        if (_memoryCache.TryGetValue("CacheKeys", out HashSet<string> keys))
        {
            return keys;
        }

        return new List<string>();
    }

    private async Task SendIdleReminderAsync(BotAdapter adapter, WebChatConversationModel conversation, string idleMessage)
    {
        try
        {
            if (conversation.ConversationReference == null || botId == null) return;

            ExceptionLogging.WriteMessageToText($"SendIdleReminderAsync: {System.Text.Json.JsonSerializer.Serialize(conversation.ConversationReference)}");


            IActivity content = null;
            Attachment attachment = null;

            using (var scope = _serviceProvider.CreateScope())
            {
                var adaptiveCardService = scope.ServiceProvider.GetRequiredService<IAdaptiveCardService>();

                attachment = adaptiveCardService.CreateCard_IdleChatMessage_PersonalScope(idleMessage, conversation);
            }

            if (attachment != null)
            {
                content = MessageFactory.Attachment(attachment);
            }
            else
            {
                content = MessageFactory.Text(idleMessage);
            }

            await adapter.ContinueConversationAsync(
                //conversation.ConversationReference.Bot.Id?.Split("@")?[0]?.ToString(),
                botId,
                conversation.ConversationReference,
                async (turnContext, cancellationToken) =>
                {
                    await turnContext.SendActivityAsync(content, cancellationToken);
                },
                CancellationToken.None
            );
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in SendIdleReminderAsync(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return;
        }        
    }

    private async Task EndChatSessionAsync(BotAdapter adapter, WebChatConversationModel conversation)
    {
        try
        {
            if (conversation.ConversationReference == null || conversation.User == null || botId == null) return;

            ExceptionLogging.WriteMessageToText($"EndChatSessionAsync: {System.Text.Json.JsonSerializer.Serialize(conversation.ConversationReference)}");


            await adapter.ContinueConversationAsync(
                //conversation.ConversationReference.Bot.Id?.Split("@")?[0]?.ToString(),
                botId,
                conversation.ConversationReference,
                async (turnContext, cancellationToken) =>
                {
                    var message = MessageFactory.Text("Chat session ended. Have a good day.");
                    await turnContext.SendActivityAsync(message, cancellationToken);

                    await Task.Delay(2000);

                    // Resolve ICustomerPortalData in a scope
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var customerPortalData = scope.ServiceProvider.GetRequiredService<ICustomerPortalData>();

                        var webchatModel = new WebChatLogModel
                        {
                            EndedOn = DateTime.UtcNow,
                            UserEmail = conversation.User.UserEmail,
                            Active = false,
                            SessionCloseRemarks = "Idle-Timeout"
                        };

                        _ = customerPortalData.DirectLineToken_InsertUpdate("U", webchatModel);
                    }

                    // Send event to Web Chat for chat completion
                    var completionEvent = new Activity
                    {
                        Type = ActivityTypes.Event,
                        Name = "endChat",
                        Value = new { message = "Chat Session Ended" }
                    };

                    await turnContext.SendActivityAsync(completionEvent, cancellationToken);
                },
                CancellationToken.None
            );

            // Remove conversation from cache
            _memoryCache.Remove(conversation.User.UserId);
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in EndChatSessionAsync(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            ExceptionLogging.WriteMessageToText($"Error in StopAsync(): {ex.Message}");
            ExceptionLogging.SendErrorToText(ex);
            return Task.CompletedTask;
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}