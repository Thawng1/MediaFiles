using HD.Station.Home.Mvc.Features.MediaFiles.Controllers;
using HD.Station.Home.Mvc.Features.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMvcFeature(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký service của MVC
            services.AddScoped<MediaService>();

            // Đăng ký Controllers + Views
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(MediaFileViewController).Assembly)
                .AddRazorRuntimeCompilation()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/Features/MediaFiles/Views/MediaFileView/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/MediaFiles/Views/Account/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Features/MediaFiles/Views/Shared/{0}.cshtml");
                    
                });

            return services;
        }
    }
}
