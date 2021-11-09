using Chat_App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Data.DbConfig.Extentions
{
    public static class DataSeedExtention
    {
        public static void SeedData(this ModelBuilder builder)
        {
            User yakov = new User
            {
                Id = 1,
                FirstName = "Yakov",
                LastName = "Kantor",
                UserName = "Yakov10105",
                UserEmail = "Yakov@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            User idan = new User
            {
                Id = 2,
                FirstName = "Idan",
                LastName = "Barzilay",
                UserName = "Idan111",
                UserEmail = "Idan@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            User yosi = new User
            {
                Id = 3,
                FirstName = "Yosi",
                LastName = "Cohen",
                UserName = "Yosi111",
                UserEmail = "Yosi@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            User elon = new User
            {
                Id = 4,
                FirstName = "Elon",
                LastName = "Mask",
                UserName = "Elon111",
                UserEmail = "Elon@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            User jeff = new User
            {
                Id = 5,
                FirstName = "Jeff",
                LastName = "Bezos",
                UserName = "Jeff122",
                UserEmail = "Jeff@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            User bil = new User
            {
                Id = 6,
                FirstName = "Bill",
                LastName = "Gates",
                UserName = "Bill8787",
                UserEmail = "Bill@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                UserAge = 22
            };
            var room12 = new Room
            {
                Id = 1,
                RoomKey = "1-2",
                Messages = new List<Message>()
            };
            var message = new Message
            {
                Id = 1,
                Text = "Hello , How are you ???",
                Date = DateTime.Now.AddDays(-84).AddHours(52),
                RoomId = 1,
                RecieverId = 2,
                SenderId = 1
            };
            builder.Entity<User>().HasData(idan,yakov,yosi,elon,jeff,bil);
            builder.Entity<Message>().HasData(message);
            builder.Entity<Room>().HasData(room12);
        }
    }
}
