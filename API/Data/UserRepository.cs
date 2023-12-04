using API.DTOs;
using API.Entities;
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

    public async Task<bool> SaveAll()
    {
        return (await _context.SaveChangesAsync() > 0);
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

    public async Task<IReadOnlyList<MemberDto>> GetMembers()
    {
        var list = await _context.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return list;
    }

    public Task<MemberDto?> GetMemberByUserName(string userName)
    {
        return _context.Users
            .Where(x => x.UserName == userName)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}