using Csp.Events.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Csp.Bindings.Observable
{
    public abstract class ObservableBase<T, TObserver> : IObservableBase, IObservable<T>, IDisposable
                                        where TObserver : IObserver<T>
    {
        protected IList<TObserver> InternalObservers { get; private set; }

        protected T InternalObservable { get; }

        protected ObservableBase(T observable)
        {
            InternalObservable = observable;
            InternalObservers = new List<TObserver>();
        }

        public IDisposable Subscribe(TObserver observer)
        {
            lock (InternalObservers)
                InternalObservers?.Add(observer);

            return Disposable.Create(() => InternalObservers?.Remove(observer));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return Subscribe((TObserver)observer);
        }


        public virtual void Update()
        {

            if (InternalObservers != null)
            {
                lock (InternalObservers)
                {
                    foreach (var obs in InternalObservers)
                        obs?.OnNext(InternalObservable);
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
                    lock (InternalObservers)
                    {
                        InternalObservers.Clear();
                        InternalObservers = null;
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

        

        #endregion
    }
}
