using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using static Dapper.SqlMapper;
using Microsoft.Extensions.Logging;
using Common.Layer.Models.AppSettings;
using System.Linq;

namespace DataAccess.Layer.DbAccess
{
    public class SQLDataAccess : ISQLDataAccess
    {
        private readonly IConfiguration _config;
        public SQLDataAccess(IConfiguration config)
        {
            this._config = config;
        }

        public async Task<IEnumerable<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection( _config.GetConnectionString(connectionId));
            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task SaveData<T>(string storedProcedure, T parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            
            await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 300);
        }

        public async Task<IEnumerable<T>> SaveData<T, U>(string storedProcedure, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: 300);
        }
        
        public async Task<IEnumerable<T>> LoadDatabyQuery<T, U>(string query, U parameters, string connectionId = "Default")
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            return await connection.QueryAsync<T>(query, parameters, commandType: CommandType.Text);
        }
        public async Task<DataSet> LoadDataSet(string storedProcedure, List<SqlParameter> sqlParams, string connectionId = "Default")
        {
            DataSet dataSet = null;
            await Task.Delay(0);
            try
            {
                using (SqlConnection conn = new SqlConnection(_config.GetConnectionString(connectionId)))
                {
                    dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 600;

                    if (sqlParams != null && sqlParams.Any())
                    {
                        foreach (SqlParameter prm in sqlParams)
                        {
                            cmd.Parameters.Add(prm);
                        }

                    }
                    conn.Open();
                    // create data adapter
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(dataSet);
                    conn.Close();
                    da.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
            return dataSet;

        }
        //
    }
}
