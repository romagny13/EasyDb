using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDbLib
{

    public class SelectQuery<T> where T : new()
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
        protected Dictionary<Type,IRelation> oneRelations;
        private Dictionary<Type,IManyRelation> manyRelations;

        public SelectQuery(IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
            : this(new ModelResolver(), queryService, easyDbInstance, modelType, mapping)
        { }

        public SelectQuery(IModelResolver modelResolver, IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
        {
            this.oneRelations = new Dictionary<Type, IRelation>();
            this.manyRelations = new Dictionary<Type, IManyRelation>();
            this.statements = new string[] { };
            this.modelResolver = modelResolver;
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.modelType = modelType;
            this.modelMapping = mapping;
        }

        public SelectQuery<T> Top(int limit)
        {
            if (this.hasLimit) { throw new Exception("Limit alerady defined"); }
            this.limit = limit;
            this.hasLimit = true;
            return this;
        }

        public SelectQuery<T> Statements(params string[] statements)
        {
            if (this.hasStatements) { throw new Exception("Statements already defined"); }
            this.statements = statements;
            this.hasStatements = true;
            return this;
        }

        public SelectQuery<T> Where(Condition condition)
        {
            if (this.hasCondition) { throw new Exception("One clause where"); }
            this.condition = new ConditionAndParameterContainer(condition);
            this.hasCondition = true;
            return this;
        }

        public SelectQuery<T> OrderBy(params string[] sorts)
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

        public SelectQuery<T> HasOne<TRelation>(string propertyToFill, Table relationMapping)
        {
            CheckPropertyName(propertyToFill);
            this.oneRelations[typeof(TRelation)] = new OneRelation<TRelation>(this.queryService, this.modelResolver, this.easyDbInstance, propertyToFill, this.modelType, this.modelMapping, relationMapping);
            this.hasOneRelations = true;
            return this;
        }

        public bool HasOneRelation<TRelation>()
        {
            return this.oneRelations.ContainsKey(typeof(TRelation));
        }

        public IRelation GetOneRelation<TRelation>()
        {
            if (!this.HasOneRelation<TRelation>()) { throw new Exception("No relation registered for " + typeof(TRelation).Name + " with the select query on " + this.modelMapping.TableName); }

            return this.oneRelations[typeof(TRelation)];
        }

        public bool HasManyRelation<TManyRelation>()
        {
            if (!this.HasManyRelation<TManyRelation>()) { throw new Exception("No many relation registered for " + typeof(TManyRelation).Name + " with the select query on " + this.modelMapping.TableName); }

            return this.manyRelations.ContainsKey(typeof(TManyRelation));
        }

        public IManyRelation GetManyRelation<TManyRelation>()
        {
            return this.manyRelations[typeof(TManyRelation)];
        }

        public SelectQuery<T> HasMany<TManyRelation>(string propertyListToFill, Table relationMapping)
        {
            CheckPropertyName(propertyListToFill);
            this.manyRelations[typeof(TManyRelation)] = new ManyRelation<TManyRelation>(this.queryService, this.modelResolver, this.easyDbInstance, propertyListToFill, this.modelType, this.modelMapping, relationMapping);
            this.hasManyRelations = true;
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
   
        protected async Task FetchModel(T model)
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
        }

        public async Task<T> ReadOneAsync()
        {
            var mainCommand = this.CreateCommand();
            var model = await mainCommand.ReadOneAsync<T>(this.modelMapping);

            await this.FetchModel(model);

            return model;
        }

        public async Task<List<T>> ReadAllAsync()
        {
            var mainCommand = this.CreateCommand();
            var models = await mainCommand.ReadAllAsync<T>(this.modelMapping);

            foreach (var model in models)
            {
                await this.FetchModel(model);
            }

            return models;
        }
    }


}
