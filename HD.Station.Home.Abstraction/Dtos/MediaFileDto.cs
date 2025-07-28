using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Station.Home.Abstraction.Dtos
{
   public class MediaFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string Format { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
        public string Status { get; set; }
        public string StoragePath { get; set; }
    }
}
