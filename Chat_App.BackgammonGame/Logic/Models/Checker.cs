using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models
{
    internal class Checker
    {
        public int checkerId;
        public Player player { get; }


        public Checker(int checkerId, Player player)
        {
            this.checkerId = checkerId;
            this.player = player;
        }
    }
}
