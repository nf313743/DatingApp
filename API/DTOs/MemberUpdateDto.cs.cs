namespace API.DTOs;

public sealed record MemberUpdateDto
{
    public string? Introduction { get; init; }

    public string? LookingFor { get; init; }

    public string? Interests { get; init; }

    public string City { get; init; } = default!;

    public string Country { get; init; } = default!;
}