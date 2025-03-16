using System.Reflection;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace BackendKit;

internal static class TypeDiscoveryExtensions
{
  public static IRequestExecutorBuilder AddDiscoveredTypes(
    this IRequestExecutorBuilder builder
  )
  {
    Assembly
      .GetEntryAssembly()!
      .GetTypes()
      .Where(static type =>
        type.CustomAttributes.Any(attr =>
          attr.AttributeType == typeof(ExtendObjectTypeAttribute)
        )
      )
      .ToList()
      .ForEach(type => builder.AddType(type));

    return builder;
  }
}
