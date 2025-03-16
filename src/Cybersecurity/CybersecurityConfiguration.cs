// using System.Security.Claims;
// using System.Security.Principal;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.DependencyInjection;

// namespace BackendKit;

// internal static class CybersecurityConfiguration
// {
//   internal static IServiceCollection AddCybersecurity(
//     this IServiceCollection services,
//     bool isDevelopment
//   )
//   {
//     if (isDevelopment)
//     {
//       services.AddCors(
//         (options) =>
//           options.AddDefaultPolicy(
//             (policy) =>
//               policy
//                 .WithOrigins("http://localhost:20180")
//                 .AllowAnyMethod()
//                 .AllowAnyHeader()
//                 .AllowCredentials()
//           )
//       );
//     }

//     return services
//       .AddAuthentication()
//       .Services.AddAuthorization()
//       .AddHttpContextAccessor()
//       .AddScoped(
//         (services) =>
//           services.GetService<IHttpContextAccessor>()!.HttpContext?.User
//           ?? new ClaimsPrincipal(
//             new ClaimsIdentity([new Claim(ClaimTypes.Name, "ARAMCO\\system")])
//           ) // TODO Custom system user?
//       );
//   }

//   internal static WebApplication UseCybersecurity(
//     this WebApplication app,
//     bool isDevelopment
//   )
//   {
//     app.UseHsts().UseHttpsRedirection();

//     if (isDevelopment)
//     {
//       app.UseCors();

//       app.Use(
//         async (context, next) =>
//         {
//           context.User = new ClaimsPrincipal(
//             new ClaimsIdentity(
//               [new Claim(ClaimTypes.Name, "ARAMCO\\testuser")],
//               "Test"
//             )
//           ); // TODO Custom test user?

//           await next();
//         }
//       );
//     }

//     app.UseAuthentication().UseAuthorization();

//     return app;
//   }
// }

// internal static class IdentityExtensions
// {
//   internal static string GetUser(this IIdentity identity) =>
//     identity.Name!["ARAMCO\\".Length..].ToUpper(); // TODO Custom domain?
// }
