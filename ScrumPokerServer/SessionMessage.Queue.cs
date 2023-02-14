using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ScrumPokerServer
{
    public partial class SessionMessage
    {
        public sealed class Queue : IProducerConsumerCollection<SessionMessage>
        {
            private readonly int? _maxCapacity;
            private object _changeToken = new object();
            private readonly AutoResetEvent _messageEnqueued;
            private bool _isDisposed = false;
            private SessionMessage _first = null;
            private SessionMessage _last = null;
            
            private int _count = 0;
            int Count { get { return _count; } }

            private readonly object _syncRoot = new object();
            object SyncRoot { get { return _syncRoot; } }

            bool ICollection.IsSynchronized { get { return true; } }

            public Queue(int? maxCapacity = null)
            {
                if (maxCapacity.HasValue && maxCapacity.Value < 1)
                    throw new ArgumentOutOfRangeException("maxCapacity");
                _maxCapacity = maxCapacity;
                _messageEnqueued = new AutoResetEvent(false);
            }

            public bool TryAdd(TraceLevel level, string message)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException("SessionMessage");
                    if (_maxCapacity.HasValue && _maxCapacity.Value <= _count)
                        return false;
                    _last = new SessionMessage(level, message, _last);
                    if (_first == null)
                        _first = _last;
                    _count++;
                    _changeToken = new object();
                }
                finally { Monitor.Exit(_syncRoot); }
                _messageEnqueued.Set();
            }

            public bool TryAdd(TraceLevel level, string message, Guid concurrencyId)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException("SessionMessage");
                    if (_maxCapacity.HasValue && _maxCapacity.Value <= _count)
                        return false;
                    _last = new SessionMessage(level, message, _last, concurrencyId);
                    if (_first == null)
                        _first = _last;
                    _count++;
                    _changeToken = new object();
                }
                finally { Monitor.Exit(_syncRoot); }
                _messageEnqueued.Set();
            }

            bool IProducerConsumerCollection<SessionMessage>.TryAdd(SessionMessage item) { return false; }

            public bool TryTake(int millisecondsTimeout, out SessionMessage item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException("SessionMessage");
                    if ((item = _first) != null)
                    {
                        if ((_first = item._next) != null)
                            item._next = null;
                        _count--;
                        _changeToken = new object();
                        _messageEnqueued.Reset();
                        return true;
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _messageEnqueued.WaitOne(millisecondsTimeout) && TryTake(out item);
            }

            public bool TryTake(TimeSpan timeout, out SessionMessage item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException("SessionMessage");
                    if ((item = _first) != null)
                    {
                        if ((_first = item._next) != null)
                            item._next = null;
                        _count--;
                        _changeToken = new object();
                        _messageEnqueued.Reset();
                        return true;
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
                return _messageEnqueued.WaitOne(timeout) && TryTake(out item);
            }

            public bool TryTake(out SessionMessage item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException("SessionMessage");
                    if ((item = _first) == null)
                        return false;
                    if ((_first = item._next) != null)
                        item._next = null;
                    _count--;
                    _changeToken = new object();
                }
                finally { Monitor.Exit(_syncRoot); }
                return true;
            }

            void  IProducerConsumerCollection<SessionMessage>.CopyTo(SessionMessage[] array, int index) { return GetItems().ToList().CopyTo(array, arrayIndex); }

            void ICollection.CopyTo(Array array, int index) { ToArray().CopyTo(array, index); }

            SessionMessage[] IProducerConsumerCollection<SessionMessage>.ToArray() { return GetItems().ToArray(); }

            private IEnumerable<SessionMessage> GetItems()
            {
                if (_isDisposed)
                    throw new ObjectDisposedException("SessionMessage");
                object changeToken = _changeToken;
                for (SessionMessage message = _first; message != null; message = message._next)
                {
                    if (!ReferenceEquals(changeToken, _changeToken))
                        throw new InvalidOperationException(_isDisposed ? "Parent collection has been disposed." : "Source collection has changed.");
                    yield return message;
                }
            }

            public IEnumerator<SessionMessage> GetEnumerator() { return GetItems().GetEnumerator(); }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetItems().ToArray().GetEnumerator(); }

            private override void Dispose(bool disposing)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_isDisposed)
                        return;
                    _isDisposed = true;
                    if (disposing)
                    {
                        _changeToken = new object();
                        _messageEnqueued.Dispose();
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }
        }
    }
}
