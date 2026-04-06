using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class RegisterChildDto
    {
      
        [MaxLength(50)]
        public required string Name { get; set; }

        [Range(2, 12, ErrorMessage = "Age must be between 2 and 12")]
        [Required(ErrorMessage = "Age is required")]
        public int Age { get; set; }
        [Required]
        public Gender Gender { get; set; }

        public DiagnosisLevel Diagnosis_Level { get; set; }
        public string ParentId { get; set; }
    }
}
