using Chat_App.Models;
using System.Collections.Generic;
using System;


namespace Chat_App.Data
{
    public class MessageRepo : IMessageRepo
    {
       public IEnumerable<Message> GetAllMessages()
        {
            throw new NotImplementedException();
        }

        public Message GetMessageById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
