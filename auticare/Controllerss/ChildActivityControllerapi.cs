using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using auticare.Models;
using Auticare.core;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace auticare.Controllerss
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChildActivityController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public ChildActivityController(AuticareDbContext context)
        {
            _context = context;
        }

        // 👇 الدالة هنا عادي جدًا
        private string CalculateLevel(int score)
        {
            if (score >= 80)
                return "متقدم";

            if (score >= 50)
                return "متوسط";

            return "مبتدئ";
        }

        [HttpPost("saveChildActivity")]
        public IActionResult SaveChildActivity([FromBody] ChildActivityDto dto, [FromQuery] int childId)
        {
            // 🔴 Validation مهم
            if (childId <= 0 || dto.ActivityId <= 0)
                return BadRequest("Invalid ChildId or ActivityId");

            var activity = _context.Set<Activity>().Find(dto.ActivityId);
            if (activity == null)
                return BadRequest("Activity not found");

            var childActivity = _context.Set<Child_Activity>()
                .FirstOrDefault(x => x.ChildId == childId && x.ActivityId == dto.ActivityId);

            if (childActivity == null)
            {
                childActivity = new Child_Activity
                {
                    ChildId = childId,
                    ActivityId = dto.ActivityId,
                    Score = dto.Score,
                    Attempts = 1,
                    Duration = dto.Duration,
                    CreatedAt = DateTime.Now,
                    level = CalculateLevel(dto.Score)
                };

                _context.Add(childActivity);
            }
            else
            {
                // 🔴 منع القسمة على صفر + حساب أدق
                var totalAttempts = childActivity.Attempts + 1;

                childActivity.Score =
                    ((childActivity.Score * childActivity.Attempts) + dto.Score)
                    / totalAttempts;

                childActivity.Attempts = totalAttempts;
                childActivity.Duration += dto.Duration;
                childActivity.CreatedAt = DateTime.Now;

                childActivity.level = CalculateLevel(childActivity.Score);

                _context.Update(childActivity);
            }

            _context.SaveChanges();

            return Ok(childActivity);
        }

        [HttpPut("UpdateProgress")]
        public IActionResult UpdateProgress(int childId, int activityId, int attempts, int duration, int score)
        {
            var record = _context.Set<Child_Activity>()
                .FirstOrDefault(ca => ca.ChildId == childId && ca.ActivityId == activityId);

            if (record == null) return NotFound();

            record.Attempts = attempts;
            record.Duration = duration;
            record.Score = score;

            _context.SaveChanges();
            return Ok(record);

        }
        [HttpGet("AllActivities")]
        public IActionResult GetAllActivities()
        {
            var result = _context.Set<Child_Activity>()
                .Include(ca => ca.Child)      // جلب بيانات الطفل
                .Include(ca => ca.Activity)   // جلب بيانات النشاط
                .Select( ca => new
                {
                    ChildId = ca.ChildId,
                    ChildName = ca.Child.Name,
                    ActivityId = ca.ActivityId,
                    ActivityName = ca.Activity.Name,
                    ca.Attempts,
                    ca.Duration,
                    ca.Score
                })
                .OrderBy(ca => ca.ChildName)   // ترتيب حسب اسم الطفل
                .ThenBy(ca => ca.ChildId)      // ثم ترتيب حسب ID الطفل
                .ToList();

            return Ok(result);
        }
        // ================= Get Activities for a Child =================
        [HttpGet("childActivities")]
        public IActionResult GetActivitiesForChild()
        {
            // 👇 هات الـ childId من التوكن (المستخدم الحالي)
            var childIdClaim = User.FindFirst("childId")?.Value;

            if (childIdClaim == null)
                return Unauthorized();

            int childId = int.Parse(childIdClaim);

            var child = _context.Set<Childern>()
                .Include(c => c.Child_Activities)
                .ThenInclude(ca => ca.Activity)
                .FirstOrDefault(c => c.ChildId == childId);

            if (child == null)
                return NotFound("Child not found.");

            var activities = child.Child_Activities.Select(ca => new
            {
                ca.ActivityId,
                ActivityName = ca.Activity.Name,
                ca.Activity.Description,

                // ✅ القيم الصح
                Score = ca.Score,
                Level = ca.level,
                Attempts = ca.Attempts,
                Duration = ca.Duration
            }).ToList();

            return Ok(activities);
        }


    }
}
