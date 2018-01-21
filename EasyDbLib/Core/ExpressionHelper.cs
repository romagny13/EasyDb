using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyDbLib
{
    public class ExpressionHelper
    {
        public static PropertyInfo GetPropertyFromExpression<TModel, TPropertyType>(Expression<Func<TModel, TPropertyType>> propertyExpression)
        {
            var member = propertyExpression.Body as MemberExpression;
            return member.Member as PropertyInfo;
        }

    }
}
