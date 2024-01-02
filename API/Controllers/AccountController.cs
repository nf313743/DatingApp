using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public sealed class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        if (await UserExists((dto.UserName)))
            return BadRequest("User Name is taken");

        var user = _mapper.Map<AppUser>(dto);

        using var hmac = new HMACSHA512();

        user.UserName = dto.UserName;
        user.PasswordHash = hmac.ComputeHash((Encoding.UTF8.GetBytes(dto.Password)));
        user.PasswordSalt = hmac.Key;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto(
            user.UserName,
            _tokenService.CreateToken(user),
            null,
            user.KnownAs,
            user.Gender);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await _context.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == dto.UserName);

        if (user is null) return Unauthorized();

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash((Encoding.UTF8.GetBytes(dto.Password)));

        var isMatch = computedHash.SequenceEqual(user.PasswordHash);

        if (!isMatch)
            return Unauthorized("Invalid password");

        return new UserDto(
            user.UserName,
            _tokenService.CreateToken(user),
            user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            user.KnownAs,
            user.Gender);
    }

    private Task<bool> UserExists(string userName)
    {
        return _context.Users.AnyAsync(x => x.UserName == userName.ToLowerInvariant());
    }
}