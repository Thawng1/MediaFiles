using HD.Station.Home.SqlServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HD.Station.Home.SqlServer.Data
{
    public class HomeDbContext : IdentityDbContext<AppUser>
    {
        public HomeDbContext(DbContextOptions<HomeDbContext> options)
            : base(options)
        {
        }

        public DbSet<MediaFile> MediaFiles { get; set; }
    }
}
