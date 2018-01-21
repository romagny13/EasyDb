using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class MappingContainer : IMappingContainer
    {
        EasyDb db;

        public Dictionary<Type, ITable> TableByModelType { get; }

        public DefaultMappingBehavior DefaultMappingBehavior { get; set; }

        public MappingContainer()
        {
            this.TableByModelType = new Dictionary<Type, ITable>();
            this.DefaultMappingBehavior = DefaultMappingBehavior.Reflection;
        }

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public Table<TModel> SetTable<TModel>(string tableName) where TModel : class, new()
        {
            Guard.IsNullOrEmpty(tableName);

            var table = new Table<TModel>(tableName);
            this.TableByModelType[typeof(TModel)] = table;
            return table;
        }

        public bool IsTableRegistered<TModel>() where TModel : class, new()
        {
            return this.TableByModelType.ContainsKey(typeof(TModel));
        }

        public Table<TModel> GetTable<TModel>() where TModel : class, new()
        {
            if (!this.IsTableRegistered<TModel>()) { Guard.Throw("No table registered for model " + typeof(TModel).Name); }

            return (Table<TModel>)this.TableByModelType[typeof(TModel)];
        }

        public Table<TModel> TryGetTable<TModel>() where TModel : class, new()
        {
            if (this.IsTableRegistered<TModel>())
            {
                return this.GetTable<TModel>();
            }
            else if (this.DefaultMappingBehavior == DefaultMappingBehavior.Reflection)
            {
                return this.DiscoverMappingFor<TModel>();
            }
            else if (this.DefaultMappingBehavior == DefaultMappingBehavior.CreateEmptyTable)
            {
                if(db.queryService == null) { Guard.Throw("No connection informations provided."); }

                var table = DbHelper.TryCreateEmptyTable<TModel>(db.queryService.DefaultPrimaryKeyName);
                if (table != null)
                {
                    this.TableByModelType[typeof(TModel)] = table;
                    return table;
                }
            }
            return null;
        }

        public Table<TModel> DiscoverMappingFor<TModel>() where TModel : class, new()
        {
            var table = DbHelper.DiscoverMappingFor<TModel>();
            this.TableByModelType[typeof(TModel)] = table;
            return (Table<TModel>)table;
        }
    }

    public enum DefaultMappingBehavior
    {
        CreateEmptyTable,
        Reflection,
        None
    }

}
