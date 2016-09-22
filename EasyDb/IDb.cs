using System.Threading.Tasks;

namespace EasyDb
{
    public interface IDb
    {
        EasyDbConnection Connection { get; }

        void Close();
        EasyDbCommand CreateCommand(string sql);
        EasyDbCommand CreateStoredProcedureCommand(string storedProcedure);
        void Initialize(string connectionString, string provider, ConnectionStrategyType connectionStrategy = ConnectionStrategyType.Automatic);
        void InitializeWithConfigurationFile(string connectionStringName = "DefaultConnection", ConnectionStrategyType connectionStrategy = ConnectionStrategyType.Automatic);
        void Open();
        Task OpenAsync();
    }
}