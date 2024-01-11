namespace API.DTOs;

public sealed class LikeDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = default!;

    public int Age { get; set; }

    public string? KnownAs { get; set; }

    public string PhotoUrl { get; set; } = default!;

    public string? City { get; set; }
}