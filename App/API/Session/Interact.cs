using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class Interact
    {
        private readonly ILogger<Interact> _logger;

        public Interact(ILogger<Interact> logger)
        {
            _logger = logger;
        }

        [Function("Interact")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var jsonIn = req.Query["input"];

            // Save request to the event store

            // Send to chat

            var jsonOut = @"{""output"":""Hello, World!""}";

            // Save response to the event store


            return new OkObjectResult(jsonOut);
        }
    }
}
