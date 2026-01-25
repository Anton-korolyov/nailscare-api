using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsCare.Api.Data;
using NailsCare.Api.Models;

namespace NailsCare.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;

        public GalleryController(IWebHostEnvironment env, AppDbContext db)
        {
            _env = env;
            _db = db;
        }

        // PUBLIC (сайт/React)
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string category)
        {
            category = (category ?? "").Trim().ToLower();

            if (category != "manicure" && category != "pedicure")
                return BadRequest("category must be manicure or pedicure");

            var images = await _db.GalleryImages
                .Where(x => x.Category == category)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(images);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(15_000_000)]
        public async Task<IActionResult> Upload(
    [FromForm] IFormFile file,
    [FromForm] string category)
        {
            if (file == null || file.Length == 0)
                return BadRequest("file is required");

            category = (category ?? "").Trim().ToLower();

            if (category != "manicure" && category != "pedicure")
                return BadRequest("category must be manicure or pedicure");

            // 🔥 ВАЖНО: путь совпадает с volume docker
            var uploadsRoot = Path.Combine("/app/uploads", category);
            Directory.CreateDirectory(uploadsRoot);

            var ext = Path.GetExtension(file.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(ext))
                return BadRequest("only jpg/jpeg/png/webp allowed");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            var image = new GalleryImage
            {
                Category = category,
                ImageUrl = $"/uploads/{category}/{fileName}"
            };

            _db.GalleryImages.Add(image);
            await _db.SaveChangesAsync();

            return Ok(image);
        }

        // ONLY ADMIN (Android)
        //[ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "Admin")]
        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //[RequestSizeLimit(15_000_000)]
        //public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string category)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("file is required");

        //    category = (category ?? "").Trim().ToLower();
        //    if (category != "manicure" && category != "pedicure")
        //        return BadRequest("category must be manicure or pedicure");

        //    var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", category);
        //    Directory.CreateDirectory(uploadsRoot);

        //    var ext = Path.GetExtension(file.FileName).ToLower();
        //    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        //    if (!allowed.Contains(ext))
        //        return BadRequest("only jpg/jpeg/png/webp allowed");

        //    var fileName = $"{Guid.NewGuid()}{ext}";
        //    var fullPath = Path.Combine(uploadsRoot, fileName);

        //    await using (var stream = System.IO.File.Create(fullPath))
        //        await file.CopyToAsync(stream);

        //    var image = new GalleryImage
        //    {
        //        Category = category,
        //        ImageUrl = $"/uploads/{category}/{fileName}"
        //    };

        //    _db.GalleryImages.Add(image);
        //    await _db.SaveChangesAsync();

        //    return Ok(image);
        //}

        // ONLY ADMIN (удаление)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var img = await _db.GalleryImages.FirstOrDefaultAsync(x => x.Id == id);
            if (img == null) return NotFound();

            var physical = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physical))
                System.IO.File.Delete(physical);

            _db.GalleryImages.Remove(img);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
