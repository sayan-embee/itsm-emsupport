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
using Common.Layer.Models.FreshService;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using DataAccess.Layer.Data.Site24x7;
using DataAccess.Layer.Data.FreshService;

namespace FreshServiceAPI.Controllers
{
    [Route("api/freshservice")]
    [ApiController]
    [TypeFilter(typeof(APIKeyAuthorization))]
    public class FreshServiceAPIController : ControllerBase
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IConfiguration _configuration;
        private readonly IFreshServiceData _freshServiceData;
        public FreshServiceAPIController(
            IOptions<AppSettingsModel> appSettings
             ,IConfiguration configuration
             , IFreshServiceData freshServiceData
            )
           
        {
           
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            this._appSettings = appSettings.Value;// ?? throw new ArgumentNullException(nameof(AppSettingsModel));
            this._freshServiceData = freshServiceData ?? throw new ArgumentNullException(nameof(IFreshServiceData));
        }

        #region Common
        private string ConvertToBase64String(string key)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
        }

        private async Task<HttpResponseMessage> APIGetRequest(string url)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{url}");
                request.Headers.Add("Authorization", "Basic " + ConvertToBase64String(_appSettings.FreshService.ApiKey));

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;
                //if (!response.IsSuccessStatusCode)
                //{
                //    return response;
                //    //string errorContent = await response.Content.ReadAsStringAsync();
                //    //var jsonObject = JObject.Parse(errorContent.Trim());

                //    //return new HttpResponseMessage()
                //    //{
                //    //    StatusCode = response.StatusCode                     
                //    //    ,
                //    //};
                //    ////errResponse.StatusCode = response.StatusCode;
                //    ////errResponse
                //    ////return StatusCode((int)response.StatusCode, new
                //    ////{
                //    ////    message = "HTTP error occurred",
                //    ////    responseCode = (int)response.StatusCode,
                //    ////    details = jsonObject["errorMessage"]?.ToString()
                //    ////});
                //}
                //else
                //{
                //    return response;
                //}

            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw;
                //return StatusCode(500, new
                //{
                //    message = "Exception occurred",
                //    responseCode = 500,
                //    details = ex?.Message.ToString()
                //});
            }

        }
        #endregion
        #region Department
        /// <summary>
        /// This methods returns all the list of departments from fresh service.
        /// </summary>
        /// <param name="query">query parameters.</param>
        /// <param name="isPaging">Allow paging true or false.</param>
        /// <param name="saveToDB">Save the record to DB or not.</param>

        /// <returns>The list of departments.</returns>
        [HttpGet]
        [Route("departments")]
        public async Task<IActionResult> Departments(string? query,bool isPaging=false,bool saveToDB=false)
        {
            try
            {
                Departments depts = new Departments();
                depts.departments = new List<Department>();

                string? link = $"{_appSettings.FreshService.DomainUrl}/departments?{query}";
                do
                {
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {
                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            Departments? tokenResponse = JsonConvert.DeserializeObject<Departments>(ret);
                            if (tokenResponse != null)
                            {
                                depts.departments = depts.departments.Concat(tokenResponse.departments);
                            }

                            if (saveToDB)
                            {
                                await _freshServiceData.Departments_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;                    

                } while (!string.IsNullOrEmpty(link));

                return this.Ok(depts);
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

        /// <summary>
        /// This method is used to call the fresh service api for getting the ticket details based on updated since date
        /// </summary>
        /// <param name="updated_since">date format in YYYY-MM-DDThh:mm:ssZ e.g. 2024-11-13T02:00:00Z</param>
        /// <param name="isPaging">Allow paging true or false.</param>
        /// <param name="saveToDB">Save the record to DB or not.</param>
        /// <returns>Return the list of tickets from Fresh Service</returns>
        [HttpGet]
        [Route("tickets")]        
        public async Task<IActionResult> Tickets(string updated_since, bool isPaging = false,int pageRowIndex=1,int pageSize=100, bool saveToDB = false)
        {
            try
            {
                Root_Ticket tkts = new Root_Ticket();
                tkts.tickets = new List<Ticket>();

                string? link = $"{_appSettings.FreshService.DomainUrl}/tickets?updated_since={updated_since}&page={pageRowIndex}&per_page={pageSize}&include=stats,requester,requested_for,department";
                do
                {
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {
                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            Root_Ticket? jsonResult = JsonConvert.DeserializeObject<Root_Ticket>(ret);
                            if (jsonResult != null)
                            {
                                tkts.tickets = tkts.tickets.Concat(jsonResult.tickets);
                            }

                            if (saveToDB)
                            {
                                await _freshServiceData.Tickets_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;

                } while (!string.IsNullOrEmpty(link));

                return this.Ok(tkts);
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
        [Route("tickets/filter")]
        public async Task<IActionResult> TicketsFiltered(string start_date,string endDate, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            try
            {
                Root_Ticket tkts = new Root_Ticket();
                tkts.tickets = new List<Ticket>();

                string? link = $"{_appSettings.FreshService.DomainUrl}/tickets/filter?query=\"created_at:>'{start_date}' AND created_at:<'{endDate}'\"&page={pageRowIndex}&per_page={pageSize}";
                do
                {
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {
                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            Root_Ticket? jsonResult = JsonConvert.DeserializeObject<Root_Ticket>(ret);
                            if (jsonResult != null)
                            {
                                tkts.tickets = tkts.tickets.Concat(jsonResult.tickets);
                            }

                            if (saveToDB)
                            {
                                await _freshServiceData.Tickets_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;

                } while (!string.IsNullOrEmpty(link));

                return this.Ok(tkts);
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
        [Route("requesters")]
        public async Task<IActionResult> Requesters(string start_date, string end_Date, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            try
            {
                RequesterModel reqModel = new RequesterModel();
                reqModel.requesters = new List<Requester>();

                string? link = $"{_appSettings.FreshService.DomainUrl}/requesters?order_type=asc&query=\"created_at:>'{start_date}' AND created_at:<'{end_Date}'\"&page={pageRowIndex}&per_page={pageSize}";
                do
                {
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {
                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            RequesterModel? jsonResult = JsonConvert.DeserializeObject<RequesterModel>(ret);
                            if (jsonResult != null)
                            {
                                reqModel.requesters = reqModel.requesters.Concat(jsonResult.requesters);
                            }

                            if (saveToDB)
                            {
                                await _freshServiceData.Requester_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;

                } while (!string.IsNullOrEmpty(link));

                return this.Ok(reqModel);
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

        private static string? GetResponseHeader_LinkParam(HttpResponseMessage response)
        {
            string? link;
            IEnumerable<string>? outLink;
            response.Headers.TryGetValues("link", out outLink);

            if (outLink != null && outLink.Any())
            {
                link = outLink.FirstOrDefault();
                if (!string.IsNullOrEmpty(link))
                {
                    link = link.Split(";")[0];
                    link = link.Replace("<", "").Replace(">", "").Trim();
                }
            }
            else
            {
                link = null;
            }

            return link;
        }
        #endregion

        #region From DB
        [HttpGet]
        [Route("departments_db")]
        public async Task<IActionResult> Departments_DB(string name, long? id=null, int? pageRowIndex=1, int? pageSize=100)
        {
            try
            {
                try
                {
                    var response = await _freshServiceData.Get_Department(name,id,pageRowIndex,pageSize);
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
        #endregion

    }
}