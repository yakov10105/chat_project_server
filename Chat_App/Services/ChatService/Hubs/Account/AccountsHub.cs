using AutoMapper;
using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Services.Auth;
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

        public IEnumerable<string> GetConnectedUsersAsync() => _connections.Values.ToList();

        public IEnumerable<string> GetAllUsersAsync()
        {
            var usersNames = new List<string>();
            _repository.GetAllUsers().ToList().ForEach(u => usersNames.Add(u.UserName));
            return usersNames;
        }


        public async Task UpdateUsersAsync()
        {
            await Clients.All.SendAsync("UpdateUsers");
        }

        public async Task ConnectAsync(string userName)
        {
            _connections[Context.ConnectionId] = userName;
            await (Task.Run(() => _repository.UpdateIsOnline(_repository.GetUserByUserName(userName).Id, online: true)));
            await Clients.All.SendAsync("UpdateUsers");
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out string userName))
            {
                Task.Run(()=> Clients.All.SendAsync("UpdateUsers"));
                _connections.Remove(Context.ConnectionId);
                _repository.UpdateIsOnline(_repository.GetUserByUserName(userName).Id, online: false);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
