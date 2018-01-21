using System.Data;

namespace EasyDbLib
{
    public interface IModelFactory<TModel>
    {
        TModel CreateModel(IDataReader reader, EasyDb db);
    }
}
