using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Exeptions
{
    internal class NoValidMoveException:Exception
    {
        public NoValidMoveException()
        {

        }

        public NoValidMoveException(String message)
            : base(message)
        {

        }
    }
}
