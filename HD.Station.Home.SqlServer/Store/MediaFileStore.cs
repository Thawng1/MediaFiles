using HD.Station.Home.SqlServer.Abtractions;
using HD.Station.Home.SqlServer.Data;
using HD.Station.Home.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Station.Home.SqlServer.Store
{
    public class MediaFileStore : IProductStore
    {
        private readonly HomeDbContext _context;

        public MediaFileStore(HomeDbContext context)
        {
            _context = context;
        }

        public List<MediaFile> GetAll()
        {
            return _context.MediaFiles.ToList();
        }
    }
}
