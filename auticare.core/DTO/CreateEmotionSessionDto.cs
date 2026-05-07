using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class CreateEmotionSessionDto
    {
        
            public string Notes { get; set; }
            public ResponseLevel ResponseLevel { get; set; }
            public string?Emotion { get; set; }
            public string? Options { get; set; }
            public int Score { get; set; }
            public int ChildId { get; set; }
        public InteractionLevel InteractionLevel { get; set; }
        
    }
}
