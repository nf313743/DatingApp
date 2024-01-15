namespace API.SingalR;

public sealed class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = new();

    public Task<bool> UserConnected(string userName, string connectionId)
    {
        var isOnline = false;

        lock (OnlineUsers)
        {
            if (OnlineUsers.TryGetValue(userName, out var list))
            {
                list.Add(connectionId);
            }
            else
            {
                OnlineUsers.Add(userName, new List<string> { connectionId });
                isOnline = true;
            }
        }

        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(string userName, string connectionId)
    {
        var isOffline = false;

        if (OnlineUsers.TryGetValue(userName, out var list))
        {
            list.Remove(connectionId);

            if (list.Count == 0)
            {
                OnlineUsers.Remove(userName);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers = [];

        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers.Select(x => x.Key).OrderBy(x => x).ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionsFromUser(string userName)
    {
        List<string> connectionIds = new List<string>();

        lock (OnlineUsers)
        {
            connectionIds = OnlineUsers.GetValueOrDefault(userName, new List<string>());
        }

        return Task.FromResult(connectionIds);
    }
}