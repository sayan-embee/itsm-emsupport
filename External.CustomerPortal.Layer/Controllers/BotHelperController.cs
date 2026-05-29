using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.WebChatBot;
using Microsoft.Extensions.Options;
using External.CustomerPortal.Layer.Services.WebChatBot;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;
using Microsoft.AspNetCore.Authorization;

namespace External.CustomerPortal.Layer.Controllers
{
    [Route("CustomerPortal/api")]
    [ApiController]
    public class BotHelperController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly AppSettingsModel _appSettings;
        private readonly WebChatSettings _webChatSettings;
        private readonly IWebChatBotService _webChatBotService;
        private readonly ICustomerPortalData _customerPortalData;

        public BotHelperController
        (
            ILogger<BotHelperController> logger
            , IConfiguration configuration
            , IOptions<AppSettingsModel> appSettings
            , IOptions<WebChatSettings> webChatSettings
            , IWebChatBotService webChatBotService
            , ICustomerPortalData customerPortalData
        )
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _webChatSettings = webChatSettings.Value ?? throw new ArgumentNullException(nameof(webChatSettings));
            _webChatBotService = webChatBotService ?? throw new ArgumentNullException(nameof(webChatBotService));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
        }

        [HttpPost]
        [Route("directLineToken")]
        [Authorize]
        public async Task<IActionResult> GetDirectLineToken(WebChatLogModel dataModel)
        {
            try
            {
                var existingTokenModel = await this._customerPortalData.DirectLineToken_Get(dataModel);
                if (existingTokenModel != null && !existingTokenModel.NeedsRefresh)
                {
                    return this.Ok(existingTokenModel);
                }
                else
                {
                    var newTokenModel = await this._webChatBotService.GenerateDirectLineToken(this._webChatSettings, dataModel);
                    if (!string.IsNullOrEmpty(newTokenModel.DirectLineToken)
                        && !string.IsNullOrEmpty(newTokenModel.ConversationId)
                        && newTokenModel.Expires_InSecs != null
                        && newTokenModel.Expires_InSecs > 0)
                    {

                        var utcDateTime = DateTime.UtcNow;
                        //if (TimeSpan.TryParse(_appSettings.UtcOffset, out TimeSpan offset))
                        //{
                        //    utcDateTime += offset;
                        //}
                        newTokenModel.CreatedOn = utcDateTime;
                        newTokenModel.ExpiredOn = utcDateTime.AddSeconds((double)newTokenModel.Expires_InSecs);

                        var dbResult = await this._customerPortalData.DirectLineToken_InsertUpdate(transactionType: "I", dataModel: dataModel);
                        if (dbResult != null && dbResult.Id != null)
                        {
                            if (int.TryParse(dbResult.Id, out int logId))
                            {
                                newTokenModel.WebChatLogId = logId;
                            }
                        }

                        return Ok(newTokenModel);
                    }
                }

                return Ok(null);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }

    }
}
