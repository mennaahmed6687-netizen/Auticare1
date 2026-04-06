using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core
{
    public class Child_Activity
    {
        public level level;

        [Key]
        public int ?child_activityId {  get; set; }
        public int ChildId {  get; set; }
        public virtual Child?Child { get; set; }    
        public int ActivityId { get; set; }
        public virtual Activity?Activity { get; set; }
        
        public int Score { get; set; }

        public int Attempts { get; set; }

        public int Duration { get; set; }
        public string? ActivityName { get; set; }
 
    }
}
