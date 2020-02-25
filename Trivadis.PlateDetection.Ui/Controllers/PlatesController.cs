using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trivadis.PlateDetection;
using Trivadis.PlateDetection.Model;

namespace Trivadis.PlateDetection.Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatesController : ControllerBase
    {
        private readonly ApplicationDatabaseContext _context;

        public PlatesController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Plates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetectedPlate>>> GetPlates()
        {
            return await _context.Plates.ToListAsync();
        }

        // GET: api/Plates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetectedPlate>> GetDetectedPlate(Guid id)
        {
            var detectedPlate = await _context.Plates.FindAsync(id);

            if (detectedPlate == null)
            {
                return NotFound();
            }

            return detectedPlate;
        }

        // PUT: api/Plates/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetectedPlate(Guid id, DetectedPlate detectedPlate)
        {
            if (id != detectedPlate.DetectedPlateId)
            {
                return BadRequest();
            }

            _context.Entry(detectedPlate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetectedPlateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Plates
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DetectedPlate>> PostDetectedPlate(DetectedPlate detectedPlate)
        {
            _context.Plates.Add(detectedPlate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetectedPlate", new { id = detectedPlate.DetectedPlateId }, detectedPlate);
        }

        // DELETE: api/Plates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DetectedPlate>> DeleteDetectedPlate(Guid id)
        {
            var detectedPlate = await _context.Plates.FindAsync(id);
            if (detectedPlate == null)
            {
                return NotFound();
            }

            _context.Plates.Remove(detectedPlate);
            await _context.SaveChangesAsync();

            return detectedPlate;
        }

        private bool DetectedPlateExists(Guid id)
        {
            return _context.Plates.Any(e => e.DetectedPlateId == id);
        }
    }
}
