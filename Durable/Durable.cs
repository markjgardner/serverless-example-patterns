using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Durable {
  public class Orchestration {

    private readonly Random _random;

    public Orchestration(Random random) {
      this._random = random;
    }

    [FunctionName ("Orchestrator")]
    public async Task<List<string>> RunOrchestrator (
      [OrchestrationTrigger] DurableOrchestrationContext context) 
    {
      var inputs = context.GetInput<IEnumerable<string>>();
      var outputs = new List<string>();

      foreach (var i in inputs) {
        outputs.Add(await context.CallActivityAsync<string> ("ActivityFunction", i));
      }

      return outputs;
    }

    [FunctionName ("ActivityFunction")]
    public string SayHello (
      [ActivityTrigger] string name, 
      ILogger log) 
    {
      log.LogInformation ($"Saying hello to {name}.");
      var delay = Convert.ToInt32(Math.Floor(_random.NextDouble() * (double)15000));
      Thread.Sleep(delay);
      return $"Hello {name}!";
    }

    [FunctionName ("EntryPoint")]
    public async Task<HttpResponseMessage> HttpStart (
      [HttpTrigger (AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, 
      [OrchestrationClient]DurableOrchestrationClient starter,
      ILogger log) 
    {
      var values = await req.Content.ReadAsAsync<IEnumerable<string>>();

      string instanceId = await starter.StartNewAsync ("Orchestrator", values);

      log.LogInformation ($"Started orchestration with ID = '{instanceId}'.");

      return starter.CreateCheckStatusResponse(req, instanceId);
    }
  }
}