using System;

namespace Csp.Events.Core
{
    

    public class EventBindingHandler<TEvent> : IDisposable where TEvent : Delegate
    {
        private TEvent _handler;
        private Action<TEvent> _removeHandler;

        EventBindingHandler(TEvent handler,
                                   Action<TEvent> addHandler,
                                   Action<TEvent> removeHandler)
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
                    _handler = null;
                    _removeHandler = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        public static EventBindingHandler<TEvent> Create(TEvent handler,
                                   Action<TEvent> addHandler,
                                   Action<TEvent> removeHandler)
        {
            return new EventBindingHandler<TEvent>(handler, addHandler, removeHandler);
        }

        public static EventBindingHandler<TEvent> Create(object src, string eventName, TEvent handler)
        {
            var type = src.GetType();
            var ev = type.GetEvent(eventName);

            return Create(handler,
                            x => ev.AddEventHandler(src, x),
                            x => ev.RemoveEventHandler(src, x));
        }
    }
}
