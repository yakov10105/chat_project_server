using Chat_App.Dtos;
using Chat_App.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.JWT
{
    public interface IJwtService
    {
        string Generate(User user);

        JwtSecurityToken Verify(string jwt);
    }
}
