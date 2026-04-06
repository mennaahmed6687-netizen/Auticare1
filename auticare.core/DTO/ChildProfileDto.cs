using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class ChildProfileDto
    {
       
        public int ChildId { get; set; }

        public  string Name { get; set; }

        public int Age { get; set; }
       
        public Gender Gender { get; set; }

        public DiagnosisLevel Diagnosis_Level { get; set; }
      
        public string ParentId { get; set; }
        public virtual ICollection<Child_Activity>? Child_Activities { get; set; } = new List<Child_Activity>();
        public object AverageScore { get; set; }
        public string OverallLevel { get; set; }
    }
}
