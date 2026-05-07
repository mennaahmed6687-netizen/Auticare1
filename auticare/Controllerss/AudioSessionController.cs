using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auticare.Controllerss
{
    [ApiController]
    [Route("api/[controller]")]
    public class AudioSessionController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public AudioSessionController(AuticareDbContext context)
        {
            _context = context;
        }

        // 🎯 حساب السكور (0 - 50 - 100)
        private int CalculateScore(ResponseSpeed speed)
        {
            return speed switch
            {
                ResponseSpeed.Fast => 100,
                ResponseSpeed.Medium => 50,
                ResponseSpeed.None => 0,
                _ => 0
            };
        }

        // 🎧 إضافة Session
        [HttpPost("add")]
        public async Task<IActionResult> Add(CreateAudioSessionDto dto)
        {
            if (dto.ChildId == null)
                return BadRequest("ChildId مطلوب");

            // 🔥 حساب السكور
            int score = CalculateScore(dto.ResponseSpeed);

            var session = new AudioSession
            {
                ChildId = dto.ChildId.Value,
                Notes = dto.Notes,
                ResponseSpeed = dto.ResponseSpeed,
                Score = score
            };

            _context.AudioSessions.Add(session);

            // 🎯 تحديث سكور الطفل
            var child = await _context.Childerns
                .FirstOrDefaultAsync(x => x.ChildId == dto.ChildId.Value);

            if (child != null)
            {
                child.score = score; // 🔥 آخر نتيجة فقط
                // لو عايزة تجميع:
                // child.score += score;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم حفظ البيانات بنجاح",
                score = score
            });
        }

        // 📊 عرض السكور
        [HttpGet("score/{childId}")]
        public async Task<IActionResult> GetScore(int childId)
        {
            var child = await _context.Childerns
                .FirstOrDefaultAsync(x => x.ChildId == childId);

            if (child == null)
                return NotFound();

            return Ok(new
            {
                score = child.score
            });
        }

        // 🔄 إعادة تعيين السكور
        [HttpPost("reset-score/{childId}")]
        public async Task<IActionResult> ResetScore(int childId)
        {
            var child = await _context.Childerns
                .FirstOrDefaultAsync(x => x.ChildId == childId);

            if (child == null)
                return NotFound();

            child.score = 0;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "تم تصفير السكور"
            });
        }
    }
}