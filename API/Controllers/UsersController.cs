using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
    {
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = (await _uow.UserRepository.GetUserGender(User.GetUserName()))!;
        userParams.CurrentUsername = User.GetUserName();

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        var users = await _uow.UserRepository.GetMembers(userParams);

        Response.AddPaginationHeader(
            new PaginationHeader(
                users.CurrentPage,
                users.PageSize,
                users.TotalCount,
                users.TotalPages));

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserById(int id)
    {
        var user = await _uow.UserRepository.GetUserById(id);
        return Ok(user);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUserByName(string username)
    {
        var user = await _uow.UserRepository.GetMemberByUserName(username);
        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto dto)
    {
        var userName = User.GetUserName();
        var user = await _uow.UserRepository.GetUserByUserName(userName);

        if (user is null)
            return NotFound();

        _mapper.Map(dto, user);

        if (await _uow.Complete())
            return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _uow.UserRepository.GetUserByUserName(User.GetUserName());

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

        if (await _uow.Complete())
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
        var user = await _uow.UserRepository.GetUserByUserName(User.GetUserName());

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

        if (await _uow.Complete())
            return NoContent();

        return BadRequest("Error setting main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _uow.UserRepository.GetUserByUserName(User.GetUserName());

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

        if (await _uow.Complete())
            return Ok();

        return BadRequest("Error deleting photo");
    }
}