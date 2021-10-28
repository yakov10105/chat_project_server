using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Chat_App.Services.Auth;
using Chat_App.Services.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Chat_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _iAuthService;

        public AuthController(IAuthenticationService iAuth)
        {
            _iAuthService = iAuth;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginUser)
        {
            var token = _iAuthService.Authenticate(loginUser);

            if (token != null)
            {
                HttpContext.Response.Headers.Add("Authorization", $"Bearer {token}");
                Request.Headers.Add("Authorization", $"Bearer {token}");

                return Ok(new {message ="Connected !" });
            }
            return Unauthorized();

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto regUser)
        {
            return Created("Success", _iAuthService.RegisterUser(regUser));
        }

        [Authorize]
        [HttpGet("user")]
        public IActionResult User()
        {
            return Ok(new { authorized = "Successs" });
        }
    }

}
