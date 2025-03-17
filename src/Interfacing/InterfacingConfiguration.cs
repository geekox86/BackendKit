using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BackendKit;

internal static class InterfacingConfiguration
{
  internal static IServiceCollection AddInterfacing(
    this IServiceCollection services,
    string appName
  )
  {
    services
      .AddGraphQLServer()
      // .AddDiagnosticEventListener<DiagnosticLogger>()
      .AddAuthorization()
      // .TryAddTypeInterceptor<UnionTypeInterceptor>()
      // .AddConvention<INamingConventions>((_) => new NamingConventions(appName))
      .AddQueryType<RootType>()
      .AddMutationType<RootType>()
      .AddUploadType()
      .AddDiscoveredTypes()
      .InitializeOnStartup();

    return services;
  }

  internal static WebApplication UseInterfacing(this WebApplication app)
  {
    app.MapGraphQL()
      .RequireAuthorization()
      .WithOptions(
        new GraphQLServerOptions()
        {
          Tool = { ServeMode = GraphQLToolServeMode.Embedded },
        }
      );

    return app;
  }
}
