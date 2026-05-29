using Common.Layer.Extensions;
using Common.Layer.Models;
using Common.Layer.Models.ContractMaster;
using Common.Layer.Models.Report;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.FreshService;
using DataAccess.Layer.Data.Site24x7;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Dynamic;
using WebAPI.Layer.Authorization;
using WebAPI.Layer.ExceptionLog;
using WebAPI.Layer.Helpers.Files;
using WebAPI.Layer.Services;

namespace WebAPI.Layer.Controllers
{
    [Route("api/")]
    [ApiController]
    //[TypeFilter(typeof(APIKeyAuthorization))]
    public class MasterAPIController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommonData _commonData;
        private readonly IFileHelper _fileHelper;

        public MasterAPIController
        (
            ILogger<MasterAPIController> logger
            , IConfiguration configuration
            , ICommonData commonData
            , IFileHelper fileHelper
        )
        {
            this._logger = logger;
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            this._commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            this._fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
        }

        [HttpPost]
        [Route("getUserAccess")]
        [Description("Check user access for teams report tab")]
        public async Task<IActionResult> GetUserAccess(UserDetailsModel userModel)
        {
            try
            {
                var result = await this._commonData.Get_M_UserAccess(userModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetTeamsTabAccess()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("getDepartmentMst")]
        [Description("Get from department master")]
        public async Task<IActionResult> GetDepartmentMaster(DepartmentMasterModel dataModel)
        {
            try
            {
                var result = await this._commonData.Get_M_Department(dataModel.name, dataModel.id, dataModel.ReportType, dataModel.active);
                if (result == null || result.Tables.Count == 0) return Ok();

                var dataTable = result.Tables[0];
                string jsonResult = JsonConvert.SerializeObject(dataTable, Formatting.Indented);

                return this.Ok(jsonResult);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetDepartmentMaster()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }


        [HttpGet]
        [Route("getReportSectionMst")]
        [Description("Get from report section master for teams report tab")]
        public async Task<IActionResult> GetReportSectionMaster(bool active, long departmentId)
        {
            try
            {
                var result = await this._commonData.Get_M_ReportSection(active, departmentId);
                if (result == null || result.Tables.Count == 0) return Ok();

                var dataTable = result.Tables[0];
                string jsonResult = JsonConvert.SerializeObject(dataTable, Formatting.Indented);

                return this.Ok(jsonResult);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetReportSection()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("getMasterData")]
        [Description("Get Category, SubCategory, Tenant, Region Master Data")]
        public async Task<IActionResult> GetMasterData()
        {
            try
            {
                dynamic dynamicObject = new ExpandoObject();
                dynamicObject.CategoryList = new List<CategoryModel>();
                dynamicObject.SubCategoryList = new List<SubCategoryModel>();
                dynamicObject.TenantList = new List<TenantModel>();
                dynamicObject.RegionList = new List<RegionModel>();

                dynamicObject.CustomerList = new List<SAP_CustomerModel>();
                dynamicObject.DepartmentList = new List<SAP_DepartmentModel>();

                var result = await this._commonData.MasterData_Get();
                if (result == null || result.Tables.Count == 0) return Ok();

                if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
                {
                    dynamicObject.CategoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(JsonConvert.SerializeObject(result.Tables[0]));
                }
                if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
                {
                    dynamicObject.SubCategoryList = JsonConvert.DeserializeObject<List<SubCategoryModel>>(JsonConvert.SerializeObject(result.Tables[1]));
                }
                if (result.Tables.Count > 2 && result.Tables[2].Rows.Count > 0)
                {
                    dynamicObject.TenantList = JsonConvert.DeserializeObject<List<TenantModel>>(JsonConvert.SerializeObject(result.Tables[2]));
                }
                if (result.Tables.Count > 3 && result.Tables[3].Rows.Count > 0)
                {
                    dynamicObject.RegionList = JsonConvert.DeserializeObject<List<RegionModel>>(JsonConvert.SerializeObject(result.Tables[3]));
                }
                if (result.Tables.Count > 4 && result.Tables[4].Rows.Count > 0)
                {
                    dynamicObject.CustomerList = JsonConvert.DeserializeObject<List<SAP_CustomerModel>>(JsonConvert.SerializeObject(result.Tables[4]));
                }
                if (result.Tables.Count > 5 && result.Tables[5].Rows.Count > 0)
                {
                    dynamicObject.DepartmentList = JsonConvert.DeserializeObject<List<SAP_DepartmentModel>>(JsonConvert.SerializeObject(result.Tables[5]));
                }

                return this.Ok(dynamicObject);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetMasterData()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }


        #region Contract Master

        [HttpPost]
        [Route("contractMaster/save")]
        [Description("Insert or update contract master data")]
        public async Task<IActionResult> ContractMasterInsertUpdate([FromForm] IFormCollection formData)
        {
            try
            {
                ContractMasterModel? formModel = null;

                var strKey = formData.Keys.Where(x => x == "eventData").FirstOrDefault();
                if (strKey != null)
                {
                    var data = formData[strKey];
                    formModel = new ContractMasterModel();
                    formModel = JsonConvert.DeserializeObject<ContractMasterModel>(JObject.Parse(data).ToString());
                }

                #region Validation

                if (formModel == null)
                {
                    return this.Problem("Required parameter: eventData");
                }

                var validationErrors = new List<string>();

                if (formModel.TransactionType == "I")
                {
                    if (formModel.TenantId == null || formModel.TenantId == 0)
                    {
                        validationErrors.Add("Required parameter: TenantId / TenantName");
                    }

                    if (string.IsNullOrEmpty(formModel.CustomerId) || string.IsNullOrEmpty(formModel.CustomerName))
                    {
                        validationErrors.Add("Required parameter: CustomerId / CustomerName");
                    }

                    if (string.IsNullOrEmpty(formModel.DepartmentName) || (formModel.DepartmentId == null || formModel.DepartmentId == 0))
                    {
                        validationErrors.Add("Required parameter: DepartmentId / DepartmentName");
                    }

                    if (formModel.CategoryId == null || formModel.CategoryId == 0)
                    {
                        validationErrors.Add("Required parameter: CategoryId");
                    }

                    if (formModel.SubCategoryId == null || formModel.SubCategoryId == 0)
                    {
                        validationErrors.Add("Required parameter: SubCategoryId");
                    }

                    if (formModel.StartDate == null || formModel.EndDate == null)
                    {
                        validationErrors.Add("Required parameter: StartDate / EndDate");
                    }

                    //if (string.IsNullOrEmpty(formModel.Acc_ManagerName) || string.IsNullOrEmpty(formModel.Acc_ManagerEmail))
                    //{
                    //    validationErrors.Add("Required parameter: Acc_ManagerName / Acc_ManagerEmail");
                    //}
                }
                else if (formModel.TransactionType == "E")
                {
                    if (formModel.Id == 0)
                    {
                        validationErrors.Add("Required parameter: Id");
                    }

                    if (formModel.ExtendSupport == null)
                    {
                        validationErrors.Add("Required parameter: ExtendSupport");
                    }
                }

                if (validationErrors.Any())
                {
                    return this.Problem(string.Join(" | ", validationErrors));
                }

                #endregion


                var dbResult = await this._commonData.ContractMaster_InsertUpdate(formModel.TransactionType, formModel);
                if (dbResult != null && !string.IsNullOrEmpty(dbResult.Id))
                {
                    if (formModel.Id == 0 && int.TryParse(dbResult.Id, out int parsedId) && parsedId > 0)
                    {
                        formModel.Id = parsedId;
                        formModel.ContractNo = dbResult.ReferenceNo;
                    }

                    _ = this.BackgroundProcess_ContractMasterInsertUpdate(formModel.TransactionType, formModel, formData.Files);
                }

                return Ok(dbResult);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetTeamsTabAccess()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }

        private async Task BackgroundProcess_ContractMasterInsertUpdate(string transactionType, ContractMasterModel dataModel, IFormFileCollection formFileList)
        {
            if (dataModel != null && dataModel.Id > 0)
            {
                if (formFileList != null && formFileList.Any())
                {
                    var uploadResult = await this._fileHelper.UploadFilesOnServer(dataModel, formFileList);
                    if (uploadResult != null)
                    {
                        await this._commonData.ContractMaster_InsertUpdate("FILES", uploadResult);
                    }
                }

                await this._fileHelper.DeleteFilesOnServer(dataModel);
            }
        }


        [HttpPost]
        [Route("contractMaster/get")]
        [Description("Get contract master data")]
        public async Task<IActionResult> ContractMasterGet(ContractMasterModel dataModel)
        {
            try
            {
                if (dataModel.Id > 0)
                {
                    var returnObject = new ContractMasterModel
                    {
                        FileList = new List<ContractMasterFilesModel>()
                    };

                    var result = await this._commonData.ContractMaster_Get(dataModel);
                    if (result == null || result.Tables.Count < 1) return Ok(returnObject);

                    var contractList = JsonConvert.DeserializeObject<List<ContractMasterModel>>(JsonConvert.SerializeObject(result.Tables[0]));
                    if (contractList != null && contractList.Count > 0)
                    {
                        returnObject = contractList[0];
                        returnObject.FileList = new List<ContractMasterFilesModel>();
                    }

                    if (result.Tables.Count > 1)
                    {
                        returnObject.FileList = JsonConvert.DeserializeObject<List<ContractMasterFilesModel>>(JsonConvert.SerializeObject(result.Tables[1])) ?? new List<ContractMasterFilesModel>();
                    }

                    return Ok(returnObject);
                }
                else
                {
                    var returnObject = new List<ContractMasterModel>();

                    var result = await this._commonData.ContractMaster_Get(dataModel);
                    if (result == null || result.Tables.Count < 1) return Ok(returnObject);

                    var contractList = JsonConvert.DeserializeObject<List<ContractMasterModel>>(JsonConvert.SerializeObject(result.Tables[0]));
                    if (contractList != null && contractList.Count > 0)
                    {
                        returnObject = contractList;
                    }

                    return Ok(returnObject);
                }                
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at ContractMasterGet()");
                ExceptionLogging.SendErrorToText(ex);
                return this.Problem(ex.Message);
            }
        }

        #endregion

    }
}
