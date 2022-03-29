using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Observr
{
    /// <summary>
    /// Is a mediator between the objects doing the notifying and objects receiving the notifications
    /// </summary>
    public class Broker : IBroker
    {
        private readonly Dictionary<Type, List<ObserverPair>> _observers = new Dictionary<Type, List<ObserverPair>>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _semaphorePublish = new SemaphoreSlim(1, 1);

        public async Task Publish<TE>(TE value, CancellationToken cancellationToken = default)
        {
            await _semaphorePublish.WaitAsync();
            try
            {
                if (_observers.ContainsKey(typeof(TE)))
                {
                    foreach (var pair in _observers[typeof(TE)].ToList()) // run over list of list doesn't cause concurrency issues
                    {
                        var observer = pair.Observer as IObserver<TE>;
                        await observer.Handle(value, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                _semaphorePublish.Release();
            }
        }

        public IDisposable Subscribe<TE>(IObserver<TE> observer)
        {
            _semaphore.Wait();
            var pair = new ObserverPair(observer);
            try
            {
                if (_observers.ContainsKey(typeof(TE)) && _observers[typeof(TE)] is object)
                    _observers[typeof(TE)].Add(pair);
                else
                    _observers[typeof(TE)] = new List<ObserverPair> { pair };
            }
            finally
            {
                _semaphore.Release();
            }

            return new UnSubscriber<TE>(_observers, pair);
        }
    }

    public class ObserverPair
    {
        public IObserver Observer { get; }
        public Guid Guid { get; }

        public ObserverPair(IObserver observer)
        {
            Observer = observer;
            Guid = Guid.NewGuid();
        }
    }
}
