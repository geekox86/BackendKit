// using System.Collections.Immutable;
// using System.Data;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Abstraction;
// using Dapper;

// namespace Infrastructure;

// public abstract class TransactionalRepository(UnitOfWork unitOfWork)
// {
//   public void Begin(IsolationLevel isolation = IsolationLevel.RepeatableRead) =>
//     _unitOfWork.Begin(isolation);

//   public void Commit() => _unitOfWork.Commit();

//   public void Rollback() => _unitOfWork.Rollback();

//   protected readonly UnitOfWork _unitOfWork = unitOfWork;

//   protected static string GetTable(object data, string suffix = "") =>
//     GetTable(data.GetType(), suffix);

//   protected static string GetTable<TData>(string suffix = "") =>
//     GetTable(typeof(TData), suffix);

//   private static string GetTable(Type dataType, string suffix = "")
//   {
//     var namespaces = dataType.Namespace!.Split(".");

//     return $"\"{namespaces[0]}\".\"{namespaces[1]}{suffix}\"";
//   }

//   protected static string ToJson(object data) =>
//     JsonSerializer.Serialize(data, _jsonOptions);

//   protected static TData FromJson<TData>(string jsonData) =>
//     JsonSerializer.Deserialize<TData>(jsonData, _jsonOptions)!;

//   private static readonly JsonSerializerOptions _jsonOptions =
//     new JsonSerializerOptions
//     {
//       WriteIndented = true,
//       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//       Converters = { new JsonStringEnumConverter() }
//     };
// }

// // TODO Filter all data by the type's version to enable coexistent versions
// // TODO Append messages to a table if configured for logging
// public class DefaultRepository(UnitOfWork unitOfWork)
//   : TransactionalRepository(unitOfWork)
// {
//   public async Task Create(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   )
//   {
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       INSERT INTO {GetTable(state)}
//       VALUES (@Data, @Meta);
//       """,
//       new
//       {
//         Data = ToJson(state),
//         Meta = ToJson(new StateMeta(at, by, causeMessage, events, state))
//       },
//       _unitOfWork.Transaction!
//     );

//     await Append(at, by, causeMessage, events, state);
//   }

//   public async Task CreateOrUpdate(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   )
//   {
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       MERGE INTO {GetTable(state)} AS "target"
//       USING (SELECT @Data AS "data") AS "source"
//       ON JSON_VALUE("source"."data", N'$.id') = JSON_VALUE("target"."data", N'$.id')
//       WHEN NOT MATCHED THEN
//         INSERT VALUES (@Data, @Meta)
//       WHEN MATCHED THEN
//         UPDATE SET "data" = @Data;
//       """,
//       new
//       {
//         Data = ToJson(state),
//         Meta = ToJson(new StateMeta(at, by, causeMessage, events, state))
//       },
//       _unitOfWork.Transaction!
//     );

//     await Append(at, by, causeMessage, events, state);
//   }

//   public async Task Update(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   )
//   {
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       UPDATE {GetTable(state)}
//       SET
//         "data" = @Data,
//         "meta" = @Meta
//       WHERE JSON_VALUE("data", N'$.id') = @Id;
//       """,
//       new
//       {
//         state.Id,
//         Data = ToJson(state),
//         Meta = ToJson(new StateMeta(at, by, causeMessage, events, state))
//       },
//       _unitOfWork.Transaction!
//     );

//     await Append(at, by, causeMessage, events, state);
//   }

//   public async Task Delete(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   )
//   {
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       DELETE {GetTable(state)}
//       WHERE JSON_VALUE("data", N'$.id') = @Id;
//       """,
//       new { state.Id },
//       _unitOfWork.Transaction!
//     );

//     await Append(at, by, causeMessage, events, state);
//   }

//   protected async Task Append(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   ) =>
//     await _unitOfWork.Connection.ExecuteAsync(
//       $"""
//       INSERT INTO {GetTable(events.First(), "Event")}
//       VALUES (@Data, @Meta);
//       """,
//       events.Select(@event => new
//       {
//         Data = ToJson(@event),
//         Meta = ToJson(new EventMeta(at, by, causeMessage, @event, state))
//       }),
//       _unitOfWork.Transaction!
//     );
// }

// internal sealed record StateMeta(
//   DateTimeOffset At,
//   string? By,
//   string CauseMessageType,
//   Guid CauseMessageId,
//   ImmutableList<string> EventTypes,
//   ImmutableList<Guid> EventIds,
//   string StateType,
//   Guid StateId
// )
// {
//   internal StateMeta(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEnumerable<IEvent> events,
//     IState state
//   )
//     : this(
//       at,
//       by,
//       causeMessage.GetType().FullName!,
//       causeMessage.Id,
//       [.. events.Select(@event => @event.GetType().FullName!)],
//       [.. events.Select(@event => @event.Id)],
//       state.GetType().FullName!,
//       state.Id
//     ) { }
// }

// internal sealed record EventMeta(
//   DateTimeOffset At,
//   string? By,
//   string CauseMessageType,
//   Guid CauseMessageId,
//   string EventType,
//   Guid EventId,
//   string StateType,
//   Guid StateId
// )
// {
//   internal EventMeta(
//     DateTimeOffset at,
//     string? by,
//     IMessage causeMessage,
//     IEvent @event,
//     IState state
//   )
//     : this(
//       at,
//       by,
//       causeMessage.GetType().FullName!,
//       causeMessage.Id,
//       @event.GetType().FullName!,
//       @event.Id,
//       state.GetType().FullName!,
//       state.Id
//     ) { }
// }
