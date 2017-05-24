using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class ManyRelation<T> : IRelation
    {
        protected IQueryService queryService;
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;
        protected string propertyToFill;
        protected Table modelMapping;
        protected Table relationMapping;

        public ManyRelation(
            IQueryService queryService, 
            IModelResolver modelResolver, 
            EasyDb easyDbInstance, 
            string propertyToFill, 
            Table modelMapping,
            Table relationMapping)
        {
            this.queryService = queryService;
            this.modelResolver = modelResolver;
            this.easyDbInstance = easyDbInstance;
            this.propertyToFill = propertyToFill;
            this.modelMapping = modelMapping;
            this.relationMapping = relationMapping;
        }


        public ForeignKeyColumn[] GetForeignKeys()
        {
            return this.relationMapping.GetForeignKeys(this.modelMapping.TableName);
        }

        public string GetQuery(ForeignKeyColumn[] foreignKeys)
        {
            return this.queryService.GetSelect(this.relationMapping)
                   + this.queryService.GetFrom(this.relationMapping.TableName)
                   + this.queryService.GetWhereHasMany(foreignKeys);
        }

        public EasyDbCommand CreateCommand(object model)
        {
            var foreignKeys = this.GetForeignKeys();
            if (foreignKeys.Length == 0) { throw new Exception("No foreign key defined in \"" + this.modelMapping.TableName + "\" for \"" + this.relationMapping.TableName + "\" (mapping)"); }

            // get query
            var query = this.GetQuery(foreignKeys);

            // create command
            var command = this.easyDbInstance.CreateCommand(query);

            // add parameters
            var type = model.GetType();
            var primaryKeys = modelMapping.GetPrimaryKeys();
            for (int i = 0; i < foreignKeys.Length; i++)
            {
                var foreignKey = foreignKeys[i];
                var parameterName = this.queryService.GetParameterName(foreignKey.ColumnName);
                var property = this.modelResolver.GetPropertyInfo(type, primaryKeys[i].PropertyName);
                if (property == null) { throw new Exception("No property found for " + primaryKeys[i].PropertyName + " in " + type.Name); }

                var value = this.modelResolver.GetValue(model, property);
                if (foreignKey.DbType.HasValue)
                {
                    command.AddParameter(parameterName, value, foreignKey.DbType.Value);
                }
                else
                {
                    command.AddParameter(parameterName, value);
                }
            }

            return command;
        }

        public async Task Fetch(object model)
        {
            var command = this.CreateCommand(model);
            var relationValue = await command.ReadAllAsync<T>(this.relationMapping);

            // resolve property to fill
            var propertyInfo = this.modelResolver.GetPropertyInfo(model.GetType(), this.propertyToFill);
            if (propertyInfo == null) { throw new Exception("No property found for the relation many with the property to fill " + this.propertyToFill + " in " + model.GetType().Name); }

            this.modelResolver.CheckAndSetValue(model, this.propertyToFill, propertyInfo, relationValue);
        }
    }

}
