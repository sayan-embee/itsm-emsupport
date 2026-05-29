using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.FreshService
{
    public class Problem
    {
        public long? agent_id { get; set; }
        public long? group_id { get; set; }
        public int? priority { get; set; }
        public int? impact { get; set; }
        public int? status { get; set; }
        public string planned_start_date { get; set; }
        public bool? known_error { get; set; }
        public string due_by { get; set; }
        public string planned_end_date { get; set; }
        public string subject { get; set; }
        public long? department_id { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; }
        public string item_category { get; set; }
        //public string description { get; set; }
        public string planned_effort { get; set; }
        public Problem_CustomFields custom_fields { get; set; }
        public string description_text { get; set; }
        public int id { get; set; }
        public long? requester_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int? workspace_id { get; set; }
        public int? tasks_dependency_type { get; set; }
    }

    public class Problem_CustomFields
    {
        public string nsd_member_name { get; set; }
        public string on_roaster_engineer { get; set; }
    }


    public class ProblemModel
    {
        public IEnumerable<Problem> problems { get; set; }
    }
  
}
