using System.Security.Claims;
using System.Security.Principal;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BackendKit;

internal static class CybersecurityConfiguration
{
  internal static IServiceCollection AddCybersecurity(
    this IServiceCollection services,
    bool isDevelopment
  )
  {
    if (isDevelopment)
    {
      services.AddCors(
        (options) =>
          options.AddDefaultPolicy(
            (policy) =>
              policy
                .WithOrigins("http://localhost:20180")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
          )
      );
    }

    return services
      .AddAuthentication()
      .Services.AddAuthorization(options =>
        options.AddPolicy(
          "Admin",
          policy =>
          {
            policy
              .RequireAuthenticatedUser()
              .RequireAssertion(context =>
              {
                ((IMiddlewareContext)context.Resource!).Result = null;
                return true;
              });
          }
        )
      )
      .AddHttpContextAccessor()
      .AddScoped(
        (services) =>
          services.GetService<IHttpContextAccessor>()!.HttpContext?.User
          ?? new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Name, "ARAMCO\\system")])
          )
      );
  }

  internal static WebApplication UseCybersecurity(
    this WebApplication app,
    bool isDevelopment
  )
  {
    if (isDevelopment)
    {
      app.UseCors();

      app.Use(
        async (context, next) =>
        {
          context.User = new ClaimsPrincipal(
            new ClaimsIdentity(
              [new Claim(ClaimTypes.Name, "ARAMCO\\testuser")],
              "Test"
            )
          );

          await next();
        }
      );
    }

    app.UseHsts().UseAuthentication().UseAuthorization();

    return app;
  }
}

internal static class IdentityExtensions
{
  internal static string GetUser(this IIdentity identity) =>
    identity.Name!["ARAMCO\\".Length..].ToUpper();
}
