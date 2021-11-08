using Chat_App.Data;
using Chat_App.Models;
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
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IRoomRepo _roomRepository;

        public ChatHub(IDictionary<string, UserConnection> connections,IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
            botUser = "MyChat Bot";
            _connections = connections;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _roomRepository = roomRepo;
        }

        public async Task JoinRoomAsync(UserConnection userConnection)
        {
            string roomKey = GetRoomId(userConnection);
            if (roomKey != "room")
            {
                int reciverId = _userRepository.GetUserByUserName(userConnection.ReciverUserName).Id;
                int senderId = _userRepository.GetUserByUserName(userConnection.SenderUserName).Id;
                await AddNewRoomAsync(roomKey);
                var room = await GetRoomByKeyAsync(roomKey);

                await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);

                _connections[Context.ConnectionId] = userConnection;

                var messages = await GetMessagesAsync(reciverId,senderId);
                foreach (var message in messages)
                {
                    await Clients.Group(roomKey).SendAsync("ReceiveMessage", message.Sender.UserName, message.Text);
                }
            }
        }

        public async Task SendMessageAsync(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string roomKey = GetRoomId(userConnection);
                var room = await GetRoomByKeyAsync(roomKey);
                int reciverId = _userRepository.GetUserByUserName(userConnection.ReciverUserName).Id;
                int senderId = _userRepository.GetUserByUserName(userConnection.SenderUserName).Id;

                await SaveNewMessageAsync(message, reciverId, senderId, room.Id);

                await Clients.Group(roomKey)
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

  

        public string GetRoomId(UserConnection userConnection)
        {
            var sb = new StringBuilder();
            if (userConnection.ReciverUserName != null && userConnection.SenderUserName != null)
            {
                var senderId = _userRepository.GetUserIdByUserName(userConnection.SenderUserName);
                var reciverId = _userRepository.GetUserIdByUserName(userConnection.ReciverUserName);
                if (senderId < reciverId)
                    sb.Append($"{senderId}-{reciverId}");
                else
                    sb.Append($"{reciverId}-{senderId}");

                return sb.ToString();
            }
            return "room";
        }

        private async Task<List<Message>> GetMessagesAsync(int reciverId, int senderId) => await Task.Run(() => _messageRepository.GetMessagesForRoom(reciverId, senderId));

        public Task SaveNewMessageAsync(string message, int reciverId, int senderId, int roomId) => Task.Run(() => _messageRepository.SaveNewMessage(message, reciverId, senderId, roomId));

        public Task AddNewRoomAsync(string roomKey) => Task.Run(() => _roomRepository.CreateRoom(roomKey));

        public Task<Room> GetRoomByKeyAsync(string roomKey) => Task.Run(() => _roomRepository.GetRoomByKey(roomKey));

        public Task SendUsersConnected(string room)
        {
            throw new NotImplementedException();
        }
    }
}
