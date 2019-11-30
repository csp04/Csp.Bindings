using System;
using System.Collections.Generic;
using System.Text;

namespace Csp.Bindings.Observable
{
    public interface IFilterObserver<T, TProperty> : IObserver<T>
    {

        Func<TProperty, bool> Filter { get; }
    }
}
