using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


public enum ReportType
{
    [Display(Name = "العيون")]
    Eye,

    [Display(Name = "السمع")]
    Hearing,

    [Display(Name = "الأعصاب")]
    Neuro,

    [Display(Name = "النطق")]
    Speech
}
namespace auticare.core
{
 public class ProgressReport
{
    [Key]
    public int Id { get; set; }

    public ReportType ProgressReportType { get; set; }

    public string? Description { get; set; }  // 👈 nullable

    public string? FilePath { get; set; }

    public DateTime ReportDate { get; set; } = DateTime.Now;

    public string ParentId { get; set; }
    public Parent Parent { get; set; }
}
}
