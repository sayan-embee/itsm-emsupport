using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Common.Layer.Models.DBModel;
using DataAccess.Layer.Data;
using WebAPI.Layer.Authorization;
using Common.Layer.Models.AppSettings;
using Microsoft.Extensions.Options;
using Common.Layer.Models;
using Newtonsoft.Json.Linq;
using System.Text;
using WebAPI.Layer.ExceptionLog;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using Common.Layer.Models.Site24x7;
using Common.Layer.Models.FreshService;
using DataAccess.Layer.DbAccess;
using DataAccess.Layer.Data.Site24x7;

namespace Site24x7API.Controllers
{
    [Route("api/site24x7")]
    [ApiController]
    [TypeFilter(typeof(APIKeyAuthorization))]
    public class Site24x7APIController : ControllerBase
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IConfiguration _configuration;
        private readonly ISite24x7Data _site24x7Data;

        public Site24x7APIController(
            IOptions<AppSettingsModel> appSettings
             , IConfiguration configuration
            , ISite24x7Data site24x7Data
            )

        {

            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            this._appSettings = appSettings.Value;// ?? throw new ArgumentNullException(nameof(AppSettingsModel));
            this._site24x7Data = site24x7Data ?? throw new ArgumentNullException(nameof(ISite24x7Data));
        }


        [HttpPost]
        [Route("generateaccesstoken")]
        public async Task<IActionResult> GenerateAccessTokenFromRefreshToken(string clientId,string clientSecret,string refreshToken)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{this._appSettings.Site24x7.AccessTokenDomainUrl}/oauth/v2/token?client_id={clientId}&client_secret={clientSecret}&refresh_token={refreshToken}&grant_type=refresh_token");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
               
                string ret = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(ret))
                {
                    var responseContent = JsonConvert.DeserializeObject<AccessTokenDetails>(ret);
                    if(responseContent!= null)
                    {
                        responseContent.client_secret = clientSecret;
                        responseContent.client_id = clientId;
                        responseContent.refresh_token= refreshToken;
                        responseContent.ExpiresStarts= DateTime.UtcNow;
                        responseContent.ExpiresOn= DateTime.UtcNow.AddSeconds(responseContent.expires_in-60);
                        try
                        {
                            await this._site24x7Data.Update(responseContent);
                        }
                        catch
                        {

                        }
                    }
                    return this.Ok(responseContent);
                }
                else
                {
                    return this.NoContent();
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)ex.StatusCode, new
                {
                    message = ex.Message,
                    responseCode = ex.StatusCode,
                    details = ex?.InnerException?.Message?.ToString()
                });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = ex.Message,
                    responseCode = HttpStatusCode.InternalServerError,
                    details = ex?.InnerException?.Message.ToString()
                });
                //return this.Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("accesstoken")]
        public async Task<IActionResult> GetAccessToken(string clientId)
        {
            try
            {
                AccessTokenDetails model = new AccessTokenDetails { client_id = clientId };
                var response=await _site24x7Data.Get(model);
                return this.Ok(response);

            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)ex.StatusCode, new
                {
                    message = ex.Message,
                    responseCode = ex.StatusCode,
                    details = ex?.InnerException?.Message?.ToString()
                });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = ex.Message,
                    responseCode = HttpStatusCode.InternalServerError,
                    details = ex?.InnerException?.Message.ToString()
                });
                //return this.Problem(ex.Message);
            }
        }


        [HttpGet]
        [Route("reports/performance/type/SERVER")]
        public async Task<IActionResult> Report_Performance_Type_Server(string token,string zaaid,int period,int metric_aggregation,string start_date,string end_date, bool saveToDB=false)
        {
            try
            {
                var client = new HttpClient();
                var url = $"{this._appSettings.Site24x7.ApiRootUrl}/api/reports/performance/type/SERVER?period={period}&metric_aggregation={metric_aggregation}";
                if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    url = url + $"&start_date={start_date}&end_date={end_date}";
                }
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Zoho-oauthtoken {token}");
                request.Headers.Add("Accept", "application/json; version=2.0");
                request.Headers.Add("Cookie", $"zaaid={zaaid}");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                // string jsonString = "{\"code\":0,\"message\":\"success\",\"data\":{\"group_data\":{\"SERVER\":{\"name\":[\"Amber-Az-ADC.ambergroupindia.com\",\"BYOD-Server\",\"spaceparts-server.internal.cloudapp.net\",\"TCAPPServer\",\"TCDBServer\"],\"availability\":[\"100\",\"100\",\"-\",\"100\",\"100\"],\"attribute_data\":[{\"0\":{\"DISKUSEDPERCENT\":29.08,\"MEMUSEDPERCENT\":57.64,\"CPUUSEDPERCENT\":7.11}},{\"0\":{\"DISKUSEDPERCENT\":35.13,\"MEMUSEDPERCENT\":91.82,\"CPUUSEDPERCENT\":15.35}},{\"0\":{\"DISKUSEDPERCENT\":\"-\",\"MEMUSEDPERCENT\":\"-\",\"CPUUSEDPERCENT\":\"-\"}},{\"0\":{\"DISKUSEDPERCENT\":49.76,\"MEMUSEDPERCENT\":20.88,\"CPUUSEDPERCENT\":29.28}},{\"0\":{\"DISKUSEDPERCENT\":20.88,\"MEMUSEDPERCENT\":88.26,\"CPUUSEDPERCENT\":2.36}}],\"tags\":[[],[],[],[],[]]}},\"info\":{\"period\":50,\"resource_type_name\":\"Monitor Type\",\"resource_type\":4,\"end_time\":\"2024-11-01T23:59:59+0530\",\"period_name\":\"Custom Period\",\"formatted_start_time\":\"1 November 2024, 12:00 AM IST\",\"metric_aggregation_name\":\"Average\",\"report_type\":16,\"formatted_generated_time\":\"18 November 2024, 8:33 PM IST\",\"formatted_end_time\":\"1 November 2024, 11:59 PM IST\",\"generated_time\":\"2024-11-18T20:33:12+0530\",\"start_time\":\"2024-11-01T00:00:00+0530\",\"metric_aggregation\":0,\"resource_name\":\"Server Monitor\",\"report_name\":\"Performance Report\",\"monitor_type\":\"SERVER\"}}}";


                if (!string.IsNullOrEmpty(jsonString))
                {
                    var responseContent = JsonConvert.DeserializeObject<PerformanceReportModel>(jsonString);

                    if (saveToDB)
                    {
                       // var dbInputResponse=JsonConvert.SerializeObject(responseContent);
                        var dbResult= await Report_Performance_Type_Server_Save(jsonString, zaaid, period, metric_aggregation, start_date, end_date);
                        if(dbResult != null && dbResult.Status==1) {
                            return this.Ok(responseContent);
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, new
                            {
                                message = dbResult?.ErrorMessage,
                                responseCode = HttpStatusCode.InternalServerError,
                                details = dbResult?.ErrorMessage
                            });
                        }
                    }
                    else
                    {                        
                        return this.Ok(responseContent);
                    }
                }
                else
                {
                    return this.NoContent();
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)ex.StatusCode, new
                {
                    message = ex.Message,
                    responseCode = ex.StatusCode,
                    details = ex?.InnerException?.Message?.ToString()
                });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = ex.Message,
                    responseCode = HttpStatusCode.InternalServerError,
                    details = ex?.InnerException?.Message.ToString()
                });
                //return this.Problem(ex.Message);
            }
        }

        private async Task<ReturnMessageModel> Report_Performance_Type_Server_Save(string jsonResult, string zaaid, int period, int metric_aggregation, string start_date, string end_date)
        {
            try
            {
                return await _site24x7Data.Per_Report_Server_InsertUpdate(jsonResult, zaaid,period,metric_aggregation,start_date,end_date);
                           }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw;
            }
        }

      

        [HttpGet]
        [Route("msp_customer")]
        public async Task<IActionResult> GetMSP_Customer()
        {
            try
            {
                var response = await _site24x7Data.GetMSP_Customer();
                return this.Ok(response);

            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)ex.StatusCode, new
                {
                    message = ex.Message,
                    responseCode = ex.StatusCode,
                    details = ex?.InnerException?.Message?.ToString()
                });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = ex.Message,
                    responseCode = HttpStatusCode.InternalServerError,
                    details = ex?.InnerException?.Message.ToString()
                });
                //return this.Problem(ex.Message);
            }
        }

        [HttpGet]
        [Route("report-performance-utilization")]
        public async Task<IActionResult> Get_R_ServerPerformanceReport(string zaaid, string start_date, string end_date)
        {
            try
            {
                var response = await _site24x7Data.Get_R_ServerPerformanceReport(zaaid,start_date,end_date, null);
                return this.Ok(response);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode((int)ex.StatusCode, new
                {
                    message = ex.Message,
                    responseCode = ex.StatusCode,
                    details = ex?.InnerException?.Message?.ToString()
                });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = ex.Message,
                    responseCode = HttpStatusCode.InternalServerError,
                    details = ex?.InnerException?.Message.ToString()
                });
                //return this.Problem(ex.Message);
            }
        }
    }
}
