namespace API.Entities;

public sealed class Connection
{
    public Connection()
    {
    }

    public Connection(string connectionId, string userName)
    {
        ConnectionId = connectionId;
        UserName = userName;
    }

    public string ConnectionId { get; set; } = default!;
    public string UserName { get; set; } = default!;
}