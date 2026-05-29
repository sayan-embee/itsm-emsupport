using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.FreshService
{
    
    
    public class DepartmentDB: Department_CustomFields
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public long? head_user_id { get; set; }
        public string head_name { get; set; }
        public long? prime_user_id { get; set; }
        public string prime_user_name { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
    public class Department
    {

        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public long? head_user_id { get; set; }
        public string head_name { get; set; }
        public long? prime_user_id { get; set; }
        public string prime_user_name { get; set; }
        public List<string> domains { get; set; }
        public Department_CustomFields custom_fields { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }

    }
    public class Department_CustomFields
    {
        public string location { get; set; }
        public string tenant { get; set; }
        public string embee_crm_id { get; set; }
        public string contact_person { get; set; }

        public string contact_number { get; set; }
        public string contact_email_id { get; set; }
        public string embee_account_manager { get; set; }

        public string engagement_start_date { get; set; }
        public string engagement_end_date { get; set; }

        public string customer_portal_access { get; set; }

        public string sap_customer_name { get; set; }
    }
    public class Departments
    {
        public IEnumerable<Department> departments { get; set; }
    }
}
