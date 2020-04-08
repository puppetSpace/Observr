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
        public static IServiceCollection AddObservr(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.Add(new ServiceDescriptor(typeof(IBroker), typeof(Broker), serviceLifetime));

            return services;
        }
    }
}
