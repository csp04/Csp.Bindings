using System;

namespace Csp.Bindings.Observable
{
    public interface IFilterObserver<T, TProperty> : IObserver<T>
    {

        Func<TProperty, bool> Filter { get; }
    }
}
