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
        protected int? limit;
        protected ConditionAndParameterContainer condition;
        protected string[] statements;
        protected string[] sorts;
        protected bool hasSorts;
        protected bool hasCondition;
        protected bool hasStatements;
        protected bool hasOneRelations;
        protected bool hasManyRelations;
        protected Dictionary<Type,IRelation> oneRelations;
        private Dictionary<Type,IRelation> manyRelations;


        public SelectQuery(IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
            : this(new ModelResolver(), queryService, easyDbInstance, modelType, mapping)
        { }

        public SelectQuery(IModelResolver modelResolver, IQueryService queryService, EasyDb easyDbInstance, Type modelType, Table mapping)
        {
            this.oneRelations = new Dictionary<Type, IRelation>();
            this.manyRelations = new Dictionary<Type, IRelation>();
            this.statements = new string[] { };
            this.modelResolver = modelResolver;
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.modelType = modelType;
            this.modelMapping = mapping;
        }

        public SelectQuery<T> Top(int limit)
        {
            this.limit = limit;
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
            this.sorts = sorts;
            this.hasSorts = true;
            return this;
        }

        public SelectQuery<T> HasOne<TRelation>(string propertyToFill, Table relationMapping)
        {
            this.oneRelations[typeof(TRelation)] = new OneRelation<TRelation>(this.queryService, this.modelResolver, this.easyDbInstance, propertyToFill, this.modelMapping, relationMapping);
            this.hasOneRelations = true;
            return this;
        }

        public bool HasOneRelation<TRelation>()
        {
            return this.oneRelations.ContainsKey(typeof(TRelation));
        }

        public bool HasManyRelation<TRelation>()
        {
            return this.manyRelations.ContainsKey(typeof(TRelation));
        }

        public IRelation GetOneRelation<TRelation>()
        {
            return this.oneRelations[typeof(TRelation)];
        }

        public IRelation GetManyRelation<TRelation>()
        {
            return this.manyRelations[typeof(TRelation)];
        }

        public SelectQuery<T> HasMany<TRelation>(string propertyListToFill, Table relationMapping)
        {
            this.manyRelations[typeof(TRelation)] = new ManyRelation<TRelation>(this.queryService, this.modelResolver, this.easyDbInstance, propertyListToFill, this.modelMapping, relationMapping);
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


        protected EasyDbCommand CreateCommand()
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
