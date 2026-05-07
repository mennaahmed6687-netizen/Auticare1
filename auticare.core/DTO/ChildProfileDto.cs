using Microsoft.AspNetCore.Http;
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
        public string Name { get; set; }
            public int Age { get; set; }
            public Gender Gender { get; set; }

            public DiagnosisLevel diagnosisLevel { get; set; }

            public double AverageScore { get; set; }
            public int TotalScore { get; set; }
            public int ActivitiesCount { get; set; }
        public ICollection<Child_Activity> Child_Activities { get; set; }
        public string OverallLevel { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageName { get; set; }
    }

    }

