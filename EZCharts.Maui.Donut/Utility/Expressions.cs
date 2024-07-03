using System.Linq.Expressions;

namespace EZCharts.Maui.Donut.Utility;

internal static class Expressions
{
    /// <summary>
    /// Creates an expression that accesses the property associated with the provided <paramref name="propertyName"/> on the provided <paramref name="type"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="InvalidOperationException"/>
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