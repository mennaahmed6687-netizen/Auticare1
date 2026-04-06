using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using auticare.Data;
using auticare.core;
using Azure;
using Microsoft.AspNetCore.JsonPatch;

namespace auticare.Controllerss
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChildrenController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public int ActivityId { get; private set; }

        public ChildrenController(AuticareDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddChild")]
        public IActionResult AddChild(
    string name,
    int age,
    Gender gender,
    int parentId,
    [FromQuery] int[] activityIds // قائمة معرفات الأنشطة
)
        {
            // 1️⃣ تحقق من وجود Parent
            var parent = _context.Parent.Find(parentId);
            if (parent == null)
                return BadRequest("Parent not found.");

            // 2️⃣ إنشاء كائن Child
            var child = new Child
            {
                Name = name,
                Age = age,
                Gender = gender,
                ParentId = parentId,
                Child_Activities = new List<Child_Activity>()
            };

            // 3️⃣ إضافة الأنشطة
            foreach (var activityId in activityIds)
            {
                // تحقق من أن النشاط موجود
                var activity = _context.Activities.Find(activityId);
                if (activity == null)
                    return BadRequest($"Activity with Id {activityId} not found.");

                // إضافة Child_Activity
                child.Child_Activities.Add(new Child_Activity
                {
                    ActivityId = activityId,
                    Score = 0,
                    Attempts = 0,
                    Duration = 0
                });
            }
            // 4️⃣ حفظ الطفل مع الأنشطة
            _context.Child.Add(child);
            _context.SaveChanges();

            // 5️⃣ إرجاع النتيجة
            return Ok(new
            {
                child.ChildId,
                child.Name,
                child.Age,
                child.Gender,
                child.ParentId,
                Activities = child.Child_Activities.Select(ca => new
                {
                    ca.child_activityId,
                    ca.ActivityId,
                    ca.Score,
                    ca.Attempts,
                    ca.Duration
                })
            });
        }
        // ================= GET =================
        [HttpGet("{id}")]
        public IActionResult GetChild(int id)
        {
            var child = _context.Child
                .Include(c => c.Parent)
                .Include(c => c.Child_Activities)
                    .ThenInclude(ca => ca.Activity)
                .FirstOrDefault(c => c.ChildId == id);

            if (child == null) return NotFound("Child not found.");

            return Ok(child);
        }

        [HttpGet]
        public IActionResult GetAllChildren()
        {
            var children = _context.Child
                .Include(c => c.Parent)
                .Include(c => c.Child_Activities)
                    .ThenInclude(ca => ca.Activity)
                .OrderBy(c => c.Name)
                .ToList();

            return Ok(children);
        }

        // ================= PUT =================
        [HttpPut("{id}")]
        public IActionResult UpdateChild(int id, [FromBody] string newName)
        {
            var child = _context.Child.FirstOrDefault(c => c.ChildId == id);
            if (child == null) return NotFound("Child not found.");

            child.Name = newName;
            _context.SaveChanges();

            return Ok(child);
        }

        [HttpPut("UpdateChildActivity/{childId}/{activityId}")]
        public IActionResult Child_Activity(int childId, int activityId, Child_Activity model)
        {
            var child = _context.Child.Find(childId);
            if (child == null)
                return BadRequest("Child not found.");

            var activity = _context.Activities.Find(activityId);
            if (activity == null)
                return BadRequest("Activity not found.");

            var childActivity = new Child_Activity
            {
                ChildId = childId,
                ActivityId = activityId,
                Duration = 45,  // ✅ تعيين هنا
                Score = 0,
                Attempts = 0
            };

            _context.Set<Child_Activity>().Add(childActivity);
            _context.SaveChanges();
            return Ok(new
            {
                model.child_activityId ,
                model.ChildId,
                model.ActivityId,
                model.Score,
                model.Duration,
                model.Attempts
            });
        }
        [HttpPatch("{id}")]
        public IActionResult UpdateChildPatch([FromRoute] int id, [FromBody] JsonPatchDocument<Child> patchDoc)
        {
            var child = _context.Child.FirstOrDefault(c => c.ChildId == id);

            if (child == null)
                return NotFound("Child not found.");

            patchDoc.ApplyTo(child);

            _context.SaveChanges();

            return Ok(child);
        }

    }
}

