using Common.Layer.Models.ContractMaster;
using Common.Layer.Models;
using Common.Layer.Models.CustomerPortal;

namespace External.CustomerPortal.Layer.Helpers.SMTP
{
    public interface ISmtpHelper
    {
        Task<(EmailModel, OTPModel)> SendOTPEmail(CustomerDetailsModel dataModel, OTPModel otpModel);
    }
}