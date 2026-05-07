using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class AiPredictionResultDto
    {
        public bool Has_Asd { get; set; }
        public double Probability { get; set; }
        public int Aq_Score { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Severity_Level { get; set; }
    }
}
