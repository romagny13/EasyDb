using System.Data.Common;

namespace EasyDbLib
{
    public class DefaultCountCommandFactory : IDefaultCountCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(ConditionAndParameterContainer condition, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example: select count(*) from [Post] where [Id]=@id

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;
            return this.db.queryService.GetSelectCount(tablename, condition);
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
