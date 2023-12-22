using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AthenaResturantWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        string imageFolder = "wwwroot";

        // GET: api/<ImagesController>
        [HttpGet]
        public IActionResult Get()
        {
            var imageFiles = Directory.GetFiles(imageFolder).Select(Path.GetFileName);
            return Ok(imageFiles);
        }

        // GET api/<ImagesController>/5
        [HttpGet("{filename}")]
        public IActionResult Get(string filename)
        {
                var imagePath = $"{imageFolder}/{filename}";
                var image = System.IO.File.OpenRead(imagePath);
                return File(image, "image/jpeg");
        }

        // POST api/<ImagesController>
        [HttpPost]
        public IActionResult Post(IFormFile image)
        {
            var imageName = image.FileName;
            var fullPath = $"{imageFolder}/{imageName}";

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            //return CreatedAtAction("GetImage", new { filename = imageName });
            return Ok(new {imagePath = fullPath});
        }

        /*
        // PUT api/<ImagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
        */

        // DELETE api/<ImagesController>/5
        [HttpDelete("{filename}")]
        public IActionResult Delete(string filename)
        {
            var imagePath = $"{imageFolder}/{filename}";
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
                return NoContent();
            }
            return NotFound();
        }
    }
}
