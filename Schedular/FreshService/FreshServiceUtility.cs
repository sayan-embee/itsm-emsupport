using Common.Layer.Models.FreshService;
using Common.Layer.Models.Report;
using DataAccess.Layer.Data.FreshService;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Schedular.ExceptionLog;
//using System;
//using System.Collections;
using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;
//using System.Text;
//using System.Text.Json;

namespace Schedular.FreshService
{
    internal class FreshServiceUtility
    {
        private readonly IConfiguration _configuration;
        private readonly IFreshServiceData _freshServiceData;
        private readonly string? _domainUrl;
        private readonly string? _freshServiceApiKey;

        private static readonly HttpClient _httpClient = CreateHttpClient();

        public FreshServiceUtility(
             IConfiguration configuration
             , IFreshServiceData freshServiceData
            //, HttpClient httpClient
            )

        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            this._freshServiceData = freshServiceData ?? throw new ArgumentNullException(nameof(IFreshServiceData));

            this._domainUrl = Convert.ToString(this._configuration["AppConfig:FreshService:domainUrl"]);
            this._freshServiceApiKey = Convert.ToString(this._configuration["AppConfig:FreshService:apikey"]);

            //_http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        private  string ConvertToBase64String(string? key)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        private  async Task<HttpResponseMessage> APIGetRequest(string url)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{url}");
                request.Headers.Add("Authorization", "Basic " + ConvertToBase64String(this._freshServiceApiKey));

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return response;

            }
            catch 
            {
                //ExceptionLogging.SendErrorToText(ex);
                throw;
            }

        }
        private string? GetResponseHeader_LinkParam(HttpResponseMessage response)
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
        internal async Task<Root_Ticket> Tickets(string updated_since, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            Root_Ticket tkts = new Root_Ticket();
            try
            {

                tkts.tickets = new List<Ticket>();
                string? link = $"{this._domainUrl}/tickets?order_type=asc&updated_since={updated_since}&page={pageRowIndex}&per_page={pageSize}&include=stats,requester,requested_for,department,tags";
                do
                {
                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching ticket data  : {link}");
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

                            if (saveToDB && jsonResult != null && jsonResult.tickets.Any())
                            {
                                ExceptionLogging.WriteMessageToText($"FreshService : Saving ticket data to DB : {link}");
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


            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                
            }
            return tkts;
        }

        internal async Task<ChangeModel> Changes(string updated_since, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            ChangeModel retModel = new ChangeModel();
            try
            {

                retModel.changes = new List<Change>();
                string? link = $"{this._domainUrl}/changes?order_type=asc&updated_since={updated_since}&page={pageRowIndex}&per_page={pageSize}";
                do
                {
                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching changes data  : {link}");
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {

                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            ChangeModel? jsonResult = JsonConvert.DeserializeObject<ChangeModel>(ret);
                            if (jsonResult != null)
                            {
                                retModel.changes = retModel.changes.Concat(jsonResult.changes);
                            }

                            if (saveToDB && jsonResult != null && jsonResult.changes.Any())
                            {
                                ExceptionLogging.WriteMessageToText($"FreshService : Saving changes data to DB : {link}");
                                await _freshServiceData.Changes_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;

                } while (!string.IsNullOrEmpty(link));


            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

            }
            return retModel;
        }

        internal async Task<RequesterModel> Requesters(string start_date,string end_Date, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            RequesterModel reqModel = new RequesterModel();
            try
            {

                reqModel.requesters = new List<Requester>();
                string? link = $"{this._domainUrl}/requesters?order_type=asc&page={pageRowIndex}&per_page={pageSize}";
                if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_Date))
                {
                    link = link+ $"&query=\"created_at:>'{start_date}' AND created_at:<'{end_Date}'\"";
                }
                do
                {
                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching requester data  : {link}");
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

                            if (saveToDB && jsonResult !=null && jsonResult.requesters.Any())
                            {
                                ExceptionLogging.WriteMessageToText($"FreshService : Saving requester data to DB : {link}");
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


            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

            }
            return reqModel;
        }
        internal async Task<Departments> Departments(string? query, bool isPaging = false, bool saveToDB = false)
        {
            Departments depts = new Departments();
            try
            {
                
                depts.departments = new List<Department>();

                string? link = $"{this._domainUrl}/departments?{query}";
                do
                {
                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching department data  : {link}");
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {
                        
                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            Departments? jsonResult = JsonConvert.DeserializeObject<Departments>(ret);
                            if (jsonResult != null)
                            {
                                depts.departments = depts.departments.Concat(jsonResult.departments);
                            }

                            if (saveToDB && jsonResult != null && jsonResult.departments.Any())
                            {
                                ExceptionLogging.WriteMessageToText($"FreshService : Saving department data to DB : {link}");
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

               
            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                
            }
            return depts;
        }


        internal async Task<ProblemModel> Problem(string updated_since, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            ProblemModel retModel = new ProblemModel();
            try
            {

                retModel.problems = new List<Problem>();
                string? link = $"{this._domainUrl}/problems?order_type=asc&updated_since={updated_since}&page={pageRowIndex}&per_page={pageSize}";
                do
                {
                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching problem data  : {link}");
                    var response = await APIGetRequest(link);
                    if (response.IsSuccessStatusCode)
                    {

                        string ret = await response.Content.ReadAsStringAsync();
                        link = GetResponseHeader_LinkParam(response);
                        if (!string.IsNullOrEmpty(ret))
                        {
                            ProblemModel? jsonResult = JsonConvert.DeserializeObject<ProblemModel>(ret);
                            if (jsonResult != null)
                            {
                                retModel.problems = retModel.problems.Concat(jsonResult.problems);
                            }

                            if (saveToDB && jsonResult != null && jsonResult.problems.Any())
                            {
                                ExceptionLogging.WriteMessageToText($"FreshService : Saving problem data to DB : {link}");
                                await _freshServiceData.Problem_InsertUpdate(ret);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (!isPaging) break;

                } while (!string.IsNullOrEmpty(link));


            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);

            }
            return retModel;
        }

        //soumik rev start 11-03-2025
    

        internal async Task<Root_Ticket> TicketsByCreatedDate(string fromDateUtc, string toDateUtc, bool isPaging = false, int pageRowIndex = 1, int pageSize = 100, bool saveToDB = false)
        {
            Root_Ticket tkts = new Root_Ticket { tickets = new List<Ticket>() };
            int totalTickets = 0;
            int totalPages = 1;
            int tktcount = 0;

            try
            {
                int maxRetries = 5;
                int retryCount = 0;
                int maxRequestsPerMinute = 120;
                int delayBetweenRequestsMs = 60000 / maxRequestsPerMinute; // 500ms for Pro Plan
                int baseDelayMs = 1000;

                string query = Uri.EscapeDataString($"created_at:>'{fromDateUtc}' AND created_at:<'{toDateUtc}'");

                do
                {
                    string link = $"{this._domainUrl}/tickets/filter?query=\"{query}\"&page={pageRowIndex}&per_page={pageSize}";

                    ExceptionLogging.WriteMessageToText($"FreshService : Fetching Tickets By Created Date data  : {link}");
                    var response = await APIGetRequest(link);



                    #region Throttle Handling
                    
                    
                    if ((int)response.StatusCode == 429)
                    {
                        if (retryCount >= maxRetries)
                        {
                            ExceptionLogging.WriteMessageToText("Exceeded max retry attempts due to rate limiting.");
                            break;
                        }

                        retryCount++;

                        string retryAfterHeader = response.Headers.TryGetValues("Retry-After", out var values) ? values.FirstOrDefault() : null;
                        int retryDelaySeconds = 5;

                        if (int.TryParse(retryAfterHeader, out int headerDelay))
                        {
                            retryDelaySeconds = headerDelay;
                        }
                        else
                        {
                            // Fallback to exponential backoff
                            retryDelaySeconds = Math.Min((int)Math.Pow(2, retryCount), 30);
                        }

                        ExceptionLogging.WriteMessageToText($"Rate limited. Retrying after {retryDelaySeconds} seconds (retry {retryCount}/{maxRetries}).");
                        await Task.Delay(retryDelaySeconds * baseDelayMs);

                        continue;
                    }

                    retryCount = 0;

                    await Task.Delay(delayBetweenRequestsMs);

                    
                    #endregion


                    if (!response.IsSuccessStatusCode)
                    {
                        ExceptionLogging.WriteMessageToText($"FreshService : API request failed with status code: {response.StatusCode}");
                        break;
                    }

                    string ret = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(ret))
                    {
                        ExceptionLogging.WriteMessageToText("FreshService : API returned empty response.");
                        break;
                    }

                    Root_Ticket? jsonResult = JsonConvert.DeserializeObject<Root_Ticket>(ret);
                    if (jsonResult?.tickets == null || !jsonResult.tickets.Any())
                    {
                        ExceptionLogging.WriteMessageToText("FreshService : No tickets found in response.");
                        break;
                    }

                    var ticketList = tkts.tickets.ToList();
                    ticketList.AddRange(jsonResult.tickets);
                    tkts.tickets = ticketList;
                    
                    tktcount += jsonResult.tickets.Count();

                    if (totalTickets == 0)
                    {
                        totalTickets = jsonResult.total;
                        totalPages = (int)Math.Ceiling((double)totalTickets / pageSize);
                    }

                    if (saveToDB)
                    {
                        ExceptionLogging.WriteMessageToText($"FreshService : Saving ticket By Created Date Data to DB for page {pageRowIndex}.");
                        await _freshServiceData.TicketsByCreatedDate_InsertUpdate(ret);
                        //await Task.Delay(1000);
                    }

                    //Console.WriteLine(tktcount);
                    //Console.WriteLine(pageRowIndex);

                    if (!isPaging || pageRowIndex >= totalPages) break;

                    pageRowIndex++;

                } while (true);
            }
            catch (HttpRequestException ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
            }

            return tkts;
        }


        //soumik rev end 11-03-2025

        //SOUMIK TICKET VIEW INSERT
        // Public entry - updated to accept CancellationToken
        public async Task TicketsByTicketIdAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var missingTickets = await _freshServiceData.Get_Missing_Ticket();

                if (missingTickets == null || !missingTickets.Any())
                    return;

                int maxDegreeOfParallelism = 8;
                int maxRetries = 4;
                TimeSpan baseDelay = TimeSpan.FromSeconds(1);
                var bag = new ConcurrentBag<Ticket>();

                using (var semaphore = new SemaphoreSlim(maxDegreeOfParallelism))
                {
                    var tasks = missingTickets.Select(ticketVal =>
                        ProcessTicketAsync(ticketVal, semaphore, bag, maxRetries, baseDelay, cancellationToken)
                    ).ToList();

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }

                var tickets = bag.ToList();
                //ExceptionLogging.WriteMessageToText($"FreshService : Saving ticket By Created Date Data to DB for page {pageRowIndex}.");

                // batch inserts (good for memory)
                const int batchSize = 50;
                foreach (var batch in tickets.Chunk(batchSize))
                {

                    var payload = new { tickets = tickets };
                    var json = JsonConvert.SerializeObject(payload, Formatting.None);

                    await _freshServiceData.MissingStats_InsertUpdate(json);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw;
            }
        }

    private async Task ProcessTicketAsync(
    MissingTicketModel ticketVal,
    SemaphoreSlim semaphore,
    ConcurrentBag<Ticket> bag,
    int maxRetries,
    TimeSpan baseDelay,
    CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var ticket = await GetTicketWithRetriesAsync(ticketVal.TicketId, maxRetries, baseDelay, cancellationToken);
                if (ticket != null)
                    bag.Add(ticket);
            }
            catch (OperationCanceledException)
            {
                // graceful cancellation
            }
            catch (Exception ex)
            {
                ExceptionLogging.WriteMessageToText($"Error processing ticket {ticketVal.TicketId}: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }


        private async Task<Ticket?> GetTicketWithRetriesAsync(
            long? ticketId,
            int maxRetries,
            TimeSpan baseDelay,
            CancellationToken cancellationToken,
            TimeSpan? perRequestTimeout = null)
        {
            if (!ticketId.HasValue) return null;
            var rng = new Random();

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CancellationTokenSource? perReqCts = null;
                try
                {
                    var ct = cancellationToken;
                    if (perRequestTimeout.HasValue)
                    {
                        perReqCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        perReqCts.CancelAfter(perRequestTimeout.Value);
                        ct = perReqCts.Token;
                    }

                    var url = $"{_domainUrl}/tickets/{WebUtility.UrlEncode(ticketId.Value.ToString())}?include=stats";

                    using var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", ConvertToBase64String(_freshServiceApiKey));

                    using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = ParseRetryAfter(response) ?? ComputeBackoffWithJitter(baseDelay, attempt, rng);
                        if (attempt == maxRetries) break;
                        await Task.Delay(retryAfter, ct).ConfigureAwait(false);
                        continue;
                    }

                    if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
                    {
                        if (attempt == maxRetries) return null;
                        var delay = ComputeBackoffWithJitter(baseDelay, attempt, rng);
                        await Task.Delay(delay, ct).ConfigureAwait(false);
                        continue;
                    }

                    if (!response.IsSuccessStatusCode) return null;

                    var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                    try
                    {
                        var wrapper = JsonConvert.DeserializeObject<WrapperTicket>(json);
                        if (wrapper?.ticket != null) return wrapper.ticket;

                        var ticket = JsonConvert.DeserializeObject<Ticket>(json);
                        return ticket;
                    }
                    catch (JsonException ex)
                    {
                        ExceptionLogging.WriteMessageToText($"JSON parse failed for ticket {ticketId}: {ex.Message}");
                        return null;
                    }
                }
                catch (HttpRequestException) when (attempt < maxRetries)
                {
                    var delay = ComputeBackoffWithJitter(baseDelay, attempt, rng);
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested && attempt < maxRetries)
                {
                    var delay = ComputeBackoffWithJitter(baseDelay, attempt, rng);
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    perReqCts?.Dispose();
                }
            }

            return null;
        }


        private TimeSpan ComputeBackoffWithJitter(TimeSpan baseDelay, int attempt, Random rng)
        {
            var exponent = Math.Min(attempt, 10);
            var millis = baseDelay.TotalMilliseconds * Math.Pow(2, exponent);
            var jitter = rng.NextDouble() * 1000 - 500; // ±500 ms jitter
            var total = millis + jitter;
            var cap = TimeSpan.FromSeconds(60).TotalMilliseconds;
            return TimeSpan.FromMilliseconds(Math.Max(0, Math.Min(total, cap)));
        }

        private TimeSpan? ParseRetryAfter(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                var raw = values.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(raw)) return null;

                if (int.TryParse(raw, out var seconds) && seconds >= 0)
                    return TimeSpan.FromSeconds(seconds);

                if (DateTimeOffset.TryParse(raw, out var dt))
                {
                    var delta = dt - DateTimeOffset.UtcNow;
                    return delta > TimeSpan.Zero ? delta : TimeSpan.Zero;
                }
            }
            return null;
        }




    }




    // SOUMIK TICKET VIEW INSERT


}
