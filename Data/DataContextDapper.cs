using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace UsersApiDotnet.Data
{
    // Provides methods to interact with the database using Dapper for executing queries and commands.
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        // Initializes a new instance of the <see cref="DataContextDapper"/> class with the provided configuration.
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        // Creates and returns a new database connection using the configured connection string.
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        // Executes a query and returns a collection of data of type <typeparamref name="T"/>.
        public IEnumerable<T> LoadData<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Query<T>(sql, parameters);
        }

        // Executes a query and returns a single result of type <typeparamref name="T"/> or the default value if no result is found.
        public T LoadDataSingle<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.QuerySingleOrDefault<T>(sql, parameters);
        }

        // Executes a SQL command and returns a boolean indicating whether the execution affected any rows.
        public bool ExecuteSql(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Execute(sql, parameters) > 0;
        }

        // Executes a SQL command and returns the number of rows affected.
        public int ExecuteSqlWithRowCount(string sql, object? parameters = null)
        {
            using IDbConnection dbConnection = CreateConnection();
            return dbConnection.Execute(sql, parameters);
        }
    }
}
