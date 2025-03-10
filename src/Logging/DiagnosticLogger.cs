using System.Globalization;
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Execution.Processing;
using HotChocolate.Resolvers;
using Microsoft.Extensions.Logging;

namespace BackendKit;

internal sealed class DiagnosticLogger(ILogger<DiagnosticLogger> logger)
  : ExecutionDiagnosticEventListener
{
  public override void RequestError(
    IRequestContext context,
    Exception exception
  ) =>
    Log(
      exception,
      """
      GraphQL request error at {timestamp} by {user}!

      {query}

      {variables}
      """,
      context.Request.Document?.ToString() ?? "NO QUERY",
      context.Variables?.ToString() ?? "NO VARIABLES"
    );

  public override void SyntaxError(IRequestContext context, IError error) =>
    Log(
      error.Exception,
      """
      GraphQL syntax error at {timestamp} by {user}!

      {message}

      {path}

      {query}

      {variables}
      """,
      error.Message,
      error.Path?.ToString() ?? "NO PATH",
      context.Request.Document?.ToString() ?? "NO QUERY",
      context.Variables?.ToString() ?? "NO VARIABLES"
    );

  public override void ValidationErrors(
    IRequestContext context,
    IReadOnlyList<IError> errors
  )
  {
    foreach (var error in errors)
    {
      Log(
        error.Exception,
        """
        GraphQL validation error at {timestamp} by {user}!

        {message}

        {path}

        {query}

        {variables}
        """,
        error.Message,
        error.Path?.ToString() ?? "NO PATH",
        context.Request.Document?.ToString() ?? "NO QUERY",
        context.Variables?.ToString() ?? "NO VARIABLES"
      );
    }
  }

  public override void ResolverError(
    IMiddlewareContext context,
    IError error
  ) =>
    Log(
      error.Exception,
      """
      GraphQL middleware resolver error at {timestamp} by {user}!

      {message}

      {path}
      """,
      error.Message,
      error.Path?.ToString() ?? context.Path.ToString()
    );

  public override void ResolverError(
    IRequestContext context,
    ISelection _selection,
    IError error
  ) =>
    Log(
      error.Exception,
      """
      GraphQL resolver error at {timestamp} by {user}!

      {message}

      {path}

      {query}

      {variables}
      """,
      error.Message,
      error.Path?.ToString() ?? "NO PATH",
      context.Request.Document?.ToString() ?? "NO QUERY",
      context.Variables?.ToString() ?? "NO VARIABLES"
    );

  public override void TaskError(IExecutionTask task, IError error) =>
    Log(
      error.Exception,
      """
      GraphQL task error at {timestamp} by {user}!

      {message}

      {path}

      {task}
      """,
      error.Message,
      error.Path?.ToString() ?? "NO PATH",
      task.Kind.ToString()
    );

  private void Log(
    Exception? exception,
    string message,
    params string[] context
  ) =>
    logger.LogError(
      exception,
      message,
      [_at.ToString("O", CultureInfo.InvariantCulture), _by, .. context]
    );

  private readonly DateTimeOffset _at = DateTimeOffset.Now;
  private readonly string _by = "ANONYMOUS";
}
