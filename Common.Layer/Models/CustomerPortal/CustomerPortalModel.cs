using Common.Layer.Models.ContractMaster;
using Common.Layer.Models.FreshService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.CustomerPortal
{
    public class CustomerPortalModel{ }
    public class CustomerSignInModel
    {
        [JsonProperty("logId")]
        public long? LogId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("signinTimeUTC")]
        public DateTime? SigninTimeUTC { get; set; }

        [JsonProperty("signoutTimeUTC")]
        public DateTime? SignoutTimeUTC { get; set; }

        [JsonProperty("clientIP")]
        public string ClientIP { get; set; }

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        [JsonProperty("deviceType")]
        public string DeviceType { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("jwtTokenId")]
        public string JWTTokenId { get; set; }

        [JsonProperty("jwtTokenExpiredOn")]
        public DateTime? JWTTokenExpiredOn { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("isSessionActive")]
        public bool? IsSessionActive { get; set; }

        [JsonProperty("signOutRemarks")]
        public string SignOutRemarks { get; set; }

        [JsonProperty("otpId")]
        public long? OTPId { get; set; }

        [JsonProperty("failedLoginAttempts")]
        public int? FailedLoginAttempts { get; set; }

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }
    }


    public class CustomerDetailsModel : CustomerFilterModel
    {
        [JsonProperty("customerId")]
        public string? CustomerId { get; set; }

        [JsonProperty("customerName")]
        public string? CustomerName { get; set; }

        [JsonProperty("customerEmail")]
        public string? CustomerEmail { get; set; }

        [JsonProperty("customerPhone")]
        public string? CustomerPhone { get; set; }

        [JsonProperty("customerAddress")]
        public string? CustomerAddress { get; set; }



        [JsonProperty("department_id")]
        public long? department_id { get; set; }

        [JsonProperty("department_name")]
        public string? department_name { get; set; }

        [JsonProperty("tenant")]
        public string? tenant { get; set; }

        [JsonProperty("embee_crm_id")]
        public string? embee_crm_id { get; set; }

        [JsonProperty("customer_portal_access")]
        public string? customer_portal_access { get; set; }

        [JsonProperty("sap_customer_name")]
        public string? sap_customer_name { get; set; }

        [JsonProperty("first_name")]
        public string? first_name { get; set; }

        [JsonProperty("last_name")]
        public string? last_name { get; set; }

        [JsonProperty("job_title")]
        public string? job_title { get; set; }

        [JsonProperty("embee_account_manager")]
        public string? embee_account_manager { get; set; }

        [JsonProperty("engagement_start_date")]
        public DateTime? engagement_start_date { get; set; }

        [JsonProperty("engagement_end_date")]
        public DateTime? engagement_end_date { get; set; }
        
    }


    public class CustomerFilterModel
    {
        [JsonProperty("transactionType")]
        public string? TransactionType { get; set; }

        [JsonProperty("totalRecords")]
        public int? TotalRecords { get; set; }

        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [JsonProperty("fromDate")]
        public string? FromDate { get; set; }

        [JsonProperty("toDate")]
        public string? ToDate { get; set; }

        [JsonProperty("statusId")]
        public int? StatusId { get; set; }

        [JsonProperty("embee_crm_id_List")]
        public string? embee_crm_id_List { get; set; }

        [JsonProperty("departmentId_List")]
        public string? DepartmentId_List { get; set; }

        [JsonProperty("ticketId_List")]
        public string? ticketId_List { get; set; }

        [JsonProperty("portalAccessMsg")]
        public string? PortalAccessMsg { get; set; }
    }
}