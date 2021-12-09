using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Observr
{
    internal class UnSubscriber<TE> : IDisposable
    {
        private readonly Dictionary<Type, List<ObserverPair>> _observers;
        private readonly ObserverPair _observer;

        public UnSubscriber(Dictionary<Type, List<ObserverPair>> observers, ObserverPair observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.ContainsKey(typeof(TE)) && _observers[typeof(TE)].FirstOrDefault(x => x.Guid == _observer.Guid) is var observer)
            {
                _observers[typeof(TE)].Remove(observer);
                if (!_observers[typeof(TE)].Any())
                    _observers.Remove(typeof(TE));
            }
        }
    }
}
