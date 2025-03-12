using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BackendKit;

public static class BackendKit
{
  public static void Run()
  {
    var appBuilder = WebApplication.CreateBuilder();
    var appConfig = appBuilder.Configuration;
    var isDev = appBuilder.Environment.IsDevelopment();

    var appName = appConfig.GetValue<string>("Application")!;
    var dbConnStr = appConfig.GetConnectionString("Database")!;
    var emailSettings = appConfig
      .GetRequiredSection("Email")
      .Get<EmailSettings>()!;

    appBuilder
      .Services.AddCybersecurity(isDev)
      .AddLogging()
      .AddPersistence(dbConnStr)
      .AddEmail(emailSettings)
      .AddInterfacing(appName);

    var app = appBuilder.Build();

    app.UseCybersecurity(isDev).UseInterfacing().Run();
  }
}
