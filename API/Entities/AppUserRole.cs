using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public sealed class AppUserRole : IdentityUserRole<int>
{
    public AppUser User { get; set; } = default!;

    public AppRole Role { get; set; } = default!;
}