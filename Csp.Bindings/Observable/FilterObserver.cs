using System;
using System.Collections.Generic;
using System.Text;

namespace Csp.Bindings.Observable
{
    internal class FilterObserver<T, TProperty> : IFilterObserver<T, TProperty>
    {
        public Func<TProperty, bool> Filter
        {
            get;
            private set;
        }

        private readonly Action<T> _do;

        public FilterObserver(Func<TProperty, bool> filter, Action<T> @do)
        {
            Filter = filter;
            _do = @do;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            _do?.Invoke(value);
        }
    }
}
