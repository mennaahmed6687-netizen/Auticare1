using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using auticare.core;
using Microsoft.EntityFrameworkCore;

namespace Auticare.core
{
    internal class AuticareDbContext:DbContext
    {
        public AuticareDbContext(DbContextOptions<AuticareDbContext> options):base(options)
        { }
        public DbSet<Child> Children { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        
        public DbSet<Activity> Activities {  get; set; }
        public DbSet<Child_Activity>Child_Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Child_Activity>()
                .HasKey(ca => new { ca.ChildId, ca.ActivityId });
        }
    }
}
