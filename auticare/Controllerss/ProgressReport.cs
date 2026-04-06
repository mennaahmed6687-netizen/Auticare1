using auticare.core;
using auticare.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace auticare.Controllerss
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressReportController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public ProgressReportController(AuticareDbContext context)
        {
            _context = context;
        }

        // ✅ Get all reports
        [HttpGet]
        public IActionResult Get()
        {
            var reports = _context.ProgressReports.ToList();
            return Ok(reports);
        }

        // ✅ Get report by id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var report = _context.ProgressReports
                .FirstOrDefault(r => r.Id == id);

            if (report == null)
                return NotFound();

            return Ok(report);
        }

        // ✅ Get reports by child (🔥 الربط)
        [HttpGet("child/{childId}")]
        public IActionResult GetByChild(int childId)
        {
            var reports = _context.ProgressReports
                .Where(r => r.ChildId == childId)
                .ToList();

            if (!reports.Any())
                return NotFound("No reports for this child");

            return Ok(reports);
        }

        // ✅ Add report
        [HttpPost]
        public IActionResult Post(ProgressReport report)
        {
            if (report == null)
                return BadRequest();

            _context.ProgressReports.Add(report);
            _context.SaveChanges();

            return Ok(report);
        }

        // ✅ Update
        [HttpPut("{id}")]
        public IActionResult Put(int id, ProgressReport updated)
        {
            var report = _context.ProgressReports
                .FirstOrDefault(r => r.Id == id);

            if (report == null)
                return NotFound();

            report.description = updated.description;
            report.ChildId = updated.ChildId;

            _context.SaveChanges();

            return Ok("Updated Successfully");
        }

        // ✅ Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var report = _context.ProgressReports
                .FirstOrDefault(r => r.Id == id);

            if (report == null)
                return NotFound();

            _context.ProgressReports.Remove(report);
            _context.SaveChanges();

            return Ok("Deleted Successfully");
       
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateChildPatch([FromRoute] int id, [FromBody] JsonPatchDocument<ProgressReport> patchDoc)
        {
            var ProgressReport = _context.ProgressReports.FirstOrDefault(c => c.Id == id);

            if (ProgressReport == null)
                return NotFound("ProgressReports not found.");

            patchDoc.ApplyTo(ProgressReport);

            _context.SaveChanges();

            return Ok(ProgressReport);
        }
    }
}
