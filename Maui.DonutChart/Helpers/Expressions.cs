using System.Linq.Expressions;

namespace Maui.DonutChart.Helpers;

internal static class Expressions
{
    internal static Func<object, TValue> CreatePropertyAccessor<TValue>(Type type, string propertyName)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(object), "obj");
        UnaryExpression castParameter = Expression.Convert(parameter, type);
        MemberExpression property = Expression.Property(castParameter, propertyName);
        UnaryExpression castProperty = Expression.Convert(property, typeof(TValue));
        Expression<Func<object, TValue>> lambda = Expression.Lambda<Func<object, TValue>>(castProperty, parameter);
        return lambda.Compile();
    }
}