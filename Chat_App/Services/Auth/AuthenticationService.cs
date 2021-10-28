using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
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

        public AuthenticationService(IUserRepo userRepo,IJwtService iJwtService)
        {
            _iUserRepo = userRepo;
            _iJwtService = iJwtService;
        }


        public User AuthenticateEmail(UserLoginDto loginUser)
        {
            return _iUserRepo.GetUserByUserName(loginUser.UserName);
        }

        public bool AuthenticatePassword(UserLoginDto loginUser, User systemUser)
        {
            if (BCrypt.Net.BCrypt.Verify(loginUser.Password, systemUser.Password))
                return true;
            return false;
                                
        }

        public string Authenticate(UserLoginDto loginUser)
        {
            var user = this.AuthenticateEmail(loginUser);
            if (user == null) return null;
            if (!this.AuthenticatePassword(loginUser, user))
                return null;

            //if we've got here - means user exist.
            return _iJwtService.Generate(user);



        }

        public User RegisterUser(UserRegisterDto regUser)
        {
            var user = new User
            {
                FirstName = regUser.FirstName,
                LastName = regUser.LastName,
                UserName = regUser.UserName,
                UserEmail = regUser.Email,
                UserAge = regUser.Age,
                Password = BCrypt.Net.BCrypt.HashPassword(regUser.Password)
            };
            _iUserRepo.CreateUser(user);
            return user;
        }
    }
}
