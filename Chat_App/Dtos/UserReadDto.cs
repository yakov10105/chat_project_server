using Chat_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Dtos
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public int UserAge { get; set; }

        public string UserEmail { get; set; }

        public bool IsOnline { get; set; }

        public int WinCoins { get; set; }

        public int RoomId { get; set; }

    }
}
