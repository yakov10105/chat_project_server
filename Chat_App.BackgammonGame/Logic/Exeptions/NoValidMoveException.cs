using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Exeptions
{
    public class NoValidMoveException:Exception
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
