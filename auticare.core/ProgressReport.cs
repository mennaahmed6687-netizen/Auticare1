using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace auticare.core
{
    public class ProgressReport
    {
        public string? description;

        [Key]
        public int? Id { get; set; }
        public int? Report { get; set; }
        public double AverageScore { get; set; }
        public int ChildId { get; set; }


        public required string RecommendedNextStep { get; set; }
        public DateTime ReportDate { get; set; }
        public required string OverallProgress { get; set; }
        public int completedactivities { get; set; }
        public Child? Child { get; set; }



    }
}
