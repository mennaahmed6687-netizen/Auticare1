using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

[Route("api/predict")]
[ApiController]
public class PredictController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly AuticareDbContext _context;

    public PredictController(HttpClient httpClient, AuticareDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    // =========================
    // Get Child From Token
    // =========================
    private async Task<int?> GetChildId()
    {
        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return null;

        var child = await _context.Childerns
            .FirstOrDefaultAsync(c => c.ParentId == parentId);

        return child?.ChildId;
    }

    // =========================
    // AI Prediction + Save
    // =========================
    [HttpPost]
    public async Task<IActionResult> Predict([FromBody] PatientDataDto data)
    {
        try
        {
            // 1️⃣ Get Child
            var childId = await GetChildId();

            if (childId == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ الطفل غير موجود"
                });
            }

            // 2️⃣ Call FastAPI
            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://fast-api1-production.up.railway.app/predict",
                content
            );

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new
                {
                    success = false,
                    message = responseText
                });
            }

            // 3️⃣ Deserialize
            var result = JsonConvert.DeserializeObject<AiPredictionResultDto>(responseText);

            if (result == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ خطأ في تحليل AI"
                });
            }

            // =========================
            // 🎯 FIX LOGIC + SEVERITY
            // =========================

            result.Has_Asd = result.Probability >= 0.5;

            if (result.Has_Asd)
            {
                result.Message = "⚠️ يوجد مؤشرات للتوحد";

                if (result.Probability >= 0.85)
                    result.Severity_Level = "شديد";
                else if (result.Probability >= 0.65)
                    result.Severity_Level = "متوسط";
                else
                    result.Severity_Level = "خفيف";
            }
            else
            {
                result.Message = "✅ لا يوجد مؤشرات";
                result.Severity_Level = "لايوجد";
            }

            // 4️⃣ Check Child Exists
            var childExists = await _context.Childerns
                .AnyAsync(c => c.ChildId == childId);

            if (!childExists)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "❌ الطفل غير موجود"
                });
            }

            // 5️⃣ Save In DB
            var aiResult = new AIResult
            {
                ChildId = childId.Value,
                Has_Asd = result.Has_Asd,
                Probability = result.Probability,
                Aq_Score = result.Aq_Score,
                Message = result.Message,
                Severity_Level = result.Severity_Level,
                CreatedAt = DateTime.Now
            };

            _context.AIResults.Add(aiResult);
            await _context.SaveChangesAsync();

            // 6️⃣ Return Result
            return Ok(new
            {
                success = true,
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // =========================
    // History
    // =========================
    [HttpGet("history/{childId}")]
    public async Task<IActionResult> GetHistory(int childId)
    {
        var data = await _context.AIResults
            .Where(x => x.ChildId == childId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return Ok(new
        {
            success = true,
            data
        });
    }
}