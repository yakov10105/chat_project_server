using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models.Fields
{
    internal class BoardField:BasicField
    {
        private int position;
        public BoardField(LinkedList<Checker> checkers , int position)
            : base(checkers , position)
        {

        }
    }
}
