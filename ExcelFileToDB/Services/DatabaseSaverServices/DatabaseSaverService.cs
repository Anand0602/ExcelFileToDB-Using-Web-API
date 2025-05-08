using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ExcelFileToDB.Services.DatabaseSaverServices
{
    public class DatabaseSaverService : IDatabaseSaverService
    {
        private readonly IConfiguration _configuration;

        public DatabaseSaverService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SaveDataAsync(List<Dictionary<string, object>> rows, string tableName)
        {
            if (rows == null || rows.Count == 0)
                throw new ArgumentException("No data to save.");

            var headers = rows[0].Keys.ToList();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            // Create table if not exists
            string checkTableQuery = $@"
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}')
            BEGIN
                CREATE TABLE {tableName} (
                    {string.Join(", ", headers.Select(h => $"[{h}] NVARCHAR(MAX)"))}
                );
            END";

            using (var command = new SqlCommand(checkTableQuery, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            // Insert data
            foreach (var rowDict in rows)
            {
                string insertQuery = $@"
                INSERT INTO {tableName} ({string.Join(", ", headers.Select(h => $"[{h}]"))})
                VALUES ({string.Join(", ", headers.Select((_, idx) => $"@value{idx}"))})";

                using var insertCommand = new SqlCommand(insertQuery, connection);
                for (int i = 0; i < headers.Count; i++)
                {
                    insertCommand.Parameters.AddWithValue($"@value{i}", rowDict[headers[i]] ?? DBNull.Value);
                }

                await insertCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
