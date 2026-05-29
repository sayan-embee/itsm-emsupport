using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.CustomerPortal;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.JWT;
using DataAccess.Layer.Data.Common;
using DataAccess.Layer.Data.CustomerPortal;
using External.CustomerPortal.Layer.ExceptionLog;
using External.CustomerPortal.Layer.Helpers.SMTP;
using External.CustomerPortal.Layer.Services.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace External.CustomerPortal.Layer.Controllers
{
    [Route("CustomerPortal/api")]
    [ApiController]
    // [TypeFilter(typeof(APIKeyAuthorization))]
    public class CustomerPortalController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICommonData _commonData;
        private readonly ICustomerPortalData _customerPortalData;
        private readonly ISmtpHelper _smtpHelper;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppSettingsModel _appSettings;

        private readonly string generalError = "There was a problem processing your request. Please try again.";
        private readonly string emailNotRegistered = "Your email id: {userEmail} is not registered with us.";
        private readonly string portalAccessDenied = "Currently you don't have access to this portal.";
        private readonly string otpSessionExpired = "Your session is no longer valid. Please request a new OTP.";
        private readonly string loginSessionExpired = "Your session has expired. Please log in again.";
        private readonly string invalidSession = "Your session is no longer valid. Please log in again.";

        public CustomerPortalController
        (
            ILogger<CustomerPortalController> logger
            , IMemoryCache cache
            , IConfiguration configuration
            , ICommonData commonData
            , ICustomerPortalData customerPortalData
            , ISmtpHelper smtpHelper
            , IJwtTokenService jwtTokenService
            , IOptions<AppSettingsModel> appSettings
        )
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
            _commonData = commonData ?? throw new ArgumentNullException(nameof(commonData));
            _customerPortalData = customerPortalData ?? throw new ArgumentNullException(nameof(customerPortalData));
            _smtpHelper = smtpHelper ?? throw new ArgumentNullException(nameof(smtpHelper));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

            generalError = _configuration.GetValue<string>("Messages:GeneralError");
            emailNotRegistered = _configuration.GetValue<string>("Messages:EmailNotRegistered");
            portalAccessDenied = _configuration.GetValue<string>("Messages:PortalAccessDenied");
            otpSessionExpired = _configuration.GetValue<string>("Messages:OTPSessionExpired");
            loginSessionExpired = _configuration.GetValue<string>("Messages:LoginSessionExpired");
            invalidSession = _configuration.GetValue<string>("Messages:InvalidSession");
        }

        [HttpPost]
        [Route("newOTP")]
        public async Task<IActionResult> GenerateOTP(CustomerSignInModel dataModel)
        {
            try
            {
                if (dataModel == null ||
                    string.IsNullOrEmpty(dataModel.UserEmail))
                {
                    throw new InvalidDataException("Required parameter: UserEmail");
                }

                bool portalAccess = false;
                string portalAccessMsg = generalError;

                var returnMessageModel = new ReturnMessageModel
                {
                    Status = 0,
                    Message = portalAccessMsg
                };

                var newObj = new CustomerDetailsModel
                {
                    CustomerEmail = dataModel.UserEmail,
                };

                var customerDetailsList = await _customerPortalData.CustomerDetails_Get(newObj);
                if (customerDetailsList != null && customerDetailsList.Count > 0)
                {
                    portalAccess = customerDetailsList.Any(x => x.customer_portal_access != null && x.customer_portal_access == "true");

                    if (!portalAccess)
                    {
                        returnMessageModel.Message = portalAccessDenied;
                        return Ok(returnMessageModel);
                    }

                    var newSessionId = Guid.NewGuid().ToString();

                    try
                    {
                        dataModel.ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString();

                        // Check for the X-Forwarded-For header (if behind a proxy/load balancer)
                        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                        {
                            var xForwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                            dataModel.ClientIP = xForwardedFor.Split(',').FirstOrDefault()?.Trim();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogging.SendErrorToText(ex);
                    }

                    newObj.CustomerName = customerDetailsList.FirstOrDefault().CustomerName;

                    dataModel.SessionId = newSessionId;
                    dataModel.UserName = customerDetailsList.FirstOrDefault().CustomerName;
                    dataModel.UserId = customerDetailsList.FirstOrDefault().CustomerId;                   

                    // Logic: Generate new OTP
                    var (email, otp) = await _smtpHelper.SendOTPEmail(newObj, null);
                    //if (email != null && email.Status && otp != null)
                    if (email != null && otp != null)
                    {
                        otp.SessionId = newSessionId;
                        email.SessionId = newSessionId;

                        var insertOTP = await _customerPortalData.OTPLog_InsertUpdate("I", otp);
                        if (insertOTP != null && int.Parse(insertOTP.Id) > 0 && email != null)
                        {
                            otp.OTP_Id = int.Parse(insertOTP.Id);
                            dataModel.OTPId = int.Parse(insertOTP.Id);

                            email.OTP_Id = int.Parse(insertOTP.Id);

                            // Set cache 
                            _cache.Set(dataModel.UserEmail, dataModel, TimeSpan.FromMinutes(5));

                            _ = _customerPortalData.EmailLog_InsertUpdate("I", email);
                            _ = _customerPortalData.SignInLog_InsertUpdate("I", dataModel);
                        }

                        var returnOtpModel = new OTPModel
                        {
                            OTP_Id = otp.OTP_Id,
                            Recipient = dataModel.UserEmail,
                            ValidityInSec = otp.ValidityInSec,
                            ExpiredOn = otp.ExpiredOn
                        };

                        return Ok(returnOtpModel);
                    }

                    // Logic: Send Previous OTP
                    //var otpResult = await this._vendorOnBoardData.GetVendorOnBoardSentOtp(dataModel.refno);
                    //var (email, otp) = await _smtpHelper.SendOTPEmail(dataModel, otpResult);
                    //if (otpResult == null && otp != null)
                    //{
                    //    var insertOTP = await this._vendorOnBoardData.InsupVendorOnBoardSentOtp("I", otp);
                    //    if (insertOTP != null && insertOTP.listId > 0 && email != null)
                    //    {
                    //        email.otp_id = (int)insertOTP.listId;
                    //    }
                    //}
                    //if (email != null && email.status)
                    //{
                    //    _ = this._vendorOnBoardData.InsupVendorOnBoardSentMail(email);
                    //}

                    return Ok(returnMessageModel);
                }
                else
                {
                    returnMessageModel.Message = emailNotRegistered.Replace("{userEmail}", dataModel.UserEmail);
                    return Ok(returnMessageModel);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("verifyOTP")]
        public async Task<IActionResult> VerifyOTP(OTPModel dataModel)
        {
            try
            {
                if (dataModel == null ||
                    string.IsNullOrEmpty(dataModel.Recipient) ||
                    string.IsNullOrEmpty(dataModel.Code))
                {
                    throw new InvalidDataException("Required parameter: Recipient | Code");
                }

                // Verify OTP
                var result = await _customerPortalData.OTPLog_InsertUpdate("U", dataModel);
                if (result != null && result.Status > 0)
                {
                    if (_cache.TryGetValue(dataModel.Recipient, out CustomerSignInModel signInModel) && signInModel != null)
                    {
                        //var utcDateTime = DateTime.UtcNow;
                        //if (TimeSpan.TryParse(_appSettings.UtcOffset, out TimeSpan offset))
                        //{
                        //    utcDateTime += offset;
                        //}
                        //var expiryMinutes = 2; // _jwtTokenService.GetTokenExpiryInMinutes();
                        //var tokenExpiry = utcDateTime.AddMinutes(expiryMinutes);
                        //var tokenExpiryUtc = TimeZoneInfo.ConvertTimeToUtc(tokenExpiry);

                        var expiryMinutes = _jwtTokenService.GetTokenExpiryInMinutes(); // _jwtTokenService.GetTokenExpiryInMinutes();
                        var tokenExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

                        signInModel.JWTTokenExpiredOn = tokenExpiry;

                        // Generate JWT token
                        var tokenModel = new JwtTokenModel
                        {
                            Role = "User",
                            UserEmail = dataModel.Recipient,
                            CustomerId = signInModel.UserId,
                            SessionId = signInModel.SessionId,
                            ExpiresOn = tokenExpiry
                        };

                        var token = _jwtTokenService.GenerateJwtToken(tokenModel);

                        result.JwtToken = token;
                        result.SessionId = signInModel.SessionId;
                        signInModel.JWTTokenId = token;

                        // Set the token in an HTTP-only cookie
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = tokenExpiry
                        };

                        Response.Cookies.Append("jwtToken", token, cookieOptions);
                        _cache.Remove(dataModel.Recipient);

                        signInModel.IsSessionActive = true;
                        _ = await _customerPortalData.SignInLog_InsertUpdate("U", signInModel);

                        result.JwtTokenExpiry = tokenExpiry;
                        return Ok(result);
                    }

                    result.Status = 0;
                    result.Message = otpSessionExpired;
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        //[HttpPost]
        //[Route("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    try
        //    {
        //        // Clear the JWT cookie
        //        Response.Cookies.Delete("jwtToken", new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict
        //        });

        //        await Task.Delay(500);

        //        return Ok(new
        //        {
        //            Status = 1,
        //            Message = "Logged out successfully."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex);
        //        return Problem(ex.Message);
        //    }
        //}
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                string token = Request.Cookies["jwtToken"] ?? Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { Status = 0, Message = "No token found for logout." });
                }

                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken;

                try
                {
                    jwtToken = handler.ReadJwtToken(token);
                }
                catch (Exception)
                {
                    return Unauthorized(new { Status = 0, Message = "Invalid or expired token." });
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
                var userEmail = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var sessionId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized(new { Status = 0, Message = "Invalid session or user information." });
                }

                var signInModel = new CustomerSignInModel
                {
                    SessionId = sessionId,
                    UserId = userId,
                    UserEmail = userEmail,
                    IsSessionActive = false,
                    SignOutRemarks = "Manual-SignOut"
                };

                _ = await _customerPortalData.SignInLog_InsertUpdate("U", signInModel);

                // Clear the JWT cookie
                Response.Cookies.Delete("jwtToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { Status = 1, Message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        #region Protected APIs


        [HttpPost]
        [Route("getCustomerDetails")]
        [Authorize]
        public async Task<IActionResult> GetCustomerDetails()
        {
            try
            {
                var returnMessageModel = new ReturnMessageModel
                {
                    Status = 0,
                    Message = generalError
                };


                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var customerId = User.FindFirst(ClaimTypes.UserData)?.Value;


                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(customerId))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = invalidSession;

                    return Unauthorized(returnMessageModel);
                }

                if (HttpContext.Response.Headers.ContainsKey("Token-Expired"))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = loginSessionExpired;

                    return Unauthorized(returnMessageModel);
                }


                var newObj = new CustomerDetailsModel
                {
                    CustomerEmail = email,
                };
                var customerDetailsList = await _customerPortalData.CustomerDetails_Get(newObj);
                if (customerDetailsList != null && customerDetailsList.Count > 0)
                {
                    return Ok(customerDetailsList);
                }

                return Unauthorized(returnMessageModel);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("getTicketDetails")]
        [Authorize]
        public async Task<IActionResult> GetFreshServiceTickets([FromBody] CustomerDetailsModel dataModel)
        {
            try
            {
                var returnMessageModel = new ReturnMessageModel
                {
                    Status = 0,
                    Message = generalError
                };


                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var customerId = User.FindFirst(ClaimTypes.UserData)?.Value;


                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(customerId))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = invalidSession;

                    return Unauthorized(returnMessageModel);
                }

                if (HttpContext.Response.Headers.ContainsKey("Token-Expired"))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = loginSessionExpired;

                    return Unauthorized(returnMessageModel);
                }

                var result = await this._customerPortalData.CP_FreshService_Tickets_Get(dataModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        [HttpPost]
        [Route("getMasterData")]
        [Authorize]
        public async Task<IActionResult> GetMasterData([FromBody] CustomerDetailsModel dataModel)
        {
            try
            {
                var returnMessageModel = new ReturnMessageModel
                {
                    Status = 0,
                    Message = generalError
                };


                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var customerId = User.FindFirst(ClaimTypes.UserData)?.Value;


                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(customerId))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = invalidSession;

                    return Unauthorized(returnMessageModel);
                }

                if (HttpContext.Response.Headers.ContainsKey("Token-Expired"))
                {
                    returnMessageModel.Status = 0;
                    returnMessageModel.Message = loginSessionExpired;

                    return Unauthorized(returnMessageModel);
                }

                var result = await this._customerPortalData.CP_CustomerWise_MasterData_Get(dataModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return Problem(ex.Message);
            }
        }


        #endregion

    }
}
