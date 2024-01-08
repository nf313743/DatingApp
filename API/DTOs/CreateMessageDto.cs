namespace API.DTOs;

public sealed class CreateMessageDto
{
    public string RecipientUserName { get; set; } = default!;

    public string Content { get; set; } = default!;
}