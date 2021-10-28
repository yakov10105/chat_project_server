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

            Room room = new Room
            {
                Id = 1,
                RoomName = "חרא על מכללת סלע",
                Members= new List<User>()
            };
            User yakov = new User
            {
                Id = 1,
                FirstName = "Yakov",
                LastName = "Kantor",
                UserName = "Yakov10105",
                UserEmail = "Yakov@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                ConfirmPassword = "1234",
                UserAge = 22,
                RecievedMessages = new List<Message>(),
                SendedMessages=new List<Message>(),
                RoomId=1

            };
            User idan = new User
            {
                Id = 2,
                FirstName = "Idan",
                LastName = "Barzilay",
                UserName = "Idan111",
                UserEmail = "Idan@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                ConfirmPassword = "1234",
                UserAge = 22,
                RecievedMessages = new List<Message>(),
                SendedMessages = new List<Message>(),
                RoomId=1
            };
            builder.Entity<User>().HasData(idan,yakov);
            builder.Entity<Room>().HasData(room);
        }
    }
}
