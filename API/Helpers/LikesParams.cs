namespace API.Helpers;

public sealed class LikesParams : PaginationParams
{
    public int UserId { get; set; }

    public string Predicate { get; set; } = default!;
}