using App.Ki.Business.Hubs.Models;

namespace App.Ki.Business.Hubs;

public class Subscription
{
    public int UserId { get; set; }
    public string ConnectionId { get; set; }
    public FeedFilter Filter { get; set; }
}

internal class ConnectionsStore
{
    public static Subscription AddConnection(int userId, string connectionId)
    {
        var subscription = new Subscription
        {
            ConnectionId = connectionId,
            UserId = userId
        };

        return subscription;
    }

    public static void RemoveConnection(string connectionId)
    {
        // 
    }
}