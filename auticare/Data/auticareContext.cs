using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using auticare.core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace auticare.Data
{
    public class AuticareDbContext : IdentityDbContext<Parent>
    {
        internal readonly IEnumerable<object> assessment;
        internal object Child_Activities;

        public AuticareDbContext (DbContextOptions<AuticareDbContext> options)
            : base(options)
        {
        }

        public DbSet<auticare.core.Child> Child { get; set; } = default!;
        public DbSet<auticare.core.Assessment> assessments{ get; set; } = default!;
        public DbSet<auticare.core.Parent> Parent { get; set; } = default!;
        public DbSet<auticare.core.Activity>Activities { get; set; } = default!;
        public DbSet<auticare.core.ProgressReport> ProgressReports { get; set; } = default!;
        public DbSet<Child_Activity> ChildActivities { get; set; }= default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Parent>()
                .HasOne(p => p.ProgressReport)
                .WithOne(pr => pr.Parent)
                .HasForeignKey<ProgressReport>(pr => pr.ParentId);
        }
    }
}
