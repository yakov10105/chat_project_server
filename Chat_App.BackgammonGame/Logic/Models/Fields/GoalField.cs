using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models.Fields
{
    internal class GoalField:BasicField
    {
        private Player player;

        public GoalField(Player player, int position)
        {
            this.player = player;
            this.checkers = new LinkedList<Checker>();
            this.position = position;
        }

        public Player getOwner()
        {
            return player;
        }
    }
}
