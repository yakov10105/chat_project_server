using Chat_App.Models;
using System.Collections.Generic;
using System;
using Chat_App.Data.DbConfig;
using System.Linq;

namespace Chat_App.Data
{
    public class RoomRepo : IRoomRepo
    {
        private readonly ChatAppDbContext _context;

        public RoomRepo(ChatAppDbContext context) => _context = context;

        public IEnumerable<Room> GetAllRooms()
        {
            throw new NotImplementedException();
        }

        public Room GetRoomById(int id)
        {
            throw new NotImplementedException();
        }

        public void CreateRoom(string roomKey)
        {
            Room room = new Room {
                RoomKey = roomKey
            };

            _context.Rooms.Add(room);
            _context.SaveChanges();
        }

        public Room GetRoomByKey(string roomKey) => _context.Rooms.FirstOrDefault(r => r.RoomKey == roomKey);

    }
}