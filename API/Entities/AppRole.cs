using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public sealed class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}