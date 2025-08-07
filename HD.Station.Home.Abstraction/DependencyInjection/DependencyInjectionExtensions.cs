using HD.Station.Home.Abstraction.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Station.Home.Abstraction.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAbstractionFeature(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MediaStorageOptions>(configuration.GetSection("MediaStorage"));
            return services;
        }
    }

}
