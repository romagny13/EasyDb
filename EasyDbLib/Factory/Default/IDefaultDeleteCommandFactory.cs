﻿using System.Data.Common;

namespace EasyDbLib
{
    public interface IDefaultDeleteCommandFactory
    {
        DbCommand GetCommand<TModel>(Check condition) where TModel : class, new();
        string GetQuery<TModel>(Check condition, Table<TModel> mapping = null) where TModel : class, new();
        void SetDb(EasyDb db);
    }
}