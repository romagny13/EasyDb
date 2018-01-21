using System;
using System.Threading.Tasks;
using System.Transactions;

namespace EasyDbLib
{
    public abstract class TransactionFactory
    {
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
                    await ExecuteAsync(db);

                    scope.Complete();

                    return true;
                }
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

        public abstract Task ExecuteAsync(EasyDb db);
    }

}
