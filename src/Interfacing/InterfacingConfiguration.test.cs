using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

// using Microsoft.AspNetCore.TestHost;

namespace BackendKitTests;

public class InterfacingConfigurationTests
{
  [Test]
  public async Task Test()
  {
    var appBuilder = new WebHostBuilder();
    using var appServer = new TestServer(appBuilder);
    using var appClient = appServer.CreateClient();

    await Assert.That(true).IsTrue();
  }
}
