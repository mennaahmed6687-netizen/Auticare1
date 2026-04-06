using auticare.core;
using auticare.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auticare.Controllerss
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public AssessmentsController(AuticareDbContext context)
        {
            _context = context;
        }

        // GET: api/Assessments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Assessment>>> Getassessments()
        {
            return await _context.assessments.ToListAsync();
        }

        // GET: api/Assessments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Assessment>> GetAssessment(int id)
        {
            var assessment = await _context.assessments.FindAsync(id);

            if (assessment == null)
            {
                return NotFound();
            }

            return assessment;
        }

        // PUT: api/Assessments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssessment(int id, Assessment assessment)
        {
            if (id != assessment.assessment_id)
            {
                return BadRequest();
            }

            _context.Entry(assessment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssessmentExists(id))
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

        // POST: api/Assessments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Assessment>> PostAssessment(Assessment assessment)
        {
            _context.assessments.Add(assessment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAssessment", new { id = assessment.assessment_id }, assessment);
        }

        // DELETE: api/Assessments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var assessment = await _context.assessments.FindAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }

            _context.assessments.Remove(assessment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AssessmentExists(int id)
        {
            return _context.assessments.Any(e => e.assessment_id == id);
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateAssessmentPatch([FromRoute] int id, [FromBody] JsonPatchDocument<Assessment> patchDoc)
        {
            var Assessment = _context.assessments.FirstOrDefault(c => c.assessment_id == id);

            if (Assessment == null)
                return NotFound("Child not found.");

            patchDoc.ApplyTo(Assessment);

            _context.SaveChanges();

            return Ok(Assessment);
        }
    }
}
