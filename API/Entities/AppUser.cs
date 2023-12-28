using System.ComponentModel.DataAnnotations;
using API.Extensions;

namespace API.Entities;

public sealed class AppUser
{
    public int Id { get; set; }

    [Required] public string UserName { get; set; } = default!;

    public byte[] PasswordHash { get; set; } = default!;

    public byte[] PasswordSalt { get; set; } = default!;

    public DateOnly DateOfBirth { get; set; }

    public string KnownAs { get; set; } = default!;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string Gender { get; set; } = default!;

    public string? Introduction { get; set; }

    public string? LookingFor { get; set; }

    public string? Interests { get; set; }

    public string City { get; set; } = default!;

    public string Country { get; set; } = default!;

    public List<Photo> Photos { get; set; } = new();

    //public int GetAge()
    //{
    //    return DateOfBirth.CalculateAge();
    //}
}