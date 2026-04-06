using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class ChildActivityDto
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int Score { get; set; }

        public int Attempts { get; set; }

        public int Duration { get; set; }
        public level Level { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
