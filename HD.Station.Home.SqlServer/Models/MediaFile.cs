namespace HD.Station.Home.SqlServer.Models
{
    public class MediaFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string Format { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
        public string StoragePath { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Hash { get; set; }
        public string? MediaInfo { get; set; }
    }
}
