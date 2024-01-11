namespace API.DTOs;
public sealed record UserDto(
    string UserName,
    string Token,
    string? PhotoUrl,
    string? KnownAs,
    string? Gender);