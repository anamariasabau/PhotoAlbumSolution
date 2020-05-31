using Fotos.Model;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fotos
{
    /// <summary>
    /// Image utilities class with methods for getting thumbnails and meta data
    /// </summary>
    public class ImageUtilities
    {
        /// <summary>
        /// ImagePath property
        /// </summary>
        public string ImagePath { get; private set; }
        /// <summary>
        /// Image property
        /// </summary>
        public MagickImage Image { get; private set; }
        public ImageUtilities(string imagePath)
        {
            this.ImagePath = imagePath;
            if(File.Exists(this.ImagePath))
                this.Image = new MagickImage(this.ImagePath);

        }
        /// <summary>
        /// Method for creating thumbnail 
        /// </summary>
        /// <param name="thumbnailWidth">Geoemtry width</param>
        /// <param name="thumbnailHeight">Geoemtry height</param>
        /// <returns>Thumbnail image</returns>
        public MagickImage GetImageThumbnail(int thumbnailWidth, int thumbnailHeight)
        {
            try
            {
                if (this.Image != null)
                {
                    MagickImage thumbnail = new MagickImage(this.Image);
                    thumbnail.Thumbnail(new MagickGeometry(thumbnailWidth, thumbnailHeight));
                    return thumbnail;
                }
                else return null;

            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Save image of MagickImage type to disk 
        /// </summary>
        /// <param name="image">Image object</param>
        /// <param name="imagePath">Image path</param>
        /// <returns>True if save is succesful, false otherwise</returns>
        public bool SaveImage(MagickImage image, string imagePath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));

                image.Write(imagePath);
                return true;

            }
            catch (IOException )
            {
                return false;
            }


        }

        /// <summary>
        /// Get metadata for an image
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Dictionary containing keys and values for metadata tags</returns>
        public Dictionary<string, string> GetMetaData(MagickImage image)
        {

            if (image != null)
            {
                var exifProfile = image.GetExifProfile();
                if (exifProfile != null)
                {
                    var exiProfileDict = exifProfile.Values.ToDictionary(o => o.Tag.ToString(), o => o.GetValue().ToString());
                    return exiProfileDict;
                }
                else
                    return null;
                
            }
            else
                return null;

        }

        /// <summary>
        /// Get metadata for class image
        /// </summary>
        /// <returns>Dictionary containing keys and values for metadata tags</returns>
        public Dictionary<string, string> GetMetaData()
        {
            try
            {
                if (this.Image != null)
                {
                    return GetMetaData(this.Image);
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Add metadata to database object
        /// </summary>
        /// <param name="metaData">Metadata dictionary</param>
        /// <param name="imageObject">Image database object</param>
        /// <returns>Updated image database object</returns>
        public Images AddMetaData( Dictionary<string,string> metaData, Images imageObject)
        {
            if (metaData != null)
            {
                string DateTimeOriginal = string.Empty;
                if (metaData.ContainsKey("DateTimeOriginal"))
                {
                    DateTimeOriginal = metaData["DateTimeOriginal"];
                    imageObject.DatetimeOriginal = DateTimeOriginal;
                }
                string DateTime = string.Empty;
                if (metaData.ContainsKey("DateTime"))
                {
                    DateTime = metaData["DateTime"];
                    imageObject.DatetimeModified = DateTime;
                }
                Int64? XResolution = null;
                if (metaData.ContainsKey("XResolution"))
                {
                    Int64 value = 0;
                    bool success = Int64.TryParse(metaData["XResolution"], out value);
                    imageObject.Xresolution = success == true ? value : XResolution;
                }

                Int64? YResolution = null;
                if (metaData.ContainsKey("YResolution"))
                {
                    Int64 value = 0;
                    bool success = Int64.TryParse(metaData["YResolution"], out value);
                    imageObject.Yresolution = success ? value : YResolution;
                }

              
            }
            return imageObject;
        }
    }
}
