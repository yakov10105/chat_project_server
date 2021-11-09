using AutoMapper;
using Chat_App.Data;
using Chat_App.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepo _messageRepository;
        private readonly IUserRepo _userRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessageRepo messageRepository, IUserRepo userRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("get-messages")]
        public ActionResult<IEnumerable<MessageReadDto>> GetMessages(string senderUserName, string recieverUserName)
        {
            var sender = _userRepository.GetUserByUserName(senderUserName);
            var reciver = _userRepository.GetUserByUserName(recieverUserName);
            if (sender != null && reciver != null)
            {
                int senderId = sender.Id;
                int reciverId = reciver.Id;
                var messages = _messageRepository.GetMessagesForRoom(reciverId, senderId);
                if (messages.Count>0)
                {
                    var list = new List<MessageReadDto>();
                    foreach (var item in messages)
                    {
                        list.Add(new MessageReadDto { Text = item.Text, Date = item.Date, SenderUserName = (_userRepository.GetUserById(item.SenderId).UserName) });
                    }
                    return Ok(list);
                }
            }
            return NotFound();
        }
    
    }
}
