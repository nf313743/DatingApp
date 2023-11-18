using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed  class UsersController : ControllerBase
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers()
    {
        var users = _context.User.ToList();
        return users;
    }
    
    [HttpGet("{id:int}")]
    public  async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = (await _context.User.FindAsync(id))!;
        return user;
    }
}