using Fotos.DbContexts;
using Fotos.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestFotos
{
    class TestUtilities
    {

        public static Mock<FotosDatabaseContext> GetMockDatabaseContext()
        {
            var data = new List<Images>
            {
                new Images { Id = 1, ImagePath = "Images/A.jpg",ImageName = "A.jpg",ImageType="image/jpeg",ImageLength = 1   },
                new Images { Id = 2, ImagePath = "Images/B.jpg",ImageName = "B.jpg",ImageType="image/jpeg",ImageLength = 1   },
                new Images { Id = 3, ImagePath = "Images/C.jpg",ImageName = "C.jpg",ImageType="image/jpeg",ImageLength = 1   },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Images>>();
            mockSet.As<IQueryable<Images>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Images>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Images>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Images>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());


            var thumbnailsdData = new List<Thumbnails>
            {
                new Thumbnails { Id = 1, Path = "Thumbnails/A_100_100.jpg",FileName = "A_100_100.jpg",Width = 100, Height = 100  },
                new Thumbnails { Id = 2, Path = "Thumbnails/B_100_100.jpg",FileName = "B_100_100.jpg",Width = 100, Height = 100 },
                new Thumbnails { Id = 3, Path = "Thumbnails/C_100_100.jpg",FileName = "C_100_100.jpg",Width = 100, Height = 100  },
            }.AsQueryable();

            var mockSetThumbnails = new Mock<DbSet<Thumbnails>>();
            mockSetThumbnails.As<IQueryable<Thumbnails>>().Setup(m => m.Provider).Returns(thumbnailsdData.Provider);
            mockSetThumbnails.As<IQueryable<Thumbnails>>().Setup(m => m.Expression).Returns(thumbnailsdData.Expression);
            mockSetThumbnails.As<IQueryable<Thumbnails>>().Setup(m => m.ElementType).Returns(thumbnailsdData.ElementType);
            mockSetThumbnails.As<IQueryable<Thumbnails>>().Setup(m => m.GetEnumerator()).Returns(thumbnailsdData.GetEnumerator());


            var imageThumbnailsData = new List<ImageThumbnails>
            {
                new ImageThumbnails { ImageId = 1, ThumbnailId = 1  },
                 new ImageThumbnails { ImageId = 2, ThumbnailId = 2  },
                 new ImageThumbnails { ImageId = 3, ThumbnailId = 3  },
            }.AsQueryable();

            var mockSetImageThumbnails = new Mock<DbSet<ImageThumbnails>>();
            mockSetImageThumbnails.As<IQueryable<ImageThumbnails>>().Setup(m => m.Provider).Returns(imageThumbnailsData.Provider);
            mockSetImageThumbnails.As<IQueryable<ImageThumbnails>>().Setup(m => m.Expression).Returns(imageThumbnailsData.Expression);
            mockSetImageThumbnails.As<IQueryable<ImageThumbnails>>().Setup(m => m.ElementType).Returns(imageThumbnailsData.ElementType);
            mockSetImageThumbnails.As<IQueryable<ImageThumbnails>>().Setup(m => m.GetEnumerator()).Returns(imageThumbnailsData.GetEnumerator());


            var mockContext = new Mock<FotosDatabaseContext>();
            mockContext.Setup(c => c.Images).Returns(mockSet.Object);
            mockContext.Setup(c => c.Thumbnails).Returns(mockSetThumbnails.Object);
            mockContext.Setup(c => c.ImageThumbnails).Returns(mockSetImageThumbnails.Object);

            return mockContext;

        }
    }
}
