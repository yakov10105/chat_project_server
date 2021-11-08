using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Chat_App.Services.Auth.Exeptions;
using Chat_App.Services.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepo _iUserRepo;
        private readonly IJwtService _iJwtService;

        public AuthenticationService(IUserRepo userRepo, IJwtService iJwtService)
        {
            _iUserRepo = userRepo;
            _iJwtService = iJwtService;
        }


        public User AuthenticateEmail(UserLoginDto loginUser) => _iUserRepo.GetUserByUserName(loginUser.UserName);

        public bool AuthenticatePassword(UserLoginDto loginUser, User systemUser) => BCrypt.Net.BCrypt.Verify(loginUser.Password, systemUser.Password);

        public string Authenticate(UserLoginDto loginUser)
        {
            var user = this.AuthenticateEmail(loginUser);
            if (user == null)
            {
                throw new Exception("One of the fields is Incorrect.");
            }
            if (!this.AuthenticatePassword(loginUser, user))
            {
                throw new Exception("One of the fields is Incorrect.");
            }
            return _iJwtService.Generate(user);
        }

        public User RegisterUser(UserCreateDto regUser)
        {
            if (!CheckUserNameNotExist(regUser))
                throw new UserNameAlreadyExistExeption("* This Username already exists in the system.");
            if (!CheckEmailNotExist(regUser))
                throw new EmailAlreadyExistExeption("* This Email already exists in the system.");
            var user = new User
            {
                FirstName = regUser.FirstName,
                LastName = regUser.LastName,
                UserName = regUser.UserName,
                UserEmail = regUser.UserEmail,
                UserAge = regUser.UserAge,
                WinCoins = 200,
                Password = BCrypt.Net.BCrypt.HashPassword(regUser.Password),
                //RoomId = 1

            };
            _iUserRepo.CreateUser(user);
            return user;
        }

        private bool CheckEmailNotExist(UserCreateDto regUser)
        {
            if (_iUserRepo.GetAllUsers().FirstOrDefault(u => u.UserEmail == regUser.UserEmail) != null)
                return false;
            else
                return true;
        }
        private bool CheckUserNameNotExist(UserCreateDto regUser)
        {
            if (_iUserRepo.GetAllUsers().FirstOrDefault(u => u.UserName == regUser.UserName) != null)
                return false;
            else
                return true;
        }
    }
}
