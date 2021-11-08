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

        public ChatHub(IDictionary<string, UserConnection> connections,IUserRepo userRepository, IMessageRepo messageRepository)
        {
            botUser = "MyChat Bot";
            _connections = connections;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task JoinRoomAsync(UserConnection userConnection)
        {
            string roomKey = GetRoomId(userConnection);
            int reciverId = _userRepository.GetUserByUserName(userConnection.ReciverUserName).Id;
            int senderId = _userRepository.GetUserByUserName(userConnection.SenderUserName).Id;


            await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);

            _connections[Context.ConnectionId] = userConnection;

            var messages = await GetMessages(userConnection);
            foreach (var message in messages)
            {
                await Clients.Group(roomKey).SendAsync("ReceiveMessage", message.Sender.UserName,message);
            }
            //await SendUsersConnected(roomId);
        }

        public async Task SendMessageAsync(string message)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out UserConnection userConnection))
            {
                string roomKey = GetRoomId(userConnection);
                int reciverId = _userRepository.GetUserByUserName(userConnection.ReciverUserName).Id;
                int senderId = _userRepository.GetUserByUserName(userConnection.SenderUserName).Id;
                _messageRepository.SaveNewMessage(message, reciverId, senderId);
                await Clients.Group(roomKey)
                             .SendAsync("ReceiveMessage", userConnection.SenderUserName, message);
            }
            
        }

        private async Task<List<Message>> GetMessages(UserConnection userConnection)
        {
            int reciverId = _userRepository.GetUserByUserName(userConnection.ReciverUserName).Id;
            int senderId = _userRepository.GetUserByUserName(userConnection.SenderUserName).Id;
            var messages = _messageRepository.GetMessagesForRoom(reciverId, senderId);
            return messages;

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

  

        public string GetRoomId(UserConnection userConnection)
        {
            var sb = new StringBuilder();
            var senderId = _userRepository.GetUserIdByUserName(userConnection.SenderUserName);
            var reciverId = _userRepository.GetUserIdByUserName(userConnection.ReciverUserName);
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
