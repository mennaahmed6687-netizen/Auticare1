using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class Parent
    {


        [Key]
        public int ParentId { get; set; }
        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public List<Child>? children { get; set; } = new List<Child>();

    }
}
