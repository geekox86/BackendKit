// using System.Data;
// using Microsoft.Data.SqlClient;
// using Rebus.Config.Outbox;
// using Rebus.Pipeline;
// using Rebus.SqlServer.Outbox;
// using Rebus.Transport;

// namespace Infrastructure;

// public sealed class UnitOfWork : IDisposable
// {
//   public UnitOfWork(string appDatabase)
//   {
//     Outbox = (OutboxConnection?)
//       MessageContext
//         .Current
//         ?.TransactionContext
//         .Items["current-outbox-connection"];

//     if (Outbox is not null)
//     {
//       Connection = Outbox.Connection;
//       Transaction = Outbox.Transaction;
//     }
//     else
//     {
//       Connection = new SqlConnection(appDatabase);
//       Connection.Open();
//     }
//   }

//   public void Begin(IsolationLevel isolation)
//   {
//     if (Outbox is not null)
//       throw new InvalidOperationException(
//         "Cannot begin a transaction managed by the bus outbox!"
//       );

//     Transaction = Connection.BeginTransaction(isolation);

//     OutboxScope = new();
//     OutboxScope.UseOutbox(Connection, Transaction);
//   }

//   public void Commit()
//   {
//     if (Outbox is not null)
//       throw new InvalidOperationException(
//         "Cannot commit a transaction managed by the bus outbox!"
//       );

//     OutboxScope!.Complete();
//     Transaction!.Commit();
//   }

//   public void Rollback()
//   {
//     if (Outbox is not null)
//       throw new InvalidOperationException(
//         "Cannot rollback a transaction managed by the bus outbox!"
//       );

//     Transaction!.Rollback();
//   }

//   public void Dispose()
//   {
//     if (Outbox is null)
//     {
//       OutboxScope?.Dispose();
//       Transaction?.Dispose();
//       Connection.Dispose();
//     }
//   }

//   public OutboxConnection? Outbox { get; init; }
//   public RebusTransactionScope? OutboxScope { get; private set; }
//   public SqlConnection Connection { get; init; }
//   public SqlTransaction? Transaction { get; private set; }
// }
