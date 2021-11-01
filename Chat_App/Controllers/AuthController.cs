using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Chat_App.Services.Auth;
using Chat_App.Services.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Chat_App.Dtos;
using Chat_App.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;

namespace Chat_App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase //Acount
    {
        private readonly IAuthenticationService _iAuthService;
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;

        public AuthController(IAuthenticationService iAuth, IUserRepo repository, IMapper mapper)
        {
            _iAuthService = iAuth;
            _repository = repository;
            _mapper = mapper;
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
                var userFromData = _repository.GetUserByUserName(loginUser.UserName);

                if (userFromData != null)
                {
                    return Ok(new
                    {
                        key = $"Bearer {token}",
                        user = _mapper.Map<UserReadDto>(userFromData)
                    });
                }
            }
            return Unauthorized();

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCreateDto userCreateDto)
        {
            var userModel = _mapper.Map<User>(userCreateDto);
            var user = _iAuthService.RegisterUser(userCreateDto);

            var userReadDto = _mapper.Map<UserReadDto>(userModel);

            return Created("Success", user);
        }

        [Authorize]
        [HttpGet("user")]
        public IActionResult User()
        {
            return Ok(new { authorized = "Successs" });
        }
    }

}
