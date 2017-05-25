using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class ManyRelation<T> : IManyRelation
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

        protected PrimaryKeyColumn[] primaryKeys;
        public PrimaryKeyColumn[] PrimaryKeys => this.primaryKeys;

        public ManyRelation(
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
            this.primaryKeys = this.GetPrimaryKeys();
        }

        protected ForeignKeyColumn[] GetForeignKeys()
        {
            var foreignKeys = this.relationMapping.GetForeignKeys(this.modelMapping.TableName);
            if (foreignKeys.Length == 0) { throw new Exception("No foreign keys provided for the many relation with " + this.relationMapping.TableName + " on " + this.modelMapping.TableName + ". Cannot resolve the query string."); }
            return foreignKeys;
        }

        protected PrimaryKeyColumn[] GetPrimaryKeys()
        {
            var primaryKeys = this.modelMapping.GetPrimaryKeys();
            if (primaryKeys.Length == 0 || primaryKeys.Length != this.foreignKeys.Length) { throw new Exception("No primary keys provided for the many relation with " + this.modelMapping.TableName); }
            return primaryKeys;
        }

        public string GetQuery()
        {
            return this.queryService.GetSelect(this.relationMapping)
                   + this.queryService.GetFrom(this.relationMapping.TableName)
                   + this.queryService.GetWhereHasMany(this.foreignKeys);
        }

        protected void CheckModelType(object model)
        {
            if (model.GetType() != this.modelType)
            {
                throw new Exception("Invalid model type " + model.GetType().Name + " for the relation one . Wait for " + this.modelType.Name);
            }
        }

        public EasyDbCommand CreateCommand(object model)
        {
            this.CheckModelType(model);
            // get query
            var query = this.GetQuery();

            // create command
            var command = this.easyDbInstance.CreateCommand(query);

            // add parameters
            var type = model.GetType();
            for (int i = 0; i < this.foreignKeys.Length; i++)
            {
                var foreignKey = this.foreignKeys[i];
                var parameterName = this.queryService.GetParameterName(foreignKey.PrimaryKeyReferenced);

                var propertyInfo = this.modelResolver.GetPropertyInfo(type, this.primaryKeys[i].PropertyName);
                if (propertyInfo == null) { throw new Exception("Property " + this.primaryKeys[i].PropertyName + " not found in " + this.modelType.Name); }

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
            var command = this.CreateCommand(model);
            var relationValue = await command.ReadAllAsync<T>(this.relationMapping);

            // resolve property to fill
            var propertyInfo = this.modelResolver.GetPropertyInfo(model.GetType(), this.propertyToFill);
            if (propertyInfo == null) { throw new Exception("Property " + this.propertyToFill + " not found in " + this.modelType.Name); }

            this.modelResolver.CheckAndSetValue(model, this.propertyToFill, propertyInfo, relationValue);
        }
    }

}
