using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Csp.Events.Core;
using Csp.Extensions.Reflections;
using Csp.Bindings.Observable;

namespace Csp.Bindings
{
    public static partial class Bindings
    {

        private static IDisposable WhenChanged<TProperty>(LambdaExpression propertyToListen, Action<TProperty> listener, object source)
        {
            var sp = ExpressionHelper.GetSourceAndPropertyName<object, TProperty>(source, propertyToListen);

            if (sp.Source is INotifyPropertyChanged npc)
            {
                var propertyName = sp.PropertyName;
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

        public static IDisposable WhenChanged<TProperty>(Expression<Func<TProperty>> propertyToListen, Action<TProperty> listener)
        {
            return WhenChanged((LambdaExpression)propertyToListen, listener, null);
        }

        public static IDisposable WhenChanged<TSource, TProperty>(this TSource @this, 
                Expression<Func<TProperty>> propertyToListen, Action<TProperty> listener) where TSource : INotifyPropertyChanged
        {
            return WhenChanged((LambdaExpression)propertyToListen, listener, @this);
        }

        public static IDisposable WhenChanged<TProperty>(Expression<Func<TProperty>> propertyToListen, Expression<Func<TProperty>> propertyToWrite)
        {
            var sp = propertyToWrite.GetSourceAndPropertyName();
            var source = sp.Source;
            var propertyName = sp.PropertyName;

            return WhenChanged(propertyToListen, value => source.Write(propertyName, value));
        }

        public static IDisposable Bind<TProperty>(Expression<Func<TProperty>> property1, Expression<Func<TProperty>> property2)
        {
            var d1 = WhenChanged(property1, property2);
            var d2 = WhenChanged(property2, property1);

            return new CompositeDisposable(d1, d2);
        }

        #region Observable

        public static ObservableProperty<T, TProperty> WhenChanged<T, TProperty>(this T @this,
                Expression<Func<TProperty>> propertyToListen) where T : INotifyPropertyChanged
        {
            return new ObservableProperty<T, TProperty>(@this, propertyToListen);
        }

        #endregion


    }
}
