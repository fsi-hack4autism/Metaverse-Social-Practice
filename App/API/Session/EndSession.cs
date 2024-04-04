using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class EndSession
    {
        private readonly ILogger<EndSession> _logger;

        public EndSession(ILogger<EndSession> logger)
        {
            _logger = logger;
        }

        [Function("EndSession")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
