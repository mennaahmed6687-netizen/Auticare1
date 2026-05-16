
using auticare.core;
using auticare.core.DTO;
using Auticare.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ProgressReportController : ControllerBase
{
    private readonly AuticareDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProgressReportController(AuticareDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // ==================================================
    // ✅ ADD REPORT (MULTIPLE FILES - SIMPLE WAY)
    // ==================================================
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromForm] CreateProgressReportDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.ParentId))
                return BadRequest(new { message = "❌ ParentId مطلوب" });

            List<string> filePaths = new List<string>();

            if (dto.File != null && dto.File.Count > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in dto.File)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string fullPath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filePaths.Add("/uploads/" + fileName);
                }
            }

            // ================= CREATE REPORT =================
            var report = new ProgressReport
            {
                ParentId = dto.ParentId,
                ProgressReportType = dto.ProgressReportType,
                Description = dto.Description,
                ReportDate = DateTime.Now,

                // 👇 تخزين الملفات كـ JSON
                FilePath = JsonSerializer.Serialize(filePaths)
            };

            _context.ProgressReports.Add(report);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "✅ تم حفظ التقرير بنجاح",
                reportId = report.Id,
                files = filePaths
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "❌ خطأ في السيرفر",
                error = ex.InnerException?.Message ?? ex.Message
            });
        }
    }



    // 📥 GET ALL REPORTS
    // ==================================================
    [HttpGet("all/{parentId}")]
    public async Task<IActionResult> GetAll(string parentId)
    {
        var reports = await _context.ProgressReports
            .Where(x => x.ParentId == parentId)
            .ToListAsync();

        var result = reports.Select(r => new
        {
            r.Id,
            ProgressReportType = r.ProgressReportType.ToString(),
            r.Description,
            r.ReportDate,

            // SAFE JSON PARSE
            files = SafeDeserialize(r.FilePath)
        });

        return Ok(result);
    }

    // ==================================================
    // 📥 GET BY TYPE
    // ==================================================
    [HttpGet("type/{parentId}/{type}")]
    public async Task<IActionResult> GetByType(string parentId, ReportType type)
    {
        var report = await _context.ProgressReports
            .FirstOrDefaultAsync(x =>
                x.ParentId == parentId &&
                x.ProgressReportType == type);

        if (report == null)
            return NotFound(new { message = "لا يوجد تقرير" });

        return Ok(new
        {
            report.Id,
            ProgressReportType = report.ProgressReportType.ToString(),
            report.Description,
            report.ReportDate,

            files = SafeDeserialize(report.FilePath)
        });
    }

    // ==================================================
    // 🔥 SAFE JSON HANDLER (IMPORTANT)
    // ==================================================
    private List<string> SafeDeserialize(string data)
    {
        if (string.IsNullOrEmpty(data))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(data) ?? new List<string>();
        }
        catch
        {
            // لو البيانات قديمة (string مش JSON)
            return new List<string> { data };
        }
    }
}