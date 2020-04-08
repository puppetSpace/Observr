using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Observr
{
    public interface IBroker
    {
        IDisposable Subscribe<TE>(IObserver<TE> observer);

        Task Publish<TE>(TE value, CancellationToken cancellationToken = default);
    }
}
