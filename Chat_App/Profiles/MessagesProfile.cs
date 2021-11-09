using AutoMapper;
using Chat_App.Dtos;
using Chat_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Profiles
{
    public class MessagesProfile : Profile
    {
        public MessagesProfile()
        {
            //Source -> Target
            CreateMap<Message, MessageReadDto>();
        }

    }
}
