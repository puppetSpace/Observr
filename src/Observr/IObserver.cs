using System;
using System.Threading;
using System.Threading.Tasks;

namespace Observr
{
	/// <summary>
	/// Do not use this interface. This interface is only used so that a list of IObserver&lt;TE&gt; can be store
	/// </summary>
	public interface IObserver
	{

	}

	/// <summary>
	/// Defines an observer that can receive notifications of type TE
	/// </summary>
	/// <typeparam name="TE">Type of the notification</typeparam>
	public interface IObserver<TE> : IObserver
	{
		Task Handle(TE value, CancellationToken cancellationToken);
	}
}
