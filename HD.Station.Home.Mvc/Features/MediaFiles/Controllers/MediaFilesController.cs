using HD.Station.Home.Abstraction.Configuration;
using HD.Station.Home.Abstraction.Dtos;
using HD.Station.Home.Mvc.Features.Service;
using HD.Station.Home.SqlServer.Abtractions;
using HD.Station.Home.SqlServer.Data;
using HD.Station.Home.SqlServer.Models;
using HD.Station.Home.SqlServer.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace HD.Station.Home.Mvc.Features.MediaFiles.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class MediaFilesController : ControllerBase
    {
        private readonly MediaService _mediaService;
        private readonly HomeDbContext _context;
    private readonly MediaStorageOptions _storageOptions;

    public MediaFilesController(MediaService mediaService, HomeDbContext context, IOptions<MediaStorageOptions> options)
    {
            _mediaService = mediaService;
            _context = context;
            _storageOptions = options.Value;
    }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] MediaUploadDto dto)
        {
            var media = await _mediaService.SaveFileAsync(dto);
            _context.MediaFiles.Add(media);
            await _context.SaveChangesAsync();
            return Ok(media);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var files = _context.MediaFiles
                .Where(f => f.Status == "Active")
                .Select(f => new MediaFileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileType = f.FileType,
                    FileSize = f.FileSize,
                    Format = f.Format,
                    UploadedAt = f.UploadedAt,
                    UploadedBy = f.UploadedBy,
                    Status = f.Status,
                    StoragePath = f.StoragePath
                }).ToList();
            return Ok(new
            {
                files.Count,
                Items = files
            });

        }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] MediaFileDto dto)
    {
        var file = await _context.MediaFiles.FindAsync(id);
        if (file == null || file.Status != "Active") return NotFound();

        file.FileName = dto.FileName;
        file.UploadedBy = dto.UploadedBy;
        // Cập nhật các thuộc tính khác nếu cần

        await _context.SaveChangesAsync();
        return Ok(file);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var file = await _context.MediaFiles.FindAsync(id);
        if (file == null) return NotFound();

        return Ok(file); // Trả về toàn bộ model gốc
    }


    [HttpGet("play/{id}")]
    public IActionResult Play(int id)
    {
        var file = _context.MediaFiles.Find(id);
        if (file == null) return NotFound();

        string physicalPath;

        if (file.StoragePath.StartsWith("ftp://"))
        {
            return BadRequest("Không thể stream trực tiếp từ FTP");
        }
        else if (file.StoragePath.StartsWith("/uploads"))
        {
            // Local
            physicalPath = Path.Combine(_storageOptions.LocalPath, file.StoragePath.TrimStart('/'));
        }
        else
        {
            // UNC
            physicalPath = Path.Combine(_storageOptions.UNCPath, file.StoragePath);
        }

        if (!System.IO.File.Exists(physicalPath))
            return NotFound($"Không tìm thấy file tại {physicalPath}");

        var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read);
        var mime = GetMimeType(file.Format);
        return File(stream, mime);
    }




    [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var file = await _context.MediaFiles.FindAsync(id);
            if (file == null) return NotFound();
            file.Status = "Deleted";
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private string GetMimeType(string ext)
        {
            return ext.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".avi" => "video/x-msvideo",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }

