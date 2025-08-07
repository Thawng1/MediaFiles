using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Station.Home.Abstraction.Configuration
{
        public class MediaStorageOptions
        {
            public string StorageMode { get; set; } = "Local";
            public string LocalPath { get; set; }
            public string UNCPath { get; set; }
            public FtpOptions Ftp { get; set; }
        }

        public class FtpOptions
        {
            public string Host { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

}
