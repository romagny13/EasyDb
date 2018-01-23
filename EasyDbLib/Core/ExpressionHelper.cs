using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyDbLib
{
    public class ExpressionHelper
    {
        public static PropertyInfo GetPropertyFromExpression<TModel, TPropertyType>(Expression<Func<TModel, TPropertyType>> expression)
        {
            var member = expression.Body as MemberExpression;
            return member.Member as PropertyInfo;
        }

    }
}
