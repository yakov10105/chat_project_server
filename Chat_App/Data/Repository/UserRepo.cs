using Chat_App.Data.DbConfig;
using Chat_App.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Chat_App.Data
{
    public class UserRepo : IUserRepo
    {
        private readonly ChatAppDbContext _context;

        public UserRepo(ChatAppDbContext context) => _context = context;

        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public void UpdateIsOnline(int id, bool online)
        {
            _context.Users.FirstOrDefault(u => u.Id == id).IsOnline = online;
            _context.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers() => _context.Users.ToList();

        public User GetUserById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

        public User GetUserByUserName(string userName)=> _context.Users.FirstOrDefault(u => u.UserName == userName);

        public void UpdateUser(User user)
        {
            //Nothing the Database will take care of that
        }

        public int GetUserIdByUserName(string userName)
        {
           var user =  _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
                return user.Id;

            return 0;
        }

        public IEnumerable<User> GetOnlineUsers() => _context.Users.Where(u => u.IsOnline == true);
    }
}