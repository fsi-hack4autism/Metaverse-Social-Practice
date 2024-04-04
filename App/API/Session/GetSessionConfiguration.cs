using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerseCraft.Session
{
    public class GetSessionConfiguration
    {
        private readonly ILogger<GetSessionConfiguration> _logger;

        public GetSessionConfiguration(ILogger<GetSessionConfiguration> logger)
        {
            _logger = logger;
        }

        [Function("GetSessionConfiguration")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = @"
{
    'session': 'session-1',
    'scene': 'FastFood',
    'configuration': {
        'lighting': 'bright'
    }
}
            ";

            return new OkObjectResult(json);
        }
    }
}
