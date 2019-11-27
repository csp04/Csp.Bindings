using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Csp.Events.Core;
using Csp.Extensions.Reflections;

namespace Csp.Bindings
{
    public static partial class PropertyBindings
    {
        public static IDisposable WhenChanged<TProperty>(Expression<Func<TProperty>> propertyToListen, Action<TProperty> listener)
        {
            var ps = propertyToListen.GetPropertyNameAndSource();

            if (ps.Source is INotifyPropertyChanged npc)
            {
                var propertyName = ps.PropertyName;
                var disposable = EventBindingHandler<PropertyChangedEventHandler>.Create(
                     (_, e) =>
                    {
                        if (propertyName.Equals(e.PropertyName))
                        {
                            if (npc.CanRead(e.PropertyName))
                            {
                                listener?.Invoke((TProperty)npc.Read(e.PropertyName));
                            }
                        }
                    },
                    x => npc.PropertyChanged += x,
                    x => npc.PropertyChanged -= x);

                return disposable;
            }

            return null;
        }
        public static IDisposable WhenChanged<TProperty>(Expression<Func<TProperty>> propertyToListen, Expression<Func<TProperty>> propertyToWrite)
        {
            var ps = propertyToWrite.GetPropertyNameAndSource();
            var source = ps.Source;
            var propertyName = ps.PropertyName;

            return WhenChanged(propertyToListen, value => source.Write(propertyName, value));
        }

        public static IDisposable Bind<TProperty>(Expression<Func<TProperty>> property1, Expression<Func<TProperty>> property2)
        {
            var d1 = WhenChanged(property1, property2);
            var d2 = WhenChanged(property2, property1);

            return new CompositeDisposable(d1, d2);
        }

        private static (string PropertyName, object Source) GetPropertyNameAndSource<T>(this Expression<Func<T>> expression)
        {
            return ((LambdaExpression)expression).GetPropertyNameAndSource();
        }
        private static (string PropertyName, object Source) GetPropertyNameAndSource(this LambdaExpression expression)
        {
            if (expression.Body is MemberExpression body)
            {
                string propertyName = body.Member.Name;
                object source = default;

                if (body.Expression is ConstantExpression constantExpression)
                {
                    source = constantExpression.Value;
                }
                else if (body.Expression is MemberExpression memberExpression)
                {
                    source = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                }

                return (propertyName, source);
            }

            throw new ArgumentException($"'{nameof(expression)}' body should be a member expression.");
        }
    }
}
