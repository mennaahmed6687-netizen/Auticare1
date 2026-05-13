using Auticare.core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using WebPush;

public class ReminderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    // Add VAPID keys used when creating VapidDetails.
    // Replace the placeholder values with your actual VAPID public/private keys.
    private const string PublicKey = "BPOBs_GBmlgJCZeGtfRTq1TJakhT80_P7RDRgUb3LQvqvlWoG_pr8-FHyQYSIFA1Y2XZsCxWRT8F0yzSJVf3ZBg";
    private const string PrivateKey = "aPgZrwLpHe1UXF-HHiO2NMRojnYq2N4WG7KXAZTLu6A";

    public ReminderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("ReminderService is running...");

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AuticareDbContext>();

                var now = DateTime.Now;

                var apps = context.Appointments
                    .Where(a => a.DateTime <= now.AddMinutes(30)
                             && a.DateTime > now
                             && !a.IsNotified)
                    .ToList();

                var client = new WebPushClient();

                var vapid = new VapidDetails(
                    "mailto:test@test.com",
                    PublicKey,
                    PrivateKey
                );

                foreach (var app in apps)
                {
                    var devices = context.PushSubscriptions
                        .Where(x => x.ParentId == app.ParentId)
                        .ToList();

                    foreach (var d in devices)
                    {
                        var push = new PushSubscription(
                            d.Endpoint,
                            d.P256dh,
                            d.Auth
                        );

                        var payload = JsonSerializer.Serialize(new
                        {
                            title = "🔔 تذكير موعد",
                            body = $"{app.Title} - {app.Location}"
                        });

                        try
                        {
                            client.SendNotification(push, payload, vapid);
                            Console.WriteLine("Notification sent ✔");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR: " + ex.Message);
                        }
                    }

                    app.IsNotified = true;
                }

                context.SaveChanges();
            }

            await Task.Delay(60000, stoppingToken);
        }
    }
}