using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fotos.DbContexts;
using Fotos.Model;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fotos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Photos : ControllerBase
    {
        private readonly FotosDatabaseContext _context;
        public static IHostEnvironment _environment;

        private readonly ILogger<Photos> _logger;

        public Photos(ILogger<Photos> logger, IHostEnvironment environment, FotosDatabaseContext context)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
        }



        /// <summary>
        /// API GET method retrieving all of the photos including the photo content and thumbnails content 
        /// </summary>
        /// <returns>Json object with list of images</returns>
        [HttpGet]
        public IActionResult GetPhotos()
        {

            try
            {
                var query = from image in _context.Images
                            join it in _context.ImageThumbnails on image.Id equals it.ImageId
                            join thumbnail in _context.Thumbnails on it.ThumbnailId equals thumbnail.Id
                            select new PhotoResult() { ImageName = image.ImageName, ImageLength = image.ImageLength, ImageType = image.ImageType, ImagePath = image.ImagePath, Thumbnail = thumbnail.ThumbnailImage, XResolution = image.Xresolution,YResolution = image.Yresolution};

                var result = query.ToList();
                if (result != null && result.Count() != 0)
                {
                    return Ok(result);
                }
                else
                    return StatusCode(500, $"No images found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while getting list of images {ex.Message}");
            }

        }


        /// <summary>
        /// API GET method retrieving a single photo by photo id. A single image can have more than one thumbnail saved in the database
        /// </summary>
        /// <param name="id">integer representing the photo id</param>
        /// <returns>A list of the selected image and associated thumbnails</returns>
        [HttpGet("{id}")]
        public IActionResult  GetPhoto(int id)
        {
            try
            {
                var query = from image in _context.Images
                            join it in _context.ImageThumbnails on image.Id equals it.ImageId
                            join thumbnail in _context.Thumbnails on it.ThumbnailId equals thumbnail.Id
                            where image.Id == id
                            select new PhotoResult() { ImageName = image.ImageName,
                                                       ImageLength = image.ImageLength, 
                                                        ImagePath = image.ImagePath,
                                                        Thumbnail = thumbnail.ThumbnailImage, 
                                                        ImageContent = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath,image.ImagePath)),
                                                        XResolution = image.Xresolution,
                                                        YResolution = image.Yresolution};

                var result = query.ToList();
                if (result != null && result.Count() != 0)
                {
                    return Ok(result);
                }
                else
                    return StatusCode(500, $"Image not found for imageId: {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while getting specific image {ex.Message}");
            }

        }


        /// <summary>
        /// API POST method for uploading an image, if the image does not already exist in the database 
        /// The following operations are performed:
        /// - Save image to disk 
        /// - Insert meta data into the database 
        /// - Create thumbnail and save thumbnail to database 
        /// - Save image - thumbnail relationshop to the database 
        /// </summary>
        /// <returns>Success message or Failed message, including error </returns>
        [HttpPost]
        public async Task<IActionResult> PostImage()
        {
            //Thumbnail size can be taken from configurations or received as part of the request
            //Paths can be speicified as part of configurations
            int thumbnailWidth = 100;
            int thumbnailHeight = 100;
            string imagesDir = "Images";
            string thumbnailsDir = "Thumbnails";

            string imagesPath = Path.Combine(_environment.ContentRootPath, imagesDir);
            string thumbnailsPath = Path.Combine(_environment.ContentRootPath, thumbnailsDir);
            string fileName = "";

            string imagePath = "";
            string thumbnailPath = "";



            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);
            if (!Directory.Exists(thumbnailsPath))
                Directory.CreateDirectory(thumbnailsPath);


            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    fileName = file.FileName;

                    //Check if image already exists 
                    var query = from image in _context.Images
                                where image.ImageName == fileName
                                select image;

                    var result = query.FirstOrDefault();
                    if(result != null)
                       return BadRequest($"The image already exists in album");



                    string thumbnailFileName = string.Format("{0}_{1}_{2}.{3}", Path.GetFileNameWithoutExtension(file.FileName), thumbnailWidth, thumbnailHeight, Path.GetExtension(file.FileName));
                    imagePath = Path.Combine(imagesPath, file.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        if (!System.IO.File.Exists(imagePath))
                            return StatusCode(500, $"Error ocurred while saving image");
                    }

                    ImageUtilities imageUtilities = new ImageUtilities(imagePath);
                    MagickImage thumbnailImage = imageUtilities.GetImageThumbnail(thumbnailWidth, thumbnailHeight);
                    thumbnailPath = Path.Combine(thumbnailsPath, thumbnailFileName);
                    bool thumbnailSaved = imageUtilities.SaveImage(thumbnailImage, thumbnailPath);
                    if (!thumbnailSaved)
                    {
                        //Delete saved image 
                        System.IO.File.Delete(imagePath);
                        return StatusCode(500, $"Error ocurred while saving thumbnail");
                    }

                    Images imageObject = new Images()
                    {
                        ImageName = fileName,
                        ImageLength = file.Length,
                        ImageType = file.ContentType,
                        ImagePath = Path.Combine(imagesDir,fileName)
                    };



                    Dictionary<string, string> metaData = imageUtilities.GetMetaData();
                    imageObject = imageUtilities.AddMetaData(metaData, imageObject);

                    Thumbnails thumbnailObject = new Thumbnails() { FileName = thumbnailFileName, Height = 100, Width = 100, DimensionsUnit = "pixels", Path = Path.Combine(thumbnailsDir, thumbnailFileName), ThumbnailImage = thumbnailImage.ToByteArray(), CreatedTime = DateTime.UtcNow.ToLongDateString() };

                    bool dbAddSuccess = AddToDatabase(_context,imageObject, thumbnailObject);
                    if(!dbAddSuccess)
                    {
                        System.IO.File.Delete(imagePath);
                        System.IO.File.Delete(Path.Combine(thumbnailsPath, thumbnailFileName));
                        return StatusCode(500, $"Error ocurred while saving information to database");
                    }

                    return Ok($"Image was uploaded successfully");
                }
                else
                    return BadRequest("No image was uploaded");
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagesPath);
                if (!string.IsNullOrEmpty(thumbnailPath) && System.IO.File.Exists(thumbnailPath))
                    System.IO.File.Delete(thumbnailPath);
                return StatusCode(500, $"Error ocurred while uploading image {ex.Message}");
            }

        }

        /// <summary>
        /// Method for adding image and thumbnail information to database
        /// </summary>
        /// <param name="_context">entity framework context</param>
        /// <param name="imageObject">image object</param>
        /// <param name="thumbnailObject">thumbnail object</param>
        /// <returns>information whether addition to database has been performed suceesfully</returns>
        private bool AddToDatabase(FotosDatabaseContext _context,Images imageObject, Thumbnails thumbnailObject)
        {
            int createdImage = -1;
            int createdThumbnail = -1;
            int createdRelation = -1;
            ImageThumbnails imageThumbnailsObject = null;
            try
            { 
            _context.Images.Add(imageObject);

             createdImage = _context.SaveChanges();

            if(createdImage == -1)
                {
                    return false;
                }
            else
                { 
            _context.Thumbnails.Add(thumbnailObject);
             createdThumbnail = _context.SaveChanges();
                    if(createdThumbnail == -1)
                    {
                        _context.Remove(imageObject);
                        return false;
                    }
                    else
                    {

                        Int64 image_id = imageObject.Id;
                        Int64 thumbnail_id = thumbnailObject.Id;

                       imageThumbnailsObject =  new ImageThumbnails() { ImageId = image_id, ThumbnailId = thumbnail_id };
                        _context.ImageThumbnails.Add(imageThumbnailsObject);
                        createdRelation = _context.SaveChanges();
                        if (createdRelation > 0)
                            return true;
                        else
                        {
                            _context.Remove(imageObject);
                            _context.Remove(thumbnailObject);
                            return false;
                        }
                    }

                }

            }catch(Exception)
            {
                //Clean up
                if (createdImage != -1 && imageObject != null)
                    _context.Remove(imageObject);
                if (createdThumbnail != -1 && thumbnailObject != null)
                    _context.Remove(thumbnailObject);
                if (createdRelation != -1 && imageThumbnailsObject != null)
                    _context.Remove(imageThumbnailsObject);
                return false;
             
            }
        }
    }
}