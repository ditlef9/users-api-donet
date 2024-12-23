using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace UsersApiDotnet.Data
{
    public class DatabaseMigration
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DatabaseMigration> _logger;

        public DatabaseMigration(IConfiguration config, ILogger<DatabaseMigration> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void InitDatabase()
        {
            _logger.LogInformation("Initializing database...");

            try
            {
                using (var connection = GetDbConnection())
                {
                    connection.Open();
                    CreateMigrationTable(connection);
                    RunMigrations(connection);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
            }
        }

        private IDbConnection GetDbConnection() => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        private void CreateMigrationTable(IDbConnection connection)
        {
            _logger.LogInformation("Creating migration table if not exists...");

            var sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Migrations' AND xtype='U')
                CREATE TABLE Migrations (
                    MigrationId INT IDENTITY PRIMARY KEY,
                    MigrationModule NVARCHAR(255),
                    MigrationName NVARCHAR(255) NOT NULL,
                    MigrationRunTimestamp DATETIME DEFAULT GETDATE()
                );";

            connection.Execute(sql);
        }

        private void RunMigrations(IDbConnection connection)
        {
            _logger.LogInformation("Running migrations...");

            var migrationsPath = Path.Combine(AppContext.BaseDirectory, "Data", "Migrations");
            if (!Directory.Exists(migrationsPath))
            {
                _logger.LogWarning("No migrations folder found. Skipping migrations.");
                return;
            }

            var migrationFiles = Directory.GetFiles(migrationsPath, "*.sql");
            foreach (var file in migrationFiles)
            {
                var migrationName = Path.GetFileName(file);
                if (!IsMigrationApplied(connection, migrationName))
                {
                    ApplyMigration(connection, migrationName, file);
                    LogMigration(connection, migrationName);
                }
            }
        }

        private bool IsMigrationApplied(IDbConnection connection, string migrationName)
        {
            var sql = "SELECT COUNT(1) FROM Migrations WHERE MigrationName = @MigrationName";
            return connection.ExecuteScalar<int>(sql, new { MigrationName = migrationName }) > 0;
        }

        private void ApplyMigration(IDbConnection connection, string migrationName, string filePath)
        {
            _logger.LogInformation($"Applying migration: {migrationName}");
            var migrationSql = File.ReadAllText(filePath);
            connection.Execute(migrationSql);
        }

        private void LogMigration(IDbConnection connection, string migrationName)
        {
            _logger.LogInformation($"Logging migration: {migrationName}");
            var sql = "INSERT INTO Migrations (MigrationModule, MigrationName) VALUES (@Module, @Name)";
            connection.Execute(sql, new { Module = "default", Name = migrationName });
        }
    }
}
