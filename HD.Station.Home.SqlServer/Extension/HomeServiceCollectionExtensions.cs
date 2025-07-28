using HD.Station.Home.SqlServer.FeatureBuilder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HomeServiceCollectionExtensions
    {
        public static IHomeFeatureBuilder AddHomeFeature(this IServiceCollection services, IConfiguration configuration)
        {
            return new HomeFeatureBuilder(services);
        }
    }
}
