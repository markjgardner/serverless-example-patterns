using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MicroserviceApi {
  public class MessageEntity : TableEntity {
    public string Message { get; set; }
  }
  public class WebApi {
    private const string tableName = "functiontable";
    private readonly CloudTableClient _tableClient;
    public WebApi (CloudTableClient tableClient) {
      _tableClient = tableClient;
    }

    [FunctionName ("AddMessage")]
    [return :Table (tableName)]
    public MessageEntity putMessage (
      [HttpTrigger ("put", Route = "messages")] HttpRequest req,
      ILogger log) 
    {
      string message = req.Query["message"];
      log.LogInformation ($"Recorded message: {message}");
      return new MessageEntity {
        PartitionKey = DateTime.Now.DayOfWeek.ToString (),
          RowKey = DateTime.Now.Ticks.ToString (),
          Message = message
      };
    }

    [FunctionName ("ReadMessage")]
    public IEnumerable<MessageEntity> getMessages (
      [HttpTrigger ("get", Route = "messages")] HttpRequest req, 
      ILogger log) 
    {
      var table = _tableClient.GetTableReference (tableName);
      var partition = req.Query["partition"];
      var query = new TableQuery<MessageEntity> ().Where ($"PartitionKey eq '{partition}'");
      return table.ExecuteQuery (query);
    }
  }
}