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
    public class ChatHub : Hub
    {
        private readonly IDictionary<string, UserConnection> _connections;
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IRoomRepo _roomRepository;

        public ChatHub(IDictionary<string, UserConnection> connections,IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
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

                await Clients.Group(roomKey)
                             .SendAsync("ReceiveMessage", userConnection.SenderUserName, message);

                await SaveNewMessageAsync(message, reciverId, senderId, room.Id);
            }
            
        }

        public async Task SendGameRequest(UserConnection userConnection)
        {
            var room = GetRoomId(userConnection);
            await Clients.Group(room).SendAsync("GetGameInvitation", userConnection.SenderUserName);
            //send only to the reciever , not both of them
            //get invitation accepted/denied
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                _connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

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
