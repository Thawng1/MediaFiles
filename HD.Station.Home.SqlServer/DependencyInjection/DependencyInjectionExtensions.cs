using HD.Station.Home.SqlServer.Abtractions;
using HD.Station.Home.SqlServer.Data;
using HD.Station.Home.SqlServer.FeatureBuilder;
using HD.Station.Home.SqlServer.Store;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
namespace Microsoft.Extensions.DependencyInjection 
{
    public static class DependencyInjectionExtensions 
    {
        public static IHomeFeatureBuilder UseSqlServer(this IHomeFeatureBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddDbContext<HomeDbContext>(options =>
     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IProductStore, MediaFileStore>();


            return builder;
        }
    }
}
