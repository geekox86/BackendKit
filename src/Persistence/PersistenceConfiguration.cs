// using System.Reflection;
// using Microsoft.Extensions.DependencyInjection;

// namespace BackendKit;

// internal static class PersistenceConfiguration
// {
//   internal static IServiceCollection AddPersistence(
//     this IServiceCollection services,
//     string appDatabase
//   )
//   {
//     services.AddScoped<UnitOfWork>((_) => new(appDatabase));

//     Assembly
//       .GetEntryAssembly()!
//       .GetTypes()
//       .Where(type =>
//         type.BaseType == typeof(DefaultRepository)
//         || type.BaseType == typeof(TransactionalRepository)
//       )
//       .Select(services.AddScoped)
//       .ToList();

//     return services;
//   }
// }
