using AngleSharp.Text;
using auticare.core;
using auticare.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace auticare.Controllerss
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly AuticareDbContext _context;

        public ActivityController(AuticareDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetActivityWithChildren()
        {
            var activities = _context.Activities
                .Include(a => a.ChildActivities)
                .ThenInclude(ca => ca.Child) // جلب الأطفال المرتبطين
                .Select(a => new
                {
                    a.ActivityId,
                    a.Name,
                    Children = a.ChildActivities.Select(ca => new
                    {
                        ca.Child.ChildId,
                        ca.Child.Name
                    }).ToList()
                })
                .ToList();

            return Ok(activities);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddActivity([FromForm] Activity dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 📁 مسارات الحفظ
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images");
            var audioFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Audio");

            // إنشاء الفولدرات
            Directory.CreateDirectory(imageFolder);
            Directory.CreateDirectory(audioFolder);

            string imageName = null;
            string audioName = null;

            // 🖼 حفظ الصورة
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var extension = Path.GetExtension(dto.Image.FileName).ToLower();

                // ✅ تحقق من نوع الصورة
                var allowedImageExt = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedImageExt.Contains(extension))
                    return BadRequest("Invalid image format");

                imageName = Guid.NewGuid() + extension;
                var imagePath = Path.Combine(imageFolder, imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
            }

            // 🔊 حفظ الصوت
            if (dto.AudioFile != null && dto.AudioFile.Length > 0)
            {
                var extension = Path.GetExtension(dto.AudioFile.FileName).ToLower();

                // ✅ تحقق من نوع الصوت
                var allowedAudioExt = new[] { ".mp3", ".wav" };
                if (!allowedAudioExt.Contains(extension))
                    return BadRequest("Invalid audio format");

                audioName = Guid.NewGuid() + extension;
                var audioPath = Path.Combine(audioFolder, audioName);

                using (var stream = new FileStream(audioPath, FileMode.Create))
                {
                    await dto.AudioFile.CopyToAsync(stream);
                }
            }

            // 💾 حفظ في الداتابيز
            var activity = new Activity
            {
                Name = dto.Name,
                Description = dto.Description,
                Level = dto.Level,
                ImageName = imageName,
                AudioName = audioName
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // 📤 Response مرتب
            return Ok(new
            {
                message = "Activity created successfully ✅",
                data = new
                {
                    activity.ActivityId,
                    activity.Name,
                    activity.Description,
                    activity.Level,
                    ImageUrl = imageName != null ? $"/Uploads/Images/{imageName}" : null,
                    AudioUrl = audioName != null ? $"/Uploads/Audio/{audioName}" : null
                }
            });
        }
        [HttpGet("ActivityAll")]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _context.Activities.ToListAsync();

            var result = activities.Select(a => new
            {
                a.ActivityId,
                a.Name,
                a.Description,
                a.Level,
                ImageUrl = a.ImageName != null ? $"/Uploads/Images/{a.ImageName}" : null,
                AudioUrl = a.AudioName != null ? $"/Uploads/Audio/{a.AudioName}" : null
            });

            return Ok(result);
        }
        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Images", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "image/jpeg");
        }
        [HttpGet("audio/{fileName}")]
        public IActionResult GetAudio(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Audio", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "audio/mpeg");
        }
        [HttpGet("search")]
        public IActionResult SearchActivities([FromQuery] string name)
        {
            // ✅ تحقق من الإدخال
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var activities = _context.Activities
                .Where(a => a.Name.ToLower().Contains(name.ToLower()))
                .Select(a => new
                {
                    a.ActivityId,
                    a.Name,
                    a.Description,
                    a.Level,

                    ImageUrl = !string.IsNullOrEmpty(a.ImageName)
                        ? $"{baseUrl}/api/Activity/image/{a.ImageName}"
                        : null,

                    AudioUrl = !string.IsNullOrEmpty(a.AudioName)
                        ? $"{baseUrl}/api/Activity/audio/{a.AudioName}"
                        : null
                })
                .ToList();

            // ✅ لو مفيش نتائج
            if (!activities.Any())
                return NotFound("No activities found");

            return Ok(activities);
        }
    }
    }

