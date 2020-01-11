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
                                                            if (observer.Filter(value))
                                                                observer.OnNext(observable);
                                                       }
                                               });

            _disposables.Add(d);
        }

        public ObservableProperty<T, TProperty> Do(Func<TProperty, bool> filter, Action<T> @do)
        {
            _disposables.Add(Subscribe(new FilterObserver<T, TProperty>(filter, @do)));
            return this;
        }

        public ObservableProperty<T, TProperty> Do(Action<T> @do)
        {
            return Do(_ => true, @do);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.Dispose();
                base.Dispose(disposing);
            }

        }

        


    }
}
