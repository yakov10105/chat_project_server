using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Services.ChatService.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private readonly string botUser;
        private readonly IDictionary<string, UserConnection> _connections;

        public ChatHub(IDictionary<string, UserConnection> connections)
        {
            botUser = "MyChat Bot";
            _connections = connections;
        }

        public async Task JoinRoomAsync(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.Room);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(userConnection.Room).SendAsync("ReceiveMessage", botUser,
                $"{userConnection.User} has joind {userConnection.Room}");

            await SendUsersConnected(userConnection.Room);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                await Clients.Group(userConnection.Room)
                             .SendAsync("ReceiveMessage", userConnection.User, message);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
                Clients.Group(userConnection.Room)
                       .SendAsync("ReceiveMessage", botUser, $"{userConnection.User} has left");

                SendUsersConnected(userConnection.Room);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersConnected(string room)
        {
            var users = _connections.Values
                .Where(c => c.Room == room)
                .Select(c => c.User);

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }
    }
}
