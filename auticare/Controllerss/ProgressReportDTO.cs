using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace auticare.core.DTO
{
    public class CreateProgressReportDto
    {
        public ReportType ProgressReportType { get; set; }

        public string? Description { get; set; }   // 👈 ممكن null

        public List<IFormFile>? File { get; set; }       // الملف اختياري
        public string ParentId { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;


    }
}
