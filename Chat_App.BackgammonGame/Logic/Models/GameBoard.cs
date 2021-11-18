using Chat_App.BackgammonGame.Logic.Exeptions;
using Chat_App.BackgammonGame.Logic.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Chat_App.BackgammonGame.Logic.Models
{
    public class GameBoard
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public BoardField[] BoardFields  { get; set; }
        public DiceCup DiceCup { get; set; }
        public EliminatedField EliminatedField { get; set; }
        public GoalField GoalFieldPlayer1 { get; set; }
        public GoalField GoalFieldPlayer2 { get; set; }
        public List<PossibleMoves> PossibleMoves { get; set; }
        public Player ActivePlayer { get; set; }
        public Checker[] Player1Checkers { get; set; }
        public Checker[] Player2Checkers { get; set; }
        public Rules Rules { get; set; }

        public GameBoard(BoardField[] boardFields, 
                         EliminatedField eliminatedField, 
                         GoalField goalFieldP1, GoalField goalFieldP2,
                         Checker[] P1Checkers, Checker[] P2Checkers, 
                         DiceCup diceCup,
                         Rules rules,
                         Player player1,
                         Player player2)
        {
            this.BoardFields = boardFields;
            this.EliminatedField = eliminatedField;
            this.GoalFieldPlayer1 = goalFieldP1;
            this.GoalFieldPlayer2 = goalFieldP2;
            this.Player1=player1; 
            this.Player2=player2;
            this.DiceCup=diceCup;
            this.Rules=rules;
            this.Player1Checkers=P1Checkers;
            this.Player2Checkers=P2Checkers;
        }

        public void MoveChecker(Player activePlayer,BasicField fromField,BasicField toField, List<PossibleMoves> possibleMoves)
        {
            try
            {
                if(Rules.ValidateMove(fromField, toField, possibleMoves))
                {
                    // If opponent has ONE checker in the field
                    if(toField.GetCheckerCount()==1 && !toField.GetPlayerInField().Equals(activePlayer))
                    { 
                        // Move opponents checker to eliminationField
                        Checker opponentChecker = toField.RemoveChecker();
                        EliminatedField.AddChecker(opponentChecker);
                    }
                    // Moves active players checker
                    Checker checker = null;
                    if (EliminatedField.HasCheckerFrom(activePlayer))
                        checker = EliminatedField.RemoveChecker(activePlayer);
                    else
                        checker = fromField.RemoveChecker();

                    toField.AddChecker(checker);
                    //remove this move from the list
                    foreach (var move in possibleMoves)
                    {
                        if(move.From.Equals(fromField)&& move.To.Equals(toField))
                        {
                            DiceCup.RemoveMove(move.Move);
                            break;
                        }
                    }

                }
            }
            catch (NoValidMoveException e)
            {

                //handle no possible move error
            }
        }

        //player position is 24 length array that tells how much checkers need to be on every field 
        // example "playerPosition[0]=2" means => on the first field of the board the player have two checkers (at the initial board)
        public void InitStartBoard(int[] player1Positions , int[] player2Positions)
        {
            // Resets all fields
            for (int i = 0; i < 24; i++)
            {
                BoardFields[i].Clear();
            }
            EliminatedField.Clear();
            GoalFieldPlayer1.Clear();
            GoalFieldPlayer2.Clear();

            // Array counters
            int p1Place = 0;
            int p2Place = 0;

            for (int i = 0; i < 24; i++)
            {
                // Player 1
                for (int j = 0; j < player1Positions[i]; j++)
                {
                    BoardFields[i].AddChecker(Player1Checkers[p1Place]);
                    p1Place++;
                } 

                // Player 2
                for (int j = 0; j < player2Positions[i]; j++)
                {
                    BoardFields[i].AddChecker(Player2Checkers[p2Place]);
                    p2Place++;
                }
            }


            // Elimination field
            for (int j = 0; j < player1Positions[25]; j++)
            {
                EliminatedField.AddChecker(Player1Checkers[p1Place]);
                p1Place++;
            }
            for (int j = 0; j < player2Positions[25]; j++)
            {
                EliminatedField.AddChecker(Player2Checkers[p2Place]);
                p2Place++;
            }


            // Goalfield P1
            for (int j = 0; j < player1Positions[27]; j++)
            {
                GoalFieldPlayer1.AddChecker(Player1Checkers[p1Place]);
                p1Place++;
            }

            // Goalfield P2
            for (int j = 0; j < player2Positions[26]; j++)
            {
                GoalFieldPlayer2.AddChecker(Player2Checkers[p2Place]);
                p2Place++;
            }
        }
    }
}
