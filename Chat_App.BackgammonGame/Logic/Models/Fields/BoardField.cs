using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models.Fields
{
    public class BoardField:BasicField
    {
        public bool _canReceive;

        public BoardField(LinkedList<Checker> checkers , int position, bool canReceive)
            : base(checkers , position)
        {
            _canReceive = canReceive;
        }
    }
}
