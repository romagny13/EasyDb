using System.Data;
using System.Data.Common;

namespace EasyDbLib
{

    public static class DbCommandExtensions
    {
        public static DbCommand AddParameter(this DbCommand command, string name, object value, DbType dbType, ParameterDirection parameterDirection)
        {
            DbHelper.AddParameter(command, name, value, parameterDirection, dbType);
            return command;
        }

        public static DbCommand AddParameter(this DbCommand command, string name, object value, ParameterDirection parameterDirection)
        {
            DbHelper.AddParameter(command, name, value, parameterDirection, null);
            return command;
        }

        public static DbCommand AddInParameter(this DbCommand command, string name, object value, DbType dbType)
        {
            DbHelper.AddParameter(command, name, value, ParameterDirection.Input, dbType);
            return command;
        }

        public static DbCommand AddInParameter(this DbCommand command, string name, object value)
        {
            DbHelper.AddParameter(command, name, value, ParameterDirection.Input, null);
            return command;
        }

        public static DbCommand AddOutParameter(this DbCommand command, string name, DbType dbType)
        {
            DbHelper.AddParameter(command, name, null, ParameterDirection.Output, dbType);
            return command;
        }

        public static DbCommand AddOutParameter(this DbCommand command, string name)
        {
            DbHelper.AddParameter(command, name, null, ParameterDirection.Output, null);
            return command;
        }
    }
}
