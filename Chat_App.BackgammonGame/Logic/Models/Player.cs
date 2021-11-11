using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models
{
    public class Player
    {
        public int id { get; }
        public String name { get; set; }
        public String color { get; set; }

        public Player()
        {

        }

        public Player(int id, String name, String color)
        {
            this.id = id;
            this.name = name;
            this.color = color;
        }
    }
}
