using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        [NotMapped]
        public IFormFile? AudioFile { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
       
        public string Description { get; set; }

        public level Level { get; set; }
        public virtual ICollection<Child_Activity> ChildActivities { get; set; }=new List<Child_Activity>();
        public string? ImageName {  get; set; }
        public string?AudioName { get; set; }

    }
}
