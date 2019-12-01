using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csp.Events.Core;

namespace Csp.Bindings.Observable
{
    public abstract class ObservableBase<T, TObserver> : IObservableBase, IObservable<T>, IDisposable
                                        where TObserver : IObserver<T>
    {
        private IList<TObserver> _observers;
        private T _observable;

        protected IList<TObserver> InternalObservers
        {
            get => _observers;
        }

        protected T InternalObservable
        {
            get => _observable;
        }

        protected ObservableBase(T observable)
        {
            _observable = observable;
            _observers = new List<TObserver>();
        }

        public IDisposable Subscribe(TObserver observer)
        {
            lock(_observers)
                _observers?.Add(observer);

            return Disposable.Create(() => _observers?.Remove(observer));
        }

        public virtual void Update()
        {
           
            if(_observers != null)
            {
                lock(_observers)
                {
                    var observers = _observers.ToList(); //create a copy
                    foreach (var obs in observers)
                        obs?.OnNext(_observable);
                }
            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock(_observers)
                    {
                        foreach (var obs in _observers)
                        {
                            obs?.OnCompleted();
                        }

                        _observers.Clear();
                        _observers = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ObservableBase()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
