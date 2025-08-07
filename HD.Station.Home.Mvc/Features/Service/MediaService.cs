
using HD.Station.Home.Abstraction.Configuration;
using HD.Station.Home.Abstraction.Dtos;
using HD.Station.Home.SqlServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;

namespace HD.Station.Home.Mvc.Features.Service
{
    public class MediaService
    {
        private readonly IWebHostEnvironment _env;
        private readonly MediaStorageOptions _storageOptions;

        public MediaService(IWebHostEnvironment env, IOptions<MediaStorageOptions> options)
        {
            _env = env;
            _storageOptions = options.Value;
        }

        public async Task<MediaFile> SaveFileAsync(MediaUploadDto dto)
        {
            var file = dto.File;
            if (file == null || file.Length == 0)
                throw new Exception("File không hợp lệ");

            string ext = Path.GetExtension(file.FileName);
            string fileType = GetFileType(ext);
            string fileName = Guid.NewGuid() + ext;
            string method = string.IsNullOrEmpty(dto.UploadMethod)
                ? _storageOptions.StorageMode
                : dto.UploadMethod;

            string fullPath = "";
            string storagePath = "";

            switch (method.ToLower())
            {
                case "local":
                    fullPath = Path.Combine(_storageOptions.LocalPath, "uploads", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    storagePath = $"/uploads/{fileName}";
                    break;

                case "unc":
                    fullPath = Path.Combine(_storageOptions.UNCPath, fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    storagePath = fileName;
                    break;

                case "ftp":
                    storagePath = await SaveFileFtpAsync(file, fileName);
                    break;


                default:
                    throw new Exception("Chế độ lưu trữ không hợp lệ.");
            }

            string hash = (method.ToLower() != "ftp" && File.Exists(fullPath))
                ? await CalculateFileHash(fullPath)
                : "";

            return new MediaFile
            {
                FileName = file.FileName,
                FileType = fileType,
                FileSize = file.Length,
                Format = ext,
                UploadedAt = DateTime.Now,
                UploadedBy = dto.UploadedBy,
                StoragePath = storagePath,
                Tags = dto.Tags,
                Description = dto.Description,
                Status = "Active",
                Hash = hash
            };
        }
        public async Task<string> SaveFileFtpAsync(IFormFile file, string fileName)
        {
            string ftpHost = _storageOptions.Ftp.Host; // vd: "127.0.0.1"
            string ftpUsername = _storageOptions.Ftp.Username;
            string ftpPassword = _storageOptions.Ftp.Password;
            string remoteDirectory = "/uploads/";
            string storagePath = remoteDirectory + fileName;

            // Bỏ qua kiểm tra SSL (nếu cần)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            // Tạo thư mục trên FTP nếu chưa có (bỏ qua lỗi nếu đã tồn tại)
            try
            {
                var mkDirRequest = (FtpWebRequest)WebRequest.Create($"ftp://{ftpHost}{remoteDirectory}");
                mkDirRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                mkDirRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                mkDirRequest.EnableSsl = true;    // nếu FTP server yêu cầu
                mkDirRequest.UsePassive = true;
                mkDirRequest.UseBinary = true;
                mkDirRequest.KeepAlive = false;
                using var mkDirResponse = (FtpWebResponse)await mkDirRequest.GetResponseAsync();
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                    throw; // Lỗi khác thì throw
                           // Nếu thư mục đã tồn tại thì bỏ qua
            }

            // Upload file
            var uploadRequest = (FtpWebRequest)WebRequest.Create($"ftp://{ftpHost}{storagePath}");
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            uploadRequest.EnableSsl = true;      // nếu cần
            uploadRequest.UsePassive = true;
            uploadRequest.UseBinary = true;
            uploadRequest.KeepAlive = false;

            using (var fileStream = file.OpenReadStream())
            using (var ftpStream = await uploadRequest.GetRequestStreamAsync())
            {
                await fileStream.CopyToAsync(ftpStream);
            }

            using var uploadResponse = (FtpWebResponse)await uploadRequest.GetResponseAsync();
            if (uploadResponse.StatusCode != FtpStatusCode.ClosingData)
            {
                throw new Exception($"FTP upload failed: {uploadResponse.StatusDescription}");
            }

            return storagePath;
        }
        private string GetFileType(string ext)
        {
            ext = ext.ToLower();
            if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }.Contains(ext)) return "image";
            if (new[] { ".mp4", ".avi", ".mov", ".webm" }.Contains(ext)) return "video";
            if (new[] { ".mp3", ".wav", ".ogg" }.Contains(ext)) return "audio";
            if (new[] { ".pdf", ".docx", ".doc", ".xls" }.Contains(ext)) return "doc";
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
