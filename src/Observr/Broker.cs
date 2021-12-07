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
		private readonly Dictionary<Type, List<IObserver>> _observers = new Dictionary<Type, List<IObserver>>();

		public async Task Publish<TE>(TE value, CancellationToken cancellationToken = default)
		{
			lock (_observers)
			{
				if (_observers.ContainsKey(typeof(TE)))
				{
				
					foreach (IObserver<TE> observer in _observers[typeof(TE)])
						await observer.Handle(value, cancellationToken).ConfigureAwait(false);

				}
			}
		}

		public IDisposable Subscribe<TE>(IObserver<TE> observer)
		{
			lock (_observers)
			{
				if (_observers.ContainsKey(typeof(TE)) && _observers[typeof(TE)] is object)
				{
					_observers[typeof(TE)].Add(observer);
				}
				else
					_observers[typeof(TE)] = new List<IObserver> { observer };

			}
			return new UnSubscriber<TE>(_observers, observer);
		}
	}
}
