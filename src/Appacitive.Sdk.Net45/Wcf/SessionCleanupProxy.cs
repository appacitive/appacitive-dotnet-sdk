using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class SessionCleanupProxy : ISession, IDisposable
    {
        public SessionCleanupProxy(ISession internalSession, TimeSpan heartbeat, TimeSpan expiryDuration)
        {
            this.Session = internalSession;
            _collector = new Timer( OnHeartbeat, null, TimeSpan.Zero, heartbeat);
            _expiryLimit = expiryDuration;
        }

        public ISession Session { get; private set; }
        private Timer _collector;
        private TimeSpan _expiryLimit;

        public object this[string key]
        {
            get
            {
                NotifyKeyActivity(key);
                return this.Session[key];
            }
            set
            {
                NotifyKeyActivity(key);
                this.Session[key] = value;
            }
        }

        public void Remove(string key)
        {
            this.Session.Remove(key);
            RemoveKey(key);
        }

        private void NotifyKeyActivity(string key)
        {
            TouchKey(key);
        }

        private void OnHeartbeat(object state)
        {
            // If busy then return.
            if (Interlocked.CompareExchange(ref _isBusy, YES, NO) != NO)
                return;
            try
            {
                // Iterate through all keys in the keys collection and expiry those with lifetime > expiry time.
                var existing = CloneKeys();
                var toBeCleaned = existing.Where(x => DateTime.Now - x.Value > _expiryLimit).ToList();
                toBeCleaned.ForEach(x => RemoveKey(x.Key));
            }
            finally
            {
                _isBusy = NO;
            }
        }

        private static readonly int YES = 1;
        private static readonly int NO = 0;
        private int _isBusy = NO;
        private readonly object _keyLock = new object();
        private Dictionary<string, DateTime> _keys = new Dictionary<string, DateTime>();

        private KeyValuePair<string, DateTime>[] CloneKeys()
        {
            lock (_keyLock)
            {
                return _keys.ToArray();
            }
        }

        private void TouchKey(string key)
        {
            lock (_keyLock)
            {
                _keys[key] = DateTime.Now;
            }
        }

        private void RemoveKey(string key)
        {
            lock (_keyLock)
            {
                if (_keys.ContainsKey(key) == true)
                    _keys.Remove(key);
            }
        }

        public void Dispose()
        {
            if (_collector != null)
            {
                _collector.Change(-1, Timeout.Infinite);
                _collector.Dispose();
                _collector = null;
            }
        }
    }
}
