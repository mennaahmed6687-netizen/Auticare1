using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace auticare.Controllerss
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EmotionSessionController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public EmotionSessionController(AuticareDbContext context)
        {
            _context = context;
        }

        // =========================
        // 💾 حفظ جلسة المشاعر
        // =========================
        [HttpPost]
        public async Task<IActionResult> SaveEmotion([FromBody] CreateEmotionSessionDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "❌ البيانات فارغة" });

            if (dto.ChildId == 0)
                return BadRequest(new { message = "❌ ChildId مطلوب" });

            // 🔍 التأكد من وجود الطفل
            var child = await _context.Childerns
                .FirstOrDefaultAsync(c => c.ChildId == dto.ChildId);

            if (child == null)
                return BadRequest(new { message = "❌ الطفل غير موجود" });

            // =========================
            // 💡 إنشاء الجلسة
            // =========================
            var session = new EmotionSession
            {
                ChildId = dto.ChildId,
                Emotion = dto.Emotion,
                ResponseLevel = dto.ResponseLevel,
                Notes = dto.Notes,
                Score = dto.Score,
                Options = dto.Options ?? "None",
                Date = DateTime.Now
            };

            _context.EmotionSessions.Add(session);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "✅ تم حفظ الجلسة بنجاح"
            });
        }

        // =========================
        // 📊 عرض جلسات الطفل
        // =========================
        [HttpGet("child/{childId}")]
        public async Task<IActionResult> GetByChild(int childId)
        {
            var sessions = await _context.EmotionSessions
                .Where(x => x.ChildId == childId)
                .OrderByDescending(x => x.Date)
                .ToListAsync();

            return Ok(sessions);
        }

        // =========================
        // 🗑 حذف جلسة
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.EmotionSessions.FindAsync(id);

            if (session == null)
                return NotFound(new { message = "❌ غير موجود" });

            _context.EmotionSessions.Remove(session);
            await _context.SaveChangesAsync();

            return Ok(new { message = "🗑 تم الحذف بنجاح" });
        }
    }
}

