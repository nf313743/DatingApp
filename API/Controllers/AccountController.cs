using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public sealed class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManger;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManger, ITokenService tokenService, IMapper mapper)
    {
        _userManger = userManger;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        if (await UserExists((dto.UserName)))
            return BadRequest("User Name is taken");

        var user = _mapper.Map<AppUser>(dto);

        user.UserName = dto.UserName;

        var result = await _userManger.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var roleResult = await _userManger.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return new UserDto(
            user.UserName,
            await _tokenService.CreateToken(user),
            null,
            user.KnownAs,
            user.Gender);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        var user = await _userManger.Users.Include(x => x.Photos).SingleOrDefaultAsync(x => x.UserName == dto.UserName);

        if (user is null) return Unauthorized();

        var result = await _userManger.CheckPasswordAsync(user, dto.Password);

        if (!result)
            return Unauthorized("Invalid Password");

        return new UserDto(
            user.UserName!,
            await _tokenService.CreateToken(user),
            user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            user.KnownAs,
            user.Gender);
    }

    private Task<bool> UserExists(string userName)
    {
        return _userManger.Users.AnyAsync(x => x.UserName == userName.ToLowerInvariant());
    }
}