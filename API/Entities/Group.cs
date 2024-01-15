using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public sealed class Group
{
    private Group()
    {
    }

    public Group(string name)
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; } = default!;

    public ICollection<Connection> Connections { get; set; } = new List<Connection>();
}