using Chat_App.Models;
using System.Collections.Generic;
using System;
using Chat_App.Data.DbConfig;
using System.Linq;

namespace Chat_App.Data
{
    public class MessageRepo : IMessageRepo
    {

        private readonly ChatAppDbContext _context;

        public MessageRepo(ChatAppDbContext context) => _context = context;

        public IEnumerable<Message> GetAllMessages()
        {
            throw new NotImplementedException();
        }

        public Message GetMessageById(int id)
        {
            throw new NotImplementedException();
        }

        public Message SaveNewMessage(string message, int reciverId, int senderId)
        {
            Message newMessage = new Message {
                                Text = message,
                                Date = DateTime.Now,
                                SenderId = senderId,
                                RecieverId = reciverId};

            _context.SaveChanges();

            return newMessage;

        }

        public List<Message> GetMessagesForRoom(int reciverId, int senderId) => _context.Messages.Where(rk => rk.SenderId == senderId && rk.RecieverId == reciverId).ToList();
    }
}
