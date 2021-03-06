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

        public Message SaveNewMessage(string message, int reciverId, int senderId, int roomId)
        {
            Message newMessage = new Message {
                Text = message,
                Date = DateTime.Now,
                SenderId = senderId,
                RecieverId = reciverId,
                RoomId = roomId,
                RecieverHasRead = false
            };
            _context.Messages.Add(newMessage);

            _context.SaveChanges();

            return newMessage;

        }

        public List<Message> GetMessagesForRoom(int reciverId, int senderId) => _context.Messages.Where(rk => (rk.SenderId == senderId && rk.RecieverId == reciverId)|| (rk.SenderId == reciverId && rk.RecieverId == senderId)).ToList();

        public List<Message> GetMessagesForUser(int toUserId, int fromUserId) => _context.Messages.Where(rk => (rk.SenderId == fromUserId && rk.RecieverId == toUserId)).ToList();

        public List<Message> GetAllMessagesForUser(int toUserId) => _context.Messages.Where(rk => (rk.RecieverId == toUserId)).ToList();

        public void UpdateHasRead(Message message)
        {
            message.RecieverHasRead = true;

            _context.SaveChanges();

        }

    }
}
