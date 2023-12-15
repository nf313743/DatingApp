namespace API.DTOs;

public sealed record MemberUpdateDto
{
    public string Introduction { get; init; } = default!;

    public string LookingFor { get; init; } = default!;

    public string Interests { get; init; } = default!;

    public string City { get; init; } = default!;

    public string Country { get; init; } = default!;
}