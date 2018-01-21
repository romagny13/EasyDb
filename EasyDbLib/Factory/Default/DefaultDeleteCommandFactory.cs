using System.Data.Common;

namespace EasyDbLib
{
    public class DefaultDeleteCommandFactory : IDefaultDeleteCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(ConditionAndParameterContainer conditionAndParameterContainer, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example: delete from [User] where [Id]=@id and ...

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;
            return this.db.queryService.GetDelete(tablename, conditionAndParameterContainer);
        }

        public DbCommand GetCommand<TModel>(Check condition) where TModel : class, new()
        {
            var conditionAndParameterContainer = condition != null ? new ConditionAndParameterContainer(condition, this.db.queryService) : null;

            var mapping = this.db.TryGetTable<TModel>();
            var query = this.GetQuery(conditionAndParameterContainer, mapping);

            var command = this.db.CreateSqlCommand(query);

            // add parameters name (@id) and value (sorted by affectation where [Id1]=@id1 and [Id2]=@id2 => add @id1 then @id2)
            if (conditionAndParameterContainer != null)
            {
                DbHelper.AddConditionParametersToCommand(command, conditionAndParameterContainer);
            }

            return command;
        }
    }

}
