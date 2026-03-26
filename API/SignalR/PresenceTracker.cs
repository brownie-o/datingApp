using System;
using System.Collections.Concurrent;

namespace API.SignalR;

public class PresenceTracker
{
  // CurrentDictionary<string: userId, ConcurrentDictionary<string, byte>: connectionId, byte is used as a placeholder value since we only care about the keys (connectionIds)
  private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> OnlineUsers = new();

  public Task UserConnected(string userId, string connectionId)
  {
    var connections = OnlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());

    connections.TryAdd(connectionId, 0);

    return Task.CompletedTask;
  }

  public Task UserDisconnected(string userId, string connectionId)
  {
    // out: give us all of the connections that matches the userId
    if (OnlineUsers.TryGetValue(userId, out var connections))
    {
      connections.TryRemove(connectionId, out _);
      if (connections.IsEmpty)
      {
        OnlineUsers.TryRemove(userId, out _);
      }
    }
    return Task.CompletedTask;
  }

  public Task<string[]> GetOnlineUsers()
  {
    return Task.FromResult(OnlineUsers.Keys.OrderBy(k => k).ToArray());
  }

  public static Task<List<string>> GetConnectionsForUser(string userId)
  {
    if (OnlineUsers.TryGetValue(userId, out var connections))
    {
      return Task.FromResult(connections.Keys.ToList());
    }

    return Task.FromResult(new List<string>());
  }
}
