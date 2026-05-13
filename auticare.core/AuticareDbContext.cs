using auticare.core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Reflection.Emit;


namespace Auticare.core
{
    // Ensure this matches the 'Parent' identity user type used in Program.cs
    public class AuticareDbContext : IdentityDbContext<Parent>
    {
        public AuticareDbContext(DbContextOptions<AuticareDbContext> options)
            : base(options)
        { }

        public DbSet<Childern> Childerns { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Child_Activity> Child_Activities { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<AudioSession> AudioSessions { get; set; }

        public DbSet<EmotionSession> EmotionSessions { get; set; }
        public DbSet<Attack> Attacks { get; set; }
        public DbSet<AIResult> AIResults { get; set; }
        public DbSet<SpeechData> SpeechDatas { get; set; }
        public DbSet<LetterItem> LetterItems { get; set; }
        public DbSet<PushSubscriptionModel> PushSubscriptions { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)

        {

            base.OnModelCreating(builder);

            // 🔗 Composite Key
            builder.Entity<Child_Activity>()
                .HasKey(ca => new { ca.ChildId, ca.ActivityId });

            // 🔗 One-to-One
       

            // ✅ دعم اللغة العربية (بشكل آمن فقط للجداول الخاصة بك)
            builder.Entity<Childern>()
                .Property(x => x.Name)
                .HasColumnType("nvarchar(max)");

            builder.Entity<Activity>()
                .Property(x => x.Name)
                .HasColumnType("nvarchar(max)");

            builder.Entity<Assessment>()
                .Property(x => x.name)
                .HasColumnType("nvarchar(max)");

            builder.Entity<Assessment>()
                .Property(x => x.category)
                .HasColumnType("nvarchar(max)");

            builder.Entity<Assessment>()
                .Property(x => x.notes)
                .HasColumnType("nvarchar(max)");


            builder.Entity<Attack>()
                .HasOne(a => a.Childern)
                .WithMany(c => c.Attacks)
                .HasForeignKey(a => a.ChildId);



            builder.Entity<ProgressReport>()
                .Property(p => p.ProgressReportType)
                .HasConversion(
                    v => GetDisplayName(v), // 👈 يخزن عربي
                    v => FromArabic(v)      // 👈 يرجع enum
               );
            builder.Entity<ProgressReport>()
       .HasOne(r => r.Parent)
       .WithMany(p => p.ProgressReports)
       .HasForeignKey(r => r.ParentId);

        }


    



public static string GetDisplayName(ReportType value)
        {
            return value.GetType()
                .GetMember(value.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? value.ToString();
        }

        public static ReportType FromArabic(string value)
        {
            return value switch
            {
                "العيون" => ReportType.Eye,
                "السمع" => ReportType.Hearing,
                "الأعصاب" => ReportType.Neuro,
                "النطق" => ReportType.Speech,
                _ => throw new Exception("نوع تقرير غير معروف: " + value)
            };
        }
    }
}

