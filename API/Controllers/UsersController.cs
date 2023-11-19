using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public sealed  class UsersController : BaseApiController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers()
    {
        var users = _context.Users.ToList();
        return users;
    }
    
    
    [HttpGet("{id:int}")]
    public  async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = (await _context.Users.FindAsync(id))!;
        return user;
    }
}