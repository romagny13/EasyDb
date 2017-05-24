using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class OneRelation<T> : IRelation
    {
        protected IQueryService queryService;
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;
        protected string propertyToFill;
        protected Table modelMapping;
        protected Table relationMapping;

        public OneRelation(
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
           return  this.modelMapping.GetForeignKeys(this.relationMapping.TableName);
        }

        public string GetQuery(ForeignKeyColumn[] foreignKeys)
        {
            // select * from table where pk = @fk and pk2=@fk2
            return this.queryService.GetSelect(this.relationMapping)
                   + this.queryService.GetFrom(this.relationMapping.TableName)
                   + this.queryService.GetWhereHasOne(foreignKeys);
        }

        public EasyDbCommand CreateCommand(object model)
        {
            var foreignKeys = this.GetForeignKeys();
            if (foreignKeys.Length == 0) { throw new Exception("No foreign key defined in \"" + this.modelMapping.TableName + "\" for \"" + this.relationMapping.TableName + "\" (mapping)"); }

            // get query
            var relationQuery = this.GetQuery(foreignKeys);

            // create command
            var command = this.easyDbInstance.CreateCommand(relationQuery);

            // add parameters
            var type = model.GetType();
            foreach (var foreignKey in foreignKeys)
            {
                var parameterName = this.queryService.GetParameterName(foreignKey.ColumnName);
                var property = this.modelResolver.GetPropertyInfo(type, foreignKey.PropertyName);
                if (property == null) { throw new Exception("No property found for " + foreignKey.PropertyName  + " in " + type.Name); }

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
            var relationCommand = this.CreateCommand(model);
            var relationValue = await relationCommand.ReadOneAsync<T>(this.relationMapping);

            // resolve property to fill
            var propertyInfo = this.modelResolver.GetPropertyInfo(model.GetType(), this.propertyToFill);
            if (propertyInfo == null) { throw new Exception("No property found for the relation one with the property to fill " + this.propertyToFill + " in " + model.GetType().Name); }

            this.modelResolver.CheckAndSetValue(model, this.propertyToFill, propertyInfo, relationValue);
        }

    }


}
