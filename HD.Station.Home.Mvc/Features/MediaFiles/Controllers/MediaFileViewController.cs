using HD.Station.Home.SqlServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HD.Station.Home.Mvc.Features.MediaFiles.Controllers
{
    public class MediaFileViewController : Controller
    {
        private readonly HomeDbContext _context;

        public MediaFileViewController(HomeDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public IActionResult Index()
        {
            var files = _context.MediaFiles
                .Where(f => f.Status == "Active")
                .ToList();

            return View(files);
        }

        public IActionResult Upload()
        {
            return View();
        }
    }
}
