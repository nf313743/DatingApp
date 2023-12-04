using API.DTOs;
using API.Entities;

namespace API.Data;

public interface IUserRepository
{
    void Update(AppUser user);

    Task<bool> SaveAll();

    Task<IEnumerable<AppUser>> GetUsers();

    ValueTask<AppUser?> GetUserById(int id);

    Task<AppUser?> GetUserByUserName(string userName);

    Task<IReadOnlyList<MemberDto>> GetMembers();

    Task<MemberDto?> GetMemberByUserName(string username);
}