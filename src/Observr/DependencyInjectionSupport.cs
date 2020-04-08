using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observr
{
    public static class DependencyInjectionSupport
    {
        public static IServiceCollection AddObservr(this IServiceCollection services)
        {
            services.AddSingleton<IBroker, Broker>();

            return services;
        }
    }
}
