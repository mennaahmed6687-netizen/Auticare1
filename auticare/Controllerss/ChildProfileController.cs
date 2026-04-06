
using auticare.core.DTO;
using auticare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using auticare.core;


[Route("api/[controller]")]
    [ApiController]
    public class ChildProfileController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public ChildProfileController(AuticareDbContext context)
        {
            _context = context;
        }

        // 👤 CHILD PROFILE
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var child = await _context.Child
                .Where(c => c.ChildId == id)
                .Select(c => new ChildProfileDto
                {
                    ChildId = c.ChildId,
                    Name = c.Name,

                    Child_Activities = (ICollection<Child_Activity>)c.Child_Activities.Select(ca => new ChildActivityDto
                    {
                        ActivityId = ca.ActivityId,
                        ActivityName = ca.ActivityId.ToString(),
                        Score = ca.Score,
                        Level = ca.level
                    }).ToList(),

                    // 🔥 متوسط السكور
                    AverageScore = c.Child_Activities.Any()
                        ? c.Child_Activities.Average(x => x.Score)
                        : 0,

                    // 🔥 المستوى العام
                    OverallLevel =
                        c.Child_Activities.Any() && c.Child_Activities.Average(x => x.Score) >= 80 ? "Excellent" :
                        c.Child_Activities.Any() && c.Child_Activities.Average(x => x.Score) >= 60 ? "Good" :
                        "Needs Improvement"
                })
                .FirstOrDefaultAsync();

            if (child == null)
                return NotFound();

            return Ok(child);
        }
    }