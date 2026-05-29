using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Layer.Models;
using Common.Layer.Models.Enum;
using DataAccess.Layer.DbAccess;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.Site24x7;
using DataAccess.Layer.Data.Site24x7;
using System.Data.SqlClient;
using System.Data;
using Common.Layer.Models.CustomerPortal;
using Common.Layer.Models.Report;

namespace DataAccess.Layer.Data.FreshService
{
    public class FreshServiceData : IFreshServiceData
    {
        private readonly ILogger _logger;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;
       

        public FreshServiceData(
            ILogger<FreshServiceData> logger
             ,TelemetryClient telemetryClient
            , IConfiguration config
            , ISQLDataAccess db)
        {
            this._logger = logger;
            this._db = db;
            this._config = config;
        }
        #region Department
        /// <summary>
        /// This method is used to insert or update department.
        /// </summary>
        /// <param name="jsonInput">Json string as input.</param>
        /// <returns>Return model with status</returns>
        public async Task<ReturnMessageModel> Departments_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_M_Department_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save Departments");
                throw;
            }

        }
        /// <summary>
        /// This method is used to get the department details.
        /// </summary>
        /// <param name="name">Name of the department.</param>
        /// <param name="id">Id of the department.</param>
        /// <returns>List of department.</returns>
        public async Task<IEnumerable<DepartmentDB>> Get_Department(string name = null, long? id = null, int? pageRowIndex = 0, int? pageSize = 100)
        {
            try
            {
                return await _db.LoadData<DepartmentDB, dynamic>(storedProcedure: "usp_Site24x7_G_GetDepartment",
                    new
                    {
                        name = name,
                        id = id,
                        STARTROWINDEX = pageRowIndex,
                        MAXIMUMROWS = pageSize
                    });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_Site24x7_G_GetDepartment from DB execution failed.");
                throw;
            }
        }
        #endregion

        #region Tickets
        public async Task<ReturnMessageModel> Tickets_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_T_Tickets_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save tickets");
                throw;
            }

        }

        public async Task<List<KernelTicketDetails>> SemanticKernel_FreshServiceTickets_GetAll(KernelTicketDetails dataModel)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("TicketId", dataModel.TicketId),
                    new SqlParameter("department_name", dataModel.department_name),
                    new SqlParameter("department_id", dataModel.department_id),
                    new SqlParameter("category", dataModel.category),
                    new SqlParameter("sub_category", dataModel.sub_category),
                    new SqlParameter("created_at", dataModel.created_at),
                    new SqlParameter("created_at", dataModel.created_at),
                };

                var dataSet = await _db.LoadDataSet(storedProcedure: "usp_SemanticKernel_T_FreshServiceTickets_GetAll", sqlParams: parameters);

                if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                    return new List<KernelTicketDetails>();

                var dataList = JsonConvert.DeserializeObject<List<KernelTicketDetails>>(JsonConvert.SerializeObject(dataSet.Tables[0])) ?? null;
                return dataList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SemanticKernel_FreshServiceTickets_GetAll() execution failed.");
                throw;
            }
        }
        //soumik Rev Start
        public async Task<ReturnMessageModel> TicketsByCreatedDate_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_T_TicketsByCreatedDate_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save tickets");
                throw;
            }

        }

        public async Task<List<MissingTicketModel>> Get_Missing_Ticket(long? TicketId = null)
        {
            try
            {
                var results = await _db.LoadData<MissingTicketModel, dynamic>(
                    storedProcedure: "usp_FreshService_T_Get_MissingTickets",
                    new
                    {
                        TicketId = TicketId,
                    });

                return results.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"usp_FreshService_T_Get_MissingTickets execution failed.");
                throw;
            }
        }

        public async Task<ReturnMessageModel> MissingStats_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_T_Missing_Stat_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save tickets");
                throw;
            }

        }

        //soumik rev end
        #endregion

        #region Requester
        public async Task<ReturnMessageModel> Requester_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_M_Requester_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save requester");
                throw;
            }

        }
        #endregion

        #region Changes
        public async Task<ReturnMessageModel> Changes_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_T_Changes_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save changes");
                throw;
            }

        }
        #endregion

        #region Reports

        public async Task<DataSet> Get_R_SummaryReport_PIVOT(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_SummaryReport_PIVOT", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_SummaryReport_PIVOT from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_SummaryLast3Months(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_SummaryLast3Months", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_SummaryLast3Months from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_SummaryResolutionPrioritySLA(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_SummaryResolutionPrioritySLA", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_SummaryResolutionPrioritySLA from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_SummaryResponsePrioritySLA(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_SummaryResponsePrioritySLA", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_SummaryResponsePrioritySLA from DB execution failed.");
                throw;
            }
        }
        //soumik rev
        public async Task<DataSet> Get_R_TicketByAvgResponseResolutionSummary(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketByAvgResponseResolutionSummary", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketByAvgResponseResolutionSummary from DB execution failed.");
                throw;
            }
        }
        public async Task<DataSet> Get_R_TicketByDailyAndMonthlySummary(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketByDailyAndMonthlySummary", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketByDailyAndMonthlySummary from DB execution failed.");
                throw;
            }
        }
        //soumik rev

        public async Task<DataSet> Get_R_TicketByCategoryAndPriority(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketByCategoryAndPriority", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketByCategoryAndPriority from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_TicketByCategoryAndType(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketByCategoryAndType", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketByCategoryAndType from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_TicketByResourceName(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketByResourceName", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketByResourceName from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_ServiceRequestByUsers(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_ServiceRequestByUsers", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_ServiceRequestByUsers from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_TicketNotClosed(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TicketNotClosed", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TicketNotClosed from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_TicketExcel(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_TickesExcel", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get usp_FreshService_R_TickesExcel from DB execution failed.");
                throw;
            }
        }
        
        #endregion

        #region Problem
        public async Task<ReturnMessageModel> Problem_InsertUpdate(string jsonInput)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_FreshService_T_Problems_InsertUpdate",
                new
                {
                    jsonInput = jsonInput
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save changes");
                throw;
            }

        }

        //soumik rev
        public async Task<DataSet> Get_R_TrendsLast3MonthsIncident(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("DepartmentId", departmentId));
                parameters.Add(new SqlParameter("Start_Date", start_date));
                parameters.Add(new SqlParameter("End_Date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_TrendsLast3MonthsIncident", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_TrendsLast3MonthsIncident from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_ResponsePerformance(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_ResponsePerformance", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_ResponsePerformance from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_ResolutionPerformance(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_ResolutionPerformance", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_ResolutionPerformance from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_CategoryWiseIncidents(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_CategoryWiseIncidents", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_CategoryWiseIncidents from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_NetworkCategoryPerformance(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_NetworkCategoryPerformance", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_NetworkCategoryPerformance from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_NetworkCategoryWiseTickets(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_NetworkCategoryWiseTickets", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_NetworkCategoryWiseTickets from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_ResourceWiseAlerts(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_ResourceWiseAlerts", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_ResourceWiseAlerts from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_TicketToolAnalysis(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_TicketToolAnalysis", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_TicketToolAnalysis from DB execution failed.");
                throw;
            }
        }

        public async Task<DataSet> Get_R_ChangeSummaryLast3MonthsTrend(long? departmentId, string start_date, string end_date)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("departmentId", departmentId));
                parameters.Add(new SqlParameter("start_date", start_date));
                parameters.Add(new SqlParameter("end_date", end_date));

                var result = await _db.LoadDataSet(storedProcedure: "usp_FreshService_R_OnMobile_ChangeSummaryLast3MonthsTrend", sqlParams: parameters);
                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"usp_FreshService_R_OnMobile_ChangeSummaryLast3MonthsTrend from DB execution failed.");
                throw;
            }
        }


        // soumik rev
        #endregion

    }
}
