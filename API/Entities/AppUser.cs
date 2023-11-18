namespace API.Entities;

public sealed class AppUser
{
    public int Id { get; set; }
    
    public string UserName { get; set; } = default!;
}