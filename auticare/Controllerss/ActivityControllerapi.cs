using auticare.core;
using auticare.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auticare.Controllerss
{
 [ApiController]
[Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public ActivityController(AuticareDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetActivityWithChildren()
        {
            var activities = _context.Activities
                .Include(a => a.ChildActivities)
                .ThenInclude(ca => ca.Child) // جلب الأطفال المرتبطين
                .Select(a => new
                {
                    a.ActivityId,
                    a.Name,
                    Children = a.ChildActivities.Select(ca => new
                    {
                        ca.Child.ChildId,
                        ca.Child.Name
                    }).ToList()
                })
                .ToList();

            return Ok(activities);
        }



        [HttpPost("add")]
        public IActionResult AddActivity(
            [FromQuery] string name,
            [FromQuery] string description,
            [FromQuery] level level)
        {
            var activity = new Activity
            {
                Name = name,
                Description = description,
                Level = level
            };

            _context.Activities.Add(activity);
            _context.SaveChanges();

            // إرجاع DTO نظيف
            var result = new
            {
                activity.ActivityId,
                activity.Name,
                activity.Description,
                activity.Level
            };

            return Ok(result);
        }
    }
    }

