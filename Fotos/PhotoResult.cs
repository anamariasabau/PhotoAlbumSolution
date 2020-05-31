using System;
using System.Reflection.Metadata;
using System.Security.Policy;

namespace Fotos
{
    public class PhotoResult
    {

        public string ImageName { get; set; }
        public Int64 ImageLength { get;set; }
        public string ImagePath { get; set; }
        public string ImageType { get; set; }
        public long? XResolution { get; set; }
        public long? YResolution { get; set; }
        public byte[] ImageContent { get; set; }
        public byte[] Thumbnail { get; set;  }
    }
}
