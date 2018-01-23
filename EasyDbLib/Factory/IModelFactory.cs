using System.Data;

namespace EasyDbLib
{
    public interface IModelFactory<TModel> where TModel : class, new()
    {
        TModel CreateModel(IDataReader reader, EasyDb db);
    }
}
