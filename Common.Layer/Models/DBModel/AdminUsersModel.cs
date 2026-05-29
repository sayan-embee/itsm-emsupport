using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Layer.Models.DBModel
{
    public class AdminUsersModel
    {
        
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }
        
        [JsonProperty("loggedUserName")]
        public string LoggedUserName { get; set; }

        [JsonProperty("loggedUserEmail")]
        public string LoggedUserEmail { get; set; }

        [JsonProperty("autoId")]
        public long AutoId { get; set; }

        [JsonProperty("adminADID")]
        public string AdminADID { get; set; }

        [JsonProperty("adminDisplayName")]
        public string AdminDisplayName { get; set; }

        [JsonProperty("adminEmail")]
        public string AdminEmail { get; set; }

        [JsonProperty("isActive")]
        public string IsActive { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdByEmail")]
        public string CreatedByEmail { get; set; }

        //[JsonProperty("createdByADID")]
        //public string CreatedByADID { get; set; }

        [JsonProperty("createdOnIST")]
        public DateTime? CreatedOnIST { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedByEmail")]
        public string UpdatedByEmail { get; set; }

        //[JsonProperty("updatedByADID")]
        //public string UpdatedByADID { get; set; }

        [JsonProperty("updatedOnIST")]
        public DateTime? UpdatedOnIST { get; set; }

    }
}
