using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models.Fields
{
    public class EliminatedField:BasicField
    {
        public EliminatedField()
        {
            this.checkers = new LinkedList<Checker>();
            this.position = 25;
        }

        // Checks if player has any checker in the elimination field
        public bool HasCheckerFrom(Player player)
        {
            foreach (var checker in checkers)
            {
                if (checker.player.Equals(player))
                {
                    return true;
                }
            }
            return false;
        }

        public Checker RemoveChecker(Player player)
        {
            Checker tempChecker = null;
            foreach (Checker checker in checkers)
            {
                if (checker.player.Equals(player))
                {
                    tempChecker = checker;
                    checkers.Remove(checker);
                    break;
                }
            }
            return tempChecker;
        }

        // Prints elimination field in console
        public void PrintField()
        {
            foreach (Checker checker in checkers)
            {
                //Console.ForegroundColor = checker.getPlayer().getColor();
                Console.Write("O");
                Console.ResetColor();
            }
        }
    }
}
