using System;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace SPA {
  public static class API {
    [FunctionName("Greeting")]
    public static string Greeting(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req, 
      ILogger log) 
    {
      var region = Environment.GetEnvironmentVariable("REGION_NAME");
      var host = Environment.GetEnvironmentVariable("WEBSITE_CURRENT_STAMPNAME");
      return string.Format("Greetings from {0} running in {1}", host, region);
    }
  }
}