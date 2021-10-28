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
        }

        public void DeleteUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }
            _context.Users.Remove(user);
        }

        public IEnumerable<User> GetAllUsers() => _context.Users.ToList();

        public User GetUserById(int id) => _context.Users.FirstOrDefault(u => u.Id == id);

        public User GetUserByUserName(string userName)=> _context.Users.FirstOrDefault(u => u.UserName == userName);

        public bool SaveChanges() => _context.SaveChanges() >= 0;

        public void UpdateUser(User user)
        {
            //Nothing the Database will take care of that
        }
    }
}