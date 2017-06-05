using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class EasyDbCommand
    {
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;

        protected DbCommand command;
        public DbCommand Command => this.command;

        public EasyDbCommand(EasyDb easyDbInstance, DbCommand command, string commandText, CommandType commandType) :
            this(new ModelResolver(), easyDbInstance, command, commandText, commandType)
        { }

        public EasyDbCommand(
            IModelResolver modelResolver, 
            EasyDb easyDbInstance, 
            DbCommand command, 
            string commandText, 
            CommandType commandType)
        {
            this.modelResolver = modelResolver;
            this.easyDbInstance = easyDbInstance;
            command.CommandType = commandType;
            command.CommandText = commandText;
            this.command = command;
        }

        protected DbParameter CreateParameter(string name, object value, DbType? dbType, ParameterDirection? direction)
        {
            var parameter = this.command.CreateParameter();
            parameter.ParameterName = name;
            if (dbType.HasValue)
            {
                parameter.DbType = dbType.Value;
            }
            if (direction.HasValue)
            {
                parameter.Direction = direction.Value;
            }
            parameter.Value = value ?? DBNull.Value;

            return parameter;
        }

        public EasyDbCommand AddParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            var parameter = this.CreateParameter(name, value, dbType, direction);
            this.command.Parameters.Add(parameter);

            return this;
        }

        public EasyDbCommand AddParameter(string name, object value, ParameterDirection direction)
        {
            var parameter = this.CreateParameter(name, value, null, direction);
            this.command.Parameters.Add(parameter);

            return this;
        }

        public EasyDbCommand AddParameter(string name, object value, DbType dbType)
        {
            var parameter = this.CreateParameter(name, value, dbType, null);
            this.command.Parameters.Add(parameter);

            return this;
        }

        public EasyDbCommand AddParameter(string name, object value)
        {
            var parameter = this.CreateParameter(name, value, null, null);
            this.command.Parameters.Add(parameter);

            return this;
        }

        public EasyDbCommand AddOutputParameter(string name)
        {
            var parameter = this.CreateParameter(name, null, null, ParameterDirection.Output);
            this.command.Parameters.Add(parameter);

            return this;
        }

        public bool HasParameter(string parameterName)
        {
            return this.Command.Parameters.Contains(parameterName);
        }

        public DbParameter GetParameter(string parameterName)
        {
            if (!this.HasParameter(parameterName)) { throw new Exception("No parameter " + parameterName + " registered"); }

            return this.Command.Parameters[parameterName];
        }

        protected async Task<EasyDbCommand> OpenAsync()
        {
            await this.easyDbInstance.OpenAsync();
            return this;
        }

        protected void Close()
        {
            this.easyDbInstance.Close();
        }

        protected async Task CheckStrategyAndOpenAsync()
        {
            if (easyDbInstance.ConnectionStrategy == ConnectionStrategy.Default)
            {
                await this.OpenAsync();
            }
        }

        protected void CheckStrategyAndClose()
        {
            if (easyDbInstance.ConnectionStrategy == ConnectionStrategy.Default)
            {
                this.Close();
            }
        }

        public async Task<List<T>> ReadAllAsync<T>(Table mapping = null)
        {
            var result = new List<T>();

            try
            {
                await this.CheckStrategyAndOpenAsync();

                using (this.command)
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var readerContainer = new ReaderContainer(reader);
                            var model = (T)this.modelResolver.Resolve(typeof(T), readerContainer, mapping);
                            result.Add(model);
                        }
                    }
                }

                this.CheckStrategyAndClose();
            }
            catch (Exception e)
            {
                this.easyDbInstance.HandleException(e, When.ReadAll, this.command);
            }

            return result;
        }

        public async Task<T> ReadOneAsync<T>(Table mapping = null)
        {
            T model = default(T);

            try
            {
                await this.CheckStrategyAndOpenAsync();

                using (this.command)
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var readerContainer = new ReaderContainer(reader);
                            model = (T)this.modelResolver.Resolve(typeof(T), readerContainer, mapping);
                        }
                    }
                }

                this.CheckStrategyAndClose();
            }
            catch (Exception e)
            {
                this.easyDbInstance.HandleException(e, When.ReadOne, this.command);
            }

            return model;
        }

        public async Task<int> NonQueryAsync()
        {
            int affectedRows = 0;

            try
            {
                await this.CheckStrategyAndOpenAsync();

                using (this.command)
                {
                    affectedRows = await this.command.ExecuteNonQueryAsync();
                }

                this.CheckStrategyAndClose();
            }
            catch (Exception e)
            {
                this.easyDbInstance.HandleException(e, When.NonQuery, this.command);
            }

            return affectedRows;
        }

        public async Task<object> ScalarAsync()
        {
            object result = null;

            try
            {
                await this.CheckStrategyAndOpenAsync();

                using (this.command)
                {
                    result = await this.command.ExecuteScalarAsync();
                }

                this.CheckStrategyAndClose();
            }
            catch (Exception e)
            {
                this.easyDbInstance.HandleException(e, When.Scalar, this.command);
            }

            return result;
        }

    }
}