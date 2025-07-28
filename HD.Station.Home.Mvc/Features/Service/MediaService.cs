using HD.Station.Home.SqlServer.Models;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;
using HD.Station.Home.Abstraction.Dtos;
namespace HD.Station.Home.Mvc.Features.Service
{
  public class MediaService
    {
        private readonly IWebHostEnvironment _env;

        public MediaService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<MediaFile> SaveFileAsync(MediaUploadDto dto)
        {
            var file = dto.File;
            if (file == null || file.Length == 0) throw new Exception("File not valid");

            string ext = Path.GetExtension(file.FileName);
            string fileType = GetFileType(ext);
            string fileName = Guid.NewGuid() + ext;


            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fullPath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string hash = await CalculateFileHash(fullPath);

            var media = new MediaFile
            {
                FileName = file.FileName,
                FileType = fileType,
                FileSize = file.Length,
                Format = ext,
                UploadedAt = DateTime.Now,
                UploadedBy = dto.UploadedBy,
                StoragePath = $"/uploads/{fileName}",
                Tags = dto.Tags,
                Description = dto.Description,
                Status = "Active",
                Hash = hash
            };

            return media;
        }

        private string GetFileType(string ext)
        {
            ext = ext.ToLower();
            if (new[] { ".jpg", ".png", ".gif" }.Contains(ext)) return "image";
            if (new[] { ".mp4", ".avi" }.Contains(ext)) return "video";
            if (new[] { ".mp3", ".wav" }.Contains(ext)) return "audio";
            if (new[] { ".pdf", ".docx" }.Contains(ext)) return "doc";
            return "unknown";
        }

        private async Task<string> CalculateFileHash(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();
            var hash = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}

