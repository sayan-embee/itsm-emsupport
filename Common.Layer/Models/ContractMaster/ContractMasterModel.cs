using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.ContractMaster
{
    public class ContractMasterModel : TransactionModel
    {
        [JsonProperty("slNo")]
        public long SlNo { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("referenceNo")]
        public string? ReferenceNo { get; set; }

        [JsonProperty("contractNo")]
        public string? ContractNo { get; set; }

        [JsonProperty("tenantId")]
        public int? TenantId { get; set; }

        [JsonProperty("tenantName")]
        public string? TenantName { get; set; }

        [JsonProperty("customerId")]
        public string? CustomerId { get; set; }

        [JsonProperty("customerName")]
        public string? CustomerName { get; set; }

        [JsonProperty("departmentId")]
        public long? DepartmentId { get; set; }

        [JsonProperty("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonProperty("categoryId")]
        public int? CategoryId { get; set; }

        [JsonProperty("categoryCode")]
        public string? CategoryCode { get; set; }

        [JsonProperty("categoryName")]
        public string? CategoryName { get; set; }

        [JsonProperty("subCategoryId")]
        public int? SubCategoryId { get; set; }

        [JsonProperty("subCategoryCode")]
        public string? SubCategoryCode { get; set; }

        [JsonProperty("subCategoryName")]
        public string? SubCategoryName { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("regionId")]
        public int? RegionId { get; set; }

        [JsonProperty("regionName")]
        public string? RegionName { get; set; }

        [JsonProperty("acc_ManagerName")]
        public string? Acc_ManagerName { get; set; }

        [JsonProperty("acc_ManagerEmail")]
        public string? Acc_ManagerEmail { get; set; }

        [JsonProperty("contactPersonName")]
        public string? ContactPersonName { get; set; }

        [JsonProperty("contactPersonEmail")]
        public string? ContactPersonEmail { get; set; }

        [JsonProperty("contactPersonPhone")]
        public string? ContactPersonPhone { get; set; }

        [JsonProperty("poNo")]
        public string? PONo { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("createdByName")]
        public string? CreatedByName { get; set; }

        [JsonProperty("createdByEmail")]
        public string? CreatedByEmail { get; set; }

        [JsonProperty("createdOn")]
        public DateTime? CreatedOn { get; set; }

        [JsonProperty("modifiedByName")]
        public string? ModifiedByName { get; set; }

        [JsonProperty("modifiedEmail")]
        public string? ModifiedEmail { get; set; }

        [JsonProperty("modifiedOn")]
        public DateTime? ModifiedOn { get; set; }

        [JsonProperty("activeStatusId")]
        public int? ActiveStatusId { get; set; }

        [JsonProperty("activeStatus")]
        public string?  ActiveStatus { get; set; }

        [JsonProperty("extendSupport")]
        public bool? ExtendSupport { get; set; }

        [JsonProperty("fileList")]
        public List<ContractMasterFilesModel>? FileList { get; set; }
    }

    public class ContractMasterFilesModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("contractId")]
        public long? ContractId { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("internalName")]
        public string? InternalName { get; set; }

        [JsonProperty("contentType")]
        public string? ContentType { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("physicalPath")]
        public string? PhysicalPath { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("createdByName")]
        public string? CreatedByName { get; set; }

        [JsonProperty("createdByEmail")]
        public string? CreatedByEmail { get; set; }

        [JsonProperty("createdOnUtc")]
        public DateTime? CreatedOnUTC { get; set; }

        [JsonProperty("modifiedByName")]
        public string? ModifiedByName { get; set; }

        [JsonProperty("modifiedEmail")]
        public string? ModifiedEmail { get; set; }

        [JsonProperty("modifiedOnUtc")]
        public DateTime? ModifiedOnUTC { get; set; }
    }
}
