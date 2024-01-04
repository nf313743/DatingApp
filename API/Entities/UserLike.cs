namespace API.Entities;

public sealed class UserLike
{
    public AppUser SourceUser { get; set; } = default!;

    public int SourceUserId { get; set; }

    public AppUser TargetUser { get; set; } = default!;

    public int TargetUserId { get; set; }
}