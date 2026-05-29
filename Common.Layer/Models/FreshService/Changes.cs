using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.FreshService
{
   
    public class Change
    {
        public long? agent_id { get; set; }
        public long? group_id { get; set; }
        public int? priority { get; set; }
        public int? impact { get; set; }
        public int? status { get; set; }
        public int? risk { get; set; }
        public int? change_type { get; set; }
        public string planned_start_date { get; set; }
        public string planned_end_date { get; set; }
        public string subject { get; set; }
        public long? department_id { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public string item_category { get; set; }
       // public string description { get; set; }
        public string planned_effort { get; set; }
        public Change_CustomFields custom_fields { get; set; }
        public string description_text { get; set; }
        public int id { get; set; }
        public long? requester_id { get; set; }
        public int? approval_status { get; set; }
        public string change_window_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int? workspace_id { get; set; }
        public int? tasks_dependency_type { get; set; }
    }

    public class Change_CustomFields
    {
        public string tenant { get; set; }
        public bool elevated_call { get; set; }
        public string on_roaster_engineer { get; set; }
        public string nsd_member_name { get; set; }
        public string resolution_remarks { get; set; }
    }

    
    public class ChangeModel
    {
        public IEnumerable<Change> changes { get; set; }
    }
}
