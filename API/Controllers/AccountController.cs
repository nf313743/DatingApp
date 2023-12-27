using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public sealed class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        if (await UserExists((dto.UserName)))
            return BadRequest("User Name is taken");

        using var hmac = new HMACSHA512();
        var user = new AppUser()
        {
            UserName = dto.UserName,
            PasswordHash = hmac.ComputeHash((Encoding.UTF8.GetBytes(dto.Password))),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto(
            user.UserName,
            _tokenService.CreateToken(user),
            null);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.Include(x=> x.Photos).SingleOrDefaultAsync(x => x.UserName == dto.UserName);

        if (user is null) return Unauthorized();

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash((Encoding.UTF8.GetBytes(dto.Password)));

        var isMatch = computedHash.SequenceEqual(user.PasswordHash);

        if (!isMatch)
            return Unauthorized("Invalid password");

        return new UserDto(
            user.UserName,
            _tokenService.CreateToken(user),
            user.Photos.FirstOrDefault(x => x.IsMain)?.Url);
    }

    private Task<bool> UserExists(string userName)
    {
        return _context.Users.AnyAsync(x => x.UserName == userName.ToLowerInvariant());
    }
}