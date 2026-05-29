using Common.Layer.Models;
using Common.Layer.Models.AppSettings;
using Common.Layer.Models.ContractMaster;
using Common.Layer.Models.CustomerPortal;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text;
using WebAPI.Layer.ExceptionLog;
using WebAPI.Layer.Services.GraphAPI;
using WebAPI.Layer.Services.SMTP;

namespace WebAPI.Layer.Helpers.SMTP
{
    public class SmtpHelper : ISmtpHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppSettingsModel _appSettings;
        private readonly ISmtpService _smtpService;
        //private readonly IGraphAPIService _graphAPIService;

        public SmtpHelper(IWebHostEnvironment env, IOptions<AppSettingsModel> appSettings, ISmtpService smtpService
            //GraphAPIService graphAPIService
            )
        {
            _env = env;
            _appSettings = appSettings.Value;
            _smtpService = smtpService;
            //_graphAPIService = graphAPIService;
        }

        public async Task<(EmailModel?, OTPModel?)> SendOTPEmail(CustomerDetailsModel dataModel, OTPModel? otpModel)
        {
            try
            {
                if (_appSettings == null)
                {
                    throw new ArgumentException("Required: App-Setting");
                }

                if (_appSettings != null && _appSettings.SMTPConfig == null)
                {
                    throw new ArgumentException("Required: App-Setting SMTPConfig");
                }

                string emailSubject = "OTP Verification";

                if (_appSettings != null && !string.IsNullOrEmpty(_appSettings.EmailSubject?.SignInOTPVerification))
                {
                    emailSubject = _appSettings.EmailSubject.SignInOTPVerification;
                }

                SMTPConfig newSmtpConfig = new SMTPConfig
                {
                    SendOTP = _appSettings.SMTPConfig.SendOTP,
                    Server = _appSettings.SMTPConfig.Server,
                    Port = _appSettings.SMTPConfig.Port,
                    UserId = _appSettings.SMTPConfig.UserId,
                    UserMail = _appSettings.SMTPConfig.UserMail,
                    DisplayName = _appSettings.SMTPConfig.DisplayName,
                    Pass = _appSettings.SMTPConfig.Pass,
                    IsCredRequired = _appSettings.SMTPConfig.IsCredRequired,
                    EnableSSL = _appSettings.SMTPConfig.EnableSSL,
                    UseGraphAPI = _appSettings.SMTPConfig.UseGraphAPI
                };

                if (otpModel != null)
                {

                    OTPModel newOtpModel = new OTPModel
                    {
                        OTP_Id = otpModel.OTP_Id,

                        ReferenceNo = dataModel.CustomerId,

                        Code = otpModel.Code,

                        Validity = otpModel.ValidityInSec <= 60
                                     ? otpModel.ValidityInSec + " secs"
                                     : (otpModel.ValidityInSec / 60) + " mins",

                        ValidityInSec = otpModel.ValidityInSec,

                        CreatedOn = otpModel.CreatedOn,

                        ExpiredOn = otpModel.ExpiredOn,

                        ExpiredOn_Formatted = otpModel.ExpiredOn?.ToString("MMM d, yyyy HH:mm:ss '(GMT +05:30)'"),

                        Recipient = dataModel.CustomerEmail
                    };

                    if (string.IsNullOrWhiteSpace(newOtpModel.Code) ||
                        string.IsNullOrWhiteSpace(newOtpModel.Recipient))
                    {
                        throw new ArgumentException("Required: OTPCode/Recipient");
                    }

                    var template = GetEmailTemplate("OTP.html");
                    var emailBody = template
                    .Replace("{{RecipientName}}", dataModel.CustomerName)
                    .Replace("{{OTPExpiry}}", newOtpModel.Validity)
                    .Replace("{{OTPExpiryDateTime}}", newOtpModel.ExpiredOn_Formatted)
                    .Replace("{{OTPCode}}", newOtpModel.Code)
                    ;

                    EmailModel newEmailModel = new EmailModel
                    {
                        Type = "OTP",
                        From = _appSettings.SMTPConfig.UserMail,
                        To = newOtpModel.Recipient,
                        Subject = emailSubject,
                        Body = emailBody,
                        Status = false,
                        Message = "OTP Sent Successfully",
                        ReferenceNo = dataModel.CustomerId,
                        OTP_Id = otpModel.OTP_Id
                    };

                    if (string.IsNullOrWhiteSpace(newEmailModel.To) ||
                        string.IsNullOrWhiteSpace(newEmailModel.Subject) ||
                        string.IsNullOrWhiteSpace(newEmailModel.Body))
                    {
                        throw new ArgumentException("Required: To/Subject/Body");
                    }

                    if (newSmtpConfig != null && newSmtpConfig.SendOTP)
                    {
                        //if (newSmtpConfig.UseGraphAPI
                        //    && _graphAPIService != null)
                        //{
                        //    var result = await _graphAPIService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        //    newEmailModel.Status = result;
                        //}
                        //else
                        //{
                        //    var result = await _smtpService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        //    newEmailModel.Status = result;
                        //}

                        var result = await _smtpService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        newEmailModel.Status = result;
                    }

                    return (newEmailModel, newOtpModel);
                }
                else
                {
                    var otpCode = GenerateOTP();

                    int validityInSecs = 60;
                    if (_appSettings?.OTPConfig?.ValidityInSec != null)
                    {
                        validityInSecs = int.Parse(_appSettings.OTPConfig.ValidityInSec);
                    }

                    var timeNow = DateTime.Now;

                    OTPModel newOtpModel = new OTPModel
                    {
                        OTP_Id = 0,

                        ReferenceNo = dataModel.CustomerId,

                        Code = otpCode,

                        Validity = validityInSecs <= 60
                                     ? validityInSecs + " secs"
                                     : (validityInSecs / 60) + " mins",

                        ValidityInSec = validityInSecs,

                        CreatedOn = timeNow,

                        ExpiredOn = timeNow.AddSeconds(validityInSecs),

                        ExpiredOn_Formatted = timeNow
                                                    .AddSeconds(validityInSecs)
                                                    .ToString("MMM d, yyyy HH:mm:ss '(GMT +05:30)'"),

                        Recipient = dataModel.CustomerEmail
                    };

                    if (string.IsNullOrWhiteSpace(newOtpModel.Code) ||
                        string.IsNullOrWhiteSpace(newOtpModel.Recipient))
                    {
                        throw new ArgumentException("Required: OTPCode/Recipient");
                    }

                    var template = GetEmailTemplate("OTP.html");
                    var emailBody = template
                    .Replace("{{RecipientName}}", dataModel.CustomerName)
                    .Replace("{{OTPExpiry}}", newOtpModel.Validity)
                    .Replace("{{OTPExpiryDateTime}}", newOtpModel.ExpiredOn_Formatted)
                    .Replace("{{OTPCode}}", newOtpModel.Code)
                    ;

                    EmailModel newEmailModel = new EmailModel
                    {
                        Type = "OTP",
                        From = _appSettings.SMTPConfig.UserMail,
                        To = newOtpModel.Recipient,
                        Subject = emailSubject,
                        Body = emailBody,
                        Status = false,
                        Message = "OTP Sent Successfully",
                        ReferenceNo = dataModel.CustomerId,
                        CreatedOn = timeNow
                    };

                    if (string.IsNullOrWhiteSpace(newEmailModel.To) ||
                        string.IsNullOrWhiteSpace(newEmailModel.Subject) ||
                        string.IsNullOrWhiteSpace(newEmailModel.Body))
                    {
                        throw new ArgumentException("Required: To/Subject/Body");
                    }

                    if (newSmtpConfig != null && newSmtpConfig.SendOTP)
                    {
                        //if (newSmtpConfig.UseGraphAPI
                        //    && _graphAPIService != null)
                        //{
                        //    var result = await _graphAPIService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        //    newEmailModel.Status = result;
                        //}
                        //else
                        //{
                        //    var result = await _smtpService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        //    newEmailModel.Status = result;
                        //}

                        var result = await _smtpService.SendEmailAsync(newSmtpConfig, newEmailModel);
                        newEmailModel.Status = result;
                    }

                    return (newEmailModel, newOtpModel);
                }

            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return (null, null);
            }
        }









        #region Private Methods

        private string GenerateOTP()
        {
            Random random = new Random();
            StringBuilder otpCode = new StringBuilder();

            otpCode.Append(random.Next(1, 10));

            for (int i = 1; i < 6; i++)
            {
                int nextDigit;
                do
                {
                    nextDigit = random.Next(0, 10);
                }
                while (otpCode[i - 1] == nextDigit + '0');

                otpCode.Append(nextDigit);
            }

            return otpCode.ToString();
        }

        private string GetEmailTemplate(string templateName)
        {
            //try
            //{
            //    var assembly = Assembly.GetExecutingAssembly();
            //    var resourceName = $"WebAPIApplication.Templates.HTML.{templateName}";

            //    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        return reader.ReadToEnd();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ExceptionLogging.SendErrorToText(ex);
            //    return string.Empty;
            //}

            try
            {
                // wwwroot/Templates/HTML folder
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "HTML");
                var filePath = Path.Combine(rootPath, templateName);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Template file '{templateName}' not found at path: {filePath}");

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                throw;
            }
        }

        #endregion
    }
}
