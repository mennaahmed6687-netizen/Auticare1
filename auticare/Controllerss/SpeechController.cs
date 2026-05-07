using Microsoft.AspNetCore.Mvc;
using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using System.Linq;
using Auticare.core;

namespace auticare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeechController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public SpeechController(AuticareDbContext context)
        {
            _context = context;
        }

        // =========================
        // 🚀 POST: Add Speech Data
        // =========================
        [HttpPost("add")]
        public IActionResult AddSpeechData([FromBody] Speechadd dto)
        {
            if (dto == null || dto.Letters == null || !dto.Letters.Any())
                return BadRequest("Invalid data");

            // 🔥 نبدأ من صفر
            int totalPoints = 0;
            int maxPoints = dto.Letters.Count * 2;

            foreach (var letter in dto.Letters)
            {
                if (letter.Status == "Mastered")
                    totalPoints += 2;
                else if (letter.Status == "InProgress")
                    totalPoints += 1;
                // غير كده = 0
            }

            // 🧮 حساب النسبة
            double score = (double)totalPoints / maxPoints * 100;

            // 🔒 ضمان إنها بين 0 و 100
            score = Math.Max(0, Math.Min(score, 100));

            var speech = new SpeechData
            {
                ChildId = dto.ChildId,
                Score = (int)Math.Round(score),
                CreatedAt = DateTime.Now,
                Notes= dto.Notes,
            };

            _context.SpeechDatas.Add(speech);
            _context.SaveChanges();

            return Ok(new
            {
                message = "تم الحساب من الصفر",
                score = speech.Score
            });
        }

        // =========================
        // 📊 GET: عرض بيانات الطفل
        // =========================
        [HttpGet("child/{childId}")]
        public IActionResult GetChildData(int childId)
        {
            var data = _context.SpeechDatas
                .Where(x => x.ChildId == childId)
                .Select(x => new
                {
                    x.Id,
                    x.ChildId,
                    x.Score,
                    x.CreatedAt,
                    x.Notes

                })
                .ToList();

            return Ok(data);
        }
    }
}