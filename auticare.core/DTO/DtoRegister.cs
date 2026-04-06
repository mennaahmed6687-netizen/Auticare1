using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class DtoRegister
    {
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password1 { get; set; }
        [Compare("Password1", ErrorMessage="Password do not match")]
        public required string Password2 { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
