using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyDb
{
    public sealed class Singleton<T> where T : new()
    {
        private static ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        private Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                return _instances.GetOrAdd(typeof(T), (t) => new T());
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapAttribute : Attribute
    {
        public string ColumnName { get; set; }
    }

    public class InstanceCreator
    {
        public static InstanceCreator Default { get { return Singleton<InstanceCreator>.Instance; } }

        public virtual ConstructorInfo GetConstructorInfo(Type type)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors;
            return constructors.FirstOrDefault(c => c.Name != ".cctor");
        }

        public object CreateInstance(Type type)
        {
            var constructorInfo = GetConstructorInfo(type);
            var parameterInfos = constructorInfo.GetParameters();
            if (parameterInfos.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var parameters = new object[parameterInfos.Length];
                foreach (var parameterInfo in parameterInfos)
                {
                    parameters[parameterInfo.Position] = CreateInstance(parameterInfo.ParameterType);
                }
                return Activator.CreateInstance(type, parameters);
            }

        }
    }

    public interface IMapper
    {
        T MapDataToObject<T>(DbDataReader reader) where T : class, new();
    }

    public class Mapper : IMapper
    {
        private InstanceCreator instanceCreator = InstanceCreator.Default;

        public static Mapper Default { get { return Singleton<Mapper>.Instance; } }

        private Dictionary<string, PropertyInfo> FindProperties(Type type)
        {
            var result = new Dictionary<string, PropertyInfo>();

            var propertiesInfos = type.GetProperties().ToList<PropertyInfo>();
            // MapAttribute?
            foreach (var property in propertiesInfos)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(MapAttribute)) as MapAttribute;
                if (attribute != null)
                {
                    // add column name / property
                    var columnName = attribute.ColumnName;
                    if (string.IsNullOrEmpty(columnName)) throw new ArgumentException("No ColumnName found on MapAttribute. Property " + property.Name + " of " + type.Name);
                    result[columnName] = property;
                }
                else
                {
                    var columnName = property.Name;
                    result[columnName] = property;
                }
            }
            return result;
        }

        public T MapDataToObject<T>(DbDataReader reader)
            where T : class, new()
        {
            Type type = typeof(T);

            var properties = FindProperties(type);
            var instance = instanceCreator.CreateInstance(type);
            // parse for each column of current line
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i); // table column name id , name, email ..
                if (properties.ContainsKey(columnName))
                {
                    var property = properties[columnName];
                    var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i); // column data value
                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType; // type of property object "System.String" ...
                    var propertyValue = columnValue == null ? null : Convert.ChangeType(columnValue, propertyType); // convert column value to property value 
                    if (propertyType == typeof(string) && propertyValue != null) propertyValue = propertyValue.ToString().TrimEnd();
                    property.SetValue(instance, propertyValue); // set property value
                }
            }
            return (T)instance;
        }
    }

    public class EasyDbCommand
    {
        private EasyDbConnection connection;
        private DbCommand command;
        private IMapper mapper;

        public EasyDbCommand() : this(Mapper.Default)
        { }

        public EasyDbCommand(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public bool IsInitialized { get; private set; } = false;

        public void Inilialize(EasyDbConnection connection, CommandType commandType, string commandText)
        {
            if (!connection.IsInitialized) throw new ArgumentException("Require a connection initialized.");

            this.connection = connection;
            command = this.connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            IsInitialized = command != null
                && command.Connection != null
                && !string.IsNullOrEmpty(command.Connection.ConnectionString);
        }

        private DbParameter GetParameter(string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            return parameter;
        }

        public EasyDbCommand AddParameter(string parameterName, object value)
        {
            if (!IsInitialized) throw new ArgumentException("Cannot add parameter. Require to initialize a command.");
            var parameter = GetParameter(parameterName, value);
            command.Parameters.Add(parameter);
            return this;
        }

        public IEnumerable<T> ReadAll<T>(Func<DbDataReader, T> mapDataToObject)
        {
            var result = new List<T>();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(mapDataToObject(reader));
                }
            }
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return result;
        }

        public async Task<IEnumerable<T>> ReadAllAsync<T>(Func<DbDataReader, T> mapDataToObject)
        {
            var result = new List<T>();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    result.Add(mapDataToObject(reader));
                }
            }

            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return result;
        }

        public T ReadOne<T>(Func<DbDataReader, T> mapDataToObject)
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = mapDataToObject(reader);
                    return result;
                }
            }
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return default(T);
        }

        public async Task<T> ReadOneAsync<T>(Func<DbDataReader, T> mapDataToObject)
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var result = mapDataToObject(reader);
                    return result;
                }
            }
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return default(T);
        }

        public IEnumerable<T> ReadAllMapTo<T>()
            where T : class, new()
        {
            return ReadAll(mapper.MapDataToObject<T>);
        }

        public async Task<IEnumerable<T>> ReadAllMapToAsync<T>()
           where T : class, new()
        {
            return await ReadAllAsync(mapper.MapDataToObject<T>);
        }

        public T ReadOneMapTo<T>()
           where T : class, new()
        {
            return ReadOne(mapper.MapDataToObject<T>);
        }

        public async Task<T> ReadOneMapToAsync<T>()
           where T : class, new()
        {
            return await ReadOneAsync(mapper.MapDataToObject<T>);
        }

        public int NonQuery()
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Open();
            int response = command.ExecuteNonQuery();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return response;
        }

        public async Task<int> NonQueryAsync()
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) await connection.OpenAsync();
            int response = await command.ExecuteNonQueryAsync();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return response;
        }

        public object Scalar()
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Open();
            object result = command.ExecuteScalar();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return result;
        }

        public async Task<object> ScalarAsync()
        {
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) await connection.OpenAsync();
            object result = await command.ExecuteScalarAsync();
            if (connection.ConnectionStrategy == ConnectionStrategyType.Automatic) connection.Close();
            return result;
        }
    }

    public class EasyDbConnection
    {
        public bool IsInitialized { get; private set; } = false;

        private DbConnection connection;
        public string ConnectionString { get; private set; }
        public string Provider { get; private set; }
        public ConnectionStrategyType ConnectionStrategy { get; private set; }
        public ConnectionState State { get { return connection.State; } }

        public void Initialize(string connectionString, string provider, ConnectionStrategyType connectionStrategy)
        {
            connection = DbProviderFactories.GetFactory(provider).CreateConnection();
            connection.ConnectionString = connectionString;

            if (connection == null) throw new ArgumentException("Cannot create a connection for provider : " + provider);

            ConnectionString = connectionString;
            Provider = provider;

            ConnectionStrategy = connectionStrategy;

            IsInitialized = connection != null && !string.IsNullOrEmpty(connection.ConnectionString);
        }

        public DbCommand CreateCommand()
        {
            if (!IsInitialized) throw new ArgumentException("Cannot create a command. Require to initialize a connection with connection string and provider before.");

            var command = connection.CreateCommand();
            return command;
        }

        public void Open()
        {
            if (!IsInitialized) throw new ArgumentException("Cannot open a connection. Require to initialize a connection with connection string and provider before.");

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task OpenAsync()
        {
            if (!IsInitialized) throw new ArgumentException("Cannot open a connection. Require to initialize a connection with connection string and provider before.");

            try
            {
                if (connection.State != ConnectionState.Open) await connection.OpenAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Close()
        {
            try
            {
                if (connection != null) connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public enum ConnectionStrategyType
    {
        Manual,
        Automatic
    }

    public class Db : IDb
    {
        public static Db Default { get { return Singleton<Db>.Instance; } }
        public EasyDbConnection Connection { get; private set; }

        public void Initialize(string connectionString, string provider, ConnectionStrategyType connectionStrategy = ConnectionStrategyType.Automatic)
        {
            // connection for provider, connection string, strategy
            Connection = new EasyDbConnection();
            Connection.Initialize(connectionString, provider, connectionStrategy);
        }

        public void InitializeWithConfigurationFile(string connectionStringName = "DefaultConnection", ConnectionStrategyType connectionStrategy = ConnectionStrategyType.Automatic)
        {
            var section = ConfigurationManager.ConnectionStrings[connectionStringName];
            if(section != null)
            {
                Connection = new EasyDbConnection();
                Initialize(section.ConnectionString, section.ProviderName, connectionStrategy);
            }
            else
            {
                throw new ArgumentNullException("No connection string " + connectionStringName + " found in configuration file");
            }
        }

        public void Open()
        {
            Connection.Open();
        }

        public async Task OpenAsync()
        {
            await Connection.OpenAsync();
        }

        public void Close()
        {
            Connection.Close();
        }

        public EasyDbCommand CreateCommand(string sql)
        {
            var command = new EasyDbCommand();
            command.Inilialize(Connection,CommandType.Text, sql);

            return command;
        }

        public EasyDbCommand CreateStoredProcedureCommand(string storedProcedure)
        {
            var command = new EasyDbCommand();
            command.Inilialize(Connection, CommandType.StoredProcedure, storedProcedure);

            return command;
        }

    }

}
