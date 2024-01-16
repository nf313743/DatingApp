using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public sealed class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<IEnumerable<AppUser>> GetUsers()
    {
        return await _context.Users
            .Include(x => x.Photos)
            .ToListAsync();
    }

    public ValueTask<AppUser?> GetUserById(int id)
    {
        return _context.Users.FindAsync(id);
    }

    public Task<AppUser?> GetUserByUserName(string userName)
    {
        return _context.Users
            .Include(x => x.Photos)
            .SingleOrDefaultAsync(x => x.UserName == userName);
    }

    public async Task<PagedList<MemberDto>> GetMembers(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        query = query.Where(x => x.UserName != userParams.CurrentUsername);
        query = query.Where(x => x.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive),
        };

        return await PagedList<MemberDto>.Create(
            query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(),
            userParams.PageNumber,
            userParams.PageSize);
    }

    public Task<MemberDto?> GetMemberByUserName(string userName)
    {
        return _context.Users
            .Where(x => x.UserName == userName)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public  Task<string> GetUserGender(string userName)
    {
        return _context.Users.Where(x => x.UserName == userName).Select(x => x.Gender).SingleAsync()!;
    }
}