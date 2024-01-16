using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public sealed class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return (await _context.Likes.FindAsync(sourceUserId, targetUserId))!;
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(x => x.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(x => x.SourceUserId == likesParams.UserId);
            users = likes.Select(x => x.TargetUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(x => x.TargetUserId == likesParams.UserId);
            users = likes.Select(x => x.SourceUser);
        }

        var likedUsers = users.Select(x => new LikeDto
        {
            Age = x.DateOfBirth.CalculateAge(),
            City = x.City,
            Id = x.Id,
            KnownAs = x.KnownAs,
            PhotoUrl = x.Photos.First(x => x.IsMain).Url,
            UserName = x.UserName!
        });

        return await PagedList<LikeDto>.Create(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public Task<AppUser> GetUserWithLikes(int userId)
    {
        return _context.Users
            .Include(x => x.LikedUsers)
            .FirstAsync(x => x.Id == userId);
    }
}