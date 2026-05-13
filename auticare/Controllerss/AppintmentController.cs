using auticare.core;
using auticare.core.DTO;
using Auticare.core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using WebPush;

[ApiController]
[Route("api/[controller]")]
[Authorize] // ✔ هنا فقط
public class NotificationController : ControllerBase
{
    private readonly AuticareDbContext _context;

    private const string PublicKey = "BPOBs_GBmlgJCZeGtfRTq1TJakhT80_P7RDRgUb3LQvqvlWoG_pr8-FHyQYSIFA1Y2XZsCxWRT8F0yzSJVf3ZBg";
    private const string PrivateKey = "aPgZrwLpHe1UXF-HHiO2NMRojnYq2N4WG7KXAZTLu6A";

    public NotificationController(AuticareDbContext context)
    {
        _context = context;
    }

    /* ==================================================
       1️⃣ Subscribe Device (FIXED)
    ================================================== */
    [HttpPost("subscribe")]
    public IActionResult Subscribe([FromBody] PushSubscriptionDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.Endpoint))
            return BadRequest("Invalid subscription");

        var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        var model = new PushSubscriptionModel
        {
            Endpoint = dto.Endpoint,
            P256dh = dto.P256dh,
            Auth = dto.Auth,
            ParentId = parentId
        };

        _context.PushSubscriptions.Add(model);
        _context.SaveChanges();

        return Ok(new { message = "Device registered" });
    }

    /* ==================================================
       2️⃣ Add Appointment (FIXED)
    ================================================== */
    [HttpPost("add-appointment")]
    public IActionResult AddAppointment([FromBody] Appointment model)
    {
        if (model == null)
            return BadRequest();

        var parentId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        model.ParentId = parentId;
        model.IsNotified = false;

        _context.Appointments.Add(model);
        _context.SaveChanges();

        return Ok(new { message = "Appointment saved" });
    }

    /* ==================================================
       3️⃣ Get Appointments (FIXED)
    ================================================== */
    [HttpGet("my-appointments")]
    public IActionResult GetAppointments()
    {
        var parentId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(parentId))
            return Unauthorized();

        var data = _context.Appointments
            .Where(a => a.ParentId == parentId)
            .OrderBy(a => a.DateTime)
            .ToList();

        return Ok(data);
    }

    /* ==================================================
       4️⃣ Send Notification
    ================================================== */
    [HttpPost("send")]
    public IActionResult Send(string parentId, string title, string body)
    {
        var devices = _context.PushSubscriptions
            .Where(x => x.ParentId == parentId)
            .ToList();

        var client = new WebPushClient();

        var vapid = new VapidDetails(
            "mailto:mennaahmed6687@gmail.com",
            PublicKey,
            PrivateKey
        );

        foreach (var d in devices)
        {
            var push = new PushSubscription(d.Endpoint, d.P256dh, d.Auth);

            var payload = JsonSerializer.Serialize(new
            {
                title,
                body
            });

            client.SendNotification(push, payload, vapid);
        }

        return Ok(new { message = "Notification sent" });
    }

    /* ==================================================
       5️⃣ Auto Reminders
    ================================================== */
    [HttpGet("check-reminders")]
    public IActionResult CheckReminders()
    {
        var now = DateTime.Now;
        var next = now.AddMinutes(30);

        var apps = _context.Appointments
            .Where(a => a.DateTime <= next && !a.IsNotified)
            .ToList();

        var client = new WebPushClient();

        var vapid = new VapidDetails(
            "mailto:auticare.project@gmail.com",
            PublicKey,
            PrivateKey
        );

        foreach (var app in apps)
        {
            var devices = _context.PushSubscriptions
                .Where(x => x.ParentId == app.ParentId)
                .ToList();

            foreach (var d in devices)
            {
                var push = new PushSubscription(d.Endpoint, d.P256dh, d.Auth);

                var payload = JsonSerializer.Serialize(new
                {
                    title = "🔔 تذكير موعد",
                    body = $"{app.Title} - {app.Location}"
                });

                client.SendNotification(push, payload, vapid);
            }

            app.IsNotified = true;
        }

        _context.SaveChanges();

        return Ok(new { message = "Checked" });
    }




}
