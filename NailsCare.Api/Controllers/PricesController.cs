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

        // ================= GET ALL PRICES =================
        [HttpGet]
        public async Task<ActionResult<List<Price>>> GetPrices()
        {
            var prices = await _db.Prices.ToListAsync();
            return Ok(prices);
        }
    }
}
