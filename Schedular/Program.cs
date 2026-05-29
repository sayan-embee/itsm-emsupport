
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
namespace Schedular
{
    using System;
    using Common.Layer.Models.FreshService;
    using System.Reflection;
    using DataAccess.Layer.Data.FreshService;
    using DataAccess.Layer.Data.Site24x7;
    using DataAccess.Layer.DbAccess;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Schedular.ExceptionLog;
    using Schedular.FreshService;
    using Schedular.Site24x7;

    public static class Program
    {
       static FreshServiceUtility? _freshServiceUtility;
       static Site24x7Utility? _site24x7Utility;

        async static Task Main(string[] args)
        {            

            // Create a host builder to configure DI and other services
            var host = Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     // Read configuration from appsettings.json
                     config.SetBasePath(Directory.GetCurrentDirectory());
                     config.AddJsonFile("config.json", optional: false, reloadOnChange: true);
                 })
                .ConfigureServices((context, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole());
                    services.AddSingleton<TelemetryClient>();
                    // Register your application services here
                    services.AddSingleton<FreshServiceUtility>();
                    services.AddSingleton<Site24x7Utility>();
                    services.AddSingleton<IFreshServiceData, FreshServiceData>();  // Register your services
                    services.AddSingleton<ISQLDataAccess, SQLDataAccess>();
                    services.AddSingleton<ISite24x7Data, Site24x7Data>();
                })
                .Build();

            if (args != null)
            {                
                switch(args[0].ToLower().Trim())
                {
                    case "call_fresh_service_department": await ProcessFreshServiceDepartmentAsync(host); break;
                    case "call_fresh_service_tickets": await ProcessFreshServiceTicketsAsync(host); break;
                    case "call_fresh_service_changes": await ProcessFreshServiceChangesAsync(host); break;
                    case "call_fresh_service_problem": await ProcessFreshServiceProblemAsync(host); break;
                    case "call_fresh_service_requesters": await ProcessFreshServiceRequestersAsync(host); break;
                    case "call_site24x7_performance_report": await ProcessSite24x7PerformanceReportAsync(host); break;
                    //soumik rev end 11-03-2025
                    case "call_fresh_service_ticketsbydate": await ProcessFreshServiceTicketsByCreatedDateAsync(host); break;
                    case "call_site24x7_performance_report_monthly": await ProcessSite24x7PerformanceReportMonthlyAsync(host); break;
                    case "call_fresh_service_tickets_Stat_Update": await FreshServiceMissingTicketStats(host); break;
                    //soumik rev start 11-03-2025
                    case "call_fresh_service_ticketsbydate_v2": await ProcessFreshServiceTicketsByCreatedDateAsyncV2(host); break;

                    default: break;
                }
            }

        }

        private static async Task ProcessFreshServiceDepartmentAsync(IHost host)
        {
            #region processing fresh service data for department
            ExceptionLogging.WriteMessageToText($"=============== FreshService Department Sync Starts ================");
            try
            {
                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();
                await _freshServiceUtility.Departments($"&page=1&per_page=100", true, true);

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

            ExceptionLogging.WriteMessageToText($"=============== FreshService Department Sync Ends ================");
            #endregion
        }
        private static async Task ProcessFreshServiceTicketsAsync(IHost host)
        {
            #region processing fresh service data for tickets
            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:SaveToDB"]);
               
                DateTime dtDate = DateTime.Now.AddDays(-backdays);
                string year = dtDate.Year.ToString();
                string month = dtDate.Month <= 9 ? "0" + dtDate.Month.ToString() : dtDate.Month.ToString();
                string day = dtDate.Day <= 9 ? "0" + dtDate.Day.ToString() : dtDate.Day.ToString();


                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();

               
                await _freshServiceUtility.Tickets($"{year}-{month}-{day}T00:00:00Z", isPaging, pageRowIndex, pageSize, saveToDB);



            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets Sync Ends ================");
            
            #endregion
        }
        private static async Task ProcessFreshServiceChangesAsync(IHost host)
        {
            #region processing fresh service data for changes
            ExceptionLogging.WriteMessageToText($"=============== FreshService changes Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Changes:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Changes:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Changes:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Changes:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Changes:SaveToDB"]);

                DateTime dtDate = DateTime.Now.AddDays(-backdays);
                string year = dtDate.Year.ToString();
                string month = dtDate.Month <= 9 ? "0" + dtDate.Month.ToString() : dtDate.Month.ToString();
                string day = dtDate.Day <= 9 ? "0" + dtDate.Day.ToString() : dtDate.Day.ToString();


                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();


                await _freshServiceUtility.Changes($"{year}-{month}-{day}T00:00:00Z", isPaging, pageRowIndex, pageSize, saveToDB);



            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== FreshService changes Sync Ends ================");
            #endregion
        }

        private static async Task ProcessFreshServiceProblemAsync(IHost host)
        {
            #region processing fresh service data for problem
            ExceptionLogging.WriteMessageToText($"=============== FreshService problem Async Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Problem:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Problem:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Problem:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Problem:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Problem:SaveToDB"]);

                DateTime dtDate = DateTime.Now.AddDays(-backdays);
                string year = dtDate.Year.ToString();
                string month = dtDate.Month <= 9 ? "0" + dtDate.Month.ToString() : dtDate.Month.ToString();
                string day = dtDate.Day <= 9 ? "0" + dtDate.Day.ToString() : dtDate.Day.ToString();


                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();


                await _freshServiceUtility.Problem($"{year}-{month}-{day}T00:00:00Z", isPaging, pageRowIndex, pageSize, saveToDB);



            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== FreshService problem Sync Ends ================");
            #endregion
        }
        private static async Task ProcessFreshServiceRequestersAsync(IHost host)
        {
            #region processing fresh service data for tickets
            ExceptionLogging.WriteMessageToText($"=============== FreshService Requesters Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Requesters:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Requesters:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Requesters:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Requesters:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Requesters:SaveToDB"]);

                var AllRecords = Convert.ToBoolean(iconfig["AppConfig:FreshService:Requesters:AllRecords"]);
                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();
                string fromDate = "", toDate="";
                if (!AllRecords)
                {
                    DateTime dtDate = DateTime.Now.AddDays(-backdays);
                    string year = dtDate.Year.ToString();
                    string month = dtDate.Month <= 9 ? "0" + dtDate.Month.ToString() : dtDate.Month.ToString();
                    string day = dtDate.Day <= 9 ? "0" + dtDate.Day.ToString() : dtDate.Day.ToString();

                    DateTime dtDateToday = DateTime.Now;
                    string yearTo = dtDateToday.Year.ToString();
                    string monthTo = dtDateToday.Month <= 9 ? "0" + dtDateToday.Month.ToString() : dtDateToday.Month.ToString();
                    string dayTo = dtDateToday.Day <= 9 ? "0" + dtDateToday.Day.ToString() : dtDateToday.Day.ToString();
                    fromDate = $"{year}-{month}-{day}T00:00:00Z";
                    toDate = $"{yearTo}-{monthTo}-{dayTo}T23:59:59Z";
                    
                }
                await _freshServiceUtility.Requesters(fromDate, toDate, isPaging, pageRowIndex, pageSize, saveToDB);



            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== FreshService Requesters Sync Ends ================");
            #endregion
        }

        private static async Task ProcessSite24x7PerformanceReportAsync(IHost host)
        {
            ExceptionLogging.WriteMessageToText($"=============== Site 24x7 Performance Report Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                int backdays = Convert.ToInt32(iconfig["AppConfig:Site24x7:PerformanceReport:NoOfBackDays"]);
                _site24x7Utility = host.Services.GetRequiredService<Site24x7Utility>();
                await _site24x7Utility.ProcessDataSyncSite24x7(DateTime.Now.AddDays(-backdays), DateTime.Now.AddDays(-backdays));

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== Site 24x7 Performance Report Sync Ends ================");

        }

        //soumik rev start 11-03-2025
        private static async Task ProcessFreshServiceTicketsByCreatedDateAsync(IHost host)
        {
            #region processing fresh service data for tickets
            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets By Created Date Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:SaveToDB"]);

                string spesificDateConfig = iconfig["AppConfig:FreshService:Tickets:SpesificDate"];
                int NoOfBackSpesificDate = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:NoOfBackSpesificDate"]);

                DateTime fromDate, toDate;

                if (!string.IsNullOrEmpty(spesificDateConfig) && DateTime.TryParse(spesificDateConfig, out DateTime spesificDate))
                {
                    fromDate = spesificDate;
                    toDate = spesificDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                else
                {
                    DateTime dtDate = DateTime.Now.AddDays(-NoOfBackSpesificDate);
                    fromDate = dtDate.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                    toDate = dtDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                fromDate = fromDate.AddHours(-5).AddMinutes(-30);
                toDate = toDate.AddHours(-5).AddMinutes(-30);

                string fromDateUtc = fromDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string toDateUtc = toDate.ToString("yyyy-MM-ddTHH:mm:ssZ");


                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();


                await _freshServiceUtility.TicketsByCreatedDate(fromDateUtc, toDateUtc, isPaging, pageRowIndex, pageSize, saveToDB);



            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets By Created Date Sync Ends ================");

            #endregion
        }

        private static async Task ProcessFreshServiceTicketsByCreatedDateAsyncV2(IHost host)
        {
            #region processing fresh service data for tickets
            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets By Created Date Sync Starts ================");

            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                var isPaging = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:Paging"]);
                int pageRowIndex = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageRowIndex"]);
                int pageSize = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:PageSize"]);
                int backdays = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:NoOfBackDays"]);
                var saveToDB = Convert.ToBoolean(iconfig["AppConfig:FreshService:Tickets:SaveToDB"]);

                int NoOfBackSpesificDate = Convert.ToInt32(iconfig["AppConfig:FreshService:Tickets:SyncDays"]);

                DateTime fromDate, toDate;

                DateTime dtDate = DateTime.Now.AddDays(-NoOfBackSpesificDate);
                fromDate = dtDate.Date.AddHours(00).AddMinutes(00).AddSeconds(00);
                toDate = dtDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                fromDate = fromDate.AddHours(-5).AddMinutes(-30);
                toDate = toDate.AddHours(-5).AddMinutes(-30);

                string fromDateUtc = fromDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                string toDateUtc = toDate.ToString("yyyy-MM-ddTHH:mm:ssZ");


                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();


                await _freshServiceUtility.TicketsByCreatedDate(fromDateUtc, toDateUtc, isPaging, pageRowIndex, pageSize, saveToDB);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

            ExceptionLogging.WriteMessageToText($"=============== FreshService Tickets By Created Date Sync Ends ================");
            #endregion
        }

        private static async Task ProcessSite24x7PerformanceReportMonthlyAsync(IHost host)
        {
            ExceptionLogging.WriteMessageToText($"=============== Site 24x7 Performance Report Monthly Sync Starts ================");
            try
            {
                IConfiguration iconfig = host.Services.GetRequiredService<IConfiguration>();
                int backMonth = Convert.ToInt32(iconfig["AppConfig:Site24x7:PerformanceReport:NoOfBackMonth"]);
                _site24x7Utility = host.Services.GetRequiredService<Site24x7Utility>();


                DateTime now = DateTime.Now;
                DateTime targetMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-backMonth);

                DateTime fromDate   = new DateTime(targetMonth.Year, targetMonth.Month, 1, 0, 0, 0);
                DateTime toDate     = new DateTime(targetMonth.Year, targetMonth.Month, 1, 0, 0, 0).AddMonths(1);

                // Format as "yyyy-MM-ddTHH:mm:ss%2B0530"
                string start_date = $"{fromDate:yyyy-MM-dd}T00:00:00%2B0530";
                string end_date = $"{toDate:yyyy-MM-dd}T00:00:00%2B0530";


                await _site24x7Utility.ProcessDataSyncMonthlySite24x7(start_date, end_date);

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            ExceptionLogging.WriteMessageToText($"=============== Site 24x7 Performance Report Monthly Sync Ends ================");

        }

        //soumik rev End 11-03-2025

        // soumik rev 05-11-2025
        private static async Task FreshServiceMissingTicketStats(IHost host)
        {
            ExceptionLogging.WriteMessageToText("=============== FreshService Missing Ticket Stats Starts ================");

            try
            {
                _freshServiceUtility = host.Services.GetRequiredService<FreshServiceUtility>();
                await _freshServiceUtility.TicketsByTicketIdAsync();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

            ExceptionLogging.WriteMessageToText("=============== FreshService Missing Ticket Stats Ends ================");
        }
        // soumik rev 05-11-2025



    }
}
