using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace API.Controllers;

[Authorize]
public sealed class UsersController : BaseApiController
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService)
    {
        _repository = repository;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _repository.GetMembers();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var user = await _repository.GetUserById(id);
        return Ok(user);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUserByName(string username)
    {
        var user = await _repository.GetMemberByUserName(username);
        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto dto)
    {
        var userName = User.GetUserName();
        var user = await _repository.GetUserByUserName(userName);

        if (user is null)
            return NotFound();

        _mapper.Map(dto, user);

        if (await _repository.SaveAll())
            return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _repository.GetUserByUserName(User.GetUserName());

        if (user is null)
            return NotFound();

        var result = await _photoService.AddPhoto(file);

        if (result.Error is not null)
            return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }

        user.Photos.Add(photo);

        if (await _repository.SaveAll())
        {
            return CreatedAtAction(
                nameof(GetUserByName),
                new { username = user.UserName },
                _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Error adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _repository.GetUserByUserName(User.GetUserName());

        if (user is null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("Already main");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if (currentMain is not null)
        {
            currentMain.IsMain = false;
        }

        photo.IsMain = true;

        if (await _repository.SaveAll())
            return NoContent();

        return BadRequest("Error setting main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _repository.GetUserByUserName(User.GetUserName());

        var photo = user!.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo is null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo");

        if (photo.PublicId is not null)
        {
            var result = await _photoService.DeletePhoto(photo.PublicId);

            if (result.Error is not null)
                return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _repository.SaveAll())
            return Ok();

        return BadRequest("Error deleting photo");
    }
}