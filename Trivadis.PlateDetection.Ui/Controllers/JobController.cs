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
    public class JobController : ControllerBase
    {
        private readonly ApplicationDatabaseContext _context;

        public JobController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.ToListAsync();
        }

        // GET: api/Jobs/5
        [HttpGet("{jobId:Guid}")]
        public async Task<ActionResult<Job>> GetJob(Guid id)
        {
            var job = await _context.Jobs
                .Include(job => job.DetectionResults)
                    .ThenInclude(detectedResult => detectedResult.DetectedPlates)
                .Include(job => job.DetectionResults)
                    .ThenInclude(detectedResult => detectedResult.DetectedPoints)
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound();
            }            

            return job;
        }

        [HttpGet("{jobId:Guid}/image")]
        public async Task<ActionResult<string>> GetImage(Guid id)
        {
            var job = await _context.Jobs
                .Include(job => job.DetectionResults).ThenInclude(dr => dr.DetectedPlates)
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound();
            }

            //string imageBase64Data = Convert.ToBase64String(job.ImageData);
            //string imageDataURL = string.Format("data:image/jpg;base64,{0}", imageBase64Data);

            //var bytes = System.Text.Encoding.UTF8.GetBytes($"data:image/jpeg;base64,{imageBase64Data}");
            return File(job.ImageData, "image/jpeg");
        }

        // PUT: api/Jobs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(Guid id, Job job)
        {
            if (id != job.JobId)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
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

        // POST: api/Jobs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Job>> PostJob(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJob", new { id = job.JobId }, job);
        }

        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> DeleteJob(Guid id)
        {
            var Job = await _context.Jobs.FindAsync(id);
            if (Job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(Job);
            await _context.SaveChangesAsync();

            return Job;
        }

        private bool JobExists(Guid id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
