using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public enum Gender
    {
        Male = 0,
        Female = 1
    }

    public enum DiagnosisLevel
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public class Childern
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
  
        public string? ImageName { get; set; }

        public DiagnosisLevel Diagnosis_Level { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }= new List<Assessment>();
        public string?ParentId { get; set; }
        public  virtual Parent?Parent { get; set; }
        public virtual ICollection<Child_Activity>? Child_Activities { get; set; }=new List<Child_Activity>();
        public virtual ICollection<ProgressReport>? Progress_Reports { get; set; }=new List<ProgressReport> ();
        public virtual ICollection<AudioSession>? AudioSessions { get; set; } = new List<AudioSession>();
        public virtual ICollection<EmotionSession>? EmotionSessions{ get; set; } = new List<EmotionSession>();
        public virtual ICollection<Attack>? Attacks { get; set; } = new List<Attack>();
        public int score { get; set; } = 0;
        public virtual ICollection<AIResult> AIResults { get; set; }
      = new List<AIResult>();
        public virtual ICollection<SpeechData> SpeechDatas { get; set; }
     = new List<SpeechData>();
        
    }
}
