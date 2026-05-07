using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Auticare.Core.Models.Admin;
using Auticare.Services.Admin;

namespace Auticare.Controllerss
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Dashboard Statistics
        [HttpGet("dashboard/stats")]
        public async Task<ActionResult<ApiResponse<DashboardStats>>> GetDashboardStats()
        {
            try
            {
                var stats = await _adminService.GetDashboardStatsAsync();
                return Ok(new ApiResponse<DashboardStats>
                {
                    Success = true,
                    Data = stats,
                    Message = "Dashboard statistics retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<DashboardStats>
                {
                    Success = false,
                    Message = "Error retrieving dashboard statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Parents Management
        [HttpGet("parents")]
        public async Task<ActionResult<ApiResponse<PagedResponse<AdminParent>>>> GetParents([FromQuery] ParentFilter filter)
        {
            try
            {
                var result = await _adminService.GetParentsAsync(filter);
                return Ok(new ApiResponse<PagedResponse<AdminParent>>
                {
                    Success = true,
                    Data = result,
                    Message = "Parents retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResponse<AdminParent>>
                {
                    Success = false,
                    Message = "Error retrieving parents",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("parents/{id}")]
        public async Task<ActionResult<ApiResponse<AdminParent>>> GetParent(string id)
        {
            try
            {
                var parent = await _adminService.GetParentByIdAsync(id);
                if (parent == null)
                {
                    return NotFound(new ApiResponse<AdminParent>
                    {
                        Success = false,
                        Message = "Parent not found"
                    });
                }

                return Ok(new ApiResponse<AdminParent>
                {
                    Success = true,
                    Data = parent,
                    Message = "Parent retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminParent>
                {
                    Success = false,
                    Message = "Error retrieving parent",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("parents")]
        public async Task<ActionResult<ApiResponse<AdminParent>>> CreateParent([FromBody] CreateParentDto dto)
        {
            try
            {
                var parent = await _adminService.CreateParentAsync(dto);
                return CreatedAtAction(nameof(GetParent), new { id = parent.Id }, new ApiResponse<AdminParent>
                {
                    Success = true,
                    Data = parent,
                    Message = "Parent created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminParent>
                {
                    Success = false,
                    Message = "Error creating parent",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("parents/{id}")]
        public async Task<ActionResult<ApiResponse<AdminParent>>> UpdateParent(string id, [FromBody] UpdateParentDto dto)
        {
            try
            {
                var parent = await _adminService.UpdateParentAsync(id, dto);
                if (parent == null)
                {
                    return NotFound(new ApiResponse<AdminParent>
                    {
                        Success = false,
                        Message = "Parent not found"
                    });
                }

                return Ok(new ApiResponse<AdminParent>
                {
                    Success = true,
                    Data = parent,
                    Message = "Parent updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminParent>
                {
                    Success = false,
                    Message = "Error updating parent",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("parents/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteParent(string id)
        {
            try
            {
                var result = await _adminService.DeleteParentAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Parent not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Parent deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting parent",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Children Management
        [HttpGet("children")]
        public async Task<ActionResult<ApiResponse<PagedResponse<AdminChild>>>> GetChildren([FromQuery] ChildFilter filter)
        {
            try
            {
                var result = await _adminService.GetChildrenAsync(filter);
                return Ok(new ApiResponse<PagedResponse<AdminChild>>
                {
                    Success = true,
                    Data = result,
                    Message = "Children retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResponse<AdminChild>>
                {
                    Success = false,
                    Message = "Error retrieving children",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("children/{id}")]
        public async Task<ActionResult<ApiResponse<AdminChild>>> GetChild(int id)
        {
            try
            {
                var child = await _adminService.GetChildByIdAsync(id);
                if (child == null)
                {
                    return NotFound(new ApiResponse<AdminChild>
                    {
                        Success = false,
                        Message = "Child not found"
                    });
                }

                return Ok(new ApiResponse<AdminChild>
                {
                    Success = true,
                    Data = child,
                    Message = "Child retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminChild>
                {
                    Success = false,
                    Message = "Error retrieving child",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("children")]
        public async Task<ActionResult<ApiResponse<AdminChild>>> CreateChild([FromBody] CreateChildDto dto)
        {
            try
            {
                var child = await _adminService.CreateChildAsync(dto);
                return CreatedAtAction(nameof(GetChild), new { id = child.Id }, new ApiResponse<AdminChild>
                {
                    Success = true,
                    Data = child,
                    Message = "Child created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminChild>
                {
                    Success = false,
                    Message = "Error creating child",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("children/{id}")]
        public async Task<ActionResult<ApiResponse<AdminChild>>> UpdateChild(int id, [FromBody] UpdateChildDto dto)
        {
            try
            {
                var child = await _adminService.UpdateChildAsync(id, dto);
                if (child == null)
                {
                    return NotFound(new ApiResponse<AdminChild>
                    {
                        Success = false,
                        Message = "Child not found"
                    });
                }

                return Ok(new ApiResponse<AdminChild>
                {
                    Success = true,
                    Data = child,
                    Message = "Child updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminChild>
                {
                    Success = false,
                    Message = "Error updating child",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("children/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteChild(int id)
        {
            try
            {
                var result = await _adminService.DeleteChildAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Child not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Child deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting child",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Games Management
        [HttpGet("games")]
        public async Task<ActionResult<ApiResponse<PagedResponse<AdminGame>>>> GetGames([FromQuery] GameFilter filter)
        {
            try
            {
                var result = await _adminService.GetGamesAsync(filter);
                return Ok(new ApiResponse<PagedResponse<AdminGame>>
                {
                    Success = true,
                    Data = result,
                    Message = "Games retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResponse<AdminGame>>
                {
                    Success = false,
                    Message = "Error retrieving games",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("games/{id}")]
        public async Task<ActionResult<ApiResponse<AdminGame>>> GetGame(int id)
        {
            try
            {
                var game = await _adminService.GetGameByIdAsync(id);
                if (game == null)
                {
                    return NotFound(new ApiResponse<AdminGame>
                    {
                        Success = false,
                        Message = "Game not found"
                    });
                }

                return Ok(new ApiResponse<AdminGame>
                {
                    Success = true,
                    Data = game,
                    Message = "Game retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminGame>
                {
                    Success = false,
                    Message = "Error retrieving game",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("games")]
        public async Task<ActionResult<ApiResponse<AdminGame>>> CreateGame([FromBody] CreateGameDto dto)
        {
            try
            {
                var game = await _adminService.CreateGameAsync(dto);
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, new ApiResponse<AdminGame>
                {
                    Success = true,
                    Data = game,
                    Message = "Game created successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminGame>
                {
                    Success = false,
                    Message = "Error creating game",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("games/{id}")]
        public async Task<ActionResult<ApiResponse<AdminGame>>> UpdateGame(int id, [FromBody] UpdateGameDto dto)
        {
            try
            {
                var game = await _adminService.UpdateGameAsync(id, dto);
                if (game == null)
                {
                    return NotFound(new ApiResponse<AdminGame>
                    {
                        Success = false,
                        Message = "Game not found"
                    });
                }

                return Ok(new ApiResponse<AdminGame>
                {
                    Success = true,
                    Data = game,
                    Message = "Game updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminGame>
                {
                    Success = false,
                    Message = "Error updating game",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("games/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteGame(int id)
        {
            try
            {
                var result = await _adminService.DeleteGameAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Game not found"
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Game deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error deleting game",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Reports Management
        [HttpGet("reports")]
        public async Task<ActionResult<ApiResponse<PagedResponse<ProgressReport>>>> GetReports([FromQuery] ReportFilter filter)
        {
            try
            {
                var result = await _adminService.GetReportsAsync(filter);
                return Ok(new ApiResponse<PagedResponse<ProgressReport>>
                {
                    Success = true,
                    Data = result,
                    Message = "Reports retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PagedResponse<ProgressReport>>
                {
                    Success = false,
                    Message = "Error retrieving reports",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult<ApiResponse<ProgressReport>>> GetReport(int id)
        {
            try
            {
                var report = await _adminService.GetReportByIdAsync(id);
                if (report == null)
                {
                    return NotFound(new ApiResponse<ProgressReport>
                    {
                        Success = false,
                        Message = "Report not found"
                    });
                }

                return Ok(new ApiResponse<ProgressReport>
                {
                    Success = true,
                    Data = report,
                    Message = "Report retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProgressReport>
                {
                    Success = false,
                    Message = "Error retrieving report",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // Messages Management
        [HttpGet("messages")]
        public async Task<ActionResult<ApiResponse<List<AdminMessage>>>> GetMessages()
        {
            try
            {
                var messages = await _adminService.GetMessagesAsync();
                return Ok(new ApiResponse<List<AdminMessage>>
                {
                    Success = true,
                    Data = messages,
                    Message = "Messages retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<AdminMessage>>
                {
                    Success = false,
                    Message = "Error retrieving messages",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("messages")]
        public async Task<ActionResult<ApiResponse<AdminMessage>>> SendMessage([FromBody] CreateMessageDto dto)
        {
            try
            {
                var message = await _adminService.SendMessageAsync(dto);
                return Ok(new ApiResponse<AdminMessage>
                {
                    Success = true,
                    Data = message,
                    Message = "Message sent successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<AdminMessage>
                {
                    Success = false,
                    Message = "Error sending message",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
