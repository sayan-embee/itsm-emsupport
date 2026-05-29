using Common.Layer.Models;
using Common.Layer.Models.CustomerPortal;
using Common.Layer.Models.FreshService;
using Common.Layer.Models.WebChatBot;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Layer.Data.CustomerPortal
{
    public interface ICustomerPortalData
    {
        Task<List<CustomerDetailsModel>?> CustomerDetails_Get(CustomerDetailsModel dataModel);
        Task<dynamic> CP_FreshService_Tickets_Get(CustomerDetailsModel dataModel);
        Task<dynamic> CP_CustomerWise_MasterData_Get(CustomerDetailsModel dataModel);

        Task<ReturnMessageModel> SignInLog_InsertUpdate(string transactionType, CustomerSignInModel dataModel);
        Task<ReturnMessageModel> OTPLog_InsertUpdate(string transactionType, OTPModel dataModel);
        Task<ReturnMessageModel> EmailLog_InsertUpdate(string transactionType, EmailModel dataModel);

        Task<WebChatLogModel?> DirectLineToken_Get(WebChatLogModel dataModel);
        Task<ReturnMessageModel> DirectLineToken_InsertUpdate(string transactionType, WebChatLogModel dataModel);

        Task<List<WebChatOptionsModel>?> WebChatOptions_Get(string categoryIdList, string subCategoryIdList, int? top = null);
        Task<WebChatSOPIndexModel?> WebChatSOPIndex_Get(int categoryId, int subCategoryId);
        Task<List<WebChatFeedbackOptionsModel>?> WebChatFeedbackOptions_Get();

        Task<ReturnMessageModel> UserConversationLog_InsertUpdate(string transactionType, WebChatUserMessageModel data);
    }
}