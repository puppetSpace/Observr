using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Observr
{
    internal class UnSubscriber<TE> : IDisposable
    {
        private readonly Dictionary<Type, List<IObserver>> _observers;
        private readonly IObserver<TE> _observer;

        public UnSubscriber(Dictionary<Type, List<IObserver>> observers, IObserver<TE> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            lock (_observers) {
                if (_observers.ContainsKey(typeof(TE)) && _observers[typeof(TE)].FirstOrDefault(x => x.GetType() == _observer.GetType()) is var observer)
                {
                    _observers[typeof(TE)].Remove(observer);
                    if (!_observers[typeof(TE)].Any())
                        _observers.Remove(typeof(TE));
                }
            }
        }
    }
}
