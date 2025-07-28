using Microsoft.Extensions.DependencyInjection;

namespace HD.Station.Home.SqlServer.FeatureBuilder
{
    public interface IHomeFeatureBuilder
    {
        IServiceCollection Services { get; }
    }

    public class HomeFeatureBuilder : IHomeFeatureBuilder
    {
        public HomeFeatureBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
