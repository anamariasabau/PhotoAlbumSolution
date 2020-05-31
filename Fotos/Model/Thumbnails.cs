using System;
using System.Collections.Generic;

namespace Fotos.Model
{
    public partial class Thumbnails
    {
        public Thumbnails()
        {
            ImageThumbnails = new HashSet<ImageThumbnails>();
        }

        public long Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public string DimensionsUnit { get; set; }
        public byte[] ThumbnailImage { get; set; }
        public string Path { get; set; }
        public string CreatedTime { get; set; }

        public virtual ICollection<ImageThumbnails> ImageThumbnails { get; set; }
    }
}
