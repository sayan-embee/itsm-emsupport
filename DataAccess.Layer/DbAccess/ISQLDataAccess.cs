using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DataAccess.Layer.DbAccess
{
    public interface ISQLDataAccess
    {
        Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default");
        Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default");
        Task<IEnumerable<T>> SaveData<T, U>(string storedProcedure, U parameters, string connectionId = "Default");
        //
        Task<IEnumerable<T>> LoadDatabyQuery<T, U>(string query, U parameters, string connectionId = "Default");
        Task<DataSet> LoadDataSet(string storedProcedure, List<SqlParameter> sqlParams, string connectionId = "Default");
        //
    }
}