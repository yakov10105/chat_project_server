using Chat_App.BackgammonGame.Logic.Models;
using Chat_App.Data;
using Chat_App.Models;
using Chat_App.Services.Hubs.Game.Models;
using Chat_App.Services.GameServices;
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
        private  GameService _gameService;
        private bool? isGameOn;

        public GameHub(IDictionary<string, GameUserConnections> connections, IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
            _connections = connections;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _roomRepository = roomRepo;
            isGameOn = null;
        }

        public async Task JoinGameAsync(string userName)
        {
            
        }
        public void StartGame(GameUserConnections gameUserConnection)
        {
            // Checkers in start position
            // Boardfield     0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 -  E  G2 G1
            int[] p1Array = { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] p2Array = { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 };

            var reciver = _userRepository.GetUserByUserName(gameUserConnection.ReciverUserName);
            var sender = _userRepository.GetUserByUserName(gameUserConnection.SenderUserName);

            _gameService = new GameService(sender, reciver);
            _gameService.InitBoardState(p1Array, p2Array);
            _gameService.StartGame();
        }
        
        public void RollDices() => _gameService.RollDices();
        public IEnumerable<int> GetDicesValue() => _gameService.GetDices();





        private string GetRoomId(GameUserConnections gameUserConnection)
        {
            var sb = new StringBuilder();
            if (gameUserConnection.ReciverUserName != null && gameUserConnection.SenderUserName != null)
            {
                var senderId = _userRepository.GetUserIdByUserName(gameUserConnection.SenderUserName);
                var reciverId = _userRepository.GetUserIdByUserName(gameUserConnection.ReciverUserName);
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
