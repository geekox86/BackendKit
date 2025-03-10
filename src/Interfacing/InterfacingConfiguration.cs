using HotChocolate.AspNetCore;
using HotChocolate.Types.Descriptors;
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
      // .AddQueryType()
      // .AddMutationType()
      .AddUploadType()
      .TryAddTypeInterceptor<UnionTypeInterceptor>()
      .AddConvention<INamingConventions>((_) => new NamingConventions(appName))
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
