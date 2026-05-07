using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Auticare.core;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChildRegisterController : ControllerBase
{
    private readonly AuticareDbContext _context;

    public ChildRegisterController(AuticareDbContext context)
    {
        _context = context;
    }

    // ================= CREATE CHILD =================
    [HttpPost("create")]
    public async Task<IActionResult> CreateChild([FromForm] CreateChildDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        string? imagePath = null;

        if (dto.Image != null)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);

            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = "images/" + fileName;
        }
        var child = new Childern
        {
            Name = dto.Name,
            Age = dto.Age,
            Gender = dto.Gender,
            Diagnosis_Level = dto.DiagnosisLevel,
            ParentId = parentId,
            ImageName = imagePath
        };

        _context.Childerns.Add(child);
        await _context.SaveChangesAsync();

        var result = new
        {
            child.ChildId,
            child.Name,
            child.Age,
            child.Gender,

            Image = child.ImageName,

            AutismLevel = child.Diagnosis_Level switch
            {
                DiagnosisLevel.Low => "منخفض",
                DiagnosisLevel.Medium => "متوسط",
                DiagnosisLevel.High => "مرتفع",
                _ => "غير معروف"
            }
        };

        return Ok(result);
    }
    // ================= PROFILE =================
    [HttpGet("profile/{childId}")]
    public async Task<IActionResult> GetProfile(int childId)
    {
        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var child = await _context.Childerns
            .Include(c => c.Child_Activities)
            .ThenInclude(a => a.Activity)
            .FirstOrDefaultAsync(c => c.ChildId == childId && c.ParentId == parentId);

        if (child == null)
            return NotFound();

        var activities = child.Child_Activities ?? new List<Child_Activity>();

        var totalScore = activities.Sum(x => x.Score);
        var avg = activities.Count == 0 ? 0 : (double)totalScore / activities.Count;

        string level = avg < 40 ? "High" :
                       avg < 70 ? "Medium" : "Low";

        return Ok(new
        {
            child.ChildId,
            child.Name,
            child.Age,
            child.Gender,

            Image = child.ImageName,   // 👈 الصورة

            AutismLevel = level,
            TotalScore = totalScore,
            AverageScore = avg,

            Activities = activities.Select(x => new
            {
                ActivityName = x.Activity.Name,
                x.Score
            })
        });
    }
    [Authorize]
    [HttpGet("my-child")]
    public async Task<IActionResult> GetMyChild()
    {
        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var child = await _context.Childerns
            .FirstOrDefaultAsync(x => x.ParentId == parentId);

        if (child == null)
            return NotFound();
        return Ok(new
        {
            child.ChildId,
            child.Name,
            child.Age,
            child.Gender,
            Image = child.ImageName,

            AutismLevel = child.Diagnosis_Level switch
            {
                DiagnosisLevel.Low => "منخفض",
                DiagnosisLevel.Medium => "متوسط",
                DiagnosisLevel.High => "مرتفع",
                _ => "غير معروف"
            }
        });
    }
    // ================= ADD SCORE =================
    [HttpPost("add-score")]
    public async Task<IActionResult> AddScore(int childId, int activityId, int score)
    {
        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        if (score < 0 || score > 100)
            return BadRequest("Score must be between 0 and 100");

        var childExists = await _context.Childerns
            .AnyAsync(c => c.ChildId == childId && c.ParentId == parentId);

        if (!childExists)
            return NotFound("Child not found");

        var activityExists = await _context.Activities
            .AnyAsync(a => a.ActivityId == activityId);

        if (!activityExists)
            return NotFound("Activity not found");

        var childActivity = new Child_Activity
        {
            ChildId = childId,
            ActivityId = activityId,
            Score = score
        };

        _context.Child_Activities.Add(childActivity);
        await _context.SaveChangesAsync();

        return Ok("Score added");
    }
}
