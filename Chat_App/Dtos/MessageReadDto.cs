using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Dtos
{
    public class MessageReadDto
    {
        //public int Id { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string SenderUserName { get; set; }
        public bool RecieverHasRead { get; set; }


        //public int RoomId { get; set; }
        //public int SenderId { get; set; }
        //public int RecieverId { get; set; }
    }
}
