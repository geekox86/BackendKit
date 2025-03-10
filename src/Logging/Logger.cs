// using Abstraction;
// using Dapper;
// using IError = Abstraction.IError;

// namespace Infrastructure;

// public sealed class Logger(Repository repository)
// {
//   public async Task Log(
//     DateTimeOffset at,
//     string by,
//     IMessage causeMessage,
//     IEnumerable<IError> errors,
//     IState? state = default
//   ) => await repository.Append(at, by, causeMessage, errors, state);
// }

// public sealed class Repository(UnitOfWork unitOfWork)
//   : TransactionalRepository(unitOfWork)
// {
//   public async Task Append(
//     DateTimeOffset at,
//     string by,
//     IMessage causeMessage,
//     IEnumerable<IError> errors,
//     IState? state
//   ) =>
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       INSERT INTO {GetTable(errors.First(), "Error")}
//       VALUES (@Data, @Meta);
//       """,
//       errors.Select(error => new
//       {
//         Data = ToJson(error),
//         Meta = ToJson(new ErrorMeta(at, by, causeMessage, error, state))
//       }),
//       _unitOfWork.Transaction!
//     );
// }

// internal sealed record ErrorMeta(
//   DateTimeOffset At,
//   string? By,
//   string CauseMessageType,
//   Guid CauseMessageId,
//   string ErrorType,
//   Guid ErrorId,
//   string? StateType,
//   Guid? StateId
// )
// {
//   internal ErrorMeta(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IError error,
//     IState? state
//   )
//     : this(
//       at,
//       by,
//       causeMessage.GetType().FullName!,
//       causeMessage.Id,
//       error.GetType().FullName!,
//       error.Id,
//       state?.GetType().FullName!,
//       state?.Id
//     ) { }
// }
