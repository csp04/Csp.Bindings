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
        private readonly Action<Exception> _onError;

        public FilterObserver(Func<TProperty, bool> filter, Action<T> @do, Action<Exception> onError)
        {
            Filter = filter;
            _do = @do;
            _onError = onError;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            _onError?.Invoke(error);
        }

        public void OnNext(T value)
        {
            _do?.Invoke(value);
        }
    }
}
