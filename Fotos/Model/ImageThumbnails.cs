using System;
using System.Collections.Generic;

namespace Fotos.Model
{
    public partial class ImageThumbnails
    {
        public long ImageId { get; set; }
        public long ThumbnailId { get; set; }

        public virtual Images Image { get; set; }
        public virtual Thumbnails Thumbnail { get; set; }
    }
}
