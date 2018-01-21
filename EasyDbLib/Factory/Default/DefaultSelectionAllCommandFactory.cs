using System.Data.Common;

namespace EasyDbLib
{
    public class DefaultSelectionAllCommandFactory : IDefaultSelectionAllCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(int? limit, ConditionAndParameterContainer conditionAndParameterContainer, string[] sorts, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example select [Id],[UserName],[RoleId] where [RoleId]=@roleid

            // all (value type/string) columns and keys => select [Id],[UserName],[RoleId]
            var columns = DbHelper.GetSelectColumns<TModel>(mapping);
            if (columns.Count == 0) { Guard.Throw("No column for select command"); }

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;

            // condition => where [RoleId]=@roleid and ...
            return this.db.queryService.GetSelect(limit, columns.ToArray(), tablename, conditionAndParameterContainer, sorts);
        }

        public DbCommand GetCommand<TModel>(int? limit, Check condition, string[] sorts) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();

            var conditionAndParameterContainer = condition != null ? new ConditionAndParameterContainer(condition, this.db.queryService) : null;

            var query = this.GetQuery(limit, conditionAndParameterContainer, sorts, mapping);

            var command = this.db.CreateSqlCommand(query);

            // add parameters name (@roleid) and value (sorted by affectation where [Id1]=@id1 and [Id2]=@id2 => add @id1 then @id2)
            if (conditionAndParameterContainer != null)
            {
                DbHelper.AddConditionParametersToCommand(command, conditionAndParameterContainer);
            }

            return command;
        }
    }



}
