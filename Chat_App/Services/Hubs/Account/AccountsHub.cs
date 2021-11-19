using AutoMapper;
using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Chat_App.Services.Auth;
using Chat_App.Services.Hubs.Game.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Services.ChatService.Hubs.Acount
{
    public class AccountsHub : Hub
    {
        private readonly IDictionary<string, string> _connections;
        private readonly IAuthenticationService _iAuthService;
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;

        public AccountsHub(IDictionary<string, string> connections, IAuthenticationService iAuth, IUserRepo repository, IMapper mapper)
        {
            _connections = connections;
            _iAuthService = iAuth;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task ConnectAsync(string userName)
        {
            _connections[Context.ConnectionId] = userName;
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            await (Task.Run(() => _repository.UpdateIsOnline(_repository.GetUserByUserName(userName).Id, online: true)));
            await SendUsersConnected();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out string userName))
            {
                _connections.Remove(Context.ConnectionId);
                _repository.UpdateIsOnline(_repository.GetUserByUserName(userName).Id, online: false);

                SendUsersConnected();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersConnected()
        {
            var users = _repository.GetOnlineUsers();

            return Clients.All.SendAsync("ConnectedUsers", _mapper.Map<IEnumerable<UserReadDto>>(users));
        }

        public Task SendTyping(UserConnection userConnection)
        {
            if (userConnection != null)
            {
                return Clients.Group(userConnection.ReciverUserName).SendAsync("ReceiveTyping", userConnection.SenderUserName, userConnection.ReciverUserName);
            }
            return Task.Run(() => "");
        }



        public async Task SendGameRequest(GameUserConnections gameUserConnection)
        {
                await Clients.Group(gameUserConnection.ReciverUserName)
                             .SendAsync("ReceiveGameInvitation", gameUserConnection.SenderUserName);

            //await Clients.Group(userName)
            //            .SendAsync("WaitForGameAccept", userName);

        }

        public async Task SetGameOn(GameUserConnections gameUserConnection)
        {

            await Clients.Group(gameUserConnection.SenderUserName)
                         .SendAsync("SetGame");

            await Clients.Group(gameUserConnection.ReciverUserName)
                         .SendAsync("SetGame");

        }




        //public IEnumerable<string> GetConnectedUsersAsync() => _connections.Values;

        //public IEnumerable<string> GetAllUsersAsync()
        //{
        //    var usersNames = new List<string>();
        //    _repository.GetAllUsers().ToList().ForEach(u => usersNames.Add(u.UserName));
        //    return usersNames;
        //}
    }
}
