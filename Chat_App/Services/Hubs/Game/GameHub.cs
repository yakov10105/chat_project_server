using Chat_App.Data;
using Chat_App.Services.Hubs.Game.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.Hubs.Game
{
    public class GameHub:Hub
    {
        private readonly IDictionary<string, GameUserConnections> _connections;
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IRoomRepo _roomRepository;

        public GameHub(IDictionary<string, GameUserConnections> connections, IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
            _connections = connections;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _roomRepository = roomRepo;
        }

        public async Task JoinRoomAsync(GameUserConnections gameUserConnection)
        {
            string roomKey = GetRoomId(gameUserConnection);
            if (roomKey != "room")
            {
                int reciverId = _userRepository.GetUserByUserName(gameUserConnection.RecieverUserName).Id;
                int senderId = _userRepository.GetUserByUserName(gameUserConnection.SenderUserName).Id;

                await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);

                _connections[Context.ConnectionId] = gameUserConnection;
            }
        }

        private string GetRoomId(GameUserConnections gameUserConnection)
        {
            var sb = new StringBuilder();
            if (gameUserConnection.RecieverUserName != null && gameUserConnection.SenderUserName != null)
            {
                var senderId = _userRepository.GetUserIdByUserName(gameUserConnection.SenderUserName);
                var reciverId = _userRepository.GetUserIdByUserName(gameUserConnection.RecieverUserName);
                if (senderId < reciverId)
                    sb.Append($"{senderId}-{reciverId}");
                else
                    sb.Append($"{reciverId}-{senderId}");

                return sb.ToString();
            }
            return "room";
        }
    }
}
