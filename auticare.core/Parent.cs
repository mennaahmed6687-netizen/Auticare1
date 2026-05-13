using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace auticare.core
{
    public class Parent : IdentityUser
    {
   

        [Required]
     
        public string Name { get; set; }  // مهم

        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public virtual List<Childern>? children { get; set; } = new List<Childern>();
        public ICollection<ProgressReport> ProgressReports { get; set; }
        = new List<ProgressReport>();
        public ICollection<PushSubscriptionModel> PushSubscriptions { get; set; }
    }
}
