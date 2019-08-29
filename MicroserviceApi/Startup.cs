using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MicroserviceApi.Startup))]
namespace MicroserviceApi {
  public class Startup : FunctionsStartup {
    public override void Configure (IFunctionsHostBuilder builder) {
      builder.Services.AddSingleton ((s) => {
        var storage = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        return storage.CreateCloudTableClient();
      });
    }
  }
}