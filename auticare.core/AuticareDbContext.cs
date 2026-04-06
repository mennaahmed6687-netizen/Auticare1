using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using auticare.core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Auticare.core
{
    public class AuticareDbContext : IdentityDbContext<Parent>
    {
        public AuticareDbContext(DbContextOptions<AuticareDbContext> options) : base(options)
        { }
        public DbSet<Child> Children { get; set; }
        
        public DbSet<Assessment> Assessments { get; set; }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Child_Activity> Child_Activities { get; set; }
        public DbSet<ProgressReport> ProgressReports { get; set; }
        public DbSet<Participation> Participations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);  // 🔥 مهم جدًا

            builder.Entity<Child_Activity>()
                .HasKey(ca => new { ca.ChildId, ca.ActivityId });
        
       
      

            builder.Entity<Parent>()
                .HasOne(p => p.ProgressReport)
                .WithOne(pr => pr.Parent)
                .HasForeignKey<ProgressReport>(pr => pr.ParentId);
        }
    }
    }

