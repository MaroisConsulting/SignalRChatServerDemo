using Microsoft.AspNetCore.SignalR;
using SignalRChatShared;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    // Store connection info (in-memory for demo purposes)
    private static ConcurrentDictionary<string, ChatUserInfo> ConnectedUsers = new();

    public async Task SendMessage(string username, string room, string message)
    {
        // Ensure user is connected
        if (ConnectedUsers.TryGetValue(Context.ConnectionId, out var user) && user.Room == room)
        {
            // Broadcast message to the room
            await Clients.Group(room).SendAsync("ReceiveMessage", new
            {
                Username = username,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // Client calls this after connecting
    public async Task JoinRoom(ChatUserInfo user)
    {
        // Save user info tied to connection
        ConnectedUsers[Context.ConnectionId] = user;

        // Add to group
        await Groups.AddToGroupAsync(Context.ConnectionId, user.Room);

        // Notify others in the room
        await Clients.Group(user.Room).SendAsync("ReceiveSystemMessage", new
        {
            Message = $"{user.Username} has joined the room.",
            Type = "join"
        });
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectedUsers.TryRemove(Context.ConnectionId, out var user))
        {
            await Clients.Group(user.Room).SendAsync("ReceiveSystemMessage", new
            {
                Message = $"{user.Username} has left the room.",
                Type = "leave"
            });
        }

        await base.OnDisconnectedAsync(exception);
    }
}
