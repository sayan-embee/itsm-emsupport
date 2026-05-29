using Common.Layer.Models.Bot;
using Common.Layer.Models.ContractMaster;
using Common.Layer.Models.Report;
using Common.Layer.Models;
using DataAccess.Layer.Data.Common;
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
using System.Text.Json;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.CustomerPortal;
using Newtonsoft.Json;
using System.Dynamic;
using Common.Layer.Models.WebChatBot;

namespace DataAccess.Layer.Data.CustomerPortal
{
    public class CustomerPortalData : ICustomerPortalData
    {
        private readonly ILogger _logger;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;

        public CustomerPortalData(
            ILogger<CustomerPortalData> logger
            , TelemetryClient telemetryClient
            , IConfiguration config
            , ISQLDataAccess db)
        {
            _logger = logger;
            _db = db;
            _config = config;
        }

        #region Customer Details

        public async Task<List<CustomerDetailsModel>?> CustomerDetails_Get(CustomerDetailsModel dataModel)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("customerEmail", dataModel.CustomerEmail),
                    new SqlParameter("department_id", dataModel.department_id),
                    new SqlParameter("tenant", dataModel.tenant),
                    new SqlParameter("embee_crm_id", dataModel.embee_crm_id),
                    new SqlParameter("engagement_start_date", dataModel.engagement_start_date),
                    new SqlParameter("engagement_end_date", dataModel.engagement_end_date),
                    new SqlParameter("customer_portal_access", dataModel.customer_portal_access),
                    new SqlParameter("sap_customer_name", dataModel.sap_customer_name)
                };

                var result = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_T_FreshService_CustomerDetails_Get", sqlParams: parameters);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                var dataList = JsonConvert.DeserializeObject<List<CustomerDetailsModel>>(JsonConvert.SerializeObject(result.Tables[0])) ?? null;
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CustomerDetails_Get() execution failed.");
                throw;
            }
        }

        public async Task<dynamic> CP_FreshService_Tickets_Get(CustomerDetailsModel dataModel)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("pageNumber", dataModel.PageNumber),
                    new SqlParameter("pageSize", dataModel.PageSize),
                    new SqlParameter("transactionType", dataModel.TransactionType),
                    new SqlParameter("departmentIds", dataModel.DepartmentId_List),
                    new SqlParameter("ticketIds", dataModel.ticketId_List),
                    new SqlParameter("start_Date", dataModel.FromDate),
                    new SqlParameter("end_Date", dataModel.ToDate),
                    new SqlParameter("statusId", dataModel.StatusId)
                };

                var dataSet = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_T_FreshServiceTickets_Get", sqlParams: parameters);

                List<dynamic> results = new List<dynamic>();
                int totalRecords = 0;

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    results = ConvertDataSetToDynamicList(dataSet);
                }

                if (dataSet.Tables.Count > 1 && dataSet.Tables[1].Rows.Count > 0)
                {
                    totalRecords = Convert.ToInt32(dataSet.Tables[1].Rows[0][0]);
                }

                return new
                {
                    TicketList = results,
                    TotalCount = totalRecords
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CP_FreshService_Tickets_Get() execution failed.");
                throw;
            }
        }

        public async Task<dynamic> CP_CustomerWise_MasterData_Get(CustomerDetailsModel dataModel)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("embee_crm_ids", dataModel.embee_crm_id_List),
                    new SqlParameter("DepartmentIds", dataModel.DepartmentId_List)
                };

                var dataSet = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_CustomerWise_MasterData_Get", sqlParams: parameters);

                return new
                {
                    CategoryList = JsonConvert.DeserializeObject<List<CategoryModel>>(JsonConvert.SerializeObject(dataSet?.Tables[0])) ?? null,
                    SubCategoryList = JsonConvert.DeserializeObject<List<SubCategoryModel>>(JsonConvert.SerializeObject(dataSet?.Tables[1])) ?? null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CP_CustomerWise_MasterData_Get() execution failed.");
                throw;
            }
        }

        #endregion


        #region OTP

        public async Task<ReturnMessageModel> OTPLog_InsertUpdate(string transactionType, OTPModel dataModel)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "EmSupport.usp_CP_T_OTPLog_InsertUpdate",
                new
                {
                    TransactionType = transactionType,

                    dataModel.ReferenceNo,
                    dataModel.Code,
                    dataModel.ValidityInSec,
                    dataModel.CreatedOn,
                    dataModel.ExpiredOn,
                    dataModel.Recipient,
                    dataModel.SessionId,
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to execute OTPLog_InsertUpdate()");
                throw;
            }
        }

        #endregion


        #region Email

        public async Task<ReturnMessageModel> EmailLog_InsertUpdate(string transactionType, EmailModel dataModel)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "EmSupport.usp_CP_T_EmailLog_InsertUpdate",
                new
                {
                    dataModel.From,
                    dataModel.To,
                    dataModel.CC,
                    dataModel.Subject,
                    dataModel.Body,
                    dataModel.Status,
                    dataModel.Type,
                    dataModel.Message,
                    dataModel.CreatedOn,
                    dataModel.ReferenceNo,
                    dataModel.OTP_Id,
                    dataModel.SessionId,
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to execute EmailLog_InsertUpdate()");
                throw;
            }
        }

        #endregion


        #region Sign In

        public async Task<ReturnMessageModel> SignInLog_InsertUpdate(string transactionType, CustomerSignInModel dataModel)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "EmSupport.usp_CP_T_SignInLog_InsertUpdate",
                new
                {
                    transactionType,

                    dataModel.UserId,
                    dataModel.UserName,
                    dataModel.UserEmail,
                    //dataModel.SigninTimeUTC,
                    //dataModel.SignoutTimeUTC,
                    dataModel.ClientIP,
                    dataModel.UserAgent,
                    dataModel.DeviceType,
                    dataModel.Location,
                    dataModel.JWTTokenId,
                    dataModel.JWTTokenExpiredOn,
                    dataModel.SessionId,
                    dataModel.IsSessionActive,
                    dataModel.SignOutRemarks,
                    dataModel.OTPId
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to execute SignInLog_InsertUpdate()");
                throw;
            }
        }

        #endregion


        #region WebChatBot

        public async Task<WebChatLogModel?> DirectLineToken_Get(WebChatLogModel dataModel)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("UserEmail", dataModel.UserEmail),
                    new SqlParameter("SessionId", dataModel.SessionId),
                    new SqlParameter("ConversationType", dataModel.ConversationType)
                };

                var result = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_T_WebChat_Log_Get", sqlParams: parameters);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                var dataList = JsonConvert.DeserializeObject<List<WebChatLogModel>>(JsonConvert.SerializeObject(result.Tables[0])) ?? null;
                return dataList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DirectLineToken_Get() execution failed.");
                throw;
            }
        }

        public async Task<ReturnMessageModel> DirectLineToken_InsertUpdate(string transactionType, WebChatLogModel dataModel)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "EmSupport.usp_CP_T_WebChat_Log_InsertUpdate",
                new
                {
                    TransactionType = transactionType,

                    dataModel.UserId,
                    dataModel.UserName,
                    dataModel.UserEmail,
                    dataModel.LogId,
                    dataModel.SessionId,
                    dataModel.DirectLineToken,
                    dataModel.ConversationId,
                    dataModel.StreamUrl,
                    dataModel.ExpiredOn,
                    dataModel.CreatedOn,

                    dataModel.StartedOn,
                    dataModel.EndedOn,
                    dataModel.Active,

                    dataModel.FeedbackRatingId,
                    dataModel.AdditionalFeedback,
                    dataModel.SatisfiedWithResolution,


                    dataModel.ConversationType,
                    dataModel.SessionCloseRemarks
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DirectLineToken_InsertUpdate() execution failed.");
                throw;
            }
        }

        public async Task<List<WebChatOptionsModel>?> WebChatOptions_Get(string categoryIdList, string subCategoryIdList, int? top = null)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("CategoryIdList", categoryIdList),
                    new SqlParameter("SubCategoryIdList", subCategoryIdList),
                    new SqlParameter("Top", top)
                };

                var result = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_M_WebChat_Options_Get", sqlParams: parameters);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                var dataList = JsonConvert.DeserializeObject<List<WebChatOptionsModel>>(JsonConvert.SerializeObject(result.Tables[0])) ?? null;
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebChatOptions_Get() execution failed.");
                throw;
            }
        }

        public async Task<WebChatSOPIndexModel?> WebChatSOPIndex_Get(int categoryId, int subCategoryId)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("CategoryId", categoryId),
                    new SqlParameter("SubCategoryId", subCategoryId),
                };

                var result = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_M_WebChat_SOPIndex_Get", sqlParams: parameters);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                var dataList = JsonConvert.DeserializeObject<List<WebChatSOPIndexModel>>(JsonConvert.SerializeObject(result.Tables[0])) ?? null;
                return dataList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebChatSOPIndex_Get() execution failed.");
                throw;
            }
        }

        public async Task<List<WebChatFeedbackOptionsModel>?> WebChatFeedbackOptions_Get()
        {
            try
            {
                var result = await _db.LoadDataSet(storedProcedure: "EmSupport.usp_CP_M_WebChat_Feedback_Get", sqlParams: null);

                if (result == null || result.Tables.Count == 0 || result.Tables[0].Rows.Count == 0)
                    return null;

                var dataList = JsonConvert.DeserializeObject<List<WebChatFeedbackOptionsModel>>(JsonConvert.SerializeObject(result.Tables[0])) ?? null;
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebChatFeedbackOptions_Get() execution failed.");
                throw;
            }
        }

        public async Task<ReturnMessageModel> UserConversationLog_InsertUpdate(string transactionType, WebChatUserMessageModel data)
        {
            try
            {
                string fileJsonInput = System.Text.Json.JsonSerializer.Serialize(data.FileList);

                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "EmSupport.usp_CP_T_WebChat_UserConversationLog_InsertUpdate",
                new
                {
                    TransactionType = transactionType,

                    MessageId = data.MessageId,
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

                    CategoryId = data.CategoryId,
                    SubCategoryId = data.SubCategoryId,

                    WebChatLogId = data.WebChatLogId,
                    MessageActivityId = data.MessageActivityId,
                    MessageSentUTC = data.MessageSentUTC,
                    FeedbackCardActivityId = data.FeedbackCardActivityId,
                    FeedbackCardSentUTC = data.FeedbackCardSentUTC,
                    LikeDislike = data.LikeDislike,
                    FeedbackReceivedUTC = data.FeedbackReceivedUTC,

                    FileJSONInput = fileJsonInput
                });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserConversationLog_InsertUpdate() execution failed.");
                throw;
            }
        }

        #endregion


        #region Helper Methods

        private List<dynamic> ConvertDataSetToDynamicList(DataSet dataSet)
        {
            var result = new List<dynamic>();

            if (dataSet.Tables.Count > 0)
            {
                var table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    var obj = new ExpandoObject() as IDictionary<string, object>;

                    foreach (DataColumn column in table.Columns)
                    {
                        obj[column.ColumnName] = row[column] != DBNull.Value ? row[column] : null;
                    }

                    result.Add(obj);
                }
            }

            return result;
        }

        #endregion
    }
}
