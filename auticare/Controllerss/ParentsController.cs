
using auticare.core;
using auticare.Data;
using auticare.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ParentsController : ControllerBase
{
    private readonly AuticareDbContext _context;

    public ParentsController(AuticareDbContext context)
    {
        _context = context;
    }

    // ================= Parent =================
    [HttpPost("addParent")]
    public IActionResult AddParent([FromQuery] string name, [FromQuery] string email, [FromQuery] string phone, [FromQuery] string password)
    {
        var parent = new Parent
        {
            UserName = name,
            Email = email,
            Phone = phone,
        
        };

        _context.Set<Parent>().Add(parent);
        _context.SaveChanges();

        return Ok(new
        {
            parent.ParentId,
            parent.UserName,
            parent.Email,
            parent.Phone
        });
    }
    // ================= GET =================
    [HttpGet("{id}")]
    public IActionResult GetParent([FromQuery] string id)
    {
        var parent = _context.Parent
            .FirstOrDefault(p => p.ParentId == id);

        if (parent == null)
            return NotFound();

        return Ok(parent);





    }

    // ================= PUT =================
    [HttpPut("{id}")]
    public IActionResult UpdateParent(string id, [FromBody] string newName)
    {
        var parent = _context.Parent.FirstOrDefault(p => p.ParentId == id);

        if (parent == null)
            return NotFound("Parent not found.");

        parent.UserName = newName;

        _context.SaveChanges();

        return Ok(parent);
    }
    // ================= DELETE =================
    [HttpDelete("{id}")]
    public IActionResult DeleteParent(string id)
    {
        var parent = _context.Parent
            .Include(p => p.children)
                .ThenInclude(c => c.Child_Activities)
            .FirstOrDefault(p => p.ParentId == id);

        if (parent == null) return NotFound("Parent not found.");

        // حذف جميع ChildActivities المرتبطة بالأطفال
        foreach (var child in parent.children)
        {
            if (child.Child_Activities.Any())
                _context.Set<Child_Activity>().RemoveRange(child.Child_Activities);
        }

        // حذف الأطفال
        if (parent.children.Any())
            _context.Child.RemoveRange(parent.children);

        // حذف الأب نفسه
        _context.Parent.Remove(parent);
        _context.SaveChanges();

        return Ok("Parent and related children/activities deleted successfully.");

    }

}
