using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class NewSession
    {
        private readonly ILogger<NewSession> _logger;

        public NewSession(ILogger<NewSession> logger)
        {
            _logger = logger;
        }

        [Function("NewSession")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
