using Common.Layer.Models.FreshService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Layer.Models.Report
{
    public class ReportModel
    {
        //
    }

    public class SlideSettings
    {
        public Dictionary<string, string> SlideHeadingText { get; set; }
        public Config Config { get; set; }
    }

    public class Config
    {
        public Heading Heading { get; set; }
        public TableHeader TableHeader { get; set; }
        public TableRows TableRows { get; set; }
        public TableFooter TableFooter { get; set; }
        public BarGraph BarGraph { get; set; }
    }

    public class Heading
    {
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public bool HasUnderline { get; set; }
    }

    public class TableHeader
    {
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public int Height { get; set; }
    }

    public class TableRows
    {
        public string EvenBackgroundColor { get; set; }
        public string OodBackgroundColor { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public int Height { get; set; }
        public int MaxCountPerTable { get; set; }
    }

    public class TableFooter
    {
        public string BackgroundColor { get; set; }
        public string FontColor { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public int Height { get; set; }
    }

    public class BarGraph
    {
        public BarGraphItem ClosedTickets { get; set; }
        public BarGraphItem Last3Months { get; set; }
    }

    public class BarGraphItem
    {
        public string BackgroundColor { get; set; }
        public string BarColor { get; set; }
    }

    public enum SlideEnum
    {
        FirstSlide,
        AgendaSlide,
        IncidentSlide,
        MonthlyServiceCover,
        ThankYouSlide,
        Last3MonthsSlide,
        TicketByAvgResponseResolution,
        DailyMonthlyActivity,
        FirstSlideOnMobile,
        IncidentTrendAnalysis,
        ResponseResolutionPerformanceDetails,
            NetworkCategory


    }

    public enum SlideCodeEnum
    {
        C001,
        C002,
        C003,
        C004,
        C005,
        C006,
        C007,
        C008,
        C009,
        C010,
        C011,
        C012,
        C013,
        C014,
        C015,
        C016,
        C017,
        C018,
        C019,
        C020,
        C021,
        C022,
        C023,
        C024,
        C027,
        C029,
        C028,
        C030,
        C032


    }

    //public static class SlideHeadingConstants
    //{
    //    //public const string ClosedTickets = "Closed Ticket Analysis Report";
    //    public const string TDResponseStatus = "Ticket Details Response Status";
    //    public const string TDResolutionStatus = "Ticket Details Resolution Status";
    //    public const string SLANotMet_INC = "SLA Not Met Ticket Details - Incident";
    //    public const string SLANotMet_SR = "SLA Not Met Ticket Details - Service Request";
    //    public const string OpenTickets = "Open Ticket Details";
    //    public const string CallBifurcation_Category = "Category Wise Call Bifurcation";
    //    public const string CallBifurcation_Priority = "Priority Wise Ticket Bifurcation";
    //    public const string Last3Months = "Ticket Analysis Report for Last 3 Months";
    //    public const string CategoryWiseLast3Months = "Category Wise Ticket Analysis Report for Last 3 Months";
    //    public const string Disk_Utilization = "Azure VM’s Disk Utilization Report";
    //    public const string Memory_Utilization = "Azure VM’s Memory Utilization Report";
    //    public const string CPU_Utilization = "Azure VM’s CPU Utilization Report";
    //    public const string Server_Wise_Auto_Ticket = "Server wise auto-ticket generation (Top – 20)";
    //    public const string Server_Wise_User_Ticket = "Ticket raised by User/Helpdesk";
    //}

    public class DepartmentMasterModel
    {
        public string name { get; set; }
        public long? id { get; set; }
        public string ReportType { get; set; }
        public bool? active { get; set; } = true;
    }
    //soumik rev
    
    public class MissingTicketModel
    {
        public long? AutoId { get; set; }
        public long? TicketId { get; set; }
        public bool? IsTicketExists { get; set; }
        public bool? IsCustTicketExists { get; set; }
        public bool? IsStatTicketExists { get; set; }
    }

    //soumik rev

    public class UserDetailsModel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("userName")]
        public string? UserName { get; set; }

        [JsonProperty("userEmail")]
        public string? UserEmail { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("teamsTab")]
        public bool? TeamsTab { get; set; }

        [JsonProperty("monthlyReportTab")]
        public bool? MonthlyReportTab { get; set; }

        [JsonProperty("contractTab")]
        public bool? ContractTab { get; set; }
    }

    public class FilterModel
    {
        [JsonProperty("departmentId")]
        public long departmentId {  get; set; }

        [JsonProperty("start_date")]
        public string? start_date { get; set; }

        [JsonProperty("end_date")]
        public string? end_date { get; set; }

        [JsonProperty("zaaid")]
        public string? zaaid { get; set; }

        [JsonProperty("slideCodeList")]
        public string? SlideCodeList { get; set; }

        [JsonProperty("slideConfigList")]
        public List<SlideConfigurations>? SlideConfigList { get; set; }
    }

    public class ParamModel
    {
        [JsonProperty("filter")]
        public FilterModel Filter { get; set; }

        [JsonProperty("slideConfig")]
        public List<SlideConfigurations> SlideConfig { get; set; }
    }
    
    public class SlideConfigurations
    {
        [JsonProperty("slideCode")]
        public string SlideCode { get; set; }

        [JsonProperty("slideName")]
        public string SlideName { get; set; }

        [JsonProperty("columns")]
        public string Columns { get; set; }

        [JsonProperty("customColumns")]
        public string CustomColumns { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }    
    }

    public class HelperModel
    {
        public string DirectoryName { get; set; }
        public string DepartmentId { get; set; }
        public string MonthName { get; set; }
        public List<SlideConfigurations> SlideConfig { get; set; }
        public DataTable? DataTableForChart { get; set; }
    }
}
