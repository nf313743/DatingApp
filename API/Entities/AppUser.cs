using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public sealed class AppUser : IdentityUser<int>
{
    public DateOnly DateOfBirth { get; set; }

    public string? KnownAs { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string? Gender { get; set; }

    public string? Introduction { get; set; }

    public string? LookingFor { get; set; }

    public string? Interests { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public List<Photo> Photos { get; set; } = new();

    public List<UserLike> LikedByUsers { get; set; } = new();

    public List<UserLike> LikedUsers { get; set; } = new();

    public List<Message> MessagesSent { get; set; } = new();

    public List<Message> MessagesReceived { get; set; } = new();

    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}