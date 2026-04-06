using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public enum DiagnosisLevel
    {
        Low, Medium, High

    }
    public enum Gender
    {
        Male,Female
    }
    public class Child
    {
        [Key]
        public int ChildId { get; set; }
        
        [MaxLength(50)]
        public required string Name { get; set; }
    
        [Range(2,  12,ErrorMessage ="Age must be between 2 and 12")]
        [Required(ErrorMessage ="Age is required")]
        public  int Age { get; set; }
        [Required]
        public Gender Gender { get; set; }

        public DiagnosisLevel Diagnosis_Level { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }= new List<Assessment>();
        public string ParentId { get; set; }
        public  virtual Parent?Parent { get; set; }
        public virtual ICollection<Child_Activity>? Child_Activities { get; set; }=new List<Child_Activity>();
        public virtual ICollection<ProgressReport>? Progress_Reports { get; set; }=new List<ProgressReport> ();
     
    }
}
