using System;
using System.Collections.Generic;

namespace Fotos.Model
{
    public partial class Images
    {
        public Images()
        {
            ImageThumbnails = new HashSet<ImageThumbnails>();
        }

        public long Id { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public long ImageLength { get; set; }
        public string ImageType { get; set; }
        public string DatetimeOriginal { get; set; }
        public string DatetimeModified { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public long? Xresolution { get; set; }
        public long? Yresolution { get; set; }
        public string Software { get; set; }
        public long? ExposureIme { get; set; }
        public string Fnumber { get; set; }
        public string Colorspace { get; set; }

        public virtual ICollection<ImageThumbnails> ImageThumbnails { get; set; }
    }
}
