using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace EasyDbLib
{
    public class PendingOperationManager : IPendingOperationManager
    {
        protected List<Func<Task>> pendingOperations;
        public IReadOnlyList<Func<Task>> PendingOperations => pendingOperations;

        public PendingOperationManager()
        {
            this.pendingOperations = new List<Func<Task>>();
        }

        public void AddPendingOperation(Func<Task> operation)
        {
            this.pendingOperations.Add(operation);
        }

        public void Clear()
        {
            this.pendingOperations.Clear();
        }

        public async Task<bool> Execute(EasyDb db)
        {
            var currentConnectionStrategy = db.WrappedConnection.ConnectionStrategy;

            if (currentConnectionStrategy == ConnectionStrategy.Auto)
            {
                db.SetConnectionStrategy(ConnectionStrategy.Manual);
            }

            try
            {
                await db.WrappedConnection.OpenAsync();

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var operation in this.pendingOperations)
                    {
                        await operation();
                    }

                    scope.Complete();

                    this.pendingOperations.Clear();
                }

                return true;
            }
            catch (Exception)
            { }
            finally
            {
                db.WrappedConnection.Close();

                // reset connection strategy
                if (currentConnectionStrategy == ConnectionStrategy.Auto)
                {
                    db.SetConnectionStrategy(ConnectionStrategy.Auto);
                }
            }

            return false;
        }

    }

}
