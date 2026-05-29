using Common.Layer.Models;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.Report;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Layer.Data.FreshService
{
    public interface IFreshServiceData
    {
        Task<ReturnMessageModel> Departments_InsertUpdate(string jsonInput);
        Task<IEnumerable<DepartmentDB>> Get_Department(string name = null, long? id = null, int? pageRowIndex = 0, int? pageSize = 100);
        Task<ReturnMessageModel> Tickets_InsertUpdate(string jsonInput);
        //soumik rev start
        Task<ReturnMessageModel> TicketsByCreatedDate_InsertUpdate(string jsonInput);
        
        Task<List<MissingTicketModel>> Get_Missing_Ticket(long? TicketId = null);

        Task<ReturnMessageModel> MissingStats_InsertUpdate(string jsonInput);

        //soumik rev end

        Task<ReturnMessageModel> Requester_InsertUpdate(string jsonInput);

        Task<ReturnMessageModel> Changes_InsertUpdate(string jsonInput);

        Task<DataSet> Get_R_SummaryReport_PIVOT(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_SummaryLast3Months(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_SummaryResolutionPrioritySLA(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_SummaryResponsePrioritySLA(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketByAvgResponseResolutionSummary(long? departmentId, string start_date, string end_date);

        Task<DataSet> Get_R_TicketByDailyAndMonthlySummary(long? departmentId, string start_date, string end_date);


        Task<DataSet> Get_R_TicketByCategoryAndType(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketByCategoryAndPriority(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketByResourceName(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_ServiceRequestByUsers(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketNotClosed(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketExcel(long? departmentId, string start_date, string end_date);

        Task<ReturnMessageModel> Problem_InsertUpdate(string jsonInput);

        Task<List<KernelTicketDetails>> SemanticKernel_FreshServiceTickets_GetAll(KernelTicketDetails dataModel);
        
        //soumik rev 
        Task<DataSet> Get_R_TrendsLast3MonthsIncident(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_ResponsePerformance(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_ResolutionPerformance(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_CategoryWiseIncidents(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_NetworkCategoryPerformance(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_NetworkCategoryWiseTickets(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_ResourceWiseAlerts(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_TicketToolAnalysis(long? departmentId, string start_date, string end_date);
        Task<DataSet> Get_R_ChangeSummaryLast3MonthsTrend(long? departmentId, string start_date, string end_date);


        //
    }
}