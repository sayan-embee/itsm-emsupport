using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace External.CustomerPortal.Layer.Plugins
{
    public class TicketPlugin
    {
        //private const string TICKET_DESCRIPTION = "Search the public website and provides the answers to user queries.";
        //private const string TICKET_TEMPLATE = @"Provide me details of the ticket 724960";
        //private const string GET_TICKET_FUNC = "get_ticket_data";

        private readonly KernelFunction _ticketSearch;

        private readonly IConfiguration _configuration;

        //private readonly AzureOpenAISettingsModel _azureOpenAISettings;
        //private readonly AISearchSettingsModel _aISearchSettings;

        public TicketPlugin(
            //IOptions<AzureOpenAISettingsModel> azureOpenAISettings
            //, IOptions<AISearchSettingsModel> aISearchSettings
            IConfiguration configuration
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

            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));


            //_azureOpenAISettings = azureOpenAISettings.Value ?? throw new ArgumentNullException(nameof(azureOpenAISettings));
            //_aISearchSettings = aISearchSettings.Value ?? throw new ArgumentNullException(nameof(aISearchSettings));
        }

        [KernelFunction]
        [Description("Provide the list of tickets")]
        //[Description("Provide the list of tickets for a specific user.")]
        public async Task<List<Ticket>> GetTicketDetails(
            //[Description("User details object containing UserId, UserEmail, and UserName.")] KernelUserContext userContext
            )
        {
            try
            {
                await Task.Delay(0);
                var filePath = @"D:\Dot-Net-Projects\ITSM_Hackathon\Embee_ITSM_Automation_V2\External.CustomerPortal.Layer\Data\tickets.json";
                string jsonString = File.ReadAllText(filePath);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonString);

                return myDeserializedClass.tickets;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [KernelFunction]
        [Description("Provide the ticket details of requested ticket id.")]
        public async Task<List<Ticket>> GetTicketDetailsBYId(
            [Description("Ticket id or ticket reference no to search.")] long id
            //[Description("User details object containing UserId, UserEmail, and UserName.")] UserContext userContext
            )
        {
            try
            {
                await Task.Delay(0);
                var filePath = @"D:\Dot-Net-Projects\ITSM_Hackathon\Embee_ITSM_Automation_V2\External.CustomerPortal.Layer\Data\tickets.json";
                string jsonString = File.ReadAllText(filePath);
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(jsonString);
                return myDeserializedClass.tickets.Where(x => x.id == id).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


    public class CustomFields
    {

        public string location { get; set; }
        public string nsd_member_name { get; set; }
        public string on_roaster_engineer { get; set; }
        public object resolution_type { get; set; }
        public object scheduled_date { get; set; }
        public object sub_location { get; set; }
        public object support_type { get; set; }
        public string tenant { get; set; }
        public string ticket_mode { get; set; }
        public object time_track_mandate { get; set; }
        public object user_type { get; set; }
        public object parent_ticket_id { get; set; }
        public object business_impact { get; set; }
        public object impacted_locations { get; set; }
        public object no_of_customers_impacted { get; set; }
        public object resolution_remarks { get; set; }
        public object resource_name { get; set; }
        public object problem_statement { get; set; }
        public string oem_case_idif_any { get; set; }
        public object sales_account_manager { get; set; }
        public object sl_no { get; set; }
        public object pid { get; set; }
        public object model { get; set; }
        public object product { get; set; }
    }

    public class RequestedFor
    {
        public string email { get; set; }
        public long id { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class Requester
    {
        public string email { get; set; }
        public long id { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class Root
    {
        public List<Ticket> tickets { get; set; }
    }

    public class Stats
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public long ticket_id { get; set; }
        public object opened_at { get; set; }
        public bool group_escalated { get; set; }
        public int? inbound_count { get; set; }
        public DateTime status_updated_at { get; set; }
        public int? outbound_count { get; set; }
        public object pending_since { get; set; }
        public object resolved_at { get; set; }
        public object closed_at { get; set; }
        public DateTime? first_assigned_at { get; set; }
        public DateTime? assigned_at { get; set; }
        public DateTime? agent_responded_at { get; set; }
        public object requester_responded_at { get; set; }
        public DateTime? first_responded_at { get; set; }
        public int? first_resp_time_in_secs { get; set; }
        public int? resolution_time_in_secs { get; set; }
    }

    public class Ticket
    {
        public string subject { get; set; }
        public long? group_id { get; set; }
        public long? department_id { get; set; }
        public object category { get; set; }
        public object sub_category { get; set; }
        public object item_category { get; set; }
        public long? requester_id { get; set; }
        public long? responder_id { get; set; }
        public DateTime due_by { get; set; }
        public bool fr_escalated { get; set; }
        public bool deleted { get; set; }
        public bool spam { get; set; }
        public object email_config_id { get; set; }
        public bool is_escalated { get; set; }
        public DateTime? fr_due_by { get; set; }
        public int? id { get; set; }
        public int? priority { get; set; }
        public int? status { get; set; }
        public int? source { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public int? workspace_id { get; set; }
        public long requested_for_id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string description_text { get; set; }
        public CustomFields custom_fields { get; set; }
        public Requester requester { get; set; }
        public RequestedFor requested_for { get; set; }
        public Stats stats { get; set; }
        public string department_name { get; set; }
        public int? tasks_dependency_type { get; set; }
    }
}
