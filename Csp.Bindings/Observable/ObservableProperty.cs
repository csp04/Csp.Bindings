using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Csp.Bindings.Observable
{
    public class ObservableProperty<T, TProperty> : ObservableBase<T, IFilterObserver<T, TProperty>> where T : INotifyPropertyChanged
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        internal ObservableProperty(T observable, Expression<Func<TProperty>> propertyExpr) : base(observable)
        {

            var d = observable.WhenChanged(propertyExpr,
                                               value =>
                                               {
                                                   lock (this.InternalObservers)
                                                       foreach (var observer in this.InternalObservers)
                                                       {
                                                           try
                                                           {
                                                               if (observer.Filter(value))
                                                                   observer.OnNext(observable);
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               observer.OnError(ex);
                                                           }
                                                       }
                                               });

            _disposables.Add(d);
        }

        public ObservableProperty<T, TProperty> Do(Func<TProperty, bool> filter, Action<T> @do, Action<Exception> onError)
        {
            _disposables.Add(Subscribe(new FilterObserver(filter, @do, onError)));
            return this;
        }

        public ObservableProperty<T, TProperty> Do(Action<T> @do, Action<Exception> onError)
        {
            return Do(_ => true, @do, onError);
        }

        public ObservableProperty<T, TProperty> Do(Action<T> @do)
        {
            return Do(_ => true, @do, null);
        }

        public ObservableProperty<T, TProperty> Do(Func<TProperty, bool> filter, Action<T> @do)
        {
            return Do(filter, @do, null);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.Dispose();
                base.Dispose(disposing);
            }

        }

        private class FilterObserver : IFilterObserver<T, TProperty>
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
}
