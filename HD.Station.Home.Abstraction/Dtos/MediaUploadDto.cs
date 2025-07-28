using Microsoft.AspNetCore.Http;

namespace HD.Station.Home.Abstraction.Dtos
{
   public class MediaUploadDto
    {
        public IFormFile File { get; set; }
        public string UploadedBy { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
    }
}
