using System;

namespace Csp.Events.Core
{
    public class EventBindingHandler<D> : IDisposable where D : Delegate
    {
        readonly D _handler;
        readonly Action<D> _removeHandler;

        EventBindingHandler(D handler,
                                   Action<D> addHandler,
                                   Action<D> removeHandler)
        {
            if (handler == null)
                throw new ArgumentException(nameof(handler));

            if (addHandler == null)
                throw new ArgumentException(nameof(addHandler));

            if (removeHandler == null)
                throw new ArgumentException(nameof(removeHandler));

            addHandler(handler);
            _handler = handler;
            _removeHandler = removeHandler;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _removeHandler(_handler);
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public static EventBindingHandler<D> Create(D handler,
                                   Action<D> addHandler,
                                   Action<D> removeHandler)
        {
            return new EventBindingHandler<D>(handler, addHandler, removeHandler);
        }
    }
}
