using Microsoft.AspNetCore.Builder;

// using Microsoft.AspNetCore.TestHost;

namespace BackendKitTests;

public class InterfacingConfigurationTests
{
  [Test]
  public async Task Test()
  {
    var appBuilder = WebApplication.CreateBuilder();
    var app = appBuilder.Build();
    // using var appServer = app.GetTestServer();
    // using var appClient = app.GetTestClient();

    await Assert.That(true).IsTrue();
  }
}
