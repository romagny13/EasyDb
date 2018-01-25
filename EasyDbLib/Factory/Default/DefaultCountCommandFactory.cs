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

        public string GetQuery<TModel>(Check condition, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example: select count(*) from [Post] where [Id]=@id

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;
            return this.db.queryService.GetSelectCount(tablename, condition);
        }

        public DbCommand GetCommand<TModel>(Check condition) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();
            var query = this.GetQuery(condition, mapping);

            var command = this.db.CreateSqlCommand(query);

            // add parameters name (@id) and value (sorted by affectation where [Id1]=@id1 and [Id2]=@id2 => add @id1 then @id2)
            if (condition != null)
            {
                // add parameter name (@id) and value (only for CheckOp)
                DbHelper.AddConditionParametersToCommand(command, condition, this.db.queryService);
            }

            return command;
        }
    }



}
