using Common.Layer.Models;
using Common.Layer.Models.Site24x7;
using DataAccess.Layer.Data.FreshService;
using DataAccess.Layer.Data.Site24x7;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Schedular.ExceptionLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Schedular.Site24x7
{
    internal class Site24x7Utility
    {

        private readonly IConfiguration _configuration;
        private readonly ISite24x7Data _site24x7Data;
        private readonly string? _domainUrl;
        private readonly string? _endPointBaseUrl;
        private readonly string? _clientId;
        private readonly string? _APIKey;
        public Site24x7Utility(
             IConfiguration configuration
             , ISite24x7Data site24x7Data
            )

        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            this._site24x7Data = site24x7Data ?? throw new ArgumentNullException(nameof(IFreshServiceData));

            this._domainUrl = Convert.ToString(this._configuration["AppConfig:Site24x7:apiRootUrl"]);
            this._endPointBaseUrl = Convert.ToString(this._configuration["EndPointBaseUrl"]);
            this._clientId = Convert.ToString(this._configuration["AppConfig:Site24x7:clientId"]);
            this._APIKey = Convert.ToString(this._configuration["API_Key"]); ;
        }
        private  async Task<string> GetAccessToken()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this._endPointBaseUrl}/api/site24x7/accesstoken?ClientId={this._clientId}");
                request.Headers.Add("API_Key", this._APIKey);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string ret = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(ret))
                {
                    var result = JsonConvert.DeserializeObject<AccessTokenDetails>(ret);
                    if (result != null)
                    {
                        if (!result.ExpiryFlag)
                        {
                            return result.access_token;
                        }
                        else
                        {
                            return await RegenerateToken(result, this._endPointBaseUrl, this._APIKey);
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }                    
                }
                return string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return string.Empty;
                
            }
        }

        private async Task<string> RegenerateToken(AccessTokenDetails model, string? endPointBaseUrl, string? APIKey)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{endPointBaseUrl}/api/site24x7/generateaccesstoken?clientId={model.client_id}&clientSecret={model.client_secret}&refreshToken={model.refresh_token}");
                request.Headers.Add("API_Key", APIKey);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string ret = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(ret))
                {
                    var result = JsonConvert.DeserializeObject<AccessTokenDetails>(ret);
                    if (result != null)
                    {
                        return result.access_token;

                    }
                    else
                    {
                        return string.Empty;
                    }

                }

                return string.Empty;

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return string.Empty;

            }

        }

        //internal static async Task<string> Report_Performance_Type_Server(string endPointBaseUrl, string APIKey, string token, string zaaid, int period, int metric_aggregation,string start_date,string end_date, bool saveToDB = false)
        //{
        //    try
        //    {               
        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, $"{endPointBaseUrl}/api/site24x7/reports/performance/type/SERVER?token={token}&zaaid={zaaid}&period={period}&metric_aggregation={metric_aggregation}&start_date={start_date}&end_date={end_date}&saveToDB={saveToDB}");
        //        request.Headers.Add("API_Key", APIKey);
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadAsStringAsync();                

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;

        //    }

        //}


        internal async Task<string> ProcessDataSyncSite24x7(DateTime fromDate,DateTime toDate)
        {
            string token = await this.GetAccessToken();

            if (!string.IsNullOrEmpty(token))
            {
                // DateTime dtToday1 = DateTime.Now.AddMonths(-1).AddDays(-18).AddDays(2);
                // DateTime dtendDate = DateTime.Now.AddMonths(-1).AddDays(-18).AddDays(2);

                var customers = await this.GetMSPCustomerFromDB();

                while (fromDate <= toDate)
                {

                    string year = fromDate.Year.ToString();
                    string month = fromDate.Month <= 9 ? "0" + fromDate.Month.ToString() : fromDate.Month.ToString();
                    string day = fromDate.Day <= 9 ? "0" + fromDate.Day.ToString() : fromDate.Day.ToString();

                    string start_date = $"{year}-{month}-{day}T00:00:00%2B0530";// + WebUtility.UrlEncode($"%2B0530");
                    string end_date = $"{year}-{month}-{day}T23:59:59%2B0530";// + WebUtility.UrlEncode($"%2B0530");


                    if (!string.IsNullOrEmpty(token))
                    {

                        if (customers != null && customers.Any())
                        {
                            foreach (var customer in customers)
                            {

                                //Average
                                await this.Report_Performance_Type_Server(token, customer.zaaid, 50, 0, start_date, end_date, true);

                                //Maximum
                                await this.Report_Performance_Type_Server(token, customer.zaaid, 50, 4, start_date, end_date, true);

                                //Minimum
                                await this.Report_Performance_Type_Server(token, customer.zaaid, 50, 5, start_date, end_date, true);


                                Task.Delay(2000).Wait();

                            }
                        }
                    }

                    fromDate = fromDate.AddDays(1);
                }
            }
            return token;
        }

        public async Task<PerformanceReportModel?> Report_Performance_Type_Server(string token, string zaaid, int period, int metric_aggregation, string start_date, string end_date, bool saveToDB = false)
        {
            PerformanceReportModel? performanceReportModel = null;
            try
            {
                var client = new HttpClient();
                var url = $"{this._domainUrl}/api/reports/performance/type/SERVER?period={period}&metric_aggregation={metric_aggregation}";
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    url = url + $"&start_date={start_date}&end_date={end_date}";
                }
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Zoho-oauthtoken {token}");
                request.Headers.Add("Accept", "application/json; version=2.0");
                request.Headers.Add("Cookie", $"zaaid={zaaid}");

                ExceptionLogging.WriteMessageToText($"Site24x7 : Fetching Report_Performance_Type_Server data  : {url}");

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ExceptionLogging.WriteMessageToText($"Site24x7 : Error at Report_Performance_Type_Server() for zaaid={zaaid} : {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                // string jsonString = "{\"code\":0,\"message\":\"success\",\"data\":{\"group_data\":{\"SERVER\":{\"name\":[\"Amber-Az-ADC.ambergroupindia.com\",\"BYOD-Server\",\"spaceparts-server.internal.cloudapp.net\",\"TCAPPServer\",\"TCDBServer\"],\"availability\":[\"100\",\"100\",\"-\",\"100\",\"100\"],\"attribute_data\":[{\"0\":{\"DISKUSEDPERCENT\":29.08,\"MEMUSEDPERCENT\":57.64,\"CPUUSEDPERCENT\":7.11}},{\"0\":{\"DISKUSEDPERCENT\":35.13,\"MEMUSEDPERCENT\":91.82,\"CPUUSEDPERCENT\":15.35}},{\"0\":{\"DISKUSEDPERCENT\":\"-\",\"MEMUSEDPERCENT\":\"-\",\"CPUUSEDPERCENT\":\"-\"}},{\"0\":{\"DISKUSEDPERCENT\":49.76,\"MEMUSEDPERCENT\":20.88,\"CPUUSEDPERCENT\":29.28}},{\"0\":{\"DISKUSEDPERCENT\":20.88,\"MEMUSEDPERCENT\":88.26,\"CPUUSEDPERCENT\":2.36}}],\"tags\":[[],[],[],[],[]]}},\"info\":{\"period\":50,\"resource_type_name\":\"Monitor Type\",\"resource_type\":4,\"end_time\":\"2024-11-01T23:59:59+0530\",\"period_name\":\"Custom Period\",\"formatted_start_time\":\"1 November 2024, 12:00 AM IST\",\"metric_aggregation_name\":\"Average\",\"report_type\":16,\"formatted_generated_time\":\"18 November 2024, 8:33 PM IST\",\"formatted_end_time\":\"1 November 2024, 11:59 PM IST\",\"generated_time\":\"2024-11-18T20:33:12+0530\",\"start_time\":\"2024-11-01T00:00:00+0530\",\"metric_aggregation\":0,\"resource_name\":\"Server Monitor\",\"report_name\":\"Performance Report\",\"monitor_type\":\"SERVER\"}}}";


                if (!string.IsNullOrEmpty(jsonString))
                {
                    performanceReportModel = JsonConvert.DeserializeObject<PerformanceReportModel>(jsonString);

                    if (saveToDB)
                    {
                        ExceptionLogging.WriteMessageToText($"Site24x7 : Saving Report_Performance_Type_Server data to DB : {url}");
                        await Report_Performance_Type_Server_Save(jsonString, zaaid, period, metric_aggregation, start_date, end_date);
                        
                    }
                }
                
            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                
                //return this.Problem(ex.Message);
            }
            return performanceReportModel;
        }

        private async Task<ReturnMessageModel> Report_Performance_Type_Server_Save(string jsonResult, string zaaid, int period, int metric_aggregation, string start_date, string end_date)
        {
            try
            {
                return await _site24x7Data.Per_Report_Server_InsertUpdate(jsonResult, zaaid, period, metric_aggregation, start_date, end_date);
            }
            catch
            {
                throw;
            }
        }
        internal async Task<IEnumerable<MSP_Customer>> GetMSPCustomerFromDB()
        {
            try
            {
                ExceptionLogging.WriteMessageToText($"Site24x7 : calling GetMSPCustomerFromDB");
                return await this._site24x7Data.GetMSP_Customer();               
            }
            catch 
            {
                throw;
            }

        }

        internal async Task<List<Rpt_PerformaceReportModel>?> Get_R_ServerPerformanceReport(string endPointBaseUrl, string APIKey, string zaaid, string start_date, string end_date)
        {
            List<Rpt_PerformaceReportModel>? result=null;
            try
            {
                result = new List<Rpt_PerformaceReportModel>();

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{endPointBaseUrl}/api/site24x7/report-performance-utilization?zaaid={zaaid}&start_date={start_date}&end_date={end_date}");
                request.Headers.Add("API_Key", APIKey);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseJson))
                {
                    result = JsonConvert.DeserializeObject<List<Rpt_PerformaceReportModel>>(responseJson);
                    
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            return result;

        }

        //SOUMIK REV START
        internal async Task<string> ProcessDataSyncMonthlySite24x7(string fromDate, string toDate)
        {
            string token = await this.GetAccessToken();

            if (!string.IsNullOrEmpty(token))
            {

                var customers = await this.GetMSPCustomerFromDB();


                    if (!string.IsNullOrEmpty(token))
                    {

                        if (customers != null && customers.Any())
                        {
                            foreach (var customer in customers)
                            {

                                //Average
                                await this.Report_Performance_Type_Server_Monthly(token, customer.zaaid, 50, 0, fromDate, toDate, true);

                                //Maximum
                                await this.Report_Performance_Type_Server_Monthly(token, customer.zaaid, 50, 4, fromDate, toDate, true);

                                //Minimum
                                await this.Report_Performance_Type_Server_Monthly(token, customer.zaaid, 50, 5, fromDate, toDate, true);


                                Task.Delay(2000).Wait();

                            }
                        }
                    }

                    //fromDate = fromDate.AddDays(1);
                //}
            }
            return token;
        }
        public async Task<PerformanceReportModel?> Report_Performance_Type_Server_Monthly(string token, string zaaid, int period, int metric_aggregation, string start_date, string end_date, bool saveToDB = false)
        {
            PerformanceReportModel? performanceReportModel = null;
            try
            {
                var client = new HttpClient();
                var url = $"{this._domainUrl}/api/reports/performance/type/SERVER?period={period}&metric_aggregation={metric_aggregation}";
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    url = url + $"&start_date={start_date}&end_date={end_date}";
                }
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Zoho-oauthtoken {token}");
                request.Headers.Add("Accept", "application/json; version=2.0");
                request.Headers.Add("Cookie", $"zaaid={zaaid}");

                ExceptionLogging.WriteMessageToText($"Site24x7 : Fetching Report_Performance_Type_Server data  : {url}");

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ExceptionLogging.WriteMessageToText($"Site24x7 : Error at Report_Performance_Type_Server() for zaaid={zaaid} : {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                // string jsonString = "{\"code\":0,\"message\":\"success\",\"data\":{\"group_data\":{\"SERVER\":{\"name\":[\"Amber-Az-ADC.ambergroupindia.com\",\"BYOD-Server\",\"spaceparts-server.internal.cloudapp.net\",\"TCAPPServer\",\"TCDBServer\"],\"availability\":[\"100\",\"100\",\"-\",\"100\",\"100\"],\"attribute_data\":[{\"0\":{\"DISKUSEDPERCENT\":29.08,\"MEMUSEDPERCENT\":57.64,\"CPUUSEDPERCENT\":7.11}},{\"0\":{\"DISKUSEDPERCENT\":35.13,\"MEMUSEDPERCENT\":91.82,\"CPUUSEDPERCENT\":15.35}},{\"0\":{\"DISKUSEDPERCENT\":\"-\",\"MEMUSEDPERCENT\":\"-\",\"CPUUSEDPERCENT\":\"-\"}},{\"0\":{\"DISKUSEDPERCENT\":49.76,\"MEMUSEDPERCENT\":20.88,\"CPUUSEDPERCENT\":29.28}},{\"0\":{\"DISKUSEDPERCENT\":20.88,\"MEMUSEDPERCENT\":88.26,\"CPUUSEDPERCENT\":2.36}}],\"tags\":[[],[],[],[],[]]}},\"info\":{\"period\":50,\"resource_type_name\":\"Monitor Type\",\"resource_type\":4,\"end_time\":\"2024-11-01T23:59:59+0530\",\"period_name\":\"Custom Period\",\"formatted_start_time\":\"1 November 2024, 12:00 AM IST\",\"metric_aggregation_name\":\"Average\",\"report_type\":16,\"formatted_generated_time\":\"18 November 2024, 8:33 PM IST\",\"formatted_end_time\":\"1 November 2024, 11:59 PM IST\",\"generated_time\":\"2024-11-18T20:33:12+0530\",\"start_time\":\"2024-11-01T00:00:00+0530\",\"metric_aggregation\":0,\"resource_name\":\"Server Monitor\",\"report_name\":\"Performance Report\",\"monitor_type\":\"SERVER\"}}}";


                if (!string.IsNullOrEmpty(jsonString))
                {
                    performanceReportModel = JsonConvert.DeserializeObject<PerformanceReportModel>(jsonString);

                    if (saveToDB)
                    {
                        ExceptionLogging.WriteMessageToText($"Site24x7 : Saving Report_Performance_Type_Server data to DB : {url}");
                        await Report_Performance_Type_Server_Monthly_Save(jsonString, zaaid, period, metric_aggregation, start_date, end_date);

                    }
                }

            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

                //return this.Problem(ex.Message);
            }
            return performanceReportModel;
        }
        private async Task<ReturnMessageModel> Report_Performance_Type_Server_Monthly_Save(string jsonResult, string zaaid, int period, int metric_aggregation, string start_date, string end_date)
        {
            try
            {
                return await _site24x7Data.Per_Report_Server_Monthly_InsertUpdate(jsonResult, zaaid, period, metric_aggregation, start_date, end_date);
            }
            catch
            {
                throw;
            }
        }
        //SOUMIK REV END
    }
}
