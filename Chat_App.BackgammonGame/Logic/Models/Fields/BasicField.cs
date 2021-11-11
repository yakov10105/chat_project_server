using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models.Fields
{
    public class BasicField
    {
        protected LinkedList<Checker> checkers = null;
        protected int position;

        public BasicField(LinkedList<Checker> checkers)
        {
            this.checkers=checkers;
        }
        public BasicField(LinkedList<Checker> checkers, int position)
        {
            this.checkers = checkers;
            this.position = position;
        }
        public BasicField()
        {

        }

        public int GetPosition()
        {
            return this.position;
        }

        // Returns the numbers of checkers in field
        public int GetCheckerCount()
        {
            return checkers.Count;
        }

        // Returns the list of checkers in the field
        public LinkedList<Checker> GetCheckers()
        {
            return checkers;
        }

        //Returns the Player that owns the first checker in list.
        public Player GetPlayerInField()
        {
            //TODO: Add check if any or no checkers.
            return checkers.First.Value.player;
        }

        //Removes a checker from field and returns it
        public Checker RemoveChecker()
        {
            Checker firstChecker = checkers.First();
            checkers.RemoveFirst();
            return firstChecker;
        }
        //Adds a checker to the list
        public void AddChecker(Checker checker)
        {
            checkers.AddFirst(checker);
        }
        //Returns specific checker from the list
        // Used for printing game board in console
        public Checker GetCheckerAt(int p)
        {
            return checkers.ElementAt(p);
        }

        // Returns the first checker in the list
        public Checker GetFirstChecker()
        {
            return checkers.First();
        }

        public void Clear()
        {
            this.checkers.Clear();
        }
    }
}
