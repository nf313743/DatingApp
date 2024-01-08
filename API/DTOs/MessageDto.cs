using API.Entities;

namespace API.DTOs;

public sealed class MessageDto
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public string SenderUserName { get; set; } = default!;

    public string SenderPhotoUrl { get; set; } = default!;

    public int RecipientId { get; set; }

    public string RecipientUserName { get; set; } = default!;

    public string RecipientPhotoUrl { get; set; } = default!;

    public string Content { get; set; } = default!;

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; }
}