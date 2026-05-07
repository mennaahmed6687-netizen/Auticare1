using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class CreateChildDto
    {
       
    
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name can't exceed 50 characters")]
        public string Name { get; set; }

        [Range(1, 12, ErrorMessage = "Age must be between 1 and 12")]
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public DiagnosisLevel DiagnosisLevel { get; set; }
        public IFormFile? Image { get; set; }
        
    }
}

