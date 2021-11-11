using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models
{
    public class DiceCup
    {
        private Dice d1;
        private Dice d2;
        static Random rand = new Random();
        private List<int> moves = new List<int>();

        public DiceCup(Dice D1, Dice D2)
        {
            this.d1 = D1;
            this.d2 = D2;
        }

        public DiceCup(Dice D1, Dice D2, int[] moves)
        {
            this.d1 = D1;
            this.d2 = D2;
            if (moves != null)
            {
                foreach (int m in moves)
                {
                    this.moves.Add(m);
                }
            }
        }

        public void RollDices()
        {
            moves.Clear();
            d1.value = rand.Next(1, 7);
            d2.value = rand.Next(1, 7);

            if(d1.value == d2.value)
            {
                for (int i = 0; i < 4; i++)
                {
                    moves.Add(d1.value);
                }
            }
            else
            {
                moves.Add(d1.value);
                moves.Add(d2.value);
            }
        }

        //Returns a list of moves
        public List<int> GetMoves()
        {
            return moves;
        }

        public void SetMoves(int[] moves)
        {
            if (moves != null)
            {
                foreach (int m in moves)
                {
                    this.moves.Add(m);
                }
            }
        }

        //Deletes a move from list of possible moves
        public void RemoveMove(int move)
        {
            for (int i = 0; i < moves.Count(); i++)
            {
                if (moves[i] == move)
                {
                    moves.RemoveAt(i);
                    break;
                }
            }
        }

        // Resets diceCup
        public void ResetDiceCup()
        {
            moves.Clear();
        }
    }
}
