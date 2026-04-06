using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class ProgressReportDTO
    {
        public int? Id { get; set; }
        public int? Report { get; set; }
        public double AverageScore { get; set; }
        public required string RecommendedNextStep { get; set; }
        public DateTime ReportDate { get; set; }
        public required string OverallProgress { get; set; }
        public int completedactivities { get; set; }
        public string ParentId { get; set; }
    }
}
