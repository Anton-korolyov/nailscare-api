using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsCare.Api.Data;
using NailsCare.Api.Models;

namespace NailsCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase

    {
        private readonly AppDbContext _db;

        public PricesController(AppDbContext db)
        {
            _db = db;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<List<Price>>> GetPrices()
        {
            var prices = await _db.Prices.ToListAsync();
            return Ok(prices);
        }

        // ================= ADD =================
        [HttpPost]
        public async Task<ActionResult<Price>> AddPrice([FromBody] Price price)
        {
            _db.Prices.Add(price);
            await _db.SaveChangesAsync();

            return Ok(price);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrice(int id, [FromBody] Price updated)
        {
            var price = await _db.Prices.FindAsync(id);

            if (price == null)
                return NotFound();

            price.Name = updated.Name;
            price.PriceValue = updated.PriceValue;

            await _db.SaveChangesAsync();

            return Ok(price);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrice(int id)
        {
            var price = await _db.Prices.FindAsync(id);

            if (price == null)
                return NotFound();

            _db.Prices.Remove(price);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
