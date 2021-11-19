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
using Chat_App.Dtos;
using System.Linq;
using Newtonsoft.Json;

namespace Chat_App.Services.Hubs.Game
{
    public class GameHub:Hub
    {
        private readonly IDictionary<string, GameUserConnections> _connections;
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IRoomRepo _roomRepository;
        private  IGameService _gameService;
        private bool? isGameOn;

        public GameHub(IDictionary<string, GameUserConnections> connections,IGameService gameService, IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
            _connections = connections;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _roomRepository = roomRepo;
            _gameService = gameService;
            isGameOn = null;
        }

        public async Task JoinGameAsync(JoinGameModel joinGameModel)
        {
            var gameConnection = GetGameUserConnection(joinGameModel);
            await Groups.AddToGroupAsync(Context.ConnectionId, joinGameModel.RoomName);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameConnection.SenderUserName);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameConnection.ReciverUserName);

            _connections[Context.ConnectionId] = gameConnection;

            StartGame(gameConnection);
            
        }

        private GameUserConnections GetGameUserConnection(JoinGameModel joinGameModel)
        {
            var user1 = _userRepository.GetUserByUserName(joinGameModel.UserName);
            var user2Id = joinGameModel.RoomName.Split("-").First((u)=>int.Parse(u)!=user1.Id);
            var user2 = _userRepository.GetUserById(int.Parse(user2Id));
            var connection = new GameUserConnections() { SenderUserName = user1.UserName, ReciverUserName = user2.UserName };
            return connection;
        }

        public void StartGame(GameUserConnections gameUserConnection)
        {
            // Checkers in start position
            // Boardfield     0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 -  E  G2 G1
            int[] p1Array = { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] p2Array = { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 };

            var reciver = _userRepository.GetUserByUserName(gameUserConnection.ReciverUserName);
            var sender = _userRepository.GetUserByUserName(gameUserConnection.SenderUserName);

            _gameService.InitAllGamePieces(sender, reciver);
            _gameService.InitBoardState(p1Array, p2Array);
            _gameService.StartGame();
        }

        public async Task<string> GetBoard() => await Task.Run(() => JsonConvert.SerializeObject(_gameService.GetGameBoard()));

        public async Task RollDices() => await Task.Run(() => _gameService.RollDices());

        public async Task<IEnumerable<int>> GetDicesValue() => await Task.Run(() => _gameService.GetDices());

        public async Task<IEnumerable<int>> GetPossibleMoves(int pos)
        {
            return await Task.Run(() => _gameService.GetPossibleMoveFromPosition(pos));
        }





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
