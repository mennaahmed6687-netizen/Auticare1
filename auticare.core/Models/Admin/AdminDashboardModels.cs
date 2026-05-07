using System.ComponentModel.DataAnnotations;

namespace Auticare.Core.Models.Admin
{
    // Dashboard Statistics
    public class DashboardStats
    {
        public int TotalParents { get; set; }
        public int TotalChildren { get; set; }
        public int TotalGames { get; set; }
        public int TotalReports { get; set; }
        public int MonthlyProgressRate { get; set; }
        public int ImprovedChildrenCount { get; set; }
        public int GameUsageRate { get; set; }
        public int ParentSatisfactionRate { get; set; }
        public List<AdminActivityItem> RecentActivities { get; set; } = new List<AdminActivityItem>();
    }

    public class AdminActivityItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Icon { get; set; } = "fa-info-circle";
        public string Color { get; set; } = "bg-info";
    }

    // Parent Model for Admin
    public class AdminParent
    {
        public string Id { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; } = "active"; // active, inactive
        public string Notes { get; set; } = string.Empty;
        public int ChildrenCount { get; set; }
    }

    // Child Model for Admin
    public class AdminChild
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty; // male, female
        public string ParentId { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public string AutismLevel { get; set; } = string.Empty; // mild, moderate, severe
        public string Status { get; set; } = "active"; // active, inactive
        public int ProgressScore { get; set; }
        public DateTime LastSession { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    // Game Model for Admin
    public class AdminGame
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty; // cognitive, motor, social, communication, emotional
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "medium"; // easy, medium, hard
        public List<string> AgeGroups { get; set; } = new List<string>();
        public int Duration { get; set; } // in minutes
        public int MaxPlayers { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Status { get; set; } = "active"; // active, inactive
        public int PlayCount { get; set; }
        public int AvgScore { get; set; }
        public int CompletionRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Progress Report Model
    public class ProgressReport
    {
        public string? FilePath;

        public int Id { get; set; }
        public int ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; }
       public ReportType ProgressReportType {  get; set; }
        public string Description { get; set;  } = string.Empty;
    
        public string Status { get; set; } = string.Empty; // improved, stable, declined
        public DateTime CreatedAt { get; set; }
    }

    // Message/Report to Parents
    public class AdminMessage
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // report, score, general
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new List<string>();
        public DateTime SentDate { get; set; }
        public string Status { get; set; } = string.Empty; // sent, pending, failed
    }

    // DTOs for Create/Update Operations
    public class CreateParentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateParentDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateChildDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public string ParentId { get; set; } = string.Empty;
        public string AutismLevel { get; set; } = "mild";
        public string Status { get; set; } = "active";
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateChildDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string AutismLevel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public class CreateGameDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "medium";
        public List<string> AgeGroups { get; set; } = new List<string>();
        public int Duration { get; set; } = 15;
        public int MaxPlayers { get; set; } = 1;
        public string Icon { get; set; } = "fa-gamepad";
        public string Status { get; set; } = "active";
    }

    public class UpdateGameDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int MaxPlayers { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class CreateMessageDto
    {
        [Required]
        public string Type { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;
        public List<string> ParentIds { get; set; } = new List<string>();
        public int? ChildId { get; set; }
        public Dictionary<string, int> Scores { get; set; } = new Dictionary<string, int>();
    }

    // API Response Wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    // Filter and Search Models
    public class ParentFilter
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public string? Governorate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ChildFilter
    {
        public string? Search { get; set; }
        public string? ParentId { get; set; }
        public string? AgeGroup { get; set; }
        public string? Status { get; set; }
        public string? AutismLevel { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GameFilter
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ReportFilter
    {
        public int? ChildId { get; set; }
        public string? ParentId { get; set; }
        public string? Period { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // Pagination Response
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
