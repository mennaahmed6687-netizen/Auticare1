using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Auticare.Core.Models.Admin;
using auticare.core;
using Auticare.core;
using auticare.Services;
using System.Net;

namespace Auticare.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly AuticareDbContext _context;
        private readonly UserManager<Parent> _userManager;
        private readonly EmailService _emailService;

        public AdminService(AuticareDbContext context, UserManager<Parent> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // Dashboard
        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var totalParents = await _userManager.Users.CountAsync();
            var totalChildren = await _context.Childerns.CountAsync();
            var totalGames = await _context.Activities.CountAsync(); // Using Activities as Games
            var totalReports = await _context.ProgressReports.CountAsync();
            var averageProgress = totalChildren == 0
                ? 0
                : (int)Math.Round(await _context.Childerns.AverageAsync(c => (double)c.score));
            var improvedChildren = await _context.Childerns.CountAsync(c => c.score >= 70);
            var gameUsageRate = totalGames == 0 || totalChildren == 0
                ? 0
                : Math.Min(100, (int)Math.Round(
                    await _context.Child_Activities.CountAsync() * 100.0 / (totalGames * totalChildren)));
            var parentSatisfactionRate = totalParents == 0
                ? 0
                : Math.Min(100, (int)Math.Round(totalReports * 100.0 / totalParents));
            var recentActivities = await GetRecentActivitiesAsync();

            return new DashboardStats
            {
                TotalParents = totalParents,
                TotalChildren = totalChildren,
                TotalGames = totalGames,
                TotalReports = totalReports,
                MonthlyProgressRate = averageProgress,
                ImprovedChildrenCount = improvedChildren,
                GameUsageRate = gameUsageRate,
                ParentSatisfactionRate = parentSatisfactionRate,
                RecentActivities = recentActivities
            };
        }

        private async Task<List<AdminActivityItem>> GetRecentActivitiesAsync()
        {
            var parentActivities = await _userManager.Users
                .OrderByDescending(p => p.Created)
                .Take(3)
                .Select(p => new AdminActivityItem
                {
                    Title = "تسجيل ولي أمر جديد",
                    Description = p.Name,
                    CreatedAt = p.Created,
                    Icon = "fa-user-plus",
                    Color = "bg-success"
                })
                .ToListAsync();

            var childActivities = await _context.Childerns
                .OrderByDescending(c => c.ChildId)
                .Take(3)
                .Select(c => new AdminActivityItem
                {
                    Title = "إضافة طفل جديد",
                    Description = c.Name,
                    CreatedAt = DateTime.Now,
                    Icon = "fa-child",
                    Color = "bg-info"
                })
                .ToListAsync();

            var reportActivities = await _context.ProgressReports
                .OrderByDescending(r => r.ReportDate)
                .Take(3)
                .Select(r => new AdminActivityItem
                {
                    Title = "تحديث تقرير تقدم",
                    Description = r.Description,
                    CreatedAt = r.ReportDate,
                    Icon = "fa-chart-line",
                    Color = "bg-warning"
                })
                .ToListAsync();

            return parentActivities
                .Concat(childActivities)
                .Concat(reportActivities)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToList();
        }

        // Parents Management
        public async Task<PagedResponse<AdminParent>> GetParentsAsync(ParentFilter filter)
        {
            var query = _context.Parents.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(p => p.Name.Contains(filter.Search) || p.Email.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var parents = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = new List<AdminParent>();
            foreach (var parent in parents)
            {
                var childrenCount = await _context.Childerns.CountAsync(c => c.ParentId == parent.Id);
                
                items.Add(new AdminParent
                {
                    Id = parent.Id,
                    Name = parent.Name,
                    Email = parent.Email,
                    Phone = parent.Phone,
                    ChildrenCount = childrenCount,
                    RegistrationDate = parent.Created,
                    Created = parent.Created,
                    Status = "active"
                });
            }

            return new PagedResponse<AdminParent>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                HasNextPage = filter.Page * filter.PageSize < totalCount,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<AdminParent?> GetParentByIdAsync(string id)
        {
            var parent = await _userManager.FindByIdAsync(id);
            if (parent == null) return null;

            var children = await _context.Childerns.Where(c => c.ParentId == parent.Id).ToListAsync();
            
            return new AdminParent
            {
                Id = parent.Id,
                Name = parent.Name,
                Email = parent.Email,
                Phone = parent.Phone,
                ChildrenCount = children.Count,
                RegistrationDate = parent.Created,
                Created = parent.Created,
                Status = "active"
            };
        }

        public async Task<AdminParent> CreateParentAsync(CreateParentDto dto)
        {
            // Generate a default password since DTO doesn't have one
            var defaultPassword = "Temp@123";
            
            var user = new Parent
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Phone = dto.Phone,
                
                Created = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, defaultPassword);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new AdminParent
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                ChildrenCount = 0,
                RegistrationDate = user.Created,
                Created = user.Created,
                Status = "active"
            };
        }

        public async Task<AdminParent?> UpdateParentAsync(string id, UpdateParentDto dto)
        {
            var parent = await _userManager.FindByIdAsync(id);
            if (parent == null) return null;

            parent.Name = dto.Name;
            parent.Phone = dto.Phone;
            
            await _userManager.UpdateAsync(parent);

            var childrenCount = await _context.Childerns.CountAsync(c => c.ParentId == parent.Id);
            
            return new AdminParent
            {
                Id = parent.Id,
                Name = parent.Name,
                Email = parent.Email,
                Phone = parent.Phone,
                ChildrenCount = childrenCount,
                RegistrationDate = parent.Created,
                Created = parent.Created,
                Status = "active"
            };
        }

        public async Task<bool> DeleteParentAsync(string id)
        {
            var parent = await _userManager.FindByIdAsync(id);
            if (parent == null) return false;

            // Delete children first
            var children = await _context.Childerns.Where(c => c.ParentId == parent.Id).ToListAsync();
            _context.Childerns.RemoveRange(children);
            await _context.SaveChangesAsync();

            // Delete parent
            await _userManager.DeleteAsync(parent);
            return true;
        }

        // Children Management
        public async Task<PagedResponse<AdminChild>> GetChildrenAsync(ChildFilter filter)
        {
            var query = _context.Childerns.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(c => c.Name.Contains(filter.Search));
            }

            if (!string.IsNullOrEmpty(filter.ParentId))
            {
                query = query.Where(c => c.ParentId == filter.ParentId);
            }

            if (!string.IsNullOrEmpty(filter.AutismLevel))
            {
                var diagnosisLevel = filter.AutismLevel.ToLower() switch
                {
                    "mild" => DiagnosisLevel.Low,
                    "moderate" => DiagnosisLevel.Medium,
                    "severe" => DiagnosisLevel.High,
                    _ => DiagnosisLevel.Medium
                };
                query = query.Where(c => c.Diagnosis_Level == diagnosisLevel);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                // All children are active in our model
                if (filter.Status == "inactive")
                {
                    return new PagedResponse<AdminChild>
                    {
                        Items = new List<AdminChild>(),
                        TotalCount = 0,
                        Page = filter.Page,
                        PageSize = filter.PageSize,
                        TotalPages = 0,
                        HasNextPage = false,
                        HasPreviousPage = false
                    };
                }
            }

            var totalCount = await query.CountAsync();
            var children = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var items = new List<AdminChild>();
            foreach (var child in children)
            {
                var parent = await _userManager.FindByIdAsync(child.ParentId ?? "");
                
                items.Add(new AdminChild
                {
                    Id = child.ChildId,
                    Name = child.Name,
                    BirthDate = DateTime.Now.AddYears(-child.Age), // Convert age to birthdate
                    Gender = child.Gender.ToString(),
                    ParentId = child.ParentId ?? "",
                    ParentName = parent?.Name ?? "",
                    AutismLevel = child.Diagnosis_Level.ToString().ToLower(),
                    Status = "active",
                    ProgressScore = child.score,
                    LastSession = DateTime.Now,
                    Notes = ""
                });
            }

            return new PagedResponse<AdminChild>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                HasNextPage = filter.Page * filter.PageSize < totalCount,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<AdminChild?> GetChildByIdAsync(int id)
        {
            var child = await _context.Childerns.FirstOrDefaultAsync(c => c.ChildId == id);
            if (child == null) return null;

            var parent = await _userManager.FindByIdAsync(child.ParentId ?? "");
            
            return new AdminChild
            {
                Id = child.ChildId,
                Name = child.Name,
                BirthDate = DateTime.Now.AddYears(-child.Age),
                Gender = child.Gender.ToString(),
                ParentId = child.ParentId ?? "",
                ParentName = parent?.Name ?? "",
                AutismLevel = child.Diagnosis_Level.ToString().ToLower(),
                Status = "active",
                ProgressScore = child.score,
                LastSession = DateTime.Now,
                Notes = ""
            };
        }

        public async Task<AdminChild> CreateChildAsync(CreateChildDto dto)
        {
            var child = new Childern
            {
                Name = dto.Name,
                Age = 5, // Calculate age from birthdate (simplified)
                Gender = dto.Gender.ToLower() == "male" ? Gender.Male : Gender.Female,
                ParentId = dto.ParentId,
                Diagnosis_Level = dto.AutismLevel.ToLower() switch
                {
                    "mild" => DiagnosisLevel.Low,
                    "moderate" => DiagnosisLevel.Medium,
                    "severe" => DiagnosisLevel.High,
                    _ => DiagnosisLevel.Medium
                },
                ImageName = null
            };

            _context.Childerns.Add(child);
            await _context.SaveChangesAsync();

            var parent = await _userManager.FindByIdAsync(dto.ParentId);
            
            return new AdminChild
            {
                Id = child.ChildId,
                Name = child.Name,
                BirthDate = dto.BirthDate,
                Gender = child.Gender.ToString(),
                ParentId = dto.ParentId,
                ParentName = parent?.Name ?? "",
                AutismLevel = dto.AutismLevel,
                Status = dto.Status,
                ProgressScore = 0,
                LastSession = DateTime.Now,
                Notes = dto.Notes
            };
        }

        public async Task<AdminChild?> UpdateChildAsync(int id, UpdateChildDto dto)
        {
            var child = await _context.Childerns.FirstOrDefaultAsync(c => c.ChildId == id);
            if (child == null) return null;

            child.Name = dto.Name;
            child.Gender = dto.Gender.ToLower() == "male" ? Gender.Male : Gender.Female;
            // Keep existing age and diagnosis level since DTO doesn't have them

            _context.Childerns.Update(child);
            await _context.SaveChangesAsync();

            var parent = await _userManager.FindByIdAsync(child.ParentId ?? "");
            
            return new AdminChild
            {
                Id = child.ChildId,
                Name = child.Name,
                BirthDate = dto.BirthDate,
                Gender = child.Gender.ToString(),
                ParentId = child.ParentId ?? "",
                ParentName = parent?.Name ?? "",
                AutismLevel = dto.AutismLevel,
                Status = dto.Status,
                ProgressScore = child.score,
                LastSession = DateTime.Now,
                Notes = dto.Notes
            };
        }

        public async Task<bool> DeleteChildAsync(int id)
        {
            var child = await _context.Childerns.FirstOrDefaultAsync(c => c.ChildId == id);
            if (child == null) return false;

            _context.Childerns.Remove(child);
            await _context.SaveChangesAsync();

            return true;
        }

        // Games Management (Simplified - using mock data for now)
        public async Task<PagedResponse<AdminGame>> GetGamesAsync(GameFilter filter)
        {
            // Return mock games data for now since Activities table has issues
            var mockGames = new List<AdminGame>
            {
                new AdminGame
                {
                    Id = 1,
                    Name = "لعبة الأشكال والألوان",
                    Category = "cognitive",
                    Description = "لعبة تعليمية لتعليم الأطفال الأشكال والألوان",
                    Difficulty = "easy",
                    AgeGroups = new List<string> { "2-4", "5-7" },
                    Duration = 15,
                    MaxPlayers = 2,
                    Icon = "fa-puzzle-piece",
                    Status = "active",
                    PlayCount = 145,
                    AvgScore = 85,
                    CompletionRate = 92,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new AdminGame
                {
                    Id = 2,
                    Name = "التواصل بالصور",
                    Category = "communication",
                    Description = "تطوير مهارات التواصل من خلال التعرف على الصور",
                    Difficulty = "medium",
                    AgeGroups = new List<string> { "5-7", "8-12" },
                    Duration = 20,
                    MaxPlayers = 1,
                    Icon = "fa-comments",
                    Status = "active",
                    PlayCount = 89,
                    AvgScore = 78,
                    CompletionRate = 88,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            // Apply search filter
            if (!string.IsNullOrEmpty(filter.Search))
            {
                mockGames = mockGames.Where(g => g.Name.Contains(filter.Search) || g.Description.Contains(filter.Search)).ToList();
            }

            // Apply other filters
            if (!string.IsNullOrEmpty(filter.Category))
            {
                mockGames = mockGames.Where(g => g.Category == filter.Category).ToList();
            }

            var totalCount = mockGames.Count;
            var items = mockGames
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResponse<AdminGame>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                HasNextPage = filter.Page * filter.PageSize < totalCount,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<AdminGame?> GetGameByIdAsync(int id)
        {
            // Return mock game for now
            var mockGames = await GetGamesAsync(new GameFilter { Page = 1, PageSize = 100 });
            return mockGames.Items.FirstOrDefault(g => g.Id == id);
        }

        public async Task<AdminGame> CreateGameAsync(CreateGameDto dto)
        {
            // Create mock game (not actually saving to database)
            var newGame = new AdminGame
            {
                Id = new Random().Next(1000, 9999),
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                Difficulty = dto.Difficulty,
                AgeGroups = dto.AgeGroups,
                Duration = dto.Duration,
                MaxPlayers = dto.MaxPlayers,
                Icon = dto.Icon,
                Status = dto.Status,
                PlayCount = 0,
                AvgScore = 0,
                CompletionRate = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return newGame;
        }

        public async Task<AdminGame?> UpdateGameAsync(int id, UpdateGameDto dto)
        {
            // Mock update - return updated game
            var game = await GetGameByIdAsync(id);
            if (game == null) return null;

            game.Name = dto.Name;
            game.Category = dto.Category;
            game.Description = dto.Description;
            game.Duration = dto.Duration;
            game.MaxPlayers = dto.MaxPlayers;
            game.Status = dto.Status;
            game.Icon = dto.Icon;
            game.UpdatedAt = DateTime.Now;

            return game;
        }

        public async Task<bool> DeleteGameAsync(int id)
        {
            // Mock delete - always return true for now
            return true;
        }

        // Reports Management (Simplified - using mock data for now)
        public async Task<PagedResponse<Auticare.Core.Models.Admin.ProgressReport>> GetReportsAsync(ReportFilter filter)
        {
            // Return mock reports data for now
            var mockReports = new List<Auticare.Core.Models.Admin.ProgressReport>
            {
                new Auticare.Core.Models.Admin.ProgressReport
                {
                    Id = 1,
                    ChildId = 1,
                    ChildName = "محمد أحمد",
                    ParentId = "1",
                    ParentName = "أحمد محمد علي",
                    ReportDate = DateTime.Now.AddDays(-1),
                  
                    Status = "improved"
                },
                new Auticare.Core.Models.Admin.ProgressReport
                {
                    Id = 2,
                    ChildId = 2,
                    ChildName = "فاطمة خالد",
                    ParentId = "2",
                    ParentName = "مريم إبراهيم خالد",
                    ReportDate = DateTime.Now.AddDays(-3),
                 
                    Status = "improved"
                }
            };

            // Apply filters
            if (filter.ChildId.HasValue)
            {
                mockReports = mockReports.Where(r => r.ChildId == filter.ChildId.Value).ToList();
            }

            if (!string.IsNullOrEmpty(filter.ParentId))
            {
                mockReports = mockReports.Where(r => r.ParentId == filter.ParentId).ToList();
            }

            var totalCount = mockReports.Count;
            var items = mockReports
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResponse<Auticare.Core.Models.Admin.ProgressReport>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                HasNextPage = filter.Page * filter.PageSize < totalCount,
                HasPreviousPage = filter.Page > 1
            };
        }

        public async Task<Auticare.Core.Models.Admin.ProgressReport?> GetReportByIdAsync(int id)
        {
            // Return mock report for now
            var mockReports = await GetReportsAsync(new ReportFilter { Page = 1, PageSize = 100 });
            return mockReports.Items.FirstOrDefault(r => r.Id == id);
        }

        // Messages Management (Simplified - no AdminMessages table)
        public async Task<List<AdminMessage>> GetMessagesAsync()
        {
            // Return empty list for now since AdminMessages table doesn't exist
            return new List<AdminMessage>();
        }

        public async Task<AdminMessage> SendMessageAsync(CreateMessageDto dto)
        {
            var recipientEmails = await GetMessageRecipientEmailsAsync(dto);
            if (recipientEmails.Count == 0)
            {
                throw new Exception("No parent emails were found for this message.");
            }

            // Create a simple message without database storage for now
            var message = new AdminMessage
            {
                Id = 1,
                Type = dto.Type,
                Subject = dto.Subject,
                Content = dto.Content,
                Recipients = recipientEmails,
                SentDate = DateTime.UtcNow,
                Status = "sent"
            };

            var safeBody = WebUtility.HtmlEncode(dto.Content).Replace("\n", "<br>");
            foreach (var email in recipientEmails)
            {
                await _emailService.SendEmailAsync(email, dto.Subject, safeBody);
            }

            return message;
        }

        private async Task<List<string>> GetMessageRecipientEmailsAsync(CreateMessageDto dto)
        {
            if (dto.SendToAll)
            {
                return await _context.Parents
                    .Where(parent => parent.Email != null && parent.Email != "")
                    .Select(parent => parent.Email!)
                    .Distinct()
                    .ToListAsync();
            }

            var recipientEmails = dto.RecipientEmails
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Select(email => email.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (recipientEmails.Count > 0)
            {
                return recipientEmails;
            }

            if (dto.ParentIds.Count == 0)
            {
                return new List<string>();
            }

            return await _context.Parents
                .Where(parent => dto.ParentIds.Contains(parent.Id) && parent.Email != null && parent.Email != "")
                .Select(parent => parent.Email!)
                .Distinct()
                .ToListAsync();
        }
    }
}
