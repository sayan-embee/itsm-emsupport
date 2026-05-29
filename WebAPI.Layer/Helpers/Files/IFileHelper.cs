using Common.Layer.Models.ContractMaster;

namespace WebAPI.Layer.Helpers.Files
{
    public interface IFileHelper
    {
        Task<ContractMasterModel?> UploadFilesOnServer(ContractMasterModel dataModel, IFormFileCollection fileList);
        Task<ContractMasterModel?> DeleteFilesOnServer(ContractMasterModel dataModel);
    }
}