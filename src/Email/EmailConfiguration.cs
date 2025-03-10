using Microsoft.Extensions.DependencyInjection;

namespace BackendKit;

internal static class EmailSenderConfiguration
{
  internal static IServiceCollection AddEmail(
    this IServiceCollection services,
    EmailSettings settings
  ) => services.AddTransient((_) => new EmailSender(settings));
}
