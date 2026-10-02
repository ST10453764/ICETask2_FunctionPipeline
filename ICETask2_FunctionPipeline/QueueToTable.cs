using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ICETask2_FunctionPipeline
{
    public class QueueToTable
    {
        private readonly ILogger<QueueToTable> _logger;

        public QueueToTable(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QueueToTable>();
        }

        [Function("QueueToTable")]
        public async Task Run([QueueTrigger("ice-task-queue", Connection = "AzureWebJobsStorage")] string queueMessage)
        {
            _logger.LogInformation( "Queue message received: {Message}", queueMessage );

            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var tableClient = new TableClient(connectionString, "ICETaskTable");

            await tableClient.CreateIfNotExistsAsync();

            var entity = new TableEntity
            {
                PartitionKey = "ICE",
                RowKey = Guid.NewGuid().ToString(),
                ["Message"] = queueMessage,
                ["ProcessedAt"] = DateTime.UtcNow
            };

            await tableClient.AddEntityAsync(entity);

            _logger.LogInformation("Message successfully added to Table Storage.");
        }
    }
}