using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fotos.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.IO;
using Fotos.DbContexts;
using UnitTestFotos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fotos.Controllers.Tests
{
    [TestClass()]
    public class ImageLinkTests
    {
       
        [TestMethod()]
        public async Task Get_WhenCalled_ReturnsEmptyRequest()
        {
            
            int id = 1;
            Mock<IHostEnvironment> environment = new Mock<IHostEnvironment>();
            var directory = Directory.GetCurrentDirectory();
            environment.Setup(c => c.ContentRootPath).Returns(directory);

            Mock<ILogger<ImageLink>> logger = new Mock<ILogger<ImageLink>>();

            Mock<FotosDatabaseContext> mockContext = TestUtilities.GetMockDatabaseContext();
            ImageLink imageLinkController = new ImageLink(logger.Object, environment.Object, mockContext.Object);


            var result = await imageLinkController.Get(id) as ObjectResult;

            //Assert 
            Assert.AreEqual(500, result.StatusCode);
        }


        [TestMethod()]
        public async Task Get_WhenCalled_ReturnsOk()
        {

            int id = 1;

            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Scheme = "htttp";
            httpContext.Request.Host = new HostString("localhost:4437");
            httpContext.Request.PathBase = "";

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            Mock <IHostEnvironment> environment = new Mock<IHostEnvironment>();
            var directory = Directory.GetCurrentDirectory();
            environment.Setup(c => c.ContentRootPath).Returns(directory);

            Mock<ILogger<ImageLink>> logger = new Mock<ILogger<ImageLink>>();

            Mock<FotosDatabaseContext> mockContext = TestUtilities.GetMockDatabaseContext();
            ImageLink imageLinkController = new ImageLink(logger.Object, environment.Object, mockContext.Object)
            {
                ControllerContext = controllerContext,
            };


            var result = await imageLinkController.Get(id) as ObjectResult;

            //Assert 
            Assert.AreEqual(200, result.StatusCode);
        }
    }
}