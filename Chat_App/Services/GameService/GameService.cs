using Chat_App.BackgammonGame.Logic.Exeptions;
using Chat_App.BackgammonGame.Logic.Models;
using Chat_App.BackgammonGame.Logic.Models.Fields;
using System;
using System.Collections.Generic;

namespace Chat_App.Services.GameService
{
    public class GameService:IGameService
    {
        public GameBoard GameBoard { get; set; }
        public GameService(GameBoard gameBoard)
        {
            GameBoard = gameBoard;
        }

        public void StartGame()
        {
            GameBoard.ActivePlayer = GameBoard.Player1;
            GameBoard.DiceCup.ResetDiceCup();
        }
        public void InitBoardState(int[] player1Position, int[] player2Position)=>GameBoard.InitStartBoard(player1Position, player2Position);
       
        public int GetNumOfCheckersInGoalFieldPlayer1()=> GameBoard.GoalFieldPlayer1.GetCheckerCount();

        public int GetNumOfCheckersInGoalFieldPlayer2()=>GameBoard.GoalFieldPlayer2.GetCheckerCount();

        public int GetNumberOfEliminatedCheckers()=>GameBoard.EliminatedField.GetCheckerCount();

        public string GetEliminatedCheckerColor(int index)=>GameBoard.EliminatedField.GetCheckerAt(index).player.color;
        
        public string GetColor(int index)=>GameBoard.BoardFields[index].GetPlayerInField().color;
        
        public string GetGoalColorPlayer1()=> GameBoard.Player1.color;

        public string GetGoalColorPlayer2()=> GameBoard.Player2.color;
        
        public void RollDices()
        {
            GameBoard.DiceCup.ResetDiceCup();
            GameBoard.DiceCup.RollDices();
            GameBoard.PossibleMoves = new List<PossibleMoves>();
            GameBoard.PossibleMoves = GameBoard.Rules.CheckPossibleMoves(GameBoard.ActivePlayer);
            CheckPlayerTurn();
        }
        public List<int> GetDices()=>GameBoard.DiceCup.GetMoves();    

        public string GetActivePlayerName()=> GameBoard.ActivePlayer.name;
        
        public string GetNonActivePlayerName()
        {
            if (GameBoard.Player1.Equals(GameBoard.ActivePlayer))
                return GameBoard.Player1.name;
            else
                return GameBoard.Player2.name;
        }
        public String GetPlayer1Name()=>GameBoard.Player1.name;
        
        public String GetPlayer2Name()=>GameBoard.Player2.name;
        
        public String Player1Color()=>GameBoard.Player1.color;
        
        public String Player2Color()=> GameBoard.Player2.color;
        
        public GameBoard GetGameBoard()=> GameBoard;
        
        public DiceCup GetDiceCup()=> GameBoard.DiceCup;

        public Player GetPlayer1() => GameBoard.Player1;

        public Player GetPlayer2() => GameBoard.Player2;

        public Player GetActivePlayer()=> GameBoard.ActivePlayer;

        public void MakeMove(Move move)
        {
            BasicField fromField;
            BasicField toField;
            /*
            0-23   = BoardFields
            25     = EliminationField
            26     = Player2 GoalField
            27     = Player1 GoalField
           */
            if(move.from == 25)
            {
                fromField = GameBoard.EliminatedField;
            }
            else if(move.from == 26 || move.from == 27)
            {
                throw new NoValidMoveException("Move is not Allowd");
            }
            else
            {
                fromField= GameBoard.BoardFields[move.from];
            }

            if (move.to == 26)
                toField = GameBoard.GoalFieldPlayer2;
            else if (move.to == 27)
                toField = GameBoard.GoalFieldPlayer1;
            else if (move.to == 25)
                throw new NoValidMoveException("Move is now allows");
            else
                toField = GameBoard.BoardFields[move.to];

            try
            {
                GameBoard.MoveChecker(GameBoard.ActivePlayer, fromField, toField, GameBoard.PossibleMoves);
            }
            catch (NoValidMoveException e)
            {

                throw new NoValidMoveException(e.Message);
            }
        }

        public void CheckPlayerTurn()
        {
            if (GameBoard.ActivePlayer.Equals(GameBoard.Player1) && !AnyMoreMoves())
                GameBoard.ActivePlayer = GameBoard.Player2;
            else if (GameBoard.ActivePlayer.Equals(GameBoard.Player2) && !AnyMoreMoves())
                GameBoard.ActivePlayer = GameBoard.Player1;
        }

        public bool CheckForWinner()
        {
            if((GameBoard.GoalFieldPlayer1.GetCheckerCount()>=15) || (GameBoard.GoalFieldPlayer2.GetCheckerCount()>=15))
                return true;
            else
                return false;
        }

        public string ReturnWinner()
        {
            string winner = "";
            if (GameBoard.GoalFieldPlayer1.GetCheckerCount() >= 15)
            {
                winner = GetPlayer1Name();
            }
            else if (GameBoard.GoalFieldPlayer2.GetCheckerCount() > 15)
            {
                winner= GetPlayer2Name();
            }
            return winner;
        }

        public bool AnyMoreMoves()
        {
            if ((GameBoard.DiceCup.GetMoves().Count <= 0) || (GameBoard.PossibleMoves.Count <= 0))
                return false;
            else
                return true;
        }

        public List<int> GetPossibleMoveFromPosition(int index)
        {
            var list = new List<int>();
            foreach (var move in GameBoard.PossibleMoves)
            {
                if (move.From.GetPosition() == index)
                {
                    list.Add(move.To.GetPosition());
                }
            }
            return list;
        }
    }

}
