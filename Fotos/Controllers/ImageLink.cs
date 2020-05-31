using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fotos.DbContexts;
using Fotos.Model;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fotos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageLink : ControllerBase
    {
        private readonly FotosDatabaseContext _context;
        public static IHostEnvironment _environment;


        private readonly ILogger<ImageLink> _logger;

        public ImageLink(ILogger<ImageLink> logger, IHostEnvironment environment, FotosDatabaseContext context)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _context = context;
        }

        /// <summary>
        /// API GET method to retrieve a shortened link to an image. 
        /// Link shortning is done using call to rebrandly API
        /// If link shortning fails, the full link is returned 
        /// Note: rebrandly API call does not recognize localhost calls as valid links 
        /// </summary>
        /// <param name="id">image identifier for which link should be returned</param>
        /// <returns>Shortened link to image</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var query = from image in _context.Images
                            where image.Id == id
                            select new { ImageName = image.ImageName, ImageLength = image.ImageLength, ImagePath = image.ImagePath, ImageType = image.ImageType };

                var result = query.ToList();
                var imageInfo = result.FirstOrDefault();
                if (result != null && result.Count() != 0 && imageInfo != null)
                {
                    string relativePath = imageInfo.ImagePath;
                    string fileName = Path.Combine(_environment.ContentRootPath, relativePath);
                    if (!System.IO.File.Exists(fileName))
                        return StatusCode(500, $"Image not found on the server for id: {id}");
                    else
                    {
                        if(Request != null)
                        { 
                        var baseUrl = new Uri($"{ Request.Scheme }://{Request.Host}{Request.PathBase}");
                        var absoluteUrl = new Uri(baseUrl, relativePath);

                        var link = await this.ShortenLink(absoluteUrl); 
                        if(link != null) 
                             return Ok(link);
                        else 
                            return Ok(absoluteUrl);
                        }
                        else
                            return StatusCode(500, $"Request object is null");
                    }
                }
                else
                    return StatusCode(500, $"Image not found for id: {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error ocurred while getting specific image {ex.Message}");
            }

        }

        /// <summary>
        /// Method performing link shortnening through call to rebrand.ly API
        /// </summary>
        /// <param name="url">URL to be shortened</param>
        /// <returns>Shortened URL or null if API call failed</returns>
        public async Task<string> ShortenLink(Uri url)
        {

            var payload = new
            {
                destination = url,
                domain = new
                {
                    fullName = "rebrand.ly"
                }
            };

            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.rebrandly.com") })
            {
                httpClient.DefaultRequestHeaders.Add("apikey", "9c144b556b544d3ab1c7e9c87e33cd40");
                httpClient.DefaultRequestHeaders.Add("workspace", "0ea846cb13fd4ae2b6a45eb5d0fd21cf");

                var body = new StringContent(
                    JsonConvert.SerializeObject(payload), System.Text.UnicodeEncoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("/v1/links", body))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();

                        String result = await response.Content.ReadAsStringAsync();
                        string link = null;
                        foreach (var x in JObject.Parse(result))
                        {
                            if (x.Key == "shortUrl")
                            {
                                link = x.Value.ToString();
                                break;
                            }

                        }

                        return link;

                            
                    }

                    return null;
                }

            }
        }
    }
}