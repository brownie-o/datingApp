using System;
using API.Interfaces;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API.DTOs;
using Company.ClassLibrary1;
using API.Entities;

namespace API.SignalR;

[Authorize]  // users will need to be authenticated to connect to this hub
public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository, IHubContext<PresenceHub> presenceHub) : Hub
{
  // the group members connect to group in signalR, they can receive the message thread
  public override async Task OnConnectedAsync()
  {
    var httpContext = Context.GetHttpContext();
    var otherUser = httpContext?.Request?.Query["userId"].ToString() ?? throw new HubException("Other user not found");
    var groupName = GetGroupName(GetUserId(), otherUser);
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    await AddToGroup(groupName);

    var messages = await messageRepository.GetMessageThread(GetUserId(), otherUser);

    await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
  }


  // while the group menbers are connected, the message can be sent via this hub, and receive live
  public async Task SendMessage(CreateMessageDto createMessageDto)
  {
    var sender = await memberRepository.GetMemberByIdAsync(GetUserId());
    var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

    if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId) throw new HubException("Cannot send message");

    var message = new Message
    {
      SenderId = sender.Id,
      RecipientId = recipient.Id,
      Content = createMessageDto.Content
    };

    // check if the member is in the group, if so, mark the message as read. 
    var groupName = GetGroupName(sender.Id, recipient.Id);
    var group = await messageRepository.GetMessageGroup(groupName);
    var userInGroup = group != null && group.Connections.Any(x => x.UserId == message.RecipientId);

    if (userInGroup)
    {
      message.DateRead = DateTime.UtcNow;
    }

    messageRepository.AddMessage(message);

    if (await messageRepository.SaveAllAsync())
    {
      await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
      var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id);
      if (connections != null && connections.Count > 0 && !userInGroup)
      {
        // presenceHub.Clients: the clients that are connected to the presence hub
        // .Clients(connections): send the message to the clients who match the connection id that are inside the connections list
        await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto());
      }
    }
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    await messageRepository.RemoveConnection(Context.ConnectionId);
    await base.OnDisconnectedAsync(exception);
  }

  // add the group to the database
  private async Task<bool> AddToGroup(string groupName)
  {
    var group = await messageRepository.GetMessageGroup(groupName);
    var connection = new Connection(Context.ConnectionId, GetUserId());

    if (group == null)
    {
      group = new Group(groupName);
      messageRepository.AddGroup(group);
    }

    group.Connections.Add(connection);
    return await messageRepository.SaveAllAsync();
  }

  private static string GetGroupName(string? caller, string? other)
  {
    var stringCompare = string.CompareOrdinal(caller, other) < 0;
    return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
  }

  private string GetUserId()
  {
    return Context.User?.GetMemberId() ?? throw new HubException("Cannot get member id");
  }
}
