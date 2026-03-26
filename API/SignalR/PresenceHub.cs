using System;
using System.Security.Claims;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

// Hub: derived from SignalR library, used to create a real-time communication channel between the server and clients. It allows for sending messages to connected clients and managing connections.
[Authorize]
public class PresenceHub(PresenceTracker presenceTracker) : Hub
{
  public override async Task OnConnectedAsync()
  {
    await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
    // UserOnline: client is going to receive the notification that a user is online. The Clients.Others.SendAsync method is used to send the message to all other connected clients except the one that just connected.
    await Clients.Others.SendAsync("UserOnline", GetUserId());

    var currentUsers = await presenceTracker.GetOnlineUsers();
    await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    await presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);
    
    await Clients.Others.SendAsync("UserOffline", GetUserId());

    var currentUsers = await presenceTracker.GetOnlineUsers();
    await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);

    await base.OnDisconnectedAsync(exception);
  }

  private string GetUserId()
  {
    return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
  }
}
