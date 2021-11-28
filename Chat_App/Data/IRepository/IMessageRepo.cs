using Chat_App.Models;
using System.Collections.Generic;
using System;


namespace Chat_App.Data
{
    public interface IMessageRepo
    {
        IEnumerable<Message> GetAllMessages();

        Message GetMessageById(int id);

        Message SaveNewMessage(string message, int reciverId, int senderId, int roomId);

        List<Message> GetMessagesForRoom(int reciverId, int senderId);

        List<Message> GetMessagesForUser(int reciverId, int senderId);

        List<Message>  GetAllMessagesForUser(int toUserId);

        void UpdateHasRead(Message message);
    }
}


