using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.FreshService
{
    public class Requester_CustomFields
    {
        public string employee_id { get; set; }
    }

    public class Requester
    {
        public bool active { get; set; }
        public string address { get; set; }
        public string background_information { get; set; }
        public bool can_see_all_changes_from_associated_departments { get; set; }
        public bool can_see_all_tickets_from_associated_departments { get; set; }
        public string created_at { get; set; }
        public Requester_CustomFields custom_fields { get; set; }
        public List<long?> department_ids { get; set; }
        public List<string> department_names { get; set; }
        public string external_id { get; set; }
        public string first_name { get; set; }
        public bool has_logged_in { get; set; }
        public long? id { get; set; }
        public bool is_agent { get; set; }
        public string job_title { get; set; }
        public string language { get; set; }
        public string last_name { get; set; }
        public long? location_id { get; set; }
        public string location_name { get; set; }
        public string mobile_phone_number { get; set; }
        public string primary_email { get; set; }
        public string reporting_manager_id { get; set; }
        public List<string> secondary_emails { get; set; }
        public string time_format { get; set; }
        public string time_zone { get; set; }
        public string updated_at { get; set; }
        public bool vip_user { get; set; }
        public string work_phone_number { get; set; }
        public long? work_schedule_id { get; set; }
    }

    public class RequesterModel
    {
        public IEnumerable<Requester> requesters { get; set; }
    }
    
}
