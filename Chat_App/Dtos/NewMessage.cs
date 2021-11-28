using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Dtos
{
    public class NewMessage
    {
        public string SenderUserName { get; set; }

        public int NumberOfNewMessages{ get; set; }
    }
}
