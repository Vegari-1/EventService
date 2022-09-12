using EventService.Model;
using EventService.Repository;
using Npgsql;
using System;

namespace EventService.IntegrationTests
{
    public static class DbExtensions
    {

        public static long CountTableRows(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName)
        {
            long totalRows = -1;
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "SELECT COUNT(*) FROM " + schemaName + ".\"" + tableName + "\"";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalRows = (long)reader[0];
                        }
                    }
                }
            }
            return totalRows;
        }

        public static void Insert(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName, Event eventEntity)
        {
            string insertQuery = "INSERT INTO " + schemaName + ".\"" + tableName + 
                                 "\" (\"Id\", \"Timestamp\", " +
                                 "\"Source\", \"RequestType\", \"Message\", \"StatusCode\", " +
                                 "\"StatusCodeText\") " +
                                 "VALUES (@Id, @Timestamp, @Source, @RequestType, @Message, " +
                                 "@StatusCode, @StatusCodeText)";
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand(insertQuery, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@Id", eventEntity.Id);
                    command.Parameters.AddWithValue("@Timestamp", eventEntity.Timestamp);
                    command.Parameters.AddWithValue("@Source", eventEntity.Source);
                    command.Parameters.AddWithValue("@RequestType", eventEntity.RequestType);
                    command.Parameters.AddWithValue("@Message", eventEntity.Message);
                    command.Parameters.AddWithValue("@StatusCode", eventEntity.StatusCode);
                    command.Parameters.AddWithValue("@StatusCodeText", eventEntity.StatusCodeText);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteById(this IntegrationWebApplicationFactory<Program, AppDbContext> factory,
            string schemaName, string tableName, Guid id)
        {
            using (var connection = new NpgsqlConnection(factory.postgresContainer.ConnectionString))
            {
                using (var command = new NpgsqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM " + schemaName + ".\"" + tableName + "\" WHERE \"Id\" = @id";
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
