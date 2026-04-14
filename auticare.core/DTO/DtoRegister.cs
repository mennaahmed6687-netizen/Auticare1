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
        [Required]
    
        public string Name { get; set; }

        

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [Required]
        public string Password1 { get; set; }

        [Required]
        [Compare("Password1", ErrorMessage = "Passwords do not match")]
        public string Password2 { get; set; }
    }
}
