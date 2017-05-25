using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class OneRelation<T> : IRelation
    {
        protected IQueryService queryService;
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;

        protected Table modelMapping;
        protected Type modelType;
       
        protected Table relationMapping;
        public Table Mapping => this.relationMapping;

        protected string propertyToFill;
        public string PropertyToFill => this.propertyToFill;

        protected ForeignKeyColumn[] foreignKeys;
        public ForeignKeyColumn[] ForeignKeys => this.foreignKeys;

        public OneRelation(
            IQueryService queryService, 
            IModelResolver modelResolver,
            EasyDb easyDbInstance, 
            string propertyToFill, 
            Type modelType,
            Table modelMapping,
            Table relationMapping)
        {
            this.queryService = queryService;
            this.modelResolver = modelResolver;
            this.easyDbInstance = easyDbInstance;
            this.propertyToFill = propertyToFill;
            this.modelType = modelType;
            this.modelMapping = modelMapping;
            this.relationMapping = relationMapping;
            this.foreignKeys = this.GetForeignKeys();
        }

        protected ForeignKeyColumn[] GetForeignKeys()
        {
           var foreignKeys = this.modelMapping.GetForeignKeys(this.relationMapping.TableName);
           if (foreignKeys.Length == 0) { throw new Exception("No foreign keys provided for the relation one with " + this.relationMapping.TableName + " on " + this.modelMapping.TableName + ". Cannot resolve the query string."); }
           return foreignKeys;
        }

        public string GetQuery()
        {

            // select * from table where pk = @fk and pk2=@fk2
            return this.queryService.GetSelect(this.relationMapping)
                   + this.queryService.GetFrom(this.relationMapping.TableName)
                   + this.queryService.GetWhereHasOne(this.foreignKeys);
        }

        protected void CheckModelType(object model)
        {
            if(model.GetType() != this.modelType)
            {
                throw new Exception("Invalid model type " + model.GetType().Name + " for the relation one . Wait for " + this.modelType.Name);
            }
        }

        public EasyDbCommand CreateCommand(object model)
        {
            this.CheckModelType(model);
            var relationQuery = this.GetQuery();

            // create command
            var command = this.easyDbInstance.CreateCommand(relationQuery);

            // add parameters
            var type = model.GetType();
            foreach (var foreignKey in foreignKeys)
            {
                var parameterName = this.queryService.GetParameterName(foreignKey.ColumnName);

                var propertyInfo = this.modelResolver.GetPropertyInfo(type, foreignKey.PropertyName);
                if (propertyInfo == null) { throw new Exception("Property " + foreignKey.PropertyName + " not found in " + this.modelType.Name); }

                var value = this.modelResolver.GetValue(model, propertyInfo);
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
            if (propertyInfo == null) { throw new Exception("Property " + this.propertyToFill + " not found in " + this.modelType.Name); }

            this.modelResolver.CheckAndSetValue(model, this.propertyToFill, propertyInfo, relationValue);
        }

    }

}
