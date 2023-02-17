using System;
using System.Diagnostics;

namespace ScrumPokerServer
{
    public partial class SessionMessage : EventArgs
    {
        private SessionMessage _next;

        private readonly TraceLevel _level;
        public TraceLevel Level { get { return _level; } }
        
        private readonly string _message;
        public string Message { get { return _message; } }
        
        private readonly Guid _concurrencyId;
        public Guid ConcurrencyId { get { return _concurrencyId; } }
        
        private SessionMessage(TraceLevel level, string message, SessionMessage previous)
        {
            _level = level;
            _message = message ?? "";
            if (previous != null)
                previous._next = this;
        } 
        
        private SessionMessage(TraceLevel level, string message, SessionMessage previous, Guid concurrencyId) : this(level, message, previous)  { _concurrencyId = concurrencyId; }
    }
}
