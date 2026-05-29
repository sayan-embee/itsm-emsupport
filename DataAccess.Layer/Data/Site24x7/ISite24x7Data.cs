using Common.Layer.Models;
using Common.Layer.Models.Site24x7;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Layer.Data.Site24x7
{
    public interface ISite24x7Data
    {
        Task<AccessTokenDetails> Get(AccessTokenDetails dataModel);
        Task<ReturnMessageModel> Update(AccessTokenDetails model);

        Task<ReturnMessageModel> Per_Report_Server_InsertUpdate(string jsonInput, string zaaid, int param_period, int param_metric_aggregation, string param_start_date, string param_end_date);
        Task<ReturnMessageModel> Per_Report_Server_Monthly_InsertUpdate(string jsonInput, string zaaid, int param_period, int param_metric_aggregation, string param_start_date, string param_end_date);
        Task<IEnumerable<MSP_Customer>> GetMSP_Customer();

        Task<IEnumerable<Rpt_PerformaceReportModel>> Get_R_ServerPerformanceReport(string zaaid, string start_date, string end_date, long? departmentId);
    }
}