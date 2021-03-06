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
    public class GameHub : Hub
    {
        private readonly IDictionary<string, GameUserConnections> _connections;
        private readonly IDictionary<GameUserConnections, GameBoard> _boards;
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IRoomRepo _roomRepository;
        private IGameService _gameService;
        const int coinsAmount = 50;

        public GameHub(IDictionary<string, GameUserConnections> connections, IDictionary<GameUserConnections, GameBoard> boards, IGameService gameService, IUserRepo userRepository, IMessageRepo messageRepository, IRoomRepo roomRepo)
        {
            _connections = connections;
            _boards = boards;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _roomRepository = roomRepo;
            _gameService = gameService;
        }

        public async Task JoinGameAsync(GameUserConnections gameUserConnections)
        {
            //var gameConnection = GetGameUserConnection(joinGameModel);
            string roomKey = GetRoomId(gameUserConnections);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameUserConnections.SenderUserName);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameUserConnections.ReciverUserName);
            _connections[Context.ConnectionId] = gameUserConnections;
            //if (_userRepository.IsEnoughCoinsForGame(gameConnection.))
            //{

            //}
            if (_boards.TryGetValue(gameUserConnections, out GameBoard gameBoard))
            {
                await Clients.Group(roomKey)
                             .SendAsync("GetRoomBoard", _gameService.GetGameBoard());
            }
            else
            {
                StartGame();
            }

        }

        public GameUserConnections GetGameUserConnection(JoinGameModel joinGameModel)
        {
            var user1 = _userRepository.GetUserByUserName(joinGameModel.UserName);
            var user2Id = joinGameModel.RoomName.Split("-").First((u) => int.Parse(u) != user1.Id);
            var user2 = _userRepository.GetUserById(int.Parse(user2Id));
            var connection = new GameUserConnections() { SenderUserName = user1.UserName, ReciverUserName = user2.UserName, IsMyTurn = joinGameModel.IsMyTurn };
            return connection;
        }

        public async void StartGame()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnection))
            {
                if (!userConnection.IsMyTurn)
                {
                    // Checkers in start position
                    // Boardfield     0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 -  E  G2 G1
                    int[] p1Array = { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    int[] p2Array = { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 };

                    var reciver = _userRepository.GetUserByUserName(userConnection.SenderUserName);//Chaged-->
                    var sender = _userRepository.GetUserByUserName(userConnection.ReciverUserName);//<--Chaged

                    await Task.Run(() =>
                    {
                        _gameService.InitAllGamePieces(sender, reciver);
                        _gameService.InitBoardState(p1Array, p2Array);
                        _gameService.StartGame();
                    }).ContinueWith((Task) => _boards[userConnection] = _gameService.GameBoard);

                }
            }
        }

        public async Task ChangePlayerTurn()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnection))
            {
                string roomKey = GetRoomId(userConnection);

                if (_gameService.CheckPlayerTurn())
                {
                    await Clients.Group(roomKey)
                                     .SendAsync("ChangeTurn");
                }
            }
        }

        public async Task RollDices() => await Task.Run(() => _gameService.RollDices());

        public async Task<IEnumerable<int>> GetDicesValue() => await Task.Run(() => _gameService.GetDices());

        public async Task<IEnumerable<int>> GetPossibleMoves(int pos)
        {
            _gameService.GetPossibleMoveFromPosition(pos);
            return await Task.Run(() => _gameService.GetPossibleMoveFromPosition(pos));
        }

        public async Task UpdatePossibleMoves()
        {
            await Task.Run(() => _gameService.UpdatePossibleMoves());
        }

        public async Task CheckForWinner()
        {
            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnection))
            {
                if (_gameService.CheckForWinner())
                {
                    string roomKey = GetRoomId(userConnection);
                    var winnerLoser = _gameService.ReturnWinnerLoser();
                    string winner = winnerLoser["winner"];
                    string loser = winnerLoser["loser"];

                    await Clients.Group(roomKey)
                                 .SendAsync("AnyWinner", winner);

                    User winnerUser = _userRepository.GetUserByUserName(winner);

                    User loserUser = _userRepository.GetUserByUserName(loser);

                    _userRepository.AddCoins(winnerUser, coinsAmount);

                    _userRepository.RemoveCoins(loserUser, coinsAmount);
                }
            }
        }



        public async Task Move(Move move)
        {
            await Task.Run(() =>
            {
                if (_gameService.AnyMoreMoves())
                {
                    _gameService.MakeMove(move);
                }
            });
            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnection))
            {
                string roomKey = GetRoomId(userConnection);

                await Clients.Group(roomKey)
                             .SendAsync("GetRoomBoard", _gameService.GetGameBoard());

                //if (_gameService.CheckPlayerTurn())
                //{
                //    await Clients.Group(roomKey)
                //                 .SendAsync("ChangeTurn");
                //}
            }
        }

        public async Task<bool> CheckIfMovesLeft() => await Task.Run(() => _gameService.AnyMoreMoves());

        public async Task<int> GetNumberOfEliminatedCheckers(bool isWhite) => await Task.Run(() => _gameService.GetNumberOfEliminatedCheckersForColor(isWhite));

        public async Task<string> GetBoard() => await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(_gameService.GetGameBoard());
        });



        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnection))
            {
                _connections.Remove(Context.ConnectionId);
            }

            if (_connections.TryGetValue(Context.ConnectionId, out GameUserConnections userConnectionn))
            {
                _connections.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
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
