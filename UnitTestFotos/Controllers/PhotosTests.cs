using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fotos.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Fotos.DbContexts;
using System.Linq;
using Moq;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Fotos.Model;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using UnitTestFotos;

namespace Fotos.Controllers.Tests
{
    [TestClass()]
    public class PhotosTests
    {

        [TestMethod()]
        public void GetPhotosTest()
        {
            List<string> imageNames = new List<string> { "A.jpg", "B.jpg", "C.jpg" };
            Mock<IHostEnvironment> environment = new Mock<IHostEnvironment>();
            var directory = Directory.GetCurrentDirectory();
            environment.Setup(c => c.ContentRootPath).Returns(directory);

            Mock<ILogger<Photos>> logger = new Mock<ILogger<Photos>>();

            Mock<FotosDatabaseContext> mockContext = TestUtilities.GetMockDatabaseContext();
            Photos photosController = new Photos(logger.Object, environment.Object, mockContext.Object);


            var result = photosController.GetPhotos() as ObjectResult;
            var items = result.Value as List<PhotoResult>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(3, items.Count);
            foreach(var name in imageNames)
            {
                Assert.IsTrue(items.Select(o => o.ImageName).Contains(name));
            }
            
        }

        [TestMethod()]
        public void GetPhotoTest_RequestInvalidId()
        {
            int id = -1;

            Mock<IHostEnvironment> environment = new Mock<IHostEnvironment>();
            Mock<ILogger<Photos>> logger = new Mock<ILogger<Photos>>();

            Mock<FotosDatabaseContext> mockContext = TestUtilities.GetMockDatabaseContext();
            Photos photosController = new Photos(logger.Object, environment.Object, mockContext.Object) ;

            
            var result = photosController.GetPhoto(id) as ObjectResult;

            // Assert
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual(string.Format("Image not found for imageId: {0}", id), result.Value);
        }


        [TestMethod()]
        public void GetPhotoTest_RequestValid()
        {
            int id = 1;
            string imageName = "A.jpg";

            Mock<IHostEnvironment> environment = new Mock<IHostEnvironment>();
            var directory = Directory.GetCurrentDirectory();
            environment.Setup(c => c.ContentRootPath).Returns(directory);

            Mock<ILogger<Photos>> logger = new Mock<ILogger<Photos>>();

            Mock<FotosDatabaseContext> mockContext = TestUtilities.GetMockDatabaseContext();
            Photos photosController = new Photos(logger.Object, environment.Object, mockContext.Object);


            var result = photosController.GetPhoto(id) as ObjectResult;
            var items = result.Value as List<PhotoResult>;

            // Assert
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(1,items.Count);
            foreach (var entry in items)
                Assert.AreEqual(imageName, entry.ImageName);

        }
       
    }
}