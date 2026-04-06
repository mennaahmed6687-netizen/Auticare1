using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ProgressReportController : ControllerBase
{
    private readonly AuticareDbContext _context;

    public ProgressReportController(AuticareDbContext context)
    {
        _context = context;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(ProgressReportDTO dto)
    {
        var parent = await _context.Parent
            .Include(p => p.ProgressReport)
            .FirstOrDefaultAsync(p => p.Id == dto.ParentId);

        if (parent == null)
            return NotFound("Parent not found");

        if (parent.ProgressReport != null)
            return BadRequest("Parent already has a ProgressReport");

        var report = new ProgressReport
        {
            Report = dto.Report,
            ParentId = (string)dto.ParentId,
            RecommendedNextStep = (string)dto.RecommendedNextStep,
            OverallProgress = (string)dto.OverallProgress
        };

        _context.ProgressReports.Add(report);
        await _context.SaveChangesAsync();

        return Ok(report);
    }

    // GET by ParentId
    [HttpGet("parent/{parentId}")]
    public async Task<IActionResult> GetByParent(string parentId)
    {
        var report = await _context.ProgressReports
            .Where(r => r.ParentId == parentId)
            .Select(r => new ProgressReportDTO
            {
                Id = r.Id,
                Report = r.Report,
                ParentId = r.ParentId,
                RecommendedNextStep=r.RecommendedNextStep,
                OverallProgress=r.OverallProgress
            })
            .FirstOrDefaultAsync();

        if (report == null)
            return NotFound();

        return Ok(report);
    }

    // PUT (Full Update)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProgressReportDTO dto)
    {
        var report = await _context.ProgressReports.FindAsync(id);

        if (report == null)
            return NotFound();

        report.Report = dto.Report;

        await _context.SaveChangesAsync();

        return Ok(report);
    }

    // PATCH (Partial Update)
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, JsonPatchDocument<ProgressReport> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest();

        var report = await _context.ProgressReports.FindAsync(id);

        if (report == null)
            return NotFound();

        patchDoc.ApplyTo(report, ModelState);

        if (!ModelState.IsValid) 
            return BadRequest();
        await _context.SaveChangesAsync();
        return Ok(report);

            }
}

