using Microsoft.EntityFrameworkCore;
using HD.Station.Home.SqlServer.Models;

namespace HD.Station.Home.SqlServer.Data
{
    public class HomeDbContext : DbContext
    {
        public HomeDbContext(DbContextOptions<HomeDbContext> options)
            : base(options)
        {
        }

        public DbSet<MediaFile> MediaFiles { get; set; }
    }
}
