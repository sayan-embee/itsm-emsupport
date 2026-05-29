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
using System.Data.SqlClient;
using System.Data;
using Common.Layer.Models.Site24x7;

namespace DataAccess.Layer.Data.Site24x7
{
    public class Site24x7Data : ISite24x7Data
    {
        private readonly ILogger _logger;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;

        public Site24x7Data(
            ILogger<AccessTokenDetails> logger
            , TelemetryClient telemetryClient
            , IConfiguration config
            , ISQLDataAccess db)
        {
            this._logger = logger;
            this._db = db;
            this._config = config;
        }
        #region
        public async Task<AccessTokenDetails?> Get(AccessTokenDetails? dataModel)
        {
            try
            {
                
                var results = await _db.LoadData<AccessTokenDetails, dynamic>(storedProcedure: "usp_Site24x7_AccessToken_Get",
           new
           {
               ClientId = dataModel.client_id ?? null
           });

                return results != null && results.Any() ? results.ToList().First() : null;

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get Access Token from DB execution failed.");
                throw;
            }
        }
        public async Task<ReturnMessageModel> Update(AccessTokenDetails model)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_Site24x7_AccessToken_Update",
                new
                {
                    ClientID = model.client_id,
                    AccessToken = model.access_token,
                    RefreshToken = model.refresh_token,
                    ExpiresIn = model.expires_in,
                    ExpiresStarts = model.ExpiresStarts,
                    ExpiresOn = model.ExpiresOn
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Update Access Token");
                throw;
            }
        }

        public async Task<ReturnMessageModel> Per_Report_Server_InsertUpdate(string jsonInput, string zaaid,int param_period,int param_metric_aggregation,string param_start_date,string param_end_date)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_Site24x7_T_Per_Report_Server_InsertUpdate",
                new
                {
                    jsonInput = jsonInput,
                    zaaid = zaaid,
                    param_period = param_period,
                    param_metric_aggregation = param_metric_aggregation,
                    param_start_date = param_start_date,
                    param_end_date = param_end_date
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save Per_Report_Server_InsertUpdate");
                throw;
            }
        }


        public async Task<IEnumerable<MSP_Customer>> GetMSP_Customer()
        {
            try
            {
                return await _db.LoadData<MSP_Customer, dynamic>(storedProcedure: "usp_Site24x7_MSP_Customer_Get", new { });  
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get MSP_Customer from DB execution failed.");
                throw;
            }
        }


        public async Task<IEnumerable<Rpt_PerformaceReportModel>> Get_R_ServerPerformanceReport(string zaaid,string start_date,string end_date, long? departmentId)
        {
            try
            {
                return await _db.LoadData<Rpt_PerformaceReportModel, dynamic>(storedProcedure: "usp_Site24x7_R_ServerPerformanceReport",
                    new {
                        departmentid = departmentId,
                        zaaid =zaaid,
                        start_date=start_date,
                        end_date=end_date
                    });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Get Rpt_PerformaceReportModel from DB execution failed.");
                throw;
            }
        }
        //soumik rev
        public async Task<ReturnMessageModel> Per_Report_Server_Monthly_InsertUpdate(string jsonInput, string zaaid, int param_period, int param_metric_aggregation, string param_start_date, string param_end_date)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_Site24x7_T_Per_Report_Server_Monthly_InsertUpdate",
                new
                {
                    jsonInput = jsonInput,
                    zaaid = zaaid,
                    param_period = param_period,
                    param_metric_aggregation = param_metric_aggregation,
                    param_start_date = param_start_date,
                    param_end_date = param_end_date
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to save Per_Report_Server_InsertUpdate");
                throw;
            }
        }

        //soumik rev
        #endregion
    }
}
