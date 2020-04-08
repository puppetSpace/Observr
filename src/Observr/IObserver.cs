using System;
using System.Threading;
using System.Threading.Tasks;

namespace Observr
{

	public interface IObserver
	{

	}

	public interface IObserver<TE> : IObserver
	{
		Task Handle(TE value, CancellationToken cancellationToken);
	}
}
