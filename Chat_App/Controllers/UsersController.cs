using AutoMapper;
using Chat_App.Data;
using Chat_App.Dtos;
using Chat_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Chat_App.Controllers
{
    //api/[name of the controller]
    //now it will be Users and it will be changed if we change the name of the controller class
    //we can hard code it to like: "api/the name of the route we want"
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase// ControllerBase Without view Support
    {
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //GET api/Users
        [HttpGet]
        public ActionResult<IEnumerable<UserReadDto>> GetAllUsers()
        {
            var users = _repository.GetAllUsers();

            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(users));
        }

        //GET api/Users/{id}
        //GET api/Users/5
        [HttpGet("{id}", Name = "GetUserById")]
        public ActionResult<UserReadDto> GetUserById(int id)
        {
            var user = _repository.GetUserById(id);
            if (user != null)
            {
                return Ok(_mapper.Map< UserReadDto>(user));
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<UserCreateDto> CreateCommand(UserCreateDto userCreateDto)
        {
            //Add Validition
            var userModel = _mapper.Map<User>(userCreateDto);
            _repository.CreateUser(userModel);
            _repository.SaveChanges();

            var userReadDto = _mapper.Map<UserReadDto>(userModel);

            return CreatedAtRoute(nameof(GetUserById), new { Id = userReadDto.Id }, userReadDto);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            var userModelFromRepo = _repository.GetUserById(id);
            if (userModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(userUpdateDto, userModelFromRepo);

            _repository.UpdateUser(userModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUserUpdate(int id, JsonPatchDocument<UserUpdateDto> patchDocument)
        {
            var userModelFromRepo = _repository.GetUserById(id);
            if (userModelFromRepo == null)//check that we have this user in the Database
            {
                return NotFound();
            }

            var userToPatch = _mapper.Map<UserUpdateDto>(userModelFromRepo);// Generating a new UserUpdateDto
            patchDocument.ApplyTo(userToPatch, ModelState);// Insert it to our user

            if (!TryValidateModel(userToPatch))//Validition check
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userToPatch, userModelFromRepo);//updating  user

            _repository.UpdateUser(userModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var userModelFromRepo = _repository.GetUserById(id);
            if (userModelFromRepo == null)//check that we have this user in the Database
            {
                return NotFound();
            }

            _repository.DeleteUser(userModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

    }
}