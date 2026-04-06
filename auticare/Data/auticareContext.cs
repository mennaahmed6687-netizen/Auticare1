using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using auticare.core;

namespace auticare.Data
{
    public class AuticareDbContext : DbContext
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

    }
}
