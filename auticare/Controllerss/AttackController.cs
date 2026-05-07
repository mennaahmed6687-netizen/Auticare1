using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/attacks")]
[ApiController]
[Authorize] // 🔐 كل الـ endpoints تحتاج تسجيل دخول
public class AttacksController : ControllerBase
{
    private readonly AuticareDbContext _context;

    public AttacksController(AuticareDbContext context)
    {
        _context = context;
    }

    // ================== 1. إضافة نوبة ==================
    [HttpPost("add")]
    public async Task<IActionResult> AddAttack([FromBody] AddAttackDto dto)
    {
        if (dto == null)
            return BadRequest("لا توجد بيانات");

        var child = await _context.Childerns
            .FirstOrDefaultAsync(c => c.ChildId == dto.ChildId);

        if (child == null)
            return NotFound("الطفل غير موجود");

        var attack = new Attack
        {
            ChildId = dto.ChildId,
            Date = DateTime.Now,

            Location = dto.Location,
            Trigger = dto.Trigger,
            Intensity = dto.Intensity,
            Duration = dto.Duration,

            Crying = dto.Crying,
            Screaming = dto.Screaming,
          
            Hitting = dto.Hitting,
            Rocking = dto.Rocking,
            Hiding = dto.Hiding
        };

        _context.Attacks.Add(attack);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم تسجيل النوبة بنجاح" });
    }

    // ================== 2. جلب كل النوبات ==================
    [HttpGet("all/{childId}")]
    public async Task<IActionResult> GetAll(int childId)
    {
        var data = await _context.Attacks
            .Where(x => x.ChildId == childId)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return Ok(data);
    }

    // ================== 3. متوسط النوبات الأسبوعي ==================
    [HttpGet("weekly-average/{childId}")]
    public async Task<IActionResult> GetWeeklyAverage(int childId)
    {
        var attacks = await _context.Attacks
            .Where(a => a.ChildId == childId)
            .ToListAsync();

        if (!attacks.Any())
        {
            return Ok(new
            {
                totalAttacks = 0,
                weeks = 0,
                averagePerWeek = 0
            });
        }

        var firstDate = attacks.Min(a => a.Date);
        var weeks = Math.Max(1, (DateTime.Now - firstDate).Days / 7);

        var average = attacks.Count / (double)weeks;

        return Ok(new
        {
            totalAttacks = attacks.Count,
            weeks,
            averagePerWeek = Math.Round(average, 2)
        });
    }
}