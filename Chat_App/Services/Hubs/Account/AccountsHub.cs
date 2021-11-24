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
        private readonly IUserRepo _userRepository;
        private readonly IMessageRepo _messageRepository;
        private readonly IMapper _mapper;

        public AccountsHub(IDictionary<string, string> connections, IAuthenticationService iAuth, IUserRepo repository, IMessageRepo messageRepository, IMapper mapper)
        {
            _connections = connections;
            _iAuthService = iAuth;
            _userRepository = repository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public async Task ConnectAsync(string userName)
        {
            _connections[Context.ConnectionId] = userName;
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
            await (Task.Run(() => _userRepository.UpdateIsOnline(_userRepository.GetUserByUserName(userName).Id, online: true)));
            await SendUsersConnected();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out string userName))
            {
                _connections.Remove(Context.ConnectionId);
                _userRepository.UpdateIsOnline(_userRepository.GetUserByUserName(userName).Id, online: false);

                SendUsersConnected();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task SendUsersConnected()
        {
            var users = _userRepository.GetOnlineUsers();

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

        }

        public async Task SetGameOn(GameUserConnections gameUserConnection)
        {

            await Clients.Group(gameUserConnection.SenderUserName)
                         .SendAsync("SetGame");

            await Clients.Group(gameUserConnection.ReciverUserName)
                         .SendAsync("SetGame");

        }

        public async Task ReceiveMessage(string roomName)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out string sender))
            {
                var user1 = _userRepository.GetUserByUserName(sender);
                var user2Id = roomName.Split("-").First((u) => int.Parse(u) != user1.Id);
                var user2 = _userRepository.GetUserById(int.Parse(user2Id));

                string room = roomName;

                await Clients.Group(user2.UserName)
                             .SendAsync("ReceiveMessage", room);
            }
        }



        public async Task<IEnumerable<NewMessage>> CheckForNewMessages()
        {
            var newMessages = new List<NewMessage>();

            if (_connections.TryGetValue(Context.ConnectionId, out string reciver))
            {
                int toUserId = _userRepository.GetUserByUserName(reciver).Id;


                var messagesToMe = _messageRepository.GetAllMessagesForUser(toUserId);

                foreach (var message in messagesToMe)
                {
                    var msg = new NewMessage { SenderUserName = message.Sender.UserName };
                    if (!message.RecieverHasRead)
                    {
                        msg.NumberOfNewMessages++;
                        if (newMessages.Exists(m => m.SenderUserName == message.Sender.UserName))
                        {
                            newMessages.Find(m => m.SenderUserName == message.Sender.UserName).NumberOfNewMessages++;
                        }
                        else
                        {
                            newMessages.Add(msg);
                        }
                    }
                }
            }

            return await Task.Run(() => newMessages);

        }

        public void ReadUnReadMessages(string roomName)
        {
            if (roomName != "")
            {
                if (_connections.TryGetValue(Context.ConnectionId, out string reciver))
                {
                    var reciverUser = _userRepository.GetUserByUserName(reciver);

                    var senderUserId = roomName.Split("-").First((u) => int.Parse(u) != reciverUser.Id);
                    var senderUser = _userRepository.GetUserById(int.Parse(senderUserId));

                    int toUserId = reciverUser.Id;
                    int fromUserId = senderUser.Id;


                    //var messagesToMe = await Task.Run(() => _messageRepository.GetMessagesForUser(reciverId, senderId));
                    var messagesToMe = _messageRepository.GetMessagesForUser(toUserId, fromUserId);
                    messagesToMe?.ForEach(m => _messageRepository.UpdateHasRead(m));
                }
            }


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
