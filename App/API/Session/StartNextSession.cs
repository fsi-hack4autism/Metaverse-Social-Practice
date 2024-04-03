using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class StartNextSession
    {
        private readonly ILogger<StartNextSession> _logger;

        public StartNextSession(ILogger<StartNextSession> logger)
        {
            _logger = logger;
        }

        [Function("StartNextSession")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var userId = req.Query["userId"];
            var requesterId = req.Query["requesterId"];

            
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
