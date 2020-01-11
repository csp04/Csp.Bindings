using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Csp.Bindings
{
    internal static class ExpressionHelper
    {
        private static MemberExpression GetMemberExp(this LambdaExpression expression)
        {
            return expression.Body as MemberExpression;
        }

        private static ConstantExpression GetConstExp(this MemberExpression expression)
        {
            Expression exp = expression;

            while (exp is MemberExpression memExp)
                exp = memExp.Expression;

            return exp as ConstantExpression;
        }

        public static (object Source, string PropertyName) GetSourceAndPropertyName
                                    (this LambdaExpression expression)
        {
            var memberExpression = expression.GetMemberExp();
            var sourceMemExpression = memberExpression.Expression as MemberExpression;

            object source = default;

            if (sourceMemExpression != null)
            {
                var constantExpression = sourceMemExpression.GetConstExp();
                var value = constantExpression.Value;
                var type = value.GetType();
                var srcMember = sourceMemExpression.Member;

                if (srcMember.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = type.GetField(srcMember.Name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    source = fieldInfo?.GetValue(value);
                }
                else if (srcMember.MemberType == MemberTypes.Property)
                {
                    var propInfo = type.GetProperty(srcMember.Name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    source = propInfo?.GetValue(value);
                }

            }

            string propertyName = memberExpression.Member.Name;

            return (source, propertyName);
        }


        public static (object Source, string PropertyName) GetSourceAndPropertyName<TProperty>
                                    (this Expression<Func<TProperty>> expression)
        {
            return GetSourceAndPropertyName((LambdaExpression)expression);
        }

        public static (object Source, string PropertyName) GetSourceAndPropertyName<TSource, TProperty>
                                   (this TSource @this, LambdaExpression expression)
        {
            var sp = GetSourceAndPropertyName(expression);

            if (sp.Source == null)
                sp.Source = @this;

            return sp;
        }

        public static (object Source, string PropertyName) GetSourceAndPropertyName<TSource, TProperty>
                                   (this TSource @this, Expression<Func<TSource, TProperty>> expression)
        {
            return GetSourceAndPropertyName<TSource, TProperty>(@this, (LambdaExpression)expression);
        }
    }
}
