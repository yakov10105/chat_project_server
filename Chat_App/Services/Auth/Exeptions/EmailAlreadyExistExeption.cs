using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.Services.Auth.Exeptions
{
    public class EmailAlreadyExistExeption : Exception
    {
        public EmailAlreadyExistExeption(string message) : base(message)
        {
        }
    }
}
