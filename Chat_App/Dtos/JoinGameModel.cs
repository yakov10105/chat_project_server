using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Dtos
{
    public class JoinGameModel
    {
        public string UserName { get; set; }
        public string RoomName { get; set; }
        public bool IsMyTurn{ get; set; }

    }
}
