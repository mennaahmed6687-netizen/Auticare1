using auticare.core;
using auticare.Data;
using auticare.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace auticare.Controllerss
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildActivityControllerapi : ControllerBase
    {

        private readonly AuticareDbContext _context;
        private int activityId;

        public ChildActivityControllerapi(AuticareDbContext context)
        {
            _context = context;
        }

        // تعيين نشاط لطفل
        // ================= Child Activity =================
        [HttpPost("addChildActivity")]
        public IActionResult AddChildActivity([FromQuery] int childId, [FromQuery] int activityId)
        {
            var child = _context.Set<Child>().Find(childId);
            if (child == null) return BadRequest("Child not found.");

            var activity = _context.Set<Activity>().Find(activityId);
            if (activity == null) return BadRequest("Activity not found.");

            var childActivity = new Child_Activity
            {
                ChildId = childId,
                ActivityId = activityId,
                Child = child,
                Activity = activity,
                Score = 0,
                Attempts = 0,
                Duration = 0
            };

            _context.Set<Child_Activity>().Add(childActivity);
            _context.SaveChanges();

            return Ok(new
            {
                childActivity.child_activityId,
                childActivity.ChildId,
                childActivity.ActivityId
            });
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
        [HttpGet("childActivities/{childId}")]
        public IActionResult GetActivitiesForChild(int childId)
        {
            var child = _context.Set<Child>()
                .Include(c => c.Child_Activities)
                .ThenInclude(ca => ca.Activity)
                .FirstOrDefault(c => c.ChildId == childId);

            if (child == null) return NotFound("Child not found.");

            var activities = child.Child_Activities.Select(ca => new
            {
                ca.ActivityId,
                ca.Activity.Name,
                ca.Activity.Description,
                ca.Activity.Level,
                ca.Score,
                ca.Attempts,
                ca.Duration
            }).ToList();

            return Ok(activities);
        }
        [HttpPatch("UpdateProgress")]
        public IActionResult UpdateProgressPatch(int childId, [FromBody] JsonPatchDocument<Child_Activity> patchDoc)
        {
            var record = _context.Set<Child_Activity>()
                .FirstOrDefault(ca => ca.ChildId == childId && ca.ActivityId == activityId);

            if (record == null) return NotFound();

            patchDoc.ApplyTo(record);

            _context.SaveChanges();
            return Ok(record);

        }

    }
}
