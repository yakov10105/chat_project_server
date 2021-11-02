using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Services.Auth.Exeptions
{
    public class UserNameAlreadyExistExeption : Exception
    {
        public UserNameAlreadyExistExeption(string message) : base(message)
        {
        }
    }
}
