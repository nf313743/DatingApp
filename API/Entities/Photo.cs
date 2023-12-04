using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public sealed class Photo
{
    public int Id { get; set; }

    public string Url { get; set; } = default!;

    public bool IsMain { get; set; }

    public string? PublicId { get; set; }

    public int AppUserId { get; set; }

    public AppUser? AppUser { get; set; }
}