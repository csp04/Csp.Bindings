using System;
using System.Collections.Generic;

namespace Csp.Bindings
{
    internal class CompositeDisposable : IDisposable
    {
        private List<IDisposable> _disposables;

        public CompositeDisposable(params IDisposable[] disposables)
        {
            _disposables = new List<IDisposable>(disposables);
        }

        public IDisposable Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
            return this;
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var disposable in _disposables)
                        disposable?.Dispose();

                    _disposables = null;
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
