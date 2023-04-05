using API.Data;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController: ControllerBase
{
    private DataContext _context;
    
    public UsersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>>  GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound("User not found");
        return user;
    }

    [HttpDelete("{id}")]
    public ActionResult<AppUser> DeleteUser(int id)
    {
       // var user = _context.Users.Remove(id);
       return new AppUser();
    }

    // [HttpPost]
    // public ActionResult<AppUser> CreateUser(AppUser body)
    // {
    //     var user = _context.Users.Add(body);
    //     
    //     return user;
    // }
}