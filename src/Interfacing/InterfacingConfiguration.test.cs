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
    using var appServer = app.GetTestServer();
    using var appClient = app.GetTestClient();

    Assert.True(true);
  }
}
