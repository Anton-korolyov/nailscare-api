using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NailsCare.Api.Data;
using NailsCare.Api.Models;

namespace NailsCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AppointmentsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("month")]
        public async Task<ActionResult<List<Appointment>>> GetMonth(int year, int month)
        {
            var list = await _db.Appointments
                .Where(x => x.Date.Year == year && x.Date.Month == month)
                .ToListAsync();

            return Ok(list);
        }
        // GET appointments by date
        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> Get(DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

            var list = await _db.Appointments
                .Where(x => x.Date.Date == date.Date)
                .OrderBy(x => x.Hour)
                .ThenBy(x => x.Minute)
                .ToListAsync();

            return Ok(list);
        }

        // ADD
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Appointment>> Add([FromBody] Appointment item)
        {
            item.Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc);

            var exists = await _db.Appointments.AnyAsync(x =>
                x.Date.Date == item.Date.Date &&
                x.Hour == item.Hour &&
                x.Minute == item.Minute);

            if (exists)
                return BadRequest("Time already booked");

            _db.Appointments.Add(item);

            await _db.SaveChangesAsync();

            return Ok(item);
        }

        // UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Appointment updated)
        {
            var item = await _db.Appointments.FindAsync(id);

            if (item == null)
                return NotFound();

            item.Name = updated.Name;
            item.Date = DateTime.SpecifyKind(updated.Date, DateTimeKind.Utc);
            item.Hour = updated.Hour;
            item.Minute = updated.Minute;

            await _db.SaveChangesAsync();

            return Ok(item);
        }

        // DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Appointments.FindAsync(id);

            if (item == null)
                return NotFound();

            _db.Appointments.Remove(item);

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
