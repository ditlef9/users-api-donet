using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace UsersApiDotnet
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        public IEnumerable<T> LoadData<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingle<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.QuerySingleOrDefault<T>(sql, parameters);
        }

        public bool ExecuteSql(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Execute(sql, parameters) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Execute(sql, parameters);
        }
    }
}
