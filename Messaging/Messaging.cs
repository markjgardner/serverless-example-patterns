using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Messaging {
  public static class Messaging {

    [FunctionName ("EntryPoint")]
    [return :ServiceBus ("functiontop", Connection = "AzureWebJobsServiceBus")]
    public static string publish (
      [HttpTrigger ("put")] HttpRequest req,
      ILogger log) 
    {
      string message = req.Query["message"];
      log.LogInformation ($"Published message: {message}");
      return message;
    }

    [FunctionName ("SubscriptionTrigger")]
    [return :Queue ("functionqueue")]
    public static string subscribe (
      [ServiceBusTrigger ("functiontop", "functionsub", Connection = "AzureWebJobsServiceBus")] string mySbMsg,
      ILogger log) 
    {
      log.LogInformation ($"Queued message: {mySbMsg}");
      return mySbMsg;
    }

    [FunctionName ("QueueTrigger")]
    [return :CosmosDB ("functiondb", "mycollection", ConnectionStringSetting = "CosmosDBConnection")]
    public static dynamic dequeue (
      [QueueTrigger ("functionqueue")] string myQueueItem,
      ILogger log) 
    {
      log.LogInformation ($"Dequeued message: {myQueueItem}");
      return new { Id = Guid.NewGuid ().ToString (), Text = myQueueItem };
    }
  }
}