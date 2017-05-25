using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDbLib
{

    public class SelectQuery<TModel> where TModel : new()
    {
        protected IQueryService queryService;
        protected IModelResolver modelResolver;
        protected EasyDb easyDbInstance;
        protected Type modelType;

        protected Table modelMapping;
        public Table Mapping => this.modelMapping;

        protected int? limit;
        protected ConditionAndParameterContainer condition;
        protected string[] statements;
        protected string[] sorts;
        protected bool hasLimit;
        protected bool hasSorts;
        protected bool hasCondition;
        protected bool hasStatements;
        protected bool hasOneRelations;
        protected bool hasManyRelations;
        protected bool hasManyToManyRelations;
        protected Dictionary<Type,IRelation> oneRelations;
        private Dictionary<Type,IManyRelation> manyRelations;
        private Dictionary<Type, IManyToManyRelation> manyToManyRelations;

        public SelectQuery(IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
            : this(new ModelResolver(), queryService, easyDbInstance, modelType, mapping)
        { }

        public SelectQuery(IModelResolver modelResolver, IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
        {
            this.oneRelations = new Dictionary<Type, IRelation>();
            this.manyRelations = new Dictionary<Type, IManyRelation>();
            this.manyToManyRelations = new Dictionary<Type, IManyToManyRelation>();
            this.statements = new string[] { };

            this.modelResolver = modelResolver;
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.modelType = modelType;
            this.modelMapping = mapping;
        }

        public SelectQuery<TModel> Top(int limit)
        {
            if (this.hasLimit) { throw new Exception("Limit alerady defined"); }
            this.limit = limit;
            this.hasLimit = true;
            return this;
        }

        public SelectQuery<TModel> Statements(params string[] statements)
        {
            if (this.hasStatements) { throw new Exception("Statements already defined"); }
            this.statements = statements;
            this.hasStatements = true;
            return this;
        }

        public SelectQuery<TModel> Where(Check condition)
        {
            if (this.hasCondition) { throw new Exception("One clause where"); }
            this.condition = new ConditionAndParameterContainer(condition);
            this.hasCondition = true;
            return this;
        }

        public SelectQuery<TModel> OrderBy(params string[] sorts)
        {
            if (this.hasSorts) { throw new Exception("One clause order by"); }
            if (sorts.Length == 0) { throw new Exception("No column provided for order by"); }
            this.sorts = sorts;
            this.hasSorts = true;
            return this;
        }

        protected void CheckPropertyName(string propertyName)
        {
            if (!NameChecker.CheckPropertyName(propertyName))
            {
                throw new Exception("Invalid property name " + propertyName);
            }
        }

        public bool HasOneRelation<TRelationModel>()
        {
            return this.oneRelations.ContainsKey(typeof(TRelationModel));
        }

        public IRelation GetOneRelation<TRelationModel>()
        {
            if (!this.HasOneRelation<TRelationModel>()) { throw new Exception("No relation registered for " + typeof(TRelationModel).Name + " with the select query on " + this.modelMapping.TableName); }

            return this.oneRelations[typeof(TRelationModel)];
        }

        public bool HasOneToManyRelation<TRelationModel>()
        {
            if (!this.HasOneToManyRelation<TRelationModel>()) { throw new Exception("No many relation registered for " + typeof(TRelationModel).Name + " with the select query on " + this.modelMapping.TableName); }

            return this.manyRelations.ContainsKey(typeof(TRelationModel));
        }

        public IManyRelation GetOneToManyRelation<TRelationModel>()
        {
            return this.manyRelations[typeof(TRelationModel)];
        }

        public bool HasManyToManyRelation<TRelationModel>()
        {
            return this.manyToManyRelations.ContainsKey(typeof(TRelationModel));
        }

        public IManyToManyRelation GetManyToManyRelation<TRelationModel>()
        {
            if (!this.HasManyToManyRelation<TRelationModel>()) { throw new Exception("No relation many to many registered for "+ typeof(TRelationModel).Name + " with the select query on " + this.modelMapping.TableName); }

            return this.manyToManyRelations[typeof(TRelationModel)];
        }

        public SelectQuery<TModel> SetZeroOne<TRelationModel>(string propertyToFill, Table relationMapping)
        {
            CheckPropertyName(propertyToFill);
            this.oneRelations[typeof(TRelationModel)] = new ZeroOneRelation<TRelationModel>(this.queryService, this.modelResolver, this.easyDbInstance, propertyToFill, this.modelType, this.modelMapping, relationMapping);
            this.hasOneRelations = true;
            return this;
        }

        public SelectQuery<TModel> SetOneToMany<TRelationModel>(string propertyListToFill, Table relationMapping)
        {
            CheckPropertyName(propertyListToFill);
            this.manyRelations[typeof(TRelationModel)] = new OneToManyRelation<TRelationModel>(this.queryService, this.modelResolver, this.easyDbInstance, propertyListToFill, this.modelType, this.modelMapping, relationMapping);
            this.hasManyRelations = true;
            return this;
        }

        public SelectQuery<TModel> SetManyToMany<TRelationModel>(string propertyListToFill, Table relationMapping, IntermediateTable intermediateTableMapping)
        {
            CheckPropertyName(propertyListToFill);
            this.manyToManyRelations[typeof(TRelationModel)] = new ManyToManyRelation<TRelationModel>(this.queryService, this.modelResolver, this.easyDbInstance, propertyListToFill, this.modelType, this.modelMapping, relationMapping, intermediateTableMapping);
            this.hasManyToManyRelations = true;
            return this;
        }

        public string GetQuery()
        {
            return this.queryService.GetSelect(this.statements, this.limit, this.modelMapping)
                   + this.queryService.GetFrom(this.modelMapping.TableName)
                   + this.queryService.GetWhere(this.condition)
                   + this.queryService.GetOrderBy(this.sorts);
        }

        public EasyDbCommand CreateCommand()
        {
            var query = this.GetQuery();

            // get condition unique keys
            var mainCommand = this.easyDbInstance.CreateCommand(query);

            if (this.hasCondition)
            {
                mainCommand.AddParameter(this.condition.Main.ParameterName, this.condition.Main.ParameterValue);

                if (this.condition.HasConditions())
                {
                    foreach (var subCondittion in this.condition.SubConditions)
                    {
                        if (subCondittion.IsConditionOp)
                        {
                            mainCommand.AddParameter(subCondittion.ParameterName, subCondittion.ParameterValue);
                        }
                    }
                }
            }
            return mainCommand;
        }
   
        protected async Task FetchModel(TModel model)
        {
            if (this.hasOneRelations)
            {
                foreach (var relation in this.oneRelations)
                {
                    await relation.Value.Fetch(model);
                }
            }
            if (this.hasManyRelations)
            {
                foreach (var relation in this.manyRelations)
                {
                    await relation.Value.Fetch(model);
                }
            }
            if (this.hasManyToManyRelations)
            {
                foreach (var relation in this.manyToManyRelations)
                {
                    await relation.Value.Fetch(model);
                }
            }
        }

        public async Task<TModel> ReadOneAsync()
        {
            var mainCommand = this.CreateCommand();
            var model = await mainCommand.ReadOneAsync<TModel>(this.modelMapping);

            await this.FetchModel(model);

            return model;
        }

        public async Task<List<TModel>> ReadAllAsync()
        {
            var mainCommand = this.CreateCommand();
            var models = await mainCommand.ReadAllAsync<TModel>(this.modelMapping);

            foreach (var model in models)
            {
                await this.FetchModel(model);
            }

            return models;
        }
    }


}
