using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class CreateSessionConfiguration
    {
        private readonly ILogger<CreateSessionConfiguration> _logger;

        public CreateSessionConfiguration(ILogger<CreateSessionConfiguration> logger)
        {
            _logger = logger;
        }

        [Function("CreateSessionConfiguration")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult("Done");
        }
    }
}
