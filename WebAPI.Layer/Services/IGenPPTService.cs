using Common.Layer.Models.Report;
using System.Data;

namespace WebAPI.Layer.Services
{
    public interface IGenPPTService
    {
        //Task<string> GenerateNSaveBarChart(IDictionary<string, object>[] aggregatedData, string directoryPath, string fileName);
        Task<DataTable> CreateDynamicDataTable<T>(IEnumerable<T> data, Func<T, bool> filter = null);
        Task<string> GeneratePpt(DataSet Datas, HelperModel helperModel);
        //, 
        Task<string> GeneratePptForOnMobile(DataSet Datas , HelperModel helperModel);
        Task<DataTable> ProcessAndMapColumnsWithCustomNames(DataTable originalTable, string columns, string customColumnNames);
    }
}