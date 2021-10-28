using Chat_App.Dtos;
using Chat_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.Auth
{
    public interface IAuthenticationService
    {
        User AuthenticateEmail(UserLoginDto loginUser);

        bool AuthenticatePassword(UserLoginDto loginUser, User systemUser);

        string Authenticate(UserLoginDto loginUser);

        User RegisterUser(UserRegisterDto regUser);
    }
}
