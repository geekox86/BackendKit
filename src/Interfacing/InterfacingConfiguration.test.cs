using BackendKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace BackendKitTests;

public class InterfacingConfigurationTests
{
  [Test]
  public async Task Test()
  {
    var appBuilder = WebApplication.CreateBuilder();

    appBuilder.WebHost.UseTestServer();
    appBuilder.Services.AddInterfacing("BackendKitTests");

    using var app = appBuilder.Build();

    await app.StartAsync();

    using var appServer = app.GetTestServer();
    using var appClient = app.GetTestClient();

    var response = await appClient.GetAsync("/graphql");
    var result = await response.Content.ReadAsStringAsync();

    await app.StopAsync();

    await Assert.That(result).IsEqualTo("Hello World!");
  }
}
