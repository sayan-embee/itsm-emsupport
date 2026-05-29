using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.WebChatBot;
using DataAccess.Layer.Data.CustomerPortal;
using DataAccess.Layer.Data.FreshService;
using Internal.TeamsBot.Layer.ExceptionLog;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Internal.TeamsBot.Layer.Plugins
{
    public class TicketPlugin
    {
        //private const string TICKET_DESCRIPTION = "Search the public website and provides the answers to user queries.";
        //private const string TICKET_TEMPLATE = @"Provide me details of the ticket 724960";
        //private const string GET_TICKET_FUNC = "get_ticket_data";

        private readonly KernelFunction _ticketSearch;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public TicketPlugin(
                IServiceProvider serviceProvider
                , IConfiguration configuration
            )
        {
            //PromptExecutionSettings settings = new()
            //{
            //    ExtensionData = new Dictionary<string, object>()
            //{
            //    { "Temperature", 0.7 },
            //    { "MaxTokens", 250 }
            //},
            //};

            //_ticketSearch = KernelFunctionFactory.CreateFromPrompt(TICKET_TEMPLATE,
            //functionName: GET_TICKET_FUNC,
            //executionSettings: settings);

            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [KernelFunction]
        [Description("Provide the list of tickets")]
        public async Task<List<KernelTicketDetails>> GetTicketDetails(
        [Description("User details object containing UserId, UserEmail, and UserName.")] KernelUserContext userContext
            )
        {
            try
            {
                List<KernelTicketDetails> ticketList = null;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _freshServiceData = scope.ServiceProvider.GetRequiredService<IFreshServiceData>();

                    var ticketModel = new KernelTicketDetails();

                    var tickets = await _freshServiceData.SemanticKernel_FreshServiceTickets_GetAll(ticketModel);
                    ticketList = tickets;
                }
                return ticketList;
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error at KernelFunction -> GetTicketDetails() - {ex.Message}");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        //[KernelFunction]
        //[Description("Provide the ticket details of requested ticket id.")]
        //public async Task<List<Ticket>> GetTicketDetailsBYId(
        //    [Description("Ticket id or ticket reference no to search.")] long id
        //    //[Description("User details object containing UserId, UserEmail, and UserName.")] UserContext userContext
        //    )
        //{
        //    try
        //    {
        //        List<Ticket> ticketList = null;
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var _freshServiceData = scope.ServiceProvider.GetRequiredService<IFreshServiceData>();
        //            var tickets = await _freshServiceData.SemanticKernel_FreshServiceTickets_GetAll();
        //            ticketList = tickets;
        //        }
        //        return ticketList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.WriteMessageToText($"Error at KernelFunction -> GetTicketDetails() - {ex.Message}");
        //        ExceptionLogging.SendErrorToText(ex);
        //        return null;
        //    }
        //}
    }
}
