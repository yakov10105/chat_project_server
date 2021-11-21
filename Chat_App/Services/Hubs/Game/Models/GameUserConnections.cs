using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Services.Hubs.Game.Models
{
    public class GameUserConnections
    {
        public string SenderUserName { get; set; }
        public string ReciverUserName { get; set; }
        public bool IsMyTurn { get; set; }
    }
}
