using System.Data.Common;

namespace EasyDbLib
{
    public class DefaultSelectionOneCommandFactory : IDefaultSelectionOneCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(Check condition, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example select [Id],[UserName],[RoleId] where [Id]=@id

            // all (value type/string) columns and keys => select [Id],[UserName],[RoleId]
            var columns = DbHelper.GetSelectColumns<TModel>(mapping);
            if (columns.Count == 0) { Guard.Throw("No column for select command"); }

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;

            // condition => where [Id]=@id and ...
            return this.db.queryService.GetSelect(null, columns.ToArray(), tablename, condition, null);
        }

        public DbCommand GetCommand<TModel>(Check condition) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();
            var query = this.GetQuery(condition, mapping);

            var command = this.db.CreateSqlCommand(query);

            // add parameters name (@id) and value (sorted by affectation where [Id1]=@id1 and [Id2]=@id2 => add @id1 then @id2)
            if (condition != null)
            {
                DbHelper.AddConditionParametersToCommand(command, condition, this.db.queryService);
            }

            return command;
        }
    }



}
