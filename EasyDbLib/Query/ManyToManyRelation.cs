using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class ManyToManyRelation<TRelationModel> : IManyToManyRelation
    {
        protected IQueryService queryService;
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;

        protected Table modelMapping;
        protected Type modelType;

        protected string propertyToFill;
        public string PropertyToFill => this.propertyToFill;

        protected Table relationMapping;
        public Table Mapping => this.relationMapping;

        private IntermediateTable intermediateTableMapping;
        public IntermediateTable IntermediateTableMapping => this.intermediateTableMapping;

        protected IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeys;
        public IntermediatePrimaryKeyColumn[] IntermediateTablePrimaryKeys => this.intermediateTablePrimaryKeys;

        protected IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeysForCheckValue;
        public IntermediatePrimaryKeyColumn[] IntermediateTablePrimaryKeysForCheckValue => this.intermediateTablePrimaryKeysForCheckValue;

        protected PrimaryKeyColumn[] primaryKeys;
        public PrimaryKeyColumn[] PrimaryKeys => this.primaryKeys;

        public ManyToManyRelation(
            IQueryService queryService, 
            IModelResolver modelResolver, 
            EasyDb easyDbInstance, 
            string propertyListToFill, 
            Type modelType, 
            Table modelMapping, 
            Table relationMapping,
            IntermediateTable intermediateTableMapping)
        {
            this.queryService = queryService;
            this.modelResolver = modelResolver;
            this.easyDbInstance = easyDbInstance;

            this.propertyToFill = propertyListToFill;
            this.modelType = modelType;
            this.modelMapping = modelMapping;
            this.relationMapping = relationMapping;
            this.intermediateTableMapping = intermediateTableMapping;

            this.intermediateTablePrimaryKeys = this.GetIntermediatePrimaryKeysForRelation();
            this.intermediateTablePrimaryKeysForCheckValue = this.GetIntermediatePrimaryKeysForCheckValue();
            this.primaryKeys = this.GetPrimaryKeys();
        }

        public string GetQuery()
        {
            return this.queryService.GetSelectWithTableName(this.relationMapping)
                 + this.queryService.GetFrom(this.intermediateTableMapping.TableName, this.relationMapping.TableName)
                 + this.queryService.GetWhereHasManyToMany(this.intermediateTablePrimaryKeys, this.IntermediateTablePrimaryKeysForCheckValue);
        }

        public IntermediatePrimaryKeyColumn[] GetIntermediatePrimaryKeysForRelation()
        {
            var primaryKeys = this.intermediateTableMapping.GetPrimaryKeys(this.relationMapping.TableName); 
            if (primaryKeys.Length == 0) { throw new Exception("No primary key provided in the intermediate table " + this.intermediateTableMapping.TableName + " with " + this.relationMapping.TableName + " (mapping)"); }

            return primaryKeys;
        }

        public IntermediatePrimaryKeyColumn[] GetIntermediatePrimaryKeysForCheckValue()
        {
            var primaryKeys = this.intermediateTableMapping.GetPrimaryKeys(this.modelMapping.TableName);
            if (primaryKeys.Length == 0) { throw new Exception("No primary key provided in the intermediate table " + this.intermediateTableMapping.TableName + " with " + this.modelMapping.TableName + " (mapping)"); }

            return primaryKeys;
        }

        public PrimaryKeyColumn[] GetPrimaryKeys()
        {
            var primaryKeys = this.modelMapping.GetPrimaryKeys();
            if (primaryKeys.Length == 0) { throw new Exception("No primary key provided in the model " + this.modelMapping.TableName + " (mapping)"); }

            return primaryKeys;
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

            for (int i = 0; i < this.intermediateTablePrimaryKeysForCheckValue.Length; i++)
            {
                var intermediatePrimaryKey = this.intermediateTablePrimaryKeysForCheckValue[i];
                var parameterName = this.queryService.GetParameterName(intermediatePrimaryKey.ColumnName);

                var primaryKey = this.primaryKeys[i];
                var propertyInfo = this.modelResolver.GetPropertyInfo(type, primaryKey.PropertyName);
                if (propertyInfo == null) { throw new Exception("Property " + primaryKey.PropertyName + " not found in " + this.modelType.Name); }

                var value = this.modelResolver.GetValue(model, propertyInfo);
                if (primaryKey.DbType.HasValue)
                {
                    command.AddParameter(parameterName, value, primaryKey.DbType.Value);
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
            var relationValue = await command.ReadAllAsync<TRelationModel>(this.relationMapping);

            // resolve property to fill
            var propertyInfo = this.modelResolver.GetPropertyInfo(model.GetType(), this.propertyToFill);
            if (propertyInfo == null) { throw new Exception("Property " + this.propertyToFill + " not found in " + this.modelType.Name); }

            this.modelResolver.CheckAndSetValue(model, this.propertyToFill, propertyInfo, relationValue);
        }
    }
}
