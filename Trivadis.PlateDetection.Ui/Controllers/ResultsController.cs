using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trivadis.PlateDetection.Model;

namespace Trivadis.PlateDetection.Ui.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDatabaseContext _context;

        public ResultsController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Results
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetectionResult>>> GetResults()
        {
            return await _context.Results.ToListAsync();
        }

        // GET: api/Results/5
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<DetectionResult>> GetDetectionResult(Guid id)
        {
            var detectionResult = await _context.Results.FindAsync(id);

            if (detectionResult == null)
            {
                return NotFound();
            }

            return detectionResult;
        }

        // PUT: api/Results/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetectionResult(Guid id, DetectionResult detectionResult)
        {
            if (id != detectionResult.DetectionResultId)
            {
                return BadRequest();
            }

            _context.Entry(detectionResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetectionResultExists(id))
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

        // POST: api/Results
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DetectionResult>> PostDetectionResult(DetectionResult detectionResult)
        {
            _context.Results.Add(detectionResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDetectionResult", new { id = detectionResult.DetectionResultId }, detectionResult);
        }

        // DELETE: api/Results/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DetectionResult>> DeleteDetectionResult(Guid id)
        {
            var detectionResult = await _context.Results.FindAsync(id);
            if (detectionResult == null)
            {
                return NotFound();
            }

            _context.Results.Remove(detectionResult);
            await _context.SaveChangesAsync();

            return detectionResult;
        }

        private bool DetectionResultExists(Guid id)
        {
            return _context.Results.Any(e => e.DetectionResultId == id);
        }
    }
}
