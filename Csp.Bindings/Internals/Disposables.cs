using System;
using System.Collections.Generic;

namespace Csp.Bindings
{
    class Disposables : IDisposable
    {
        IEnumerable<IDisposable> _disposables;

        public Disposables(params IDisposable[] disposables)
        {
            _disposables = disposables;
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
                        disposable.Dispose();

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
