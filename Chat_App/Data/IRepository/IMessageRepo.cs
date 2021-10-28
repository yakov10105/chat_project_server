using Chat_App.Models;
using System.Collections.Generic;
using System;


namespace Chat_App.Data
{
    public interface IMessageRepo
    {
        IEnumerable<Message> GetAllMessages();

        Message GetMessageById(int id);
    }
}


