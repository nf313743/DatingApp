using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public sealed record RegisterDto
{
    [Required]
    public required string UserName { get; init; } = default!;

    [Required]
    public required string KnownAs { get; init; } = default!;

    [Required]
    public required string Gender { get; init; } = default!;

    [Required]
    public DateOnly? DateOfBirth { get; init; }
    public required string City { get; init; } = default!;

    [Required]
    public required string Country { get; init; } = default!;

    [StringLength(8, MinimumLength = 4)]
    public required string Password { get; init; } = default!;
}