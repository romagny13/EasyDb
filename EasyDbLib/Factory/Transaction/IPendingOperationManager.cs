using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IPendingOperationManager
    {
        IReadOnlyList<Func<Task>> PendingOperations { get; }

        void AddPendingOperation(Func<Task> operation);
        void Clear();
        Task<bool> Execute(EasyDb db);
    }
}