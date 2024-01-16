using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);

    Task<IEnumerable<AppUser>> GetUsers();

    ValueTask<AppUser?> GetUserById(int id);

    Task<AppUser?> GetUserByUserName(string userName);

    Task<PagedList<MemberDto>> GetMembers(UserParams userParams);

    Task<MemberDto?> GetMemberByUserName(string username);

    Task<string> GetUserGender(string userName);
}