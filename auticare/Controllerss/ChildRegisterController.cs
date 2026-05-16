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
   

    [HttpGet("my-children")]
    public async Task<IActionResult> GetMyChildren()
    {
        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        // 1️⃣ نجيب البيانات خام بدون أي switch
        var children = await _context.Childerns
            .Where(c => c.ParentId == parentId)
            .ToListAsync();

        if (children == null || children.Count == 0)
            return Ok(new List<object>());

        // 2️⃣ نحول بعد ما خرجنا من EF (هنا عادي switch)
        var result = children.Select(child => new
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

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChildById(int id)
    {
        // ================= GET PARENT ID =================
        var parentId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        // ================= GET CHILD =================
        var child = await _context.Childerns

            .FirstOrDefaultAsync(c =>

                c.ChildId == id &&

                c.ParentId == parentId
            );

        // ================= NOT FOUND =================
        if (child == null)
            return NotFound();

        // ================= RETURN =================
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
}