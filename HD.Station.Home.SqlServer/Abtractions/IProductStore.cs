using HD.Station.Home.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Station.Home.SqlServer.Abtractions
{
    public interface IProductStore
    {
        List<MediaFile> GetAll();
    }
}
