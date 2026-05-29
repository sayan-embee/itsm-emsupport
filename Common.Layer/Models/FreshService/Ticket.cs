using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.FreshService
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Ticket_CustomFields
    {
        //public object cf_location { get; set; }
        //public object closure_code { get; set; }
        //public object customer_monthly_report { get; set; }
        //public object customer_review { get; set; }
        //public object dispatch_type { get; set; }
        //public object elevated_call { get; set; }
        //public object escalated_ticket { get; set; }
        //public object itss_hp_support_state { get; set; }
        public string location { get; set; }
        public string major_incident_type { get; set; }
        public string nsd_member_name { get; set; }
        public string oem_case_id_logged { get; set; }
        public string on_roaster_engineer { get; set; }
        public string resolution_type { get; set; }
        //public object scheduled_date { get; set; }
        //public object sub_location { get; set; }
        public string support_type { get; set; }
        public string tenant { get; set; }
        public string ticket_mode { get; set; }
        public string ticket_monitoring_owner { get; set; }
        public string time_track_mandate { get; set; }
        public string user_type { get; set; }
        public string parent_ticket_id { get; set; }
        //public object escalation_note { get; set; }
        //public object business_impact { get; set; }
        //public object impacted_locations { get; set; }
        //public object no_of_customers_impacted { get; set; }
        public string resolution_remarks { get; set; }
        public string resource_name { get; set; }
        public string problem_statement { get; set; }
        public string oem_case_idif_any { get; set; }
        public string sales_account_manager { get; set; }
        public string sl_no { get; set; }
        public string pid { get; set; }
        public string model { get; set; }
        public string product { get; set; }
    }

    public class Ticket_RequestedFor
    {
        public string email { get; set; }
        public long id { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class Ticket_Requester
    {
        public string email { get; set; }
        public long id { get; set; }
        public string mobile { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class Root_Ticket
    {
        public IEnumerable<Ticket> tickets { get; set; }
        //soumik rev
        public int total {get;set;}
        //soumik rev
    }

    public class WrapperTicket
    {
        public Ticket ticket { get; set; }
    }

    public class Stats
    {
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public long ticket_id { get; set; }
        public string opened_at { get; set; }
        public bool group_escalated { get; set; }
        public int? inbound_count { get; set; }
        public string status_updated_at { get; set; }
        public int? outbound_count { get; set; }
        public string pending_since { get; set; }
        public string resolved_at { get; set; }
        public string closed_at { get; set; }
        public string first_assigned_at { get; set; }
        public string assigned_at { get; set; }
        public string agent_responded_at { get; set; }
        public string requester_responded_at { get; set; }
        public string first_responded_at { get; set; }
        public int? first_resp_time_in_secs { get; set; }
        public int? resolution_time_in_secs { get; set; }
    }

    public class Ticket
    {
        public string subject { get; set; }
        public long? group_id { get; set; }
        public long? department_id { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public string item_category { get; set; }
        public long? requester_id { get; set; }
        public long? responder_id { get; set; }
        public string due_by { get; set; }
        public bool fr_escalated { get; set; }
        public bool deleted { get; set; }
       // public bool spam { get; set; }
        //public object email_config_id { get; set; }
        //public List<object> fwd_emails { get; set; }
        //public List<object> reply_cc_emails { get; set; }
       // public List<object> cc_emails { get; set; }
        public bool is_escalated { get; set; }
        public string fr_due_by { get; set; }
        public int id { get; set; }
        public int? priority { get; set; }
        public int? status { get; set; }
        public int? source { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int workspace_id { get; set; }
        public long? requested_for_id { get; set; }
       // public object to_emails { get; set; }
        public string type { get; set; }
        //public string description { get; set; }
        public string description_text { get; set; }
        public Ticket_CustomFields custom_fields { get; set; }
        public Ticket_Requester requester { get; set; }
        public Ticket_RequestedFor requested_for { get; set; }
        public Stats stats { get; set; }
        public string department_name { get; set; }
        public int? tasks_dependency_type { get; set; }
        //soumik rev
        public List<string> tags { get; set; }
        //soumik rev

    }

}
