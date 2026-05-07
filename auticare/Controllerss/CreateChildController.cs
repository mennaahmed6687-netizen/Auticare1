using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Mvc;


namespace auticare.Controllerss
{
    public class CreateChildController
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ChildController : ControllerBase
        {
            private readonly AuticareDbContext _context;

            public ChildController(AuticareDbContext context)
            {
                _context = context;
            }

            [HttpPost]
            public async Task<IActionResult> RegisterChild(CreateChildDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto.Age > 12)
                    return BadRequest("Age cannot be more than 12");

                var child = new Childern
                {
                    Name = dto.Name,
                    Age = dto.Age,
                    Gender = dto.Gender
                };

                _context.Childerns.Add(child);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetChildById), new { id = child.ChildId }, new
                {
                    Message = "Child registered successfully",
                    child.ChildId,
                    child.Name,
                    child.Age,
                    child.Gender
                });
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetChildById(int id)
            {
                var child = await _context.Childerns.FindAsync(id);

                if (child == null)
                    return NotFound("Child not found");

                return Ok(child);
            }
        }
    }
}
