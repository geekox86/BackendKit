using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace BackendKitTests;

public class InterfacingConfigurationTests
{
  [Fact]
  public void Test()
  {
    var appBuilder = WebApplication.CreateBuilder();
    var app = appBuilder.Build();
    using var appServer = new TestServer(app.Services);
    using var appClient = appServer.CreateClient();
  }
}
