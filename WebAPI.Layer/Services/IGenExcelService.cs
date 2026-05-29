using Common.Layer.Models.Report;
using System.Data;

namespace WebAPI.Layer.Services
{
    public interface IGenExcelService
    {
        string GenerateExcelNReturnPath(DataSet data, ParamModel paramModel);
    }
}