namespace API.DTOs;

public sealed record PhotoDto
{
    public int Id { get; set; }

    public string Url { get; set; } = default!;

    public bool IsMain { get; set; }
}