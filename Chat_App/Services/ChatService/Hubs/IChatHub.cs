using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Services.ChatService.Hubs
{
    public interface IChatHub
    {
        Task JoinRoomAsync(UserConnection userConnection);

        Task SendMessageAsync(string message);

        Task OnDisconnectedAsync(Exception exception);

        Task SendUsersConnected(string room);
    }
}
