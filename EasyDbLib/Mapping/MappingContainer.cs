using System.Collections.Generic;

namespace EasyDbLib
{
    public class MappingContainer
    {
        protected Dictionary<string, Mapping> mappings;

        public static MappingContainer Default => Singleton<MappingContainer>.Instance;

        public MappingContainer()
        {
            this.mappings = new Dictionary<string, Mapping>();
        }

        public MappingContainer Add(string tableName, Mapping mapping)
        {
            this.mappings[tableName] = mapping;
            return this;
        }

        public bool Has(string tableName)
        {
            return this.mappings.ContainsKey(tableName);
        }

        public Mapping Get(string tableName)
        {
            return this.mappings[tableName];
        }
    }
}