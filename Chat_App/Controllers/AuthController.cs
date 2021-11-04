using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Chat_App.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Chat_App.Services.Auth.Exeptions;
using System;
using System.Linq;

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
            try
            {
                var token = _iAuthService.Authenticate(loginUser);
                var userFromData = _repository.GetUserByUserName(loginUser.UserName);

                if (userFromData != null)
                {
                    //Add IsOnline = true
                    _repository.UpdateIsOnline(userFromData.Id, true);
                    return Ok(new
                    {
                        key = $"Bearer {token}",
                        user = _mapper.Map<UserReadDto>(userFromData)
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
            return BadRequest(new { error = "Something gone wrong , try again later." });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCreateDto userCreateDto)
        {
            if (this.TryValidateModel(userCreateDto))
            {
                try
                {
                    var userModel = _mapper.Map<User>(userCreateDto);
                    var user = _iAuthService.RegisterUser(userCreateDto);

                    var userReadDto = _mapper.Map<UserReadDto>(userModel);

                    return Created("Success", user);
                }
                catch (EmailAlreadyExistExeption emailE)
                {
                    ModelState.AddModelError("emailError", emailE.Message);
                    return BadRequest(ModelState);
                }
                catch (UserNameAlreadyExistExeption usernameE)
                {
                    ModelState.AddModelError("userNameError", usernameE.Message);
                    return BadRequest(ModelState);
                } 
            }
            return BadRequest();
        }
    }

}
