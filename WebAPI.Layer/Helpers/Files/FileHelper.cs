using Common.Layer.Models.AppSettings;
using Common.Layer.Models.ContractMaster;
using Microsoft.Extensions.Options;
using WebAPI.Layer.ExceptionLog;
using WebAPI.Layer.Helpers.Files;

namespace WebAPI.Layer.Helpers
{
    public class FileHelper : IFileHelper
    {
        private readonly AppSettingsModel _appSettings;

        public FileHelper(IOptions<AppSettingsModel> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private async Task<string> CheckOrCreateDirectory(string refNo)
        {
            try
            {
                if (!string.IsNullOrEmpty(refNo))
                {
                    var modifiedCaseNumber = refNo;

                    var mainDirectoryPath = System.IO.Directory.GetCurrentDirectory() + @"\ContractMasterFiles";
                    if (!System.IO.Directory.Exists(mainDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(mainDirectoryPath);
                    }

                    var subDirectoryPath = Path.Combine(mainDirectoryPath, modifiedCaseNumber);
                    if (!System.IO.Directory.Exists(subDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(subDirectoryPath);
                    }

                    return subDirectoryPath;
                }

                await Task.Delay(0);
                return string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return string.Empty;
            }
        }

        private async Task<bool> UploadFile(string filePath, IFormFile file)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && file != null)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return false;
            }
        }

        public async Task<ContractMasterModel?> UploadFilesOnServer(ContractMasterModel dataModel, IFormFileCollection fileList)
        {
            try
            {
                var directoryPath = await CheckOrCreateDirectory(dataModel.ContractNo);
                if (!string.IsNullOrEmpty(directoryPath) && dataModel.FileList != null)
                {
                    foreach (var eachFile in fileList)
                    {
                        if (eachFile != null && eachFile.Length > 0)
                        {
                            FileInfo fi = new FileInfo(eachFile.FileName);
                            string ext = fi.Extension;
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fi.Name);
                            string formattedDate = DateTime.Now.ToString("ddMMyyyyHHmmss");

                            var objectIndex = dataModel.FileList
                                .FindIndex(f => string.Equals(f.InternalName, eachFile.Name, StringComparison.OrdinalIgnoreCase));

                            if (objectIndex >= 0)
                            {
                                var objectToUpdate = dataModel.FileList[objectIndex];

                                if (objectToUpdate != null)
                                {
                                    //string customFileName = $"{fileNameWithoutExtension}_{objectToUpdate.DocTypeId}_{formattedDate}{ext}";
                                    string customFileName = $"{fileNameWithoutExtension}_{formattedDate}{ext}";

                                    objectToUpdate.ContractId = dataModel.Id;
                                    objectToUpdate.Name = eachFile.FileName;
                                    objectToUpdate.ContentType = eachFile.ContentType;
                                    objectToUpdate.InternalName = customFileName;

                                    string filePath = Path.Combine(directoryPath, customFileName);
                                    if (!string.IsNullOrEmpty(filePath))
                                    {
                                        //var uploadResult = await UploadFile(filePath, eachFile);                                     

                                        //if (uploadResult)
                                        //{
                                        //    objectToUpdate.FilePath = filePath;
                                        //    objectToUpdate.FileUrl = $"{_appSettings.DomainUrl}/files/{dataModel.ContractNo}/{customFileName}";
                                        //}

                                        using (var stream = new FileStream(filePath, FileMode.Create))
                                        {
                                            eachFile.CopyTo(stream);
                                        }

                                        objectToUpdate.PhysicalPath = filePath;
                                        objectToUpdate.Url = $"{_appSettings.AppDomainUrl}/files/{dataModel.ContractNo}/{customFileName}";
                                    }

                                    dataModel.FileList[objectIndex] = objectToUpdate;
                                }
                            }
                        }
                    }

                    return dataModel;
                }

                return null;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<ContractMasterModel?> DeleteFilesOnServer(ContractMasterModel dataModel)
        {
            try
            {
                if (string.IsNullOrEmpty(dataModel.ContractNo))
                {
                    return null;
                }

                var directoryPath = await CheckOrCreateDirectory(dataModel.ContractNo);
                if (!string.IsNullOrEmpty(directoryPath) && dataModel.FileList != null)
                {
                    foreach (var eachFile in dataModel.FileList)
                    {
                        if (eachFile != null && eachFile.Id > 0 && (!eachFile?.Active ?? false))
                        {
                            foreach (var file in Directory.GetFiles(directoryPath))
                            {
                                var fileName = Path.GetFileName(file);
                                if (!string.IsNullOrEmpty(eachFile?.InternalName) && fileName.Equals(eachFile.InternalName, StringComparison.OrdinalIgnoreCase))
                                {
                                    File.Delete(file);
                                }
                            }
                        }
                    }

                    return dataModel;
                }

                return null;
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }
    }
}
