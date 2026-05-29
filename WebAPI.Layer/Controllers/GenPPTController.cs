using Common.Layer.Models.Report;
using Common.Layer.Models.Site24x7;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.FreshService;
using DataAccess.Layer.Data.Site24x7;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using WebAPI.Layer.Authorization;
using WebAPI.Layer.ExceptionLog;
using WebAPI.Layer.Services;

namespace WebAPI.Layer.Controllers
{
    [Route("api/")]
    [ApiController]
    //[TypeFilter(typeof(APIKeyAuthorization))]
    public class GenPPTController : ControllerBase
    {
        private readonly IFreshServiceData _freshServiceData;
        private readonly ISite24x7Data _site24X7Data;
        private readonly ICommonData _commonData;
        private readonly ILogger _logger;
        private readonly IGenPPTService _genPPTService;
        private readonly IGenExcelService _genExcelService;
        private readonly IConfiguration _configuration;


        public GenPPTController
        (
            IFreshServiceData freshServiceData
            , ISite24x7Data site24X7Data
            , ICommonData commonData
            , IGenPPTService genPPTService
            , IGenExcelService genExcelService
            , ILogger<GenPPTController> logger
            , IConfiguration configuration
        )
        {
            this._freshServiceData = freshServiceData;
            this._site24X7Data = site24X7Data;
            this._genPPTService = genPPTService;
            this._genExcelService = genExcelService;
            this._commonData = commonData;
            this._logger = logger;
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
        }

        [HttpPost]
        [Route("GetReports")]
        public async Task<IActionResult> GetReports(FilterModel filter)
        {
            try
            {
                if (filter.departmentId == 0)
                {
                    throw new ArgumentException("The Department is null or empty.");
                }
                //if (filter.departmentId == 1)
                //{
                //    this.GetReportsForOnMobile(filter);
                //    return;
                //}


                if (string.IsNullOrEmpty(filter.start_date) || string.IsNullOrEmpty(filter.end_date))
                {
                    throw new ArgumentException("Start or end Date is null or empty.");
                }

                var tasks = new List<Task>();
                List<Exception> taskExceptions = new List<Exception>();

                DataSet? DS_SummaryReport_PIVOT = null;
                DataSet? DS_SummaryResponsePrioritySLA = null;
                DataSet? DS_SummaryResolutionPrioritySLA = null;
                DataSet? DS_SLANotMet = null;
                DataSet? DS_TicketNotClosed = null;
                DataSet? DS_TicketByCategoryAndType = null;
                DataSet? DS_TicketByCategoryAndPriority = null;
                DataSet? DS_SummaryLast3Months = null;
                DataSet? DS_CatagoryWiseLast3Months = null;
                IEnumerable<Rpt_PerformaceReportModel>? DS_ServerPerformanceReport = null;
                DataSet? DS_TicketByResourceName = null;
                DataSet? DS_ServiceRequestByUsers = null;
                DataSet? DS_TicketByAvgResponseResolutionSummary = null;
                DataSet? DS_TicketByDailyAndMonthlySummary = null;


                // Create DataSet For SlideData
                DataSet SlidedataSet = new DataSet("SlideDatas");
                DataTable? Tbl_Last3MonthsForChart = null;

                if (!string.IsNullOrWhiteSpace(filter.SlideCodeList))
                {
                    string[] splitCodes = filter.SlideCodeList.Split(',').Select(code => code.Trim()).ToArray();


                    // Database Processing

                    if (splitCodes.Contains("C001"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_SummaryReport_PIVOT = await this._freshServiceData.Get_R_SummaryReport_PIVOT(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C001: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C002"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_SummaryResponsePrioritySLA = await this._freshServiceData.Get_R_SummaryResponsePrioritySLA(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C002: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C003"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_SummaryResolutionPrioritySLA = await this._freshServiceData.Get_R_SummaryResolutionPrioritySLA(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C003: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C004") || splitCodes.Contains("C005"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_SLANotMet = await this._freshServiceData.Get_R_SummaryResolutionPrioritySLA(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C004/C005: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C006"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketNotClosed = await this._freshServiceData.Get_R_TicketNotClosed(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C006: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C007"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketByCategoryAndType = await this._freshServiceData.Get_R_TicketByCategoryAndType(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C007: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C008"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketByCategoryAndPriority = await this._freshServiceData.Get_R_TicketByCategoryAndPriority(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C008: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C009"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                string newStartDate = filter.start_date;

                                // Parse the string into a DateTime object
                                DateTime startDate = DateTime.ParseExact(filter.start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                // Define the IST time zone
                                TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                                // Convert the start date to IST
                                DateTime istStartDate = TimeZoneInfo.ConvertTime(startDate, istTimeZone);

                                // Subtract 2 months
                                DateTime istNewStartDate = istStartDate.AddMonths(-2);

                                // Format the new date as dd/MM/yyyy
                                newStartDate = istNewStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                                DS_SummaryLast3Months = await this._freshServiceData.Get_R_SummaryLast3Months(filter.departmentId, newStartDate, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C009: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C010"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                string newStartDate = filter.start_date;

                                // Parse the string into a DateTime object
                                DateTime startDate = DateTime.ParseExact(filter.start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                // Define the IST time zone
                                TimeZoneInfo istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                                // Convert the start date to IST
                                DateTime istStartDate = TimeZoneInfo.ConvertTime(startDate, istTimeZone);

                                // Subtract 2 months
                                DateTime istNewStartDate = istStartDate.AddMonths(-2);

                                // Format the new date as dd/MM/yyyy
                                newStartDate = istNewStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);


                                DS_CatagoryWiseLast3Months = await this._freshServiceData.Get_R_SummaryLast3Months(filter.departmentId, newStartDate, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C010: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C011") || splitCodes.Contains("C012") || splitCodes.Contains("C013"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_ServerPerformanceReport = await this._site24X7Data.Get_R_ServerPerformanceReport(filter.zaaid, filter.start_date, filter.end_date, filter.departmentId);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C011/C012/C013: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C014"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketByResourceName = await this._freshServiceData.Get_R_TicketByResourceName(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C014: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C015"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_ServiceRequestByUsers = await this._freshServiceData.Get_R_ServiceRequestByUsers(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C015: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C022"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketByAvgResponseResolutionSummary = await this._freshServiceData.Get_R_TicketByAvgResponseResolutionSummary(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C022: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C023"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketByDailyAndMonthlySummary = await this._freshServiceData.Get_R_TicketByDailyAndMonthlySummary
                                (filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C023: {ex.Message}", ex));
                            }
                        }));
                    }


                    await Task.WhenAll(tasks);


                    // DataTable Processing

                    if (splitCodes.Contains("C001"))
                    {
                        #region Incident Report Slide

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C001").FirstOrDefault();

                        if (DS_SummaryReport_PIVOT != null && DS_SummaryReport_PIVOT.Tables.Count > 0)
                        {
                            DataTable InciDentSummaryReportData = DS_SummaryReport_PIVOT.Tables[0].Copy();

                            InciDentSummaryReportData.Columns.Remove("departmentid");
                            InciDentSummaryReportData.Columns.Remove("name");

                            if (InciDentSummaryReportData.Columns.Contains("ZZZ-Grand Total"))
                            {
                                InciDentSummaryReportData.Columns["ZZZ-Grand Total"].ColumnName = "Grand Total";
                            }

                            if (InciDentSummaryReportData.Columns.Contains("type"))
                            {
                                InciDentSummaryReportData.Columns["type"].ColumnName = "Ticket Type";
                            }

                            if (InciDentSummaryReportData.Columns.Contains("category"))
                            {
                                InciDentSummaryReportData.Columns["category"].ColumnName = "Category";
                            }

                            foreach (DataRow row in InciDentSummaryReportData.Rows)
                            {
                                foreach (DataColumn column in InciDentSummaryReportData.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            InciDentSummaryReportData.TableName = slideConfig?.SlideName;
                            InciDentSummaryReportData.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C001";
                            InciDentSummaryReportData.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(InciDentSummaryReportData);
                        }

                        #endregion
                    }

                    #region Ticket Details Analyzation

                    if (splitCodes.Contains("C002"))
                    {
                        #region Response Status

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C002").FirstOrDefault();

                        if (DS_SummaryResponsePrioritySLA != null
                            && DS_SummaryResponsePrioritySLA?.Tables.Count > 0 
                            && DS_SummaryResponsePrioritySLA.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable ResponseStatusTable = DS_SummaryResponsePrioritySLA.Tables[0].DefaultView
                        .ToTable(false, "type", "statustype", "Urgent", "High", "Medium", "Low", "GrandTotal", "AchievedPercentage");

                            if (ResponseStatusTable.Columns.Contains("type"))
                            {
                                ResponseStatusTable.Columns["type"].ColumnName = "Ticket Type";
                            }

                            if (ResponseStatusTable.Columns.Contains("Urgent"))
                            {
                                ResponseStatusTable.Columns["Urgent"].ColumnName = "Urgent";
                            }

                            if (ResponseStatusTable.Columns.Contains("High"))
                            {
                                ResponseStatusTable.Columns["High"].ColumnName = "High";
                            }

                            if (ResponseStatusTable.Columns.Contains("Medium"))
                            {
                                ResponseStatusTable.Columns["Medium"].ColumnName = "Medium";
                            }

                            if (ResponseStatusTable.Columns.Contains("Low"))
                            {
                                ResponseStatusTable.Columns["Low"].ColumnName = "Low";
                            }

                            if (ResponseStatusTable.Columns.Contains("statustype"))
                            {
                                ResponseStatusTable.Columns["statustype"].ColumnName = "Response Status";
                            }

                            if (ResponseStatusTable.Columns.Contains("GrandTotal"))
                            {
                                ResponseStatusTable.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            if (ResponseStatusTable.Columns.Contains("AchievedPercentage"))
                            {
                                ResponseStatusTable.Columns["AchievedPercentage"].ColumnName = "Achieved Percentage (%)";
                            }

                            foreach (DataRow row in ResponseStatusTable.Rows)
                            {
                                foreach (DataColumn column in ResponseStatusTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            ResponseStatusTable.TableName = slideConfig?.SlideName;
                            ResponseStatusTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C002";
                            ResponseStatusTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(ResponseStatusTable);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C003"))
                    {
                        #region Resolution Status

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C003").FirstOrDefault();

                        if (DS_SummaryResolutionPrioritySLA != null
                            && DS_SummaryResolutionPrioritySLA?.Tables.Count > 0 
                            && DS_SummaryResolutionPrioritySLA.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable ResolutionStatusTable = DS_SummaryResolutionPrioritySLA.Tables[0].DefaultView
                            .ToTable(false, "type", "statustype", "Urgent", "High", "Medium", "Low", "GrandTotal", "AchievedPercentage");

                            if (ResolutionStatusTable.Columns.Contains("type"))
                            {
                                ResolutionStatusTable.Columns["type"].ColumnName = "Ticket Type";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Urgent"))
                            {
                                ResolutionStatusTable.Columns["Urgent"].ColumnName = "Urgent";
                            }

                            if (ResolutionStatusTable.Columns.Contains("High"))
                            {
                                ResolutionStatusTable.Columns["High"].ColumnName = "High";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Medium"))
                            {
                                ResolutionStatusTable.Columns["Medium"].ColumnName = "Medium";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Low"))
                            {
                                ResolutionStatusTable.Columns["Low"].ColumnName = "Low";
                            }

                            if (ResolutionStatusTable.Columns.Contains("statustype"))
                            {
                                ResolutionStatusTable.Columns["statustype"].ColumnName = "Resolution Status";
                            }

                            if (ResolutionStatusTable.Columns.Contains("GrandTotal"))
                            {
                                ResolutionStatusTable.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            if (ResolutionStatusTable.Columns.Contains("AchievedPercentage"))
                            {
                                ResolutionStatusTable.Columns["AchievedPercentage"].ColumnName = "Achieved Percentage (%)";
                            }

                            foreach (DataRow row in ResolutionStatusTable.Rows)
                            {
                                foreach (DataColumn column in ResolutionStatusTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            ResolutionStatusTable.TableName = slideConfig?.SlideName;
                            ResolutionStatusTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C003";
                            ResolutionStatusTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(ResolutionStatusTable);
                        }

                        #endregion
                    }

                    #endregion

                    if (splitCodes.Contains("C004") || splitCodes.Contains("C005"))
                    {
                        #region SLA Not Met Response Ticket Details

                        if (DS_SLANotMet != null
                            && DS_SLANotMet?.Tables.Count > 1
                            && DS_SLANotMet.Tables[1]?.Rows.Count > 0)
                        {
                            //removed "on_roaster_engineer, resolution_remarks"

                            // Filter for "Incident"
                            DataView dvSLAIncident = new DataView(DS_SLANotMet.Tables[1])
                            {
                                RowFilter = "type = 'incident' OR type = 'Incident'"
                            };

                            DataTable Tbl_SLAIncident = dvSLAIncident.ToTable(false, "SlNo", "id", "created_at_display", "type", "subject", "StatusName");

                            // Filter for "service request"
                            DataView dvSLAService = new DataView(DS_SLANotMet.Tables[1])
                            {
                                RowFilter = "type = 'service request' OR type = 'Service Request'"
                            };

                            DataTable Tbl_SLAService = dvSLAService.ToTable(false, "SlNo", "id", "created_at_display", "type", "subject", "StatusName");

                            Tbl_SLAIncident.Columns.Add("Remarks");
                            Tbl_SLAService.Columns.Add("Remarks");

                            if (splitCodes.Contains("C004"))
                            {
                                if (Tbl_SLAIncident.Rows.Count > 0)
                                {
                                    var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C004").FirstOrDefault();

                                    //foreach (DataRow row in Tbl_SLAService.Rows)
                                    //{
                                    //    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    //    {
                                    //        row["resolution_remarks"] = string.Empty;
                                    //    }
                                    //}

                                    // Reassign new SlNo based on the filtered order
                                    int incidentSlNo = 1;
                                    foreach (DataRow row in Tbl_SLAIncident.Rows)
                                    {
                                        row["SlNo"] = incidentSlNo++;  // Assign new SlNo and increment
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("SlNo"))
                                    {
                                        Tbl_SLAIncident.Columns["SlNo"].ColumnName = "SL";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("id"))
                                    {
                                        Tbl_SLAIncident.Columns["id"].ColumnName = "Ticket Id";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("created_at_display"))
                                    {
                                        Tbl_SLAIncident.Columns["created_at_display"].ColumnName = "Created Time";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("type"))
                                    {
                                        Tbl_SLAIncident.Columns["type"].ColumnName = "Ticket Type";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("subject"))
                                    {
                                        Tbl_SLAIncident.Columns["subject"].ColumnName = "Subject";
                                    }

                                    //if (Tbl_SLAIncident.Columns.Contains("on_roaster_engineer"))
                                    //{
                                    //    Tbl_SLAIncident.Columns["on_roaster_engineer"].ColumnName = "Engineer";
                                    //}

                                    if (Tbl_SLAIncident.Columns.Contains("resolution_remarks"))
                                    {
                                        Tbl_SLAIncident.Columns["resolution_remarks"].ColumnName = "Remarks";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("StatusName"))
                                    {
                                        Tbl_SLAIncident.Columns["StatusName"].ColumnName = "Status";
                                    }

                                    Tbl_SLAIncident.TableName = slideConfig?.SlideName;
                                    Tbl_SLAIncident.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C004";
                                    Tbl_SLAIncident.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                    SlidedataSet.Tables.Add(Tbl_SLAIncident);
                                }
                            }

                            if (splitCodes.Contains("C005"))
                            {
                                if (Tbl_SLAService.Rows.Count > 0)
                                {
                                    var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C005").FirstOrDefault();

                                    //foreach (DataRow row in Tbl_SLAService.Rows)
                                    //{
                                    //    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    //    {
                                    //        row["resolution_remarks"] = string.Empty;
                                    //    }
                                    //}

                                    int serviceSlNo = 1;
                                    foreach (DataRow row in Tbl_SLAService.Rows)
                                    {
                                        row["SlNo"] = serviceSlNo++;  // Assign new SlNo and increment
                                    }

                                    if (Tbl_SLAService.Columns.Contains("SlNo"))
                                    {
                                        Tbl_SLAService.Columns["SlNo"].ColumnName = "SL";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("id"))
                                    {
                                        Tbl_SLAService.Columns["id"].ColumnName = "Ticket Id";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("created_at_display"))
                                    {
                                        Tbl_SLAService.Columns["created_at_display"].ColumnName = "Created Time";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("type"))
                                    {
                                        Tbl_SLAService.Columns["type"].ColumnName = "Ticket Type";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("subject"))
                                    {
                                        Tbl_SLAService.Columns["subject"].ColumnName = "Subject";
                                    }

                                    //if (Tbl_SLAService.Columns.Contains("on_roaster_engineer"))
                                    //{
                                    //    Tbl_SLAService.Columns["on_roaster_engineer"].ColumnName = "Engineer";
                                    //}

                                    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    {
                                        Tbl_SLAService.Columns["resolution_remarks"].ColumnName = "Remarks";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("StatusName"))
                                    {
                                        Tbl_SLAService.Columns["StatusName"].ColumnName = "Status";
                                    }

                                    Tbl_SLAService.TableName = slideConfig?.SlideName;
                                    Tbl_SLAService.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C005";
                                    Tbl_SLAService.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                    SlidedataSet.Tables.Add(Tbl_SLAService);
                                }
                            }
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C006"))
                    {
                        #region Ticket not closed

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C006").FirstOrDefault();

                        if (DS_TicketNotClosed != null && DS_TicketNotClosed.Tables.Count > 0)
                        {
                            // removed resolution_remarks

                            //DataTable TicketNotClosedDataTable = TicketNotClosedDataSet.Tables[0].Copy();
                            DataTable TicketNotClosedDataTable = DS_TicketNotClosed.Tables[0].DefaultView
                                .ToTable(false, "SlNo", "id", "created_at_display", "RequesterEmail", "subject");

                            //foreach (DataRow row in TicketNotClosedDataTable.Rows)
                            //{
                            //    if (TicketNotClosedDataTable.Columns.Contains("resolution_remarks"))
                            //    {
                            //        row["resolution_remarks"] = string.Empty;
                            //    }
                            //}

                            TicketNotClosedDataTable.Columns.Add("Remarks");

                            if (TicketNotClosedDataTable.Columns.Contains("SlNo"))
                            {
                                TicketNotClosedDataTable.Columns["SlNo"].ColumnName = "SL";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("id"))
                            {
                                TicketNotClosedDataTable.Columns["id"].ColumnName = "Ticket Id";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("created_at_display"))
                            {
                                TicketNotClosedDataTable.Columns["created_at_display"].ColumnName = "Created Time";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("subject"))
                            {
                                TicketNotClosedDataTable.Columns["subject"].ColumnName = "Subject";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("resolution_remarks"))
                            {
                                TicketNotClosedDataTable.Columns["resolution_remarks"].ColumnName = "Remarks";
                            }

                            TicketNotClosedDataTable.TableName = slideConfig?.SlideName;
                            TicketNotClosedDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C006";
                            TicketNotClosedDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketNotClosedDataTable);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C007"))
                    {
                        #region Category Wise Call Bifurcation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C007").FirstOrDefault();

                        if (DS_TicketByCategoryAndType != null
                            && DS_TicketByCategoryAndType?.Tables.Count > 0
                            && DS_TicketByCategoryAndType.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_CategoryAndType = DS_TicketByCategoryAndType.Tables[0].DefaultView
                        .ToTable(false, "category", "sub_category", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal");

                            if (Tbl_CategoryAndType.Columns.Contains("category"))
                            {
                                Tbl_CategoryAndType.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("sub_category"))
                            {
                                Tbl_CategoryAndType.Columns["sub_category"].ColumnName = "Sub-Category";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_CategoryAndType.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_CategoryAndType.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("GrandTotal"))
                            {
                                Tbl_CategoryAndType.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }


                            foreach (DataRow row in Tbl_CategoryAndType.Rows)
                            {
                                foreach (DataColumn column in Tbl_CategoryAndType.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            Tbl_CategoryAndType.TableName = slideConfig?.SlideName;
                            Tbl_CategoryAndType.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C007";
                            Tbl_CategoryAndType.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(Tbl_CategoryAndType);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C008"))
                    {
                        #region Priority wise Ticket Bifurcation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C008").FirstOrDefault();

                        if (DS_TicketByCategoryAndPriority != null
                            && DS_TicketByCategoryAndPriority?.Tables.Count > 0
                            && DS_TicketByCategoryAndPriority.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_SubCategoryAndType = DS_TicketByCategoryAndPriority.Tables[0].DefaultView
                        .ToTable(false, "category", "sub_category", "Urgent", "high", "medium", "Low", "GrandTotal");

                            if (Tbl_SubCategoryAndType.Columns.Contains("category"))
                            {
                                Tbl_SubCategoryAndType.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("sub_category"))
                            {
                                Tbl_SubCategoryAndType.Columns["sub_category"].ColumnName = "Sub-Category";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("high"))
                            {
                                Tbl_SubCategoryAndType.Columns["high"].ColumnName = "High";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("medium"))
                            {
                                Tbl_SubCategoryAndType.Columns["medium"].ColumnName = "Medium";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("GrandTotal"))
                            {
                                Tbl_SubCategoryAndType.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_SubCategoryAndType.Rows)
                            {
                                foreach (DataColumn column in Tbl_SubCategoryAndType.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            Tbl_SubCategoryAndType.TableName = slideConfig?.SlideName;
                            Tbl_SubCategoryAndType.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C008";
                            Tbl_SubCategoryAndType.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(Tbl_SubCategoryAndType);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C009"))
                    {
                        #region Report for Last 3 Months                        

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C009").FirstOrDefault();

                        if (DS_SummaryLast3Months != null
                            && DS_SummaryLast3Months?.Tables.Count > 2
                            && DS_SummaryLast3Months.Tables[2]?.Rows.Count > 0)
                        {
                            DataTable Tbl_Last3Months = DS_SummaryLast3Months.Tables[2].DefaultView
                        .ToTable(false, "monthname", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal", "RowType");

                            if (Tbl_Last3Months.Columns.Contains("monthname"))
                            {
                                Tbl_Last3Months.Columns["monthname"].ColumnName = "Months";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_Last3Months.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_Last3Months.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("GrandTotal"))
                            {
                                Tbl_Last3Months.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_Last3Months.Rows)
                            {
                                foreach (DataColumn column in Tbl_Last3Months.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            DataTable NewTbl_Last3Months = Tbl_Last3Months.Copy();

                            // Remove the "RowType" column from the cloned structure
                            if (NewTbl_Last3Months.Columns.Contains("RowType"))
                            {
                                NewTbl_Last3Months.Columns.Remove("RowType");
                            }

                            NewTbl_Last3Months.TableName = slideConfig?.SlideName;
                            NewTbl_Last3Months.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C009";
                            NewTbl_Last3Months.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(NewTbl_Last3Months);

                            Tbl_Last3Months.TableName = (slideConfig?.SlideName) + " Graphical View";
                            Tbl_Last3MonthsForChart = Tbl_Last3Months;
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C010"))
                    {
                        #region Category Wise Ticket Analysis Report for Last 3 Months

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C010").FirstOrDefault();

                        if (DS_CatagoryWiseLast3Months != null
                            && DS_CatagoryWiseLast3Months?.Tables.Count > 0
                            && DS_CatagoryWiseLast3Months.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_Last3Months = DS_CatagoryWiseLast3Months.Tables[0].DefaultView
                        .ToTable(false, "category", "monthname", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal", "RowType");

                            if (Tbl_Last3Months.Columns.Contains("category"))
                            {
                                Tbl_Last3Months.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_Last3Months.Columns.Contains("monthname"))
                            {
                                Tbl_Last3Months.Columns["monthname"].ColumnName = "Months";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_Last3Months.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_Last3Months.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("GrandTotal"))
                            {
                                Tbl_Last3Months.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_Last3Months.Rows)
                            {
                                foreach (DataColumn column in Tbl_Last3Months.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            DataTable NewTbl_Last3Months = Tbl_Last3Months.Copy();

                            // Remove the "RowType" column from the cloned structure
                            if (NewTbl_Last3Months.Columns.Contains("RowType"))
                            {
                                NewTbl_Last3Months.Columns.Remove("RowType");
                            }

                            NewTbl_Last3Months.TableName = slideConfig?.SlideName;
                            NewTbl_Last3Months.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C010";
                            NewTbl_Last3Months.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(NewTbl_Last3Months);

                            //Tbl_Last3Months.TableName = (slideConfig?.SlideName ?? SlideHeadingConstants.CategoryWiseLast3Months) + " Graphical View";
                            //Tbl_Last3MonthsForChart = Tbl_Last3Months;
                        }

                        #endregion
                    }

                    #region Server Utilization

                    if (DS_ServerPerformanceReport != null && DS_ServerPerformanceReport.Any())
                    {
                        if (splitCodes.Contains("C011"))
                        {
                            #region Azure VM’s CPU Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C011").FirstOrDefault();

                            var CPUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "CPU");
                            if (CPUtilization?.Rows.Count > 0)
                            {
                                DataTable Tbl_CPUtilization = CPUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_CPUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_CPUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_CPUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_CPUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_CPUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_CPUtilization.TableName = slideConfig?.SlideName;
                                Tbl_CPUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C011";
                                Tbl_CPUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_CPUtilization);
                            }

                            #endregion
                        }

                        if (splitCodes.Contains("C012"))
                        {
                            #region Azure VM’s Memory  Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C012").FirstOrDefault();

                            var MemoryUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "Memory");
                            if (MemoryUtilization?.Rows.Count > 0)
                            {

                                DataTable Tbl_MemoryUtilization = MemoryUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_MemoryUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_MemoryUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_MemoryUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_MemoryUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_MemoryUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_MemoryUtilization.TableName = slideConfig?.SlideName;
                                Tbl_MemoryUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C012";
                                Tbl_MemoryUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_MemoryUtilization);
                            }

                            #endregion
                        }

                        if (splitCodes.Contains("C013"))
                        {
                            #region AzureVM’s Disk Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C013").FirstOrDefault();

                            var DiskUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "Disk");
                            if (DiskUtilization?.Rows.Count > 0)
                            {
                                DataTable Tbl_DiskUtilization = DiskUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_DiskUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_DiskUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_DiskUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_DiskUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_DiskUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_DiskUtilization.TableName = slideConfig?.SlideName;
                                Tbl_DiskUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C013";
                                Tbl_DiskUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_DiskUtilization);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    if (splitCodes.Contains("C014"))
                    {
                        #region Server wise auto-ticket generation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C014").FirstOrDefault();

                        if (DS_TicketByResourceName != null && DS_TicketByResourceName.Tables.Count > 0)
                        {
                            DataTable TicketByResourceNameDataTable = DS_TicketByResourceName.Tables[0].DefaultView
                                .ToTable(false, "resource_name", "NoOfTickets");

                            if (TicketByResourceNameDataTable.Columns.Contains("resource_name"))
                            {
                                TicketByResourceNameDataTable.Columns["resource_name"].ColumnName = "Resource Name";
                            }

                            if (TicketByResourceNameDataTable.Columns.Contains("NoOfTickets"))
                            {
                                TicketByResourceNameDataTable.Columns["NoOfTickets"].ColumnName = "Count";
                            }


                            TicketByResourceNameDataTable.TableName = slideConfig?.SlideName;
                            TicketByResourceNameDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C014";
                            TicketByResourceNameDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketByResourceNameDataTable);
                        }

                        #endregion
                    }

                    
                    if (splitCodes.Contains("C015"))
                    {
                        #region Ticket raised by User/Helpdesk

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C015").FirstOrDefault();

                        if (DS_ServiceRequestByUsers != null
                            && DS_ServiceRequestByUsers.Tables.Count > 0
                            && DS_ServiceRequestByUsers.Tables[0].Rows.Count > 1
                            )
                        {
                            DataTable TicketByResourceNameDataTable = DS_ServiceRequestByUsers.Tables[0].DefaultView
                                .ToTable(false, "Requester", "Count");

                            if (TicketByResourceNameDataTable.Columns.Contains("resource_name"))
                            {
                                TicketByResourceNameDataTable.Columns["resource_name"].ColumnName = "Resource Name";
                            }

                            foreach (DataRow row in TicketByResourceNameDataTable.Rows)
                            {
                                foreach (DataColumn column in TicketByResourceNameDataTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            TicketByResourceNameDataTable.TableName = slideConfig?.SlideName;
                            TicketByResourceNameDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C015";
                            TicketByResourceNameDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketByResourceNameDataTable);
                        }

                        #endregion
                    }

                    //soumik rev

                    if (splitCodes.Contains("C022"))
                    {
                        var slideConfig = filter.SlideConfigList?
                            .FirstOrDefault(e => e.SlideCode == "C022") ?? new SlideConfigurations { };

                        if (DS_TicketByAvgResponseResolutionSummary != null
                            && DS_TicketByAvgResponseResolutionSummary.Tables.Count > 0
                            && DS_TicketByAvgResponseResolutionSummary.Tables[0].Rows.Count > 1)
                        {

                            

                            DataTable TicketByAvgResponseResolutionSummary = DS_TicketByAvgResponseResolutionSummary.Tables[0].DefaultView
                                .ToTable();

                            TicketByAvgResponseResolutionSummary.TableName = slideConfig?.SlideName ?? "C022";
                            TicketByAvgResponseResolutionSummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C022";
                            TicketByAvgResponseResolutionSummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;


                            SlidedataSet.Tables.Add(TicketByAvgResponseResolutionSummary);

                        }
                    }

                    if (splitCodes.Contains("C023"))
                    {
                        //DS_TicketByDailyAndMonthlySummary

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C023").FirstOrDefault();

                        if (
                            DS_TicketByDailyAndMonthlySummary != null 
                            && 
                            DS_TicketByDailyAndMonthlySummary.Tables.Count >= 1)
                        {
                            // Create combined table
                            DataTable CombinedTicketSummary = new DataTable();
                            CombinedTicketSummary.TableName = slideConfig?.SlideName ?? "C023";

                            // Keep same extended properties pattern
                            CombinedTicketSummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                            CombinedTicketSummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;

                            // Add TableType so we can filter later: values will be "Table1", "Table2", "Table3"
                            CombinedTicketSummary.Columns.Add("TableType", typeof(string));

                            // Helper local function to ensure columns from a source table exist in combined table
                            void EnsureColumnsInCombined(DataTable source)
                            {
                                foreach (DataColumn col in source.Columns)
                                {
                                    if (!CombinedTicketSummary.Columns.Contains(col.ColumnName))
                                    {
                                        CombinedTicketSummary.Columns.Add(col.ColumnName, col.DataType ?? typeof(string));
                                    }
                                }
                            }

                            // ------------------------------
                            // Table 1 (original Tables[0]) - keep column names
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 0
                                && DS_TicketByDailyAndMonthlySummary.Tables[0].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t1 = DS_TicketByDailyAndMonthlySummary.Tables[0].DefaultView
                                    .ToTable(false, "ActivityName", "ActivityCount");

                                // Rename columns exactly
                                if (t1.Columns.Contains("ActivityName"))
                                    t1.Columns["ActivityName"].ColumnName = "Daily and Monthly SR Activity Analysis";

                                if (t1.Columns.Contains("ActivityCount"))
                                    t1.Columns["ActivityCount"].ColumnName = "Activity Count";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t1);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t1.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table1";

                                    foreach (DataColumn col in t1.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 2 (original Tables[1])
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 1
                                && DS_TicketByDailyAndMonthlySummary.Tables[1].Rows.Count > 0)
                            {
                                DataTable t2 = DS_TicketByDailyAndMonthlySummary.Tables[1].DefaultView
                                    .ToTable(false, "IncidentName", "SRName", "ActivityName");

                                // Rename as original
                                if (t2.Columns.Contains("IncidentName"))
                                    t2.Columns["IncidentName"].ColumnName = "Incident";

                                if (t2.Columns.Contains("SRName"))
                                    t2.Columns["SRName"].ColumnName = "Service Request";

                                if (t2.Columns.Contains("ActivityName"))
                                    t2.Columns["ActivityName"].ColumnName = "Daily / Monthly / Operational Calls";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t2);

                                // Copy rows and tag as Table2
                                foreach (DataRow row in t2.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table2";

                                    foreach (DataColumn col in t2.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 3 (original Tables[2])
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 2
                                && DS_TicketByDailyAndMonthlySummary.Tables[2].Rows.Count > 0)
                            {
                                DataTable t3 = DS_TicketByDailyAndMonthlySummary.Tables[2].DefaultView
                                    .ToTable(false, "ActivityName", "ActivityCount");

                                // Rename exactly as in your original code
                                if (t3.Columns.Contains("ActivityName"))
                                    t3.Columns["ActivityName"].ColumnName = "Daily & Monthly SR Activity (Team Wise Bifurcation)";

                                if (t3.Columns.Contains("ActivityCount"))
                                    t3.Columns["ActivityCount"].ColumnName = "Count";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t3);

                                // Copy rows and tag as Table3
                                foreach (DataRow row in t3.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table3";

                                    foreach (DataColumn col in t3.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // Finally add the combined table once (you can filter using "TableType" later)
                            SlidedataSet.Tables.Add(CombinedTicketSummary);
                        }


                        //{

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[0].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[0].DefaultView
                        //            .ToTable(false, "ActivityName", "ActivityCount");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily and Monthly SR Activity Analysis";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityCount"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityCount"].ColumnName = "Activity Count";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode != null ? slideConfig?.SlideCode + " - Table 1" : "CO23  - Table 1";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[1].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[1].DefaultView
                        //            .ToTable(false, "IncidentName", "SRName", "ActivityName");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("IncidentName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["IncidentName"].ColumnName = "Incident";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("SRName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["SRName"].ColumnName = "Service Request";
                        //        }
                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily / Monthly / Operational Calls";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode != null ? slideConfig?.SlideCode + " - Table 2" : "CO23  - Table 2";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[2].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[2].DefaultView
                        //            .ToTable(false, "ActivityName", "ActivityCount");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily & Monthly SR Activity (Team Wise Bifurcation)";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityCount"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityCount"].ColumnName = "Count";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode !=null ? slideConfig?.SlideCode + " - Table 3" : "CO23  - Table 3";
                        //        // later will change name in service
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //}

                    }

                    //soumik rev

                    string[] staticSlideCodes = { "C016", "C017", "C018", "C019", "C020", "C021" };

                    foreach (string code in staticSlideCodes)
                    {
                        if (splitCodes.Contains(code))
                        {
                            var slideConfig = filter.SlideConfigList?.FirstOrDefault(e => e.SlideCode == code);

                            var datatable = new DataTable
                            {
                                TableName = code
                            };

                            datatable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? code;
                            datatable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                            SlidedataSet.Tables.Add(datatable);
                        }
                    }

                }


                // PPT Processing

                try
                {
                    string mainPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents", filter.departmentId.ToString(), DateTime.Now.ToString("MMMMyyyy"), "Presentations");
                    if (!System.IO.Directory.Exists(mainPath))
                    {
                        System.IO.Directory.CreateDirectory(mainPath);
                    }

                    //string JSONfilePath = "D:\\Json\\SlidedataSet.json";
                    string JSONfilePath = Path.Combine(mainPath, "SlidedataSet.json");
                    
                    //_ = Task.Run(() => this.SaveDataSetToJson(SlidedataSet, JSONfilePath));

                    //string JSONfilePath2 = "D:\\Json\\Tbl_Last3MonthsForChart.json";
                    if (Tbl_Last3MonthsForChart != null && Tbl_Last3MonthsForChart.Rows.Count > 0)
                    {
                        string JSONfilePath2 = Path.Combine(mainPath, "Tbl_Last3MonthsForChart.json");                        
                        //_ = Task.Run(() => this.SaveDataTableToJson(Tbl_Last3MonthsForChart, JSONfilePath2));
                    }
                }
                catch (Exception ex)
                {

                }

                string FilePath = "";

                try
                {
                    string MonthName = DateTime.ParseExact(filter.start_date, "dd/MM/yyyy", null).ToString("MMMM yyyy");
                    var dataObj = new HelperModel
                    {
                        DirectoryName = filter.departmentId.ToString(),
                        DepartmentId = filter.departmentId.ToString(),
                        MonthName = MonthName,
                        DataTableForChart = Tbl_Last3MonthsForChart
                    };

                    if (SlidedataSet != null
                        && SlidedataSet.Tables.Count > 0)
                    {
                        DataSet sortedDataSet = sortedDataSet = SortDataSetTablesBySortOrder(SlidedataSet);
                        if (sortedDataSet != null)
                        {
                            FilePath = await this._genPPTService.GeneratePpt(sortedDataSet, dataObj);
                        }
                        else
                        {
                            FilePath = await this._genPPTService.GeneratePpt(SlidedataSet, dataObj);
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    throw;
                }


                if (taskExceptions.Any())
                {
                    foreach (var ex in taskExceptions)
                    {
                        this._logger.LogError(ex, "Task execution error");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }


                return this.Ok(new { Message = FilePath });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GenerateBarChart()");
                return this.Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetReportInPPT")]
        public async Task<IActionResult> GetReportInPPT(ParamModel paramModel)
        {
            try
            {
                
                if (paramModel.Filter.departmentId == 0)
                {
                    throw new ArgumentException("The Department is null or empty.");
                }

                if (string.IsNullOrEmpty(paramModel.Filter.start_date) || string.IsNullOrEmpty(paramModel.Filter.end_date))
                {
                    throw new ArgumentException("Start or end Date is null or empty.");
                }


                // Extract required slides and their sort order
                var requiredSlides = paramModel.SlideConfig.AsEnumerable()
                    .Select(row => new
                    {
                        SlideCode = row.SlideCode,
                        SlideName = row.SlideName,                        
                        SortOrder = row.SortOrder
                    })
                    .OrderBy(slide => slide.SortOrder)
                    .ToList();


                // Create DataSet For SlideData
                DataSet SlidedataSet = new DataSet("SlideDatas");

                var tasks = requiredSlides.Select(slide =>
                {
                    return slide.SlideCode switch
                    {
                        var code when code == SlideCodeEnum.C001.ToString() => Task.Run(async () =>
                        {
                            var responseStatus = await _freshServiceData.Get_R_SummaryReport_PIVOT(
                                paramModel.Filter.departmentId,
                                paramModel.Filter.start_date,
                                paramModel.Filter.end_date);

                            if (responseStatus?.Tables.Count > 0 &&
                                responseStatus.Tables[0]?.Rows.Count > 0)
                            {
                                DataTable reportTable = responseStatus.Tables[0];

                                var mappingRow = paramModel.SlideConfig
                                .FirstOrDefault(row => row.SlideCode == SlideCodeEnum.C001.ToString());

                                var columns = mappingRow?.Columns;
                                var customColumnNames = mappingRow?.CustomColumns;

                                // Process the DataTable with selected columns and custom names
                                var processedTable = await _genPPTService.ProcessAndMapColumnsWithCustomNames(reportTable, columns, customColumnNames);

                                // Add the processed DataTable to the DataSet
                                processedTable.TableName = slide.SlideName.ToString();
                                lock (SlidedataSet)
                                {
                                    SlidedataSet.Tables.Add(processedTable);
                                }
                            }                            
                        }),

                        var code when code == SlideCodeEnum.C002.ToString() => Task.Run(async () =>
                        {
                            var responseStatus = await this._freshServiceData.Get_R_SummaryResponsePrioritySLA(paramModel.Filter.departmentId,
                                paramModel.Filter.start_date,
                                paramModel.Filter.end_date);

                            if (responseStatus?.Tables.Count > 0 &&
                                responseStatus.Tables[0]?.Rows.Count > 0)
                            {
                                DataTable reportTable = responseStatus.Tables[0];
                                var mappingRow = paramModel.SlideConfig
                                .FirstOrDefault(row => row.SlideCode == SlideCodeEnum.C002.ToString());

                                var columns = mappingRow?.Columns;
                                var customColumnNames = mappingRow?.CustomColumns;

                                // Process the DataTable with selected columns and custom names
                                var processedTable = await _genPPTService.ProcessAndMapColumnsWithCustomNames(reportTable, columns, customColumnNames);

                                // Add the processed DataTable to the DataSet
                                processedTable.TableName = slide.SlideName.ToString();
                                lock (SlidedataSet)
                                {
                                    SlidedataSet.Tables.Add(processedTable);
                                }
                            }                            
                        }),

                        _ => Task.CompletedTask
                    };
                });

                // Wait for all tasks to complete
                await Task.WhenAll(tasks);

                string FilePath = "";

                try
                {
                    string MonthName = DateTime.ParseExact(paramModel.Filter.start_date, "dd/MM/yyyy", null).ToString("MMMM yyyy");
                    var dataObj = new HelperModel
                    {
                        DirectoryName = paramModel.Filter.departmentId.ToString(),
                        DepartmentId = paramModel.Filter.departmentId.ToString(),
                        MonthName = MonthName,
                        SlideConfig = paramModel.SlideConfig,
                        DataTableForChart = new DataTable()
                    };

                    FilePath = await this._genPPTService.GeneratePpt(SlidedataSet, dataObj);
                }
                catch (Exception ex)
                {
                    throw;
                }

                return this.Ok(new { Message = FilePath });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GenerateBarChart()");
                return this.Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetReportInExcel")]
        public async Task<IActionResult> GetReportInExcel(FilterModel filter)
        {
            try
            {
                var paramModel = new ParamModel
                {
                    Filter = filter,
                    SlideConfig = new List<SlideConfigurations>()
                };

                if (filter == null)
                {
                    throw new ArgumentException("Filter parameter is null or empty.");
                }

                if (filter.departmentId == 0)
                {
                    throw new ArgumentException("Department is null or empty.");
                }

                if (string.IsNullOrEmpty(filter.start_date) || string.IsNullOrEmpty(filter.end_date))
                {
                    throw new ArgumentException("Start or End Date is null or empty.");
                }

                var filePath = "";

                var dbResult = await this._freshServiceData.Get_R_TicketExcel(filter.departmentId, filter.start_date, filter.end_date);
                if (dbResult?.Tables.Count > 0 && dbResult?.Tables[0]?.Rows.Count > 0)
                {
                    var excelResult = this._genExcelService.GenerateExcelNReturnPath(dbResult, paramModel);
                    filePath = excelResult;
                }

                return this.Ok(new { Message = filePath });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GetReportInExcel()");
                return this.Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("DownloadFile")]
        [Description("Downloadanybyfilepath")]
        public async Task<IActionResult> DownloadFile(string filePath)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!System.IO.File.Exists(filePath))
                        return NotFound();
                    var content = await System.IO.File.ReadAllBytesAsync(filePath);
                    new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);
                    return File(content, contentType, filePath);
                }
                catch
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }



        //soumik rev 25-11-2025

        [HttpPost]
        [Route("GetReportsForOnMobile")]
        public async Task<IActionResult> GetReportsForOnMobile(FilterModel filter)
        {
            try
            {
                if (filter.departmentId == 0)
                {
                    throw new ArgumentException("The Department is null or empty.");
                }

                if (string.IsNullOrEmpty(filter.start_date) || string.IsNullOrEmpty(filter.end_date))
                {
                    throw new ArgumentException("Start or end Date is null or empty.");
                }

                var tasks = new List<Task>();
                List<Exception> taskExceptions = new List<Exception>();

                DataSet? DS_TrendsLast3MonthsIncident = null;
                DataSet? DS_SummaryResponsePrioritySLA = null;
                DataSet? DS_SummaryResolutionPrioritySLA = null;
                DataSet? ResponsePerformance = null;
                DataSet? ResolutionPerformance = null;
                DataSet? DS_SLANotMet = null;
                DataSet? DS_TicketNotClosed = null;
                DataSet? DS_TicketByCategoryAndType = null;
                DataSet? DS_TicketByCategoryAndPriority = null;
                DataSet? DS_SummaryLast3Months = null;
                DataSet? DS_CatagoryWiseLast3Months = null;
                IEnumerable<Rpt_PerformaceReportModel>? DS_ServerPerformanceReport = null;
                DataSet? DS_TicketByResourceName = null;
                DataSet? DS_ServiceRequestByUsers = null;
                DataSet? DS_TicketByAvgResponseResolutionSummary = null;
                DataSet? DS_TicketByDailyAndMonthlySummary = null;

                DataSet? DS_CategoryWiseIncidents = null;
                DataSet? DS_NetworkCategoryPerformance = null;
                DataSet? DS_NetworkCategoryWiseTickets = null;
                DataSet? DS_ResourceWiseAlerts = null;
                DataSet? DS_TicketToolAnalysis = null;
                DataSet? DS_ChangeSummaryLast3MonthsTrend = null;

                




                // Create DataSet For SlideData
                DataSet SlidedataSet = new DataSet("SlideDatas");
                DataTable? Tbl_Last3MonthsForChart = null;

                if (!string.IsNullOrWhiteSpace(filter.SlideCodeList))
                {
                    string[] splitCodes = filter.SlideCodeList.Split(',').Select(code => code.Trim()).ToArray();


                    // Database Processing

                    if (splitCodes.Contains("C024"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TrendsLast3MonthsIncident = await this._freshServiceData.Get_R_TrendsLast3MonthsIncident(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C001: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C025"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                ResponsePerformance = await this._freshServiceData.Get_R_ResponsePerformance(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C025: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C026"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                ResolutionPerformance = await this._freshServiceData.Get_R_ResolutionPerformance(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C025: {ex.Message}", ex));
                            }
                        }));
                    }


                    if (splitCodes.Contains("C027"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_CategoryWiseIncidents = 
                                await this._freshServiceData.Get_R_CategoryWiseIncidents(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C025: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C028"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_NetworkCategoryPerformance =
                                await this._freshServiceData.Get_R_NetworkCategoryPerformance(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C025: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C029"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_NetworkCategoryWiseTickets =
                                await this._freshServiceData.Get_R_NetworkCategoryWiseTickets(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C029: {ex.Message}", ex));
                            }
                        }));
                    }
                    
                    if (splitCodes.Contains("C030"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_TicketToolAnalysis =
                                await this._freshServiceData.Get_R_TicketToolAnalysis(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C030: {ex.Message}", ex));
                            }
                        }));
                    }

                    if (splitCodes.Contains("C031"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_ResourceWiseAlerts =
                                await this._freshServiceData.Get_R_ResourceWiseAlerts(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C029: {ex.Message}", ex));
                            }
                        }));
                    }
                    if (splitCodes.Contains("C032"))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                DS_ChangeSummaryLast3MonthsTrend =
                                await this._freshServiceData.Get_R_ChangeSummaryLast3MonthsTrend(filter.departmentId, filter.start_date, filter.end_date);
                            }
                            catch (Exception ex)
                            {
                                taskExceptions.Add(new Exception($"Error fetching data for C0: {ex.Message}", ex));
                            }
                        }));
                    }



                    await Task.WhenAll(tasks);


                    // DataTable Processing

                    if (splitCodes.Contains("C024"))
                    {
                        #region Incident Report Slide

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C024").FirstOrDefault();

                        if (DS_TrendsLast3MonthsIncident != null && DS_TrendsLast3MonthsIncident.Tables.Count > 0)
                        {
                            DataTable InciDentSummaryReportData = DS_TrendsLast3MonthsIncident.Tables[0].Copy();

                            InciDentSummaryReportData.TableName = slideConfig?.SlideName ?? "Incident Trend View(Last 3 Months)";
                            InciDentSummaryReportData.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C024";
                            InciDentSummaryReportData.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            
                           

                            SlidedataSet.Tables.Add(InciDentSummaryReportData);

                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C025"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C025").FirstOrDefault();

                        if (ResponsePerformance != null && ResponsePerformance.Tables.Count > 0)
                        {
                            DataTable ResponsePerformanceDT = ResponsePerformance.Tables[0].Copy();

                        ResponsePerformanceDT.TableName = slideConfig?.SlideName ?? "Response Performance";
                        ResponsePerformanceDT.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C025";
                        ResponsePerformanceDT.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                        ResponsePerformanceDT.Columns.Remove("DepartmentId");
                        ResponsePerformanceDT.Columns.Remove("DepartmentName");

                        SlidedataSet.Tables.Add(ResponsePerformanceDT);

                    }
                }

                    if (splitCodes.Contains("C026"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C026").FirstOrDefault();

                        if (ResolutionPerformance != null && ResolutionPerformance.Tables.Count > 0)
                        {
                            DataTable ResolutionPerformanceReportData = ResolutionPerformance.Tables[0].Copy();

                            ResolutionPerformanceReportData.TableName = slideConfig?.SlideName ?? "Resolution Performance";
                            ResolutionPerformanceReportData.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C026";
                            ResolutionPerformanceReportData.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                        ResolutionPerformanceReportData.Columns.Remove("DepartmentId");
                        ResolutionPerformanceReportData.Columns.Remove("DepartmentName");
                        SlidedataSet.Tables.Add(ResolutionPerformanceReportData);

                    }
                }


                    if (splitCodes.Contains("C027"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C027").FirstOrDefault();

                        if (DS_CategoryWiseIncidents != null && DS_CategoryWiseIncidents.Tables.Count > 0)
                        {
                            DataTable CategoryWiseIncidents = DS_CategoryWiseIncidents.Tables[0].Copy();

                        CategoryWiseIncidents.TableName = slideConfig?.SlideName ?? "Category Wise Incident Tickets";
                        CategoryWiseIncidents.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C027";
                        CategoryWiseIncidents.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                        CategoryWiseIncidents.Columns.Remove("DepartmentId");
                        CategoryWiseIncidents.Columns.Remove("DepartmentName");

                        DataRow totalRow = CategoryWiseIncidents.NewRow();
                        // Label for the first column (e.g., "Ticket Type")
                        totalRow["CategoryName"] = "Grand Total";

                        for (int i = 1; i < CategoryWiseIncidents.Columns.Count; i++)
                        {
                            string colName = CategoryWiseIncidents.Columns[i].ColumnName;

                            // Sum the column values (assuming numeric)
                            int sum = CategoryWiseIncidents.AsEnumerable()
                                .Sum(r => r.Field<int>(colName));

                            totalRow[colName] = sum;
                        }
                        // Add the total row to the end of the table
                        CategoryWiseIncidents.Rows.Add(totalRow);

                        SlidedataSet.Tables.Add(CategoryWiseIncidents);

                        }
                    }

                    if (splitCodes.Contains("C028"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C028").FirstOrDefault();

                        if (DS_NetworkCategoryPerformance != null && DS_NetworkCategoryPerformance.Tables.Count > 0)
                        {

                            if (DS_NetworkCategoryPerformance.Tables.Count > 3
                                && DS_NetworkCategoryPerformance.Tables[3].Rows.Count > 0)
                            {
                                DataTable NetworkCategoryPerformance = DS_NetworkCategoryPerformance.Tables[3].Copy();

                                NetworkCategoryPerformance.TableName = slideConfig?.SlideName ?? "Network Performance Report";
                                NetworkCategoryPerformance.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C028";
                                NetworkCategoryPerformance.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                                NetworkCategoryPerformance.Columns.Remove("DepartmentId");
                                NetworkCategoryPerformance.Columns.Remove("DepartmentName");
                                NetworkCategoryPerformance.Columns.Remove("Month");
                                NetworkCategoryPerformance.Columns.Remove("Year");
                                NetworkCategoryPerformance.Columns.Remove("Date");
                                NetworkCategoryPerformance.Columns["NameMonth"].ColumnName = "Month";
                                NetworkCategoryPerformance.Columns["Sev1"].ColumnName = "Sev 1";
                                NetworkCategoryPerformance.Columns["Sev2"].ColumnName = "Sev 2";
                                NetworkCategoryPerformance.Columns["Sev3"].ColumnName = "Sev 3";
                                NetworkCategoryPerformance.Columns["Sev4"].ColumnName = "Sev 4";

                                SlidedataSet.Tables.Add(NetworkCategoryPerformance);

                            }



                            // prepare data table for chart 
                            DataTable CombinedTicketSummary = new DataTable();
                            CombinedTicketSummary.Columns.Add("TableType", typeof(string));
                                
                            // Helper local function to ensure columns from a source table exist in combined table
                            void EnsureColumnsInCombined(DataTable source)
                            {
                                foreach (DataColumn col in source.Columns)
                                {
                                    if (!CombinedTicketSummary.Columns.Contains(col.ColumnName))
                                    {
                                        CombinedTicketSummary.Columns.Add(col.ColumnName, col.DataType ?? typeof(string));
                                    }
                                }
                            }

                            // ------------------------------
                            // Table 1 (original Tables[0]) - keep column names
                            // ------------------------------
                            if (DS_NetworkCategoryPerformance.Tables.Count > 0
                                && DS_NetworkCategoryPerformance.Tables[0].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t1 = DS_NetworkCategoryPerformance.Tables[0].DefaultView
                                    .ToTable(false, "TicketType", "Total");

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t1);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t1.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table1";

                                    foreach (DataColumn col in t1.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 2 (original Tables[1]) - keep column names
                            // ------------------------------
                            if (DS_NetworkCategoryPerformance.Tables.Count > 1
                                && DS_NetworkCategoryPerformance.Tables[1].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t2 = DS_NetworkCategoryPerformance.Tables[1].DefaultView
                                    .ToTable(false, "Priority", "WithInSLA", "SLAViolated");

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t2);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t2.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table2";

                                    foreach (DataColumn col in t2.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 3 (original Tables[2]) - keep column names
                            // ------------------------------
                            if (DS_NetworkCategoryPerformance.Tables.Count > 2
                                && DS_NetworkCategoryPerformance.Tables[2].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t3 = DS_NetworkCategoryPerformance.Tables[2].DefaultView
                                    .ToTable(false, "Priority", "WithInSLA", "SLAViolated");

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t3);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t3.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table3";

                                    foreach (DataColumn col in t3.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            Tbl_Last3MonthsForChart = CombinedTicketSummary.Copy();
                        }
                    }

                    if (splitCodes.Contains("C029"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C029").FirstOrDefault();

                        //if (ResolutionPerformance != null && ResolutionPerformance.Tables.Count > 0)
                        //{
                        DataTable NetworkCategoryTicket = DS_NetworkCategoryWiseTickets.Tables[0].Copy();

                        NetworkCategoryTicket.TableName = slideConfig?.SlideName ?? "Network Category Wise Tickets";
                        NetworkCategoryTicket.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C029";
                        NetworkCategoryTicket.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                        NetworkCategoryTicket.Columns.Remove("DepartmentId");
                        NetworkCategoryTicket.Columns.Remove("DepartmentName");

                        DataRow totalRow = NetworkCategoryTicket.NewRow();
                        // Label for the first column (e.g., "Ticket Type")
                        totalRow["CategoryName"] = "Grand Total";

                        for (int i = 1; i < NetworkCategoryTicket.Columns.Count; i++)
                        {
                            string colName = NetworkCategoryTicket.Columns[i].ColumnName;

                            // Sum the column values (assuming numeric)
                            int sum = NetworkCategoryTicket.AsEnumerable()
                                .Sum(r => r.Field<int>(colName));

                            totalRow[colName] = sum;
                        }
                        // Add the total row to the end of the table
                        NetworkCategoryTicket.Rows.Add(totalRow);

                        SlidedataSet.Tables.Add(NetworkCategoryTicket);

                        //}
                    }


                    if (splitCodes.Contains("C030"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C030").FirstOrDefault();

                        if (DS_TicketToolAnalysis != null && DS_TicketToolAnalysis.Tables.Count > 0)
                        {

                            // prepare data table for chart 
                            DataTable CombinedTicketSummary = new DataTable();
                            CombinedTicketSummary.Columns.Add("TableType", typeof(string));

                            // Helper local function to ensure columns from a source table exist in combined table
                            void EnsureColumnsInCombined(DataTable source)
                            {
                                foreach (DataColumn col in source.Columns)
                                {
                                    if (!CombinedTicketSummary.Columns.Contains(col.ColumnName))
                                    {
                                        CombinedTicketSummary.Columns.Add(col.ColumnName, col.DataType ?? typeof(string));
                                    }
                                }
                            }

                            // ------------------------------
                            // Table 1 (original Tables[0]) - keep column names
                            // ------------------------------
                            if (DS_TicketToolAnalysis.Tables.Count > 0
                                && DS_TicketToolAnalysis.Tables[0].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t1 = DS_TicketToolAnalysis.Tables[0].DefaultView
                                    .ToTable(false, "Requester", "TicketCount");
                                t1.Columns["Requester"].ColumnName = "Ticket Type";
                                t1.Columns["TicketCount"].ColumnName = "Total";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t1);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t1.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table1";

                                    foreach (DataColumn col in t1.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 2 (original Tables[1]) - keep column names
                            // ------------------------------
                            if (DS_TicketToolAnalysis.Tables.Count > 1
                                && DS_TicketToolAnalysis.Tables[1].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t2 = DS_TicketToolAnalysis.Tables[1].DefaultView
                                    .ToTable(false, "WeekDay", "TicketCount");
                                t2.Columns["WeekDay"].ColumnName = "Ticket Type";
                                t2.Columns["TicketCount"].ColumnName = "Total";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t2);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t2.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table2";

                                    foreach (DataColumn col in t2.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 3 (original Tables[2]) - keep column names
                            // ------------------------------
                            if (DS_TicketToolAnalysis.Tables.Count > 2
                                && DS_TicketToolAnalysis.Tables[2].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t3 = DS_TicketToolAnalysis.Tables[2].DefaultView
                                    .ToTable(false, "WeekNumber", "TicketCount");

                                t3.Columns["WeekNumber"].ColumnName = "Ticket Type";
                                t3.Columns["TicketCount"].ColumnName = "Total";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t3);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t3.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table3";

                                    foreach (DataColumn col in t3.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }




                            CombinedTicketSummary.TableName = slideConfig?.SlideName ?? "Tickets Tool Analysis";
                            CombinedTicketSummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C030";
                            CombinedTicketSummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(CombinedTicketSummary);

                    }
                }


                    if (splitCodes.Contains("C031"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C031").FirstOrDefault();

                        //if (ResolutionPerformance != null && ResolutionPerformance.Tables.Count > 0)
                        //{
                        DataTable ResourceWiseAlerts = DS_ResourceWiseAlerts.Tables[0].Copy();

                        ResourceWiseAlerts.TableName = slideConfig?.SlideName ?? "Resource wise Alert Tickets Analysis – Top Talkers";
                        ResourceWiseAlerts.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C031";
                        ResourceWiseAlerts.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                        ResourceWiseAlerts.Columns.Remove("DepartmentId");
                        ResourceWiseAlerts.Columns.Remove("DepartmentName");



                        SlidedataSet.Tables.Add(ResourceWiseAlerts);

                        //}
                    }

                    if (splitCodes.Contains("C032"))
                    {

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C032").FirstOrDefault();

                        if (DS_ChangeSummaryLast3MonthsTrend != null && DS_ChangeSummaryLast3MonthsTrend.Tables.Count > 0)
                        {
                            DataTable ChangeSummaryLast3MonthsTrend = DS_ChangeSummaryLast3MonthsTrend.Tables[0].Copy();

                        ChangeSummaryLast3MonthsTrend.TableName = slideConfig?.SlideName ?? "Change Summary - Last 3 Months Trend";
                        ChangeSummaryLast3MonthsTrend.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C032";
                        ChangeSummaryLast3MonthsTrend.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                        ChangeSummaryLast3MonthsTrend.Columns.Remove("DepartmentId");
                        ChangeSummaryLast3MonthsTrend.Columns.Remove("DepartmentName");
                        ChangeSummaryLast3MonthsTrend.Columns.Remove("MonthDate");
                        ChangeSummaryLast3MonthsTrend.Columns["MonthName"].ColumnName = "Ticket Type";




                            SlidedataSet.Tables.Add(ChangeSummaryLast3MonthsTrend);

                        }
                    }










                    #region Ticket Details Analyzation

                    if (splitCodes.Contains("C002"))
                    {
                        #region Response Status

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C002").FirstOrDefault();

                        if (DS_SummaryResponsePrioritySLA != null
                            && DS_SummaryResponsePrioritySLA?.Tables.Count > 0
                            && DS_SummaryResponsePrioritySLA.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable ResponseStatusTable = DS_SummaryResponsePrioritySLA.Tables[0].DefaultView
                        .ToTable(false, "type", "statustype", "Urgent", "High", "Medium", "Low", "GrandTotal", "AchievedPercentage");

                            if (ResponseStatusTable.Columns.Contains("type"))
                            {
                                ResponseStatusTable.Columns["type"].ColumnName = "Ticket Type";
                            }

                            if (ResponseStatusTable.Columns.Contains("Urgent"))
                            {
                                ResponseStatusTable.Columns["Urgent"].ColumnName = "Urgent";
                            }

                            if (ResponseStatusTable.Columns.Contains("High"))
                            {
                                ResponseStatusTable.Columns["High"].ColumnName = "High";
                            }

                            if (ResponseStatusTable.Columns.Contains("Medium"))
                            {
                                ResponseStatusTable.Columns["Medium"].ColumnName = "Medium";
                            }

                            if (ResponseStatusTable.Columns.Contains("Low"))
                            {
                                ResponseStatusTable.Columns["Low"].ColumnName = "Low";
                            }

                            if (ResponseStatusTable.Columns.Contains("statustype"))
                            {
                                ResponseStatusTable.Columns["statustype"].ColumnName = "Response Status";
                            }

                            if (ResponseStatusTable.Columns.Contains("GrandTotal"))
                            {
                                ResponseStatusTable.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            if (ResponseStatusTable.Columns.Contains("AchievedPercentage"))
                            {
                                ResponseStatusTable.Columns["AchievedPercentage"].ColumnName = "Achieved Percentage (%)";
                            }

                            foreach (DataRow row in ResponseStatusTable.Rows)
                            {
                                foreach (DataColumn column in ResponseStatusTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            ResponseStatusTable.TableName = slideConfig?.SlideName;
                            ResponseStatusTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C002";
                            ResponseStatusTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(ResponseStatusTable);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C003"))
                    {
                        #region Resolution Status

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C003").FirstOrDefault();

                        if (DS_SummaryResolutionPrioritySLA != null
                            && DS_SummaryResolutionPrioritySLA?.Tables.Count > 0
                            && DS_SummaryResolutionPrioritySLA.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable ResolutionStatusTable = DS_SummaryResolutionPrioritySLA.Tables[0].DefaultView
                            .ToTable(false, "type", "statustype", "Urgent", "High", "Medium", "Low", "GrandTotal", "AchievedPercentage");

                            if (ResolutionStatusTable.Columns.Contains("type"))
                            {
                                ResolutionStatusTable.Columns["type"].ColumnName = "Ticket Type";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Urgent"))
                            {
                                ResolutionStatusTable.Columns["Urgent"].ColumnName = "Urgent";
                            }

                            if (ResolutionStatusTable.Columns.Contains("High"))
                            {
                                ResolutionStatusTable.Columns["High"].ColumnName = "High";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Medium"))
                            {
                                ResolutionStatusTable.Columns["Medium"].ColumnName = "Medium";
                            }

                            if (ResolutionStatusTable.Columns.Contains("Low"))
                            {
                                ResolutionStatusTable.Columns["Low"].ColumnName = "Low";
                            }

                            if (ResolutionStatusTable.Columns.Contains("statustype"))
                            {
                                ResolutionStatusTable.Columns["statustype"].ColumnName = "Resolution Status";
                            }

                            if (ResolutionStatusTable.Columns.Contains("GrandTotal"))
                            {
                                ResolutionStatusTable.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            if (ResolutionStatusTable.Columns.Contains("AchievedPercentage"))
                            {
                                ResolutionStatusTable.Columns["AchievedPercentage"].ColumnName = "Achieved Percentage (%)";
                            }

                            foreach (DataRow row in ResolutionStatusTable.Rows)
                            {
                                foreach (DataColumn column in ResolutionStatusTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            ResolutionStatusTable.TableName = slideConfig?.SlideName;
                            ResolutionStatusTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C003";
                            ResolutionStatusTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(ResolutionStatusTable);
                        }

                        #endregion
                    }

                    #endregion

                    if (splitCodes.Contains("C004") || splitCodes.Contains("C005"))
                    {
                        #region SLA Not Met Response Ticket Details

                        if (DS_SLANotMet != null
                            && DS_SLANotMet?.Tables.Count > 1
                            && DS_SLANotMet.Tables[1]?.Rows.Count > 0)
                        {
                            //removed "on_roaster_engineer, resolution_remarks"

                            // Filter for "Incident"
                            DataView dvSLAIncident = new DataView(DS_SLANotMet.Tables[1])
                            {
                                RowFilter = "type = 'incident' OR type = 'Incident'"
                            };

                            DataTable Tbl_SLAIncident = dvSLAIncident.ToTable(false, "SlNo", "id", "created_at_display", "type", "subject", "StatusName");

                            // Filter for "service request"
                            DataView dvSLAService = new DataView(DS_SLANotMet.Tables[1])
                            {
                                RowFilter = "type = 'service request' OR type = 'Service Request'"
                            };

                            DataTable Tbl_SLAService = dvSLAService.ToTable(false, "SlNo", "id", "created_at_display", "type", "subject", "StatusName");

                            Tbl_SLAIncident.Columns.Add("Remarks");
                            Tbl_SLAService.Columns.Add("Remarks");

                            if (splitCodes.Contains("C004"))
                            {
                                if (Tbl_SLAIncident.Rows.Count > 0)
                                {
                                    var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C004").FirstOrDefault();

                                    //foreach (DataRow row in Tbl_SLAService.Rows)
                                    //{
                                    //    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    //    {
                                    //        row["resolution_remarks"] = string.Empty;
                                    //    }
                                    //}

                                    // Reassign new SlNo based on the filtered order
                                    int incidentSlNo = 1;
                                    foreach (DataRow row in Tbl_SLAIncident.Rows)
                                    {
                                        row["SlNo"] = incidentSlNo++;  // Assign new SlNo and increment
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("SlNo"))
                                    {
                                        Tbl_SLAIncident.Columns["SlNo"].ColumnName = "SL";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("id"))
                                    {
                                        Tbl_SLAIncident.Columns["id"].ColumnName = "Ticket Id";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("created_at_display"))
                                    {
                                        Tbl_SLAIncident.Columns["created_at_display"].ColumnName = "Created Time";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("type"))
                                    {
                                        Tbl_SLAIncident.Columns["type"].ColumnName = "Ticket Type";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("subject"))
                                    {
                                        Tbl_SLAIncident.Columns["subject"].ColumnName = "Subject";
                                    }

                                    //if (Tbl_SLAIncident.Columns.Contains("on_roaster_engineer"))
                                    //{
                                    //    Tbl_SLAIncident.Columns["on_roaster_engineer"].ColumnName = "Engineer";
                                    //}

                                    if (Tbl_SLAIncident.Columns.Contains("resolution_remarks"))
                                    {
                                        Tbl_SLAIncident.Columns["resolution_remarks"].ColumnName = "Remarks";
                                    }

                                    if (Tbl_SLAIncident.Columns.Contains("StatusName"))
                                    {
                                        Tbl_SLAIncident.Columns["StatusName"].ColumnName = "Status";
                                    }

                                    Tbl_SLAIncident.TableName = slideConfig?.SlideName;
                                    Tbl_SLAIncident.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C004";
                                    Tbl_SLAIncident.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                    SlidedataSet.Tables.Add(Tbl_SLAIncident);
                                }
                            }

                            if (splitCodes.Contains("C005"))
                            {
                                if (Tbl_SLAService.Rows.Count > 0)
                                {
                                    var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C005").FirstOrDefault();

                                    //foreach (DataRow row in Tbl_SLAService.Rows)
                                    //{
                                    //    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    //    {
                                    //        row["resolution_remarks"] = string.Empty;
                                    //    }
                                    //}

                                    int serviceSlNo = 1;
                                    foreach (DataRow row in Tbl_SLAService.Rows)
                                    {
                                        row["SlNo"] = serviceSlNo++;  // Assign new SlNo and increment
                                    }

                                    if (Tbl_SLAService.Columns.Contains("SlNo"))
                                    {
                                        Tbl_SLAService.Columns["SlNo"].ColumnName = "SL";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("id"))
                                    {
                                        Tbl_SLAService.Columns["id"].ColumnName = "Ticket Id";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("created_at_display"))
                                    {
                                        Tbl_SLAService.Columns["created_at_display"].ColumnName = "Created Time";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("type"))
                                    {
                                        Tbl_SLAService.Columns["type"].ColumnName = "Ticket Type";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("subject"))
                                    {
                                        Tbl_SLAService.Columns["subject"].ColumnName = "Subject";
                                    }

                                    //if (Tbl_SLAService.Columns.Contains("on_roaster_engineer"))
                                    //{
                                    //    Tbl_SLAService.Columns["on_roaster_engineer"].ColumnName = "Engineer";
                                    //}

                                    if (Tbl_SLAService.Columns.Contains("resolution_remarks"))
                                    {
                                        Tbl_SLAService.Columns["resolution_remarks"].ColumnName = "Remarks";
                                    }

                                    if (Tbl_SLAService.Columns.Contains("StatusName"))
                                    {
                                        Tbl_SLAService.Columns["StatusName"].ColumnName = "Status";
                                    }

                                    Tbl_SLAService.TableName = slideConfig?.SlideName;
                                    Tbl_SLAService.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C005";
                                    Tbl_SLAService.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                    SlidedataSet.Tables.Add(Tbl_SLAService);
                                }
                            }
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C006"))
                    {
                        #region Ticket not closed

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C006").FirstOrDefault();

                        if (DS_TicketNotClosed != null && DS_TicketNotClosed.Tables.Count > 0)
                        {
                            // removed resolution_remarks

                            //DataTable TicketNotClosedDataTable = TicketNotClosedDataSet.Tables[0].Copy();
                            DataTable TicketNotClosedDataTable = DS_TicketNotClosed.Tables[0].DefaultView
                                .ToTable(false, "SlNo", "id", "created_at_display", "RequesterEmail", "subject");

                            //foreach (DataRow row in TicketNotClosedDataTable.Rows)
                            //{
                            //    if (TicketNotClosedDataTable.Columns.Contains("resolution_remarks"))
                            //    {
                            //        row["resolution_remarks"] = string.Empty;
                            //    }
                            //}

                            TicketNotClosedDataTable.Columns.Add("Remarks");

                            if (TicketNotClosedDataTable.Columns.Contains("SlNo"))
                            {
                                TicketNotClosedDataTable.Columns["SlNo"].ColumnName = "SL";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("id"))
                            {
                                TicketNotClosedDataTable.Columns["id"].ColumnName = "Ticket Id";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("created_at_display"))
                            {
                                TicketNotClosedDataTable.Columns["created_at_display"].ColumnName = "Created Time";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("subject"))
                            {
                                TicketNotClosedDataTable.Columns["subject"].ColumnName = "Subject";
                            }

                            if (TicketNotClosedDataTable.Columns.Contains("resolution_remarks"))
                            {
                                TicketNotClosedDataTable.Columns["resolution_remarks"].ColumnName = "Remarks";
                            }

                            TicketNotClosedDataTable.TableName = slideConfig?.SlideName;
                            TicketNotClosedDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C006";
                            TicketNotClosedDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketNotClosedDataTable);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C007"))
                    {
                        #region Category Wise Call Bifurcation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C007").FirstOrDefault();

                        if (DS_TicketByCategoryAndType != null
                            && DS_TicketByCategoryAndType?.Tables.Count > 0
                            && DS_TicketByCategoryAndType.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_CategoryAndType = DS_TicketByCategoryAndType.Tables[0].DefaultView
                        .ToTable(false, "category", "sub_category", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal");

                            if (Tbl_CategoryAndType.Columns.Contains("category"))
                            {
                                Tbl_CategoryAndType.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("sub_category"))
                            {
                                Tbl_CategoryAndType.Columns["sub_category"].ColumnName = "Sub-Category";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_CategoryAndType.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_CategoryAndType.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_CategoryAndType.Columns.Contains("GrandTotal"))
                            {
                                Tbl_CategoryAndType.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }


                            foreach (DataRow row in Tbl_CategoryAndType.Rows)
                            {
                                foreach (DataColumn column in Tbl_CategoryAndType.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            Tbl_CategoryAndType.TableName = slideConfig?.SlideName;
                            Tbl_CategoryAndType.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C007";
                            Tbl_CategoryAndType.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(Tbl_CategoryAndType);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C008"))
                    {
                        #region Priority wise Ticket Bifurcation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C008").FirstOrDefault();

                        if (DS_TicketByCategoryAndPriority != null
                            && DS_TicketByCategoryAndPriority?.Tables.Count > 0
                            && DS_TicketByCategoryAndPriority.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_SubCategoryAndType = DS_TicketByCategoryAndPriority.Tables[0].DefaultView
                        .ToTable(false, "category", "sub_category", "Urgent", "high", "medium", "Low", "GrandTotal");

                            if (Tbl_SubCategoryAndType.Columns.Contains("category"))
                            {
                                Tbl_SubCategoryAndType.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("sub_category"))
                            {
                                Tbl_SubCategoryAndType.Columns["sub_category"].ColumnName = "Sub-Category";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("high"))
                            {
                                Tbl_SubCategoryAndType.Columns["high"].ColumnName = "High";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("medium"))
                            {
                                Tbl_SubCategoryAndType.Columns["medium"].ColumnName = "Medium";
                            }

                            if (Tbl_SubCategoryAndType.Columns.Contains("GrandTotal"))
                            {
                                Tbl_SubCategoryAndType.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_SubCategoryAndType.Rows)
                            {
                                foreach (DataColumn column in Tbl_SubCategoryAndType.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            Tbl_SubCategoryAndType.TableName = slideConfig?.SlideName;
                            Tbl_SubCategoryAndType.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C008";
                            Tbl_SubCategoryAndType.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(Tbl_SubCategoryAndType);
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C009"))
                    {
                        #region Report for Last 3 Months                        

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C009").FirstOrDefault();

                        if (DS_SummaryLast3Months != null
                            && DS_SummaryLast3Months?.Tables.Count > 2
                            && DS_SummaryLast3Months.Tables[2]?.Rows.Count > 0)
                        {
                            DataTable Tbl_Last3Months = DS_SummaryLast3Months.Tables[2].DefaultView
                        .ToTable(false, "monthname", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal", "RowType");

                            if (Tbl_Last3Months.Columns.Contains("monthname"))
                            {
                                Tbl_Last3Months.Columns["monthname"].ColumnName = "Months";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_Last3Months.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_Last3Months.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("GrandTotal"))
                            {
                                Tbl_Last3Months.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_Last3Months.Rows)
                            {
                                foreach (DataColumn column in Tbl_Last3Months.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            DataTable NewTbl_Last3Months = Tbl_Last3Months.Copy();

                            // Remove the "RowType" column from the cloned structure
                            if (NewTbl_Last3Months.Columns.Contains("RowType"))
                            {
                                NewTbl_Last3Months.Columns.Remove("RowType");
                            }

                            NewTbl_Last3Months.TableName = slideConfig?.SlideName;
                            NewTbl_Last3Months.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C009";
                            NewTbl_Last3Months.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(NewTbl_Last3Months);

                            Tbl_Last3Months.TableName = (slideConfig?.SlideName) + " Graphical View";
                            Tbl_Last3MonthsForChart = Tbl_Last3Months;
                        }

                        #endregion
                    }

                    if (splitCodes.Contains("C010"))
                    {
                        #region Category Wise Ticket Analysis Report for Last 3 Months

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C010").FirstOrDefault();

                        if (DS_CatagoryWiseLast3Months != null
                            && DS_CatagoryWiseLast3Months?.Tables.Count > 0
                            && DS_CatagoryWiseLast3Months.Tables[0]?.Rows.Count > 0)
                        {
                            DataTable Tbl_Last3Months = DS_CatagoryWiseLast3Months.Tables[0].DefaultView
                        .ToTable(false, "category", "monthname", "ChangeRequest", "Incident", "ServiceRequest", "Problem", "GrandTotal", "RowType");

                            if (Tbl_Last3Months.Columns.Contains("category"))
                            {
                                Tbl_Last3Months.Columns["category"].ColumnName = "Category";
                            }

                            if (Tbl_Last3Months.Columns.Contains("monthname"))
                            {
                                Tbl_Last3Months.Columns["monthname"].ColumnName = "Months";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ChangeRequest"))
                            {
                                Tbl_Last3Months.Columns["ChangeRequest"].ColumnName = "Change Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("ServiceRequest"))
                            {
                                Tbl_Last3Months.Columns["ServiceRequest"].ColumnName = "Service Request";
                            }

                            if (Tbl_Last3Months.Columns.Contains("GrandTotal"))
                            {
                                Tbl_Last3Months.Columns["GrandTotal"].ColumnName = "Grand Total";
                            }

                            foreach (DataRow row in Tbl_Last3Months.Rows)
                            {
                                foreach (DataColumn column in Tbl_Last3Months.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            DataTable NewTbl_Last3Months = Tbl_Last3Months.Copy();

                            // Remove the "RowType" column from the cloned structure
                            if (NewTbl_Last3Months.Columns.Contains("RowType"))
                            {
                                NewTbl_Last3Months.Columns.Remove("RowType");
                            }

                            NewTbl_Last3Months.TableName = slideConfig?.SlideName;
                            NewTbl_Last3Months.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C010";
                            NewTbl_Last3Months.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(NewTbl_Last3Months);

                            //Tbl_Last3Months.TableName = (slideConfig?.SlideName ?? SlideHeadingConstants.CategoryWiseLast3Months) + " Graphical View";
                            //Tbl_Last3MonthsForChart = Tbl_Last3Months;
                        }

                        #endregion
                    }

                    #region Server Utilization

                    if (DS_ServerPerformanceReport != null && DS_ServerPerformanceReport.Any())
                    {
                        if (splitCodes.Contains("C011"))
                        {
                            #region Azure VM’s CPU Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C011").FirstOrDefault();

                            var CPUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "CPU");
                            if (CPUtilization?.Rows.Count > 0)
                            {
                                DataTable Tbl_CPUtilization = CPUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_CPUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_CPUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_CPUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_CPUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_CPUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_CPUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_CPUtilization.TableName = slideConfig?.SlideName;
                                Tbl_CPUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C011";
                                Tbl_CPUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_CPUtilization);
                            }

                            #endregion
                        }

                        if (splitCodes.Contains("C012"))
                        {
                            #region Azure VM’s Memory  Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C012").FirstOrDefault();

                            var MemoryUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "Memory");
                            if (MemoryUtilization?.Rows.Count > 0)
                            {

                                DataTable Tbl_MemoryUtilization = MemoryUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_MemoryUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_MemoryUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_MemoryUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_MemoryUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_MemoryUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_MemoryUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_MemoryUtilization.TableName = slideConfig?.SlideName;
                                Tbl_MemoryUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C012";
                                Tbl_MemoryUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_MemoryUtilization);
                            }

                            #endregion
                        }

                        if (splitCodes.Contains("C013"))
                        {
                            #region AzureVM’s Disk Utilization Report

                            var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C013").FirstOrDefault();

                            var DiskUtilization = await this._genPPTService.CreateDynamicDataTable(DS_ServerPerformanceReport, r => r.UtilizationType == "Disk");
                            if (DiskUtilization?.Rows.Count > 0)
                            {
                                DataTable Tbl_DiskUtilization = DiskUtilization.DefaultView
                                    .ToTable(false, "ServerName", "Average", "Minimum", "Maximum");

                                if (Tbl_DiskUtilization.Columns.Contains("ServerName"))
                                {
                                    Tbl_DiskUtilization.Columns["ServerName"].ColumnName = "Server Name";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Average"))
                                {
                                    Tbl_DiskUtilization.Columns["Average"].ColumnName = "Average (%)";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Minimum"))
                                {
                                    Tbl_DiskUtilization.Columns["Minimum"].ColumnName = "Minimum (%)";
                                }

                                if (Tbl_DiskUtilization.Columns.Contains("Maximum"))
                                {
                                    Tbl_DiskUtilization.Columns["Maximum"].ColumnName = "Maximum (%)";
                                }

                                Tbl_DiskUtilization.TableName = slideConfig?.SlideName;
                                Tbl_DiskUtilization.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C013";
                                Tbl_DiskUtilization.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                                SlidedataSet.Tables.Add(Tbl_DiskUtilization);
                            }

                            #endregion
                        }
                    }

                    #endregion

                    if (splitCodes.Contains("C014"))
                    {
                        #region Server wise auto-ticket generation

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C014").FirstOrDefault();

                        if (DS_TicketByResourceName != null && DS_TicketByResourceName.Tables.Count > 0)
                        {
                            DataTable TicketByResourceNameDataTable = DS_TicketByResourceName.Tables[0].DefaultView
                                .ToTable(false, "resource_name", "NoOfTickets");

                            if (TicketByResourceNameDataTable.Columns.Contains("resource_name"))
                            {
                                TicketByResourceNameDataTable.Columns["resource_name"].ColumnName = "Resource Name";
                            }

                            if (TicketByResourceNameDataTable.Columns.Contains("NoOfTickets"))
                            {
                                TicketByResourceNameDataTable.Columns["NoOfTickets"].ColumnName = "Count";
                            }


                            TicketByResourceNameDataTable.TableName = slideConfig?.SlideName;
                            TicketByResourceNameDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C014";
                            TicketByResourceNameDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketByResourceNameDataTable);
                        }

                        #endregion
                    }


                    if (splitCodes.Contains("C015"))
                    {
                        #region Ticket raised by User/Helpdesk

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C015").FirstOrDefault();

                        if (DS_ServiceRequestByUsers != null
                            && DS_ServiceRequestByUsers.Tables.Count > 0
                            && DS_ServiceRequestByUsers.Tables[0].Rows.Count > 1
                            )
                        {
                            DataTable TicketByResourceNameDataTable = DS_ServiceRequestByUsers.Tables[0].DefaultView
                                .ToTable(false, "Requester", "Count");

                            if (TicketByResourceNameDataTable.Columns.Contains("resource_name"))
                            {
                                TicketByResourceNameDataTable.Columns["resource_name"].ColumnName = "Resource Name";
                            }

                            foreach (DataRow row in TicketByResourceNameDataTable.Rows)
                            {
                                foreach (DataColumn column in TicketByResourceNameDataTable.Columns)
                                {
                                    if (row[column] != DBNull.Value)
                                    {
                                        string cellValue = row[column].ToString();
                                        if (cellValue.Contains("ZZZ-"))
                                        {
                                            row[column] = cellValue.Replace("ZZZ-", ""); // Replace the word
                                        }
                                    }
                                    else
                                    {
                                        row[column] = 0;
                                    }
                                }
                            }

                            TicketByResourceNameDataTable.TableName = slideConfig?.SlideName;
                            TicketByResourceNameDataTable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C015";
                            TicketByResourceNameDataTable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;
                            SlidedataSet.Tables.Add(TicketByResourceNameDataTable);
                        }

                        #endregion
                    }

                    //soumik rev

                    if (splitCodes.Contains("C022"))
                    {
                        var slideConfig = filter.SlideConfigList?
                            .FirstOrDefault(e => e.SlideCode == "C022") ?? new SlideConfigurations { };

                        if (DS_TicketByAvgResponseResolutionSummary != null
                            && DS_TicketByAvgResponseResolutionSummary.Tables.Count > 0
                            && DS_TicketByAvgResponseResolutionSummary.Tables[0].Rows.Count > 1)
                        {



                            DataTable TicketByAvgResponseResolutionSummary = DS_TicketByAvgResponseResolutionSummary.Tables[0].DefaultView
                                .ToTable();

                            TicketByAvgResponseResolutionSummary.TableName = slideConfig?.SlideName ?? "C022";
                            TicketByAvgResponseResolutionSummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C022";
                            TicketByAvgResponseResolutionSummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;


                            SlidedataSet.Tables.Add(TicketByAvgResponseResolutionSummary);

                        }
                    }

                    if (splitCodes.Contains("C023"))
                    {
                        //DS_TicketByDailyAndMonthlySummary

                        var slideConfig = filter.SlideConfigList?.Where(e => e.SlideCode == "C023").FirstOrDefault();

                        if (
                            DS_TicketByDailyAndMonthlySummary != null
                            &&
                            DS_TicketByDailyAndMonthlySummary.Tables.Count >= 1)
                        {
                            // Create combined table
                            DataTable CombinedTicketSummary = new DataTable();
                            CombinedTicketSummary.TableName = slideConfig?.SlideName ?? "C023";

                            // Keep same extended properties pattern
                            CombinedTicketSummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                            CombinedTicketSummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;

                            // Add TableType so we can filter later: values will be "Table1", "Table2", "Table3"
                            CombinedTicketSummary.Columns.Add("TableType", typeof(string));

                            // Helper local function to ensure columns from a source table exist in combined table
                            void EnsureColumnsInCombined(DataTable source)
                            {
                                foreach (DataColumn col in source.Columns)
                                {
                                    if (!CombinedTicketSummary.Columns.Contains(col.ColumnName))
                                    {
                                        CombinedTicketSummary.Columns.Add(col.ColumnName, col.DataType ?? typeof(string));
                                    }
                                }
                            }

                            // ------------------------------
                            // Table 1 (original Tables[0]) - keep column names
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 0
                                && DS_TicketByDailyAndMonthlySummary.Tables[0].Rows.Count > 0)
                            {
                                // Build the same projection
                                DataTable t1 = DS_TicketByDailyAndMonthlySummary.Tables[0].DefaultView
                                    .ToTable(false, "ActivityName", "ActivityCount");

                                // Rename columns exactly
                                if (t1.Columns.Contains("ActivityName"))
                                    t1.Columns["ActivityName"].ColumnName = "Daily and Monthly SR Activity Analysis";

                                if (t1.Columns.Contains("ActivityCount"))
                                    t1.Columns["ActivityCount"].ColumnName = "Activity Count";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t1);

                                // Copy rows and tag them as Table1
                                foreach (DataRow row in t1.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table1";

                                    foreach (DataColumn col in t1.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 2 (original Tables[1])
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 1
                                && DS_TicketByDailyAndMonthlySummary.Tables[1].Rows.Count > 0)
                            {
                                DataTable t2 = DS_TicketByDailyAndMonthlySummary.Tables[1].DefaultView
                                    .ToTable(false, "IncidentName", "SRName", "ActivityName");

                                // Rename as original
                                if (t2.Columns.Contains("IncidentName"))
                                    t2.Columns["IncidentName"].ColumnName = "Incident";

                                if (t2.Columns.Contains("SRName"))
                                    t2.Columns["SRName"].ColumnName = "Service Request";

                                if (t2.Columns.Contains("ActivityName"))
                                    t2.Columns["ActivityName"].ColumnName = "Daily / Monthly / Operational Calls";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t2);

                                // Copy rows and tag as Table2
                                foreach (DataRow row in t2.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table2";

                                    foreach (DataColumn col in t2.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // ------------------------------
                            // Table 3 (original Tables[2])
                            // ------------------------------
                            if (DS_TicketByDailyAndMonthlySummary.Tables.Count > 2
                                && DS_TicketByDailyAndMonthlySummary.Tables[2].Rows.Count > 0)
                            {
                                DataTable t3 = DS_TicketByDailyAndMonthlySummary.Tables[2].DefaultView
                                    .ToTable(false, "ActivityName", "ActivityCount");

                                // Rename exactly as in your original code
                                if (t3.Columns.Contains("ActivityName"))
                                    t3.Columns["ActivityName"].ColumnName = "Daily & Monthly SR Activity (Team Wise Bifurcation)";

                                if (t3.Columns.Contains("ActivityCount"))
                                    t3.Columns["ActivityCount"].ColumnName = "Count";

                                // Ensure combined has these columns (with same datatypes)
                                EnsureColumnsInCombined(t3);

                                // Copy rows and tag as Table3
                                foreach (DataRow row in t3.Rows)
                                {
                                    DataRow newRow = CombinedTicketSummary.NewRow();
                                    newRow["TableType"] = "Table3";

                                    foreach (DataColumn col in t3.Columns)
                                    {
                                        newRow[col.ColumnName] = row[col] ?? DBNull.Value;
                                    }

                                    CombinedTicketSummary.Rows.Add(newRow);
                                }
                            }

                            // Finally add the combined table once (you can filter using "TableType" later)
                            SlidedataSet.Tables.Add(CombinedTicketSummary);
                        }


                        //{

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[0].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[0].DefaultView
                        //            .ToTable(false, "ActivityName", "ActivityCount");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily and Monthly SR Activity Analysis";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityCount"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityCount"].ColumnName = "Activity Count";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode != null ? slideConfig?.SlideCode + " - Table 1" : "CO23  - Table 1";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[1].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[1].DefaultView
                        //            .ToTable(false, "IncidentName", "SRName", "ActivityName");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("IncidentName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["IncidentName"].ColumnName = "Incident";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("SRName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["SRName"].ColumnName = "Service Request";
                        //        }
                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily / Monthly / Operational Calls";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode != null ? slideConfig?.SlideCode + " - Table 2" : "CO23  - Table 2";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //    if (DS_TicketByDailyAndMonthlySummary.Tables[2].Rows.Count > 0) //
                        //    {

                        //        DataTable TicketByDailyAndMonthlySummary =
                        //            DS_TicketByDailyAndMonthlySummary.Tables[2].DefaultView
                        //            .ToTable(false, "ActivityName", "ActivityCount");

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityName"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityName"].ColumnName = "Daily & Monthly SR Activity (Team Wise Bifurcation)";
                        //        }

                        //        if (TicketByDailyAndMonthlySummary.Columns.Contains("ActivityCount"))
                        //        {
                        //            TicketByDailyAndMonthlySummary.Columns["ActivityCount"].ColumnName = "Count";
                        //        }

                        //        TicketByDailyAndMonthlySummary.TableName = slideConfig?.SlideCode !=null ? slideConfig?.SlideCode + " - Table 3" : "CO23  - Table 3";
                        //        // later will change name in service
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? "C023";
                        //        TicketByDailyAndMonthlySummary.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder ?? 0;
                        //        SlidedataSet.Tables.Add(TicketByDailyAndMonthlySummary);

                        //    }

                        //}

                    }

                    //soumik rev

                    string[] staticSlideCodes = { "C016", "C017", "C018", "C019", "C020", "C021" };

                    foreach (string code in staticSlideCodes)
                    {
                        if (splitCodes.Contains(code))
                        {
                            var slideConfig = filter.SlideConfigList?.FirstOrDefault(e => e.SlideCode == code);

                            var datatable = new DataTable
                            {
                                TableName = code
                            };

                            datatable.ExtendedProperties["Code"] = slideConfig?.SlideCode ?? code;
                            datatable.ExtendedProperties["SortOrder"] = slideConfig?.SortOrder;

                            SlidedataSet.Tables.Add(datatable);
                        }
                    }

                }


                // PPT Processing

                try
                {
                    string mainPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Documents", filter.departmentId.ToString(), DateTime.Now.ToString("MMMMyyyy"), "Presentations");
                    if (!System.IO.Directory.Exists(mainPath))
                    {
                        System.IO.Directory.CreateDirectory(mainPath);
                    }

                    //string JSONfilePath = "D:\\Json\\SlidedataSet.json";
                    string JSONfilePath = Path.Combine(mainPath, "SlidedataSet.json");

                    //_ = Task.Run(() => this.SaveDataSetToJson(SlidedataSet, JSONfilePath));

                    //string JSONfilePath2 = "D:\\Json\\Tbl_Last3MonthsForChart.json";
                    if (Tbl_Last3MonthsForChart != null && Tbl_Last3MonthsForChart.Rows.Count > 0)
                    {
                        string JSONfilePath2 = Path.Combine(mainPath, "Tbl_Last3MonthsForChart.json");
                        //_ = Task.Run(() => this.SaveDataTableToJson(Tbl_Last3MonthsForChart, JSONfilePath2));
                    }
                }
                catch (Exception ex)
                {

                }

                string FilePath = "";

                try
                {
                    string MonthName = DateTime.ParseExact(filter.start_date, "dd/MM/yyyy", null).ToString("MMMM yyyy");
                    var dataObj = new HelperModel
                    {
                        DirectoryName = filter.departmentId.ToString(),
                        DepartmentId = filter.departmentId.ToString(),
                        MonthName = MonthName,
                        DataTableForChart = Tbl_Last3MonthsForChart
                    };

                    if (SlidedataSet != null
                        && SlidedataSet.Tables.Count > 0)
                    {
                        DataSet sortedDataSet = sortedDataSet = SortDataSetTablesBySortOrder(SlidedataSet);
                        if (sortedDataSet != null)
                        {
                            FilePath = await this._genPPTService.GeneratePptForOnMobile(SlidedataSet,dataObj);
                        }
                        else
                        {
                            FilePath = await this._genPPTService.GeneratePptForOnMobile(SlidedataSet, dataObj);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }


                if (taskExceptions.Any())
                {
                    foreach (var ex in taskExceptions)
                    {
                        this._logger.LogError(ex, "Task execution error");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }


                return this.Ok(new { Message = FilePath });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Execution failed at GenerateBarChart()");
                return this.Problem(ex.Message);
            }
        }

        //soumik rev 25-11-2025






        public async Task SaveDataSetToJson(DataSet dataSet, string filePath)
        {
            try
            {
                //string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
                string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented, new DataSetConverter());
                System.IO.File.WriteAllText(filePath, json);
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public DataSet? LoadDataSetFromJson(string filePath)
        {
            try
            {
                string json = System.IO.File.ReadAllText(filePath);
                //return JsonConvert.DeserializeObject<DataSet>(json);
                return JsonConvert.DeserializeObject<DataSet>(json, new DataSetConverter());
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task SaveDataTableToJson(DataTable table, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(table, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, json);
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
        }

        public DataSet SortDataSetTablesBySortOrder(DataSet dataSet, bool ascending = true)
        {
            try
            {
                // Convert tables to a list and sort based on ExtendedProperties["SortOrder"]
                var sortedTables = dataSet.Tables.Cast<DataTable>()
                    .OrderBy(t => t.ExtendedProperties.ContainsKey("SortOrder")
                                  ? Convert.ToInt32(t.ExtendedProperties["SortOrder"])
                                  : int.MaxValue) // Default to max value if SortOrder is missing
                    .ToList();

                // Reverse the list if descending order is required
                if (!ascending)
                {
                    sortedTables.Reverse();
                }

                // Create a new DataSet
                DataSet sortedDataSet = new DataSet();

                foreach (var table in sortedTables)
                {
                    // Clone the table structure (without data)
                    DataTable clonedTable = table.Clone();

                    // Copy ExtendedProperties (like "SortOrder")
                    foreach (var key in table.ExtendedProperties.Keys)
                    {
                        clonedTable.ExtendedProperties[key] = table.ExtendedProperties[key];
                    }

                    // Import all rows to the cloned table
                    foreach (DataRow row in table.Rows)
                    {
                        clonedTable.ImportRow(row);
                    }

                    // Add cloned table to new DataSet
                    sortedDataSet.Tables.Add(clonedTable);
                }

                return sortedDataSet;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}