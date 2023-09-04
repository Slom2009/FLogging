using Xunit;
using Serilog;
using System.Data.SqlClient;
using System.Linq;

namespace LoggerApp.Tests
{
    public class LoggingTests
    {
        [Fact]
        public void TestLogMessage()
        {
            // Arrange
            var logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(
                    connectionString: "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;",
                    tableName: "Logs",
                    autoCreateSqlTable: true)
                .CreateLogger();

            var testMessage = "Test log message";

            // Act
            logger.Information(testMessage);

            // Assert
            using (var connection = new SqlConnection("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"))
            {
                connection.Open();

                var command = new SqlCommand("SELECT TOP 1 Message FROM Logs ORDER BY Timestamp DESC", connection);
                var lastLogMessage = (string)command.ExecuteScalar();

                Assert.Equal(testMessage, lastLogMessage);
            }
        }
    }
}
