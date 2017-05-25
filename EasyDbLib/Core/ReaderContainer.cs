using System.Data.Common;

namespace EasyDbLib
{
    public class ReaderContainer : IReaderContainer
    {
        protected DbDataReader reader;

        public ReaderContainer(DbDataReader reader)
        {
            this.reader = reader;
        }

        public string GetName(int index)
        {
            return this.reader.GetName(index);
        }

        public object GetValue(int index)
        {
            return this.reader.GetValue(index);
        }

        public bool IsDBNull(int index)
        {
            return this.reader.IsDBNull(index);
        }

        public int FieldCount => this.reader.FieldCount;

    }
}