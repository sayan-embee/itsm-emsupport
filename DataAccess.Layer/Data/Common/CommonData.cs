using Common.Layer.Models;
using Common.Layer.Models.Enum;
using Common.Layer.Models.Report;
using Common.Layer.Models.Site24x7;
using DataAccess.Layer.DbAccess;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Common.Layer.Models.Bot;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using Common.Layer.Models.ContractMaster;

namespace DataAccess.Layer.Data.Common
{
    public class CommonData : ICommonData
    {
        private readonly ILogger _logger;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;

        public CommonData(
            ILogger<CommonData> logger
            , TelemetryClient telemetryClient
            , IConfiguration config
            , ISQLDataAccess db)
        {
            this._logger = logger;
            this._db = db;
            this._config = config;
        }

        #region Teams-Bot-Conversation

        public async Task<ConversationModel> Get_M_ConversationByUserId(Guid userId)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("dbo.USP_G_ConversationByUserId", new { UserId = userId });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable execute Get_M_ConversationByUserId - user id :{userId}.");
                throw;
            }
        }

        public async Task<ReturnMessageModel> BotInstallUninstall_InsertUpdate(ConversationModel conversation)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "Usp_bot_Conversation_InsUp",
                new
                {
                    ActivityId = conversation.ActivityId,
                    ConversationId = conversation.ConversationId,
                    RecipientId = conversation.RecipientId,
                    RecipientName = conversation.RecipientName,
                    ServiceUrl = conversation.ServiceUrl,
                    UserEmail = conversation.UserEmail,
                    TenantId = conversation.TenantId,
                    UserId = conversation.UserId,
                    UserName = conversation.UserName,
                    UserPrincipalName = conversation.UserPrincipalName,
                    BotActiveInactive = conversation.Active,
                    AppName = conversation.AppName,
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable execute Get_M_BotInstallUninstall");
                throw;
            }
        }

        #endregion


        #region Teams-Tab-Access Check

        public async Task<UserDetailsModel?> Get_M_UserAccess(UserDetailsModel userModel)
        {
            try
            {
                var results = await _db.LoadData<UserDetailsModel, dynamic>(storedProcedure: "usp_M_GetReportUserAccess",
                new
                {
                    UserEmail = userModel.UserEmail,
                    TeamsTab = userModel.TeamsTab,
                    MonthlyReportTab = userModel.MonthlyReportTab,
                    ContractTab = userModel.ContractTab,
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get_M_UserAccess() execution failed.");
                throw;
            }
        }

        #endregion


        #region Teams-BOT-UserSearch

        public async Task<ReturnMessageModel> TeamsBot_UserSearch_InsertUpdate(UserMessageModel data)
        {
            try
            {
                string fileJsonInput = JsonSerializer.Serialize(data.FileList);

                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_TeamsBot_T_UserSearch_InsertUpdate",
                new
                {
                    Name = data.Name,
                    Email = data.Email,
                    UPN = data.UPN,
                    ADID = data.ADID,
                    ChannelId = data.ChannelId,
                    ConversationType = data.ConversationType,
                    ConversationId = data.ConversationId,
                    TenantId = data.TenantId,
                    ChatId = data.ChatId,
                    LocalTimestamp = data.LocalTimestamp,
                    Locale = data.Locale,
                    ServiceUrl = data.ServiceUrl,
                    Text = data.Text,
                    TextFormat = data.TextFormat,
                    Timestamp = data.Timestamp,
                    Response = data.Response,
                    Intent = data.Intent,
                    QuerySucceed = data.QuerySucceed,
                    FileJSONInput = fileJsonInput
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable execute TeamsBot_UserSearch_InsertUpdate");
                throw;
            }
        }

        #endregion


        #region Teams-Tab-Report-Master

        public async Task<DataSet> Get_M_Department(string name, long? id, string ReportType, bool? active)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("name", name));
                parameters.Add(new SqlParameter("id", id));
                parameters.Add(new SqlParameter("ReportType", ReportType));
                parameters.Add(new SqlParameter("active", active));

                var result = await _db.LoadDataSet(storedProcedure: "usp_R_GetDepartment", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_R_GetDepartment from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_M_ReportSection(bool? active, long? departmentId)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("active", active));
                parameters.Add(new SqlParameter("DepartmentId", departmentId));

                var result = await _db.LoadDataSet(storedProcedure: "usp_R_GetReportSection", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_R_GetReportSection from DB execution failed.");
                throw;
            }
        }

        #endregion


        #region Category-SubCategory-Tenant-Region-Master

        /// <summary>
        /// Get Category-SubCategory-Tenant-Region-MasterData
        /// </summary>
        /// <returns>Returns Category in DataTable1, SubCategory in DataTable2, Tenant in DataTable3, Region in DataTable4</returns>
        public async Task<DataSet> MasterData_Get()
        {
            try
            {
                //List<SqlParameter> parameters = new List<SqlParameter>();
                //parameters.Add(new SqlParameter("active", active));

                var result = await _db.LoadDataSet(storedProcedure: "usp_CP_MasterData_Get", sqlParams: null);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterData_Get() execution failed.");
                throw;
            }
        }

        #endregion


        #region Category Master

        public async Task<List<CategoryModel>?> GetCategoryMaster()
        {
            try
            {
                var categoryMasterList = new List<CategoryModel>
                {
                    //new CategoryModel("001", "Azure/AWS", ""),
                    //new CategoryModel("002", "MWP", ""),
                    //new CategoryModel("003", "SI & Infra", ""),
                    //new CategoryModel("004", "HyBrid", "")
                };

                await Task.Delay(1000);
                return categoryMasterList;

                //var results = await _db.LoadData<AccessTokenDetails, dynamic>(storedProcedure: "usp_Site24x7_AccessToken_Get",
                //new
                //{
                //    ClientId = dataModel.client_id ?? null
                //});

                //return results != null && results.Any() ? results.ToList().First() : null;

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get Access Token from DB execution failed.");
                throw;
            }
        }

        #endregion


        #region Sub-Category Master

        public async Task<List<SubCategoryModel>?> GetSubCategoryMaster(string code = "")
        {
            try
            {
                var subCategoryMasterList = new List<SubCategoryModel>
                {
                    //// Azure/AWS Subcategories
                    //new SubCategoryMaster("001", "111", "Azure All", ""),
                    //new SubCategoryMaster("001", "112", "Azure Infra", ""),
                    //new SubCategoryMaster("001", "113", "Azure DBM", ""),
                    //new SubCategoryMaster("001", "114", "Azure Analytics", ""),
                    //new SubCategoryMaster("001", "115", "Azure Security", ""),
                    //new SubCategoryMaster("001", "116", "AWS", ""),
                    //new SubCategoryMaster("001", "117", "Public Cloud", ""),
                    //new SubCategoryMaster("001", "118", "Azure Cloud Native", ""),

                    //// MWP Subcategories
                    //new SubCategoryMaster("002", "221", "O365", ""),
                    //new SubCategoryMaster("002", "222", "M365", ""),
                    //new SubCategoryMaster("002", "223", "MWP Security", ""),
                    //new SubCategoryMaster("002", "224", "MWP OnPrem", ""),
                    //new SubCategoryMaster("002", "225", "EMS", ""),
                    //new SubCategoryMaster("002", "226", "MWP", ""),
                    //new SubCategoryMaster("002", "227", "MWP Power Platform", ""),
                    //new SubCategoryMaster("002", "228", "MWP App", ""),

                    //// SI & Infra Subcategories
                    //new SubCategoryMaster("003", "331", "Infra ES", ""),
                    //new SubCategoryMaster("003", "332", "Infra EUS", ""),
                    //new SubCategoryMaster("003", "333", "On Prem Database", ""),
                    //new SubCategoryMaster("003", "334", "On Prem Backup", ""),
                    //new SubCategoryMaster("003", "335", "On Prem Network", ""),
                    //new SubCategoryMaster("003", "336", "Open-Source OS", ""),
                    //new SubCategoryMaster("003", "337", "Virtualization", ""),


                    //// HyBrid Subcategories
                    //new SubCategoryMaster("004", "441", "Azure + MWP", ""),
                    //new SubCategoryMaster("004", "442", "Azure + Infra", ""),
                    //new SubCategoryMaster("004", "443", "Azure + MWP + Infra", ""),
                    //new SubCategoryMaster("004", "444", "RE", ""),
                    //new SubCategoryMaster("004", "445", "RE + NOC", ""),
                    //new SubCategoryMaster("004", "446", "All Combination", ""),
                    //new SubCategoryMaster("004", "447", "Infra + Virtual Env", ""),
                    //new SubCategoryMaster("004", "448", "Network + Security", ""),
                };

                var filteredSubCategories = subCategoryMasterList
                .Where(sc => sc.CategoryId.ToString() == code)
                .ToList();

                await Task.Delay(1000);
                return filteredSubCategories;

                //var results = await _db.LoadData<AccessTokenDetails, dynamic>(storedProcedure: "usp_Site24x7_AccessToken_Get",
                //new
                //{
                //    ClientId = dataModel.client_id ?? null
                //});

                //return results != null && results.Any() ? results.ToList().First() : null;

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get Access Token from DB execution failed.");
                throw;
            }
        }

        #endregion


        #region Contract-Master

        public async Task<ReturnMessageModel> ContractMaster_InsertUpdate(string transactionType, ContractMasterModel dataModel)
        {
            try
            {
                string fileJsonInput = JsonSerializer.Serialize(dataModel.FileList);

                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_CP_T_ContractMaster_InsertUpdate",
                new
                {
                    TransactionType = transactionType,

                    Id = dataModel.Id,
                    ContractNo = dataModel.ContractNo,
                    TenantId = dataModel.TenantId,
                    TenantName = dataModel.TenantName,
                    CustomerId = dataModel.CustomerId,
                    CustomerName = dataModel.CustomerName,
                    DepartmentId = dataModel.DepartmentId,
                    DepartmentName = dataModel.DepartmentName,
                    StartDate = dataModel.StartDate,
                    EndDate = dataModel.EndDate,
                    Acc_ManagerName = dataModel.Acc_ManagerName,
                    Acc_ManagerEmail = dataModel.Acc_ManagerEmail,
                    PONo = dataModel.PONo,
                    CategoryId = dataModel.CategoryId,
                    SubCategoryId = dataModel.SubCategoryId,
                    RegionId = dataModel.RegionId,
                    Active = dataModel.Active,

                    CreatedByName = dataModel.CreatedByName,
                    CreatedByEmail = dataModel.CreatedByEmail,
                    ModifiedByName = dataModel.ModifiedByName,
                    ModifiedEmail = dataModel.ModifiedEmail,

                    ExtendSupport = dataModel.ExtendSupport,

                    FilesJSONInput = fileJsonInput
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to execute ContractMaster_InsertUpdate()");
                throw;
            }
        }

        public async Task<DataSet> ContractMaster_Get(ContractMasterModel dataModel)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("Id", dataModel.Id));
                parameters.Add(new SqlParameter("ContractNo", dataModel.ContractNo));
                parameters.Add(new SqlParameter("TenantId", dataModel.TenantId));
                parameters.Add(new SqlParameter("TenantName", dataModel.TenantName));
                parameters.Add(new SqlParameter("CustomerId", dataModel.CustomerId));
                parameters.Add(new SqlParameter("CustomerName", dataModel.CustomerName));
                parameters.Add(new SqlParameter("DepartmentId", dataModel.DepartmentId));
                parameters.Add(new SqlParameter("DepartmentName", dataModel.DepartmentName));
                parameters.Add(new SqlParameter("CategoryId", dataModel.CategoryId));
                parameters.Add(new SqlParameter("SubCategoryId", dataModel.SubCategoryId));
                parameters.Add(new SqlParameter("StartDate", dataModel.StartDate));
                parameters.Add(new SqlParameter("EndDate", dataModel.EndDate));
                parameters.Add(new SqlParameter("RegionId", dataModel.RegionId));
                parameters.Add(new SqlParameter("Acc_ManagerName", dataModel.Acc_ManagerName));
                parameters.Add(new SqlParameter("Acc_ManagerEmail", dataModel.Acc_ManagerEmail));
                parameters.Add(new SqlParameter("PONo", dataModel.PONo));
                parameters.Add(new SqlParameter("Active", dataModel.Active));

                var result = await _db.LoadDataSet(storedProcedure: "usp_CP_T_ContractMaster_Get", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"ContractMaster_Get() execution failed.");
                throw;
            }
        }

        #endregion


    }
}
