using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ICETask2_FunctionPipeline
{
    public class HttpToQueue
    {
        private readonly ILogger<HttpToQueue> _logger;

        public HttpToQueue(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpToQueue>();
        }

        [Function("HttpToQueue")]
        public async Task<HttpToQueueOutput> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            _logger.LogInformation("Received message: {Message}", requestBody);

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync("Message received and sent to the queue.");

            return new HttpToQueueOutput
            {
                HttpResponse = response,
                QueueMessage = requestBody
            };
        }
    }

    public class HttpToQueueOutput
    {
        [HttpResult]
        public HttpResponseData HttpResponse { get; set; }

        [QueueOutput("ice-task-queue",Connection = "AzureWebJobsStorage")]
        public string QueueMessage { get; set; }
    }
}