using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public sealed class AppUser
{
    public int Id { get; set; }

    [Required] public string UserName { get; set; } = default!;

    public byte[] PasswordHash { get; set; } = default!;

    public byte[] PasswordSalt { get; set; } = default!;
}