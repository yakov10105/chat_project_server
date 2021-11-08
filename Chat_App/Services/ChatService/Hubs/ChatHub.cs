using Chat_App.Data;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.ChatService.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private readonly string botUser;
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IUserRepo _repository;

        public ChatHub(IDictionary<string, UserConnection> connections,IUserRepo repo)
        {
            botUser = "MyChat Bot";
            _connections = connections;
            _repository = repo;
        }

        public async Task JoinRoomAsync(UserConnection userConnection)
        {
            string roomId = GetRoomId(userConnection);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            _connections[Context.ConnectionId] = userConnection;

            await Clients.Group(roomId).SendAsync("ReceiveMessage", botUser,
                $"{userConnection.SenderUserName} has joind {roomId}");

            //await SendUsersConnected(roomId);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string roomId = GetRoomId(userConnection);
                await Clients.Group(roomId)
                             .SendAsync("ReceiveMessage", userConnection.SenderUserName, message);
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string roomId = GetRoomId(userConnection);
                _connections.Remove(Context.ConnectionId);
                Clients.Group(roomId)
                       .SendAsync("ReceiveMessage", botUser, $"{userConnection.SenderUserName} has left");

                //SendUsersConnected(roomId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        //public Task SendUsersConnected(string room)
        //{
        //    var users = _connections.Values
        //        .Where(c => c.Room == room)
        //        .Select(c => c.User);

        //    return Clients.Group(room).SendAsync("UsersInRoom", users);
        //}

  

        private string GetRoomId(UserConnection userConnection)
        {
            var sb = new StringBuilder();
            var senderId = _repository.GetUserIdByUserName(userConnection.SenderUserName);
            var reciverId = _repository.GetUserIdByUserName(userConnection.ReciverUserName);
            if (senderId < reciverId)
                sb.Append($"{senderId}-{reciverId}");
            else
                sb.Append($"{reciverId}-{senderId}");

            return sb.ToString();
        }

        public Task SendUsersConnected(string room)
        {
            throw new NotImplementedException();
        }
    }
}
