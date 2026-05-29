using Common.Layer.Models;
using Common.Layer.Models.Bot;
using Common.Layer.Models.ContractMaster;
using Common.Layer.Models.Enum;
using Common.Layer.Models.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Layer.Data.Common
{
    public interface ICommonData
    {
        Task<ConversationModel> Get_M_ConversationByUserId(Guid userId);
        Task<ReturnMessageModel> BotInstallUninstall_InsertUpdate(ConversationModel conversation);        
        Task<DataSet> Get_M_Department(string name, long? id, string ReportType, bool? active);
        Task<DataSet> Get_M_ReportSection(bool? active, long? departmentId);
        Task<UserDetailsModel?> Get_M_UserAccess(UserDetailsModel userModel);
        Task<ReturnMessageModel> TeamsBot_UserSearch_InsertUpdate(UserMessageModel dataModel);

        Task<DataSet> MasterData_Get();

        Task<ReturnMessageModel> ContractMaster_InsertUpdate(string transactionType, ContractMasterModel dataModel);
        Task<DataSet> ContractMaster_Get(ContractMasterModel dataModel);

        Task<List<CategoryModel>?> GetCategoryMaster();
        Task<List<SubCategoryModel>?> GetSubCategoryMaster(string code = "");
    }
}