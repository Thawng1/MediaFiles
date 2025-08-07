using HD.Station.Home.SqlServer.Abtractions;
using HD.Station.Home.SqlServer.Data;
using HD.Station.Home.SqlServer.Models;
using HD.Station.Home.SqlServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlServerDependencyInjectionExtensions
    {
        public static IServiceCollection UseSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HomeDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductStore, MediaFileStore>();

            return services;
        }
    }
}
