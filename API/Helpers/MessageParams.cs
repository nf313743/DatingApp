namespace API.Helpers;

public sealed class MessageParams : PaginationParams
{
    public string? UserName { get; set; }

    public string Container { get; set; } = "Unread";
}