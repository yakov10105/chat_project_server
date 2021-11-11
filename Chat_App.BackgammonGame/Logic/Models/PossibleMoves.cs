using Chat_App.BackgammonGame.Logic.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models
{
    public class PossibleMoves
    {
        public BasicField From { get; }
        public BasicField To { get; }
        public int Move { get; }

        public PossibleMoves(BasicField from, BasicField to, int move)
        {
            this.From = from;
            this.To = to;
            this.Move = move;
        }
    }
}
