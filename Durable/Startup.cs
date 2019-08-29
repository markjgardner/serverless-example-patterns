using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Durable.Startup))]
namespace Durable {
  public class Startup : FunctionsStartup {
    public override void Configure (IFunctionsHostBuilder builder) {
      builder.Services.AddSingleton ((s) => {
        return new Random();
      });
    }
  }
}