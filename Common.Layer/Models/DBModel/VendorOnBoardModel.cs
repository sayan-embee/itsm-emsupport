using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Layer.Models.DBModel
{
    public class VendorOnBoard
    {
        public int? listId { get; set; }
        public string sys_key { get; set; }
        public string sys_key_description { get; set; }
        public string group_key { get; set; }
        public string group_key_description { get; set; }
        public string vendor_account_group { get; set; }
        public string vendor_type { get; set; }
        public string company_code { get; set; }
        public string company_code_description { get; set; }
        public string vendor_name { get; set; }
        public string vendor_email { get; set; }
        public string vendor_phoneno { get; set; }
        public string requester_name { get; set; }
        public string requester_email { get; set; }

    }


    public class ReturnModel
    {
        public long? listId { get; set; }
        public bool? status { get; set; }
        public string message { get; set; }
    }
}
