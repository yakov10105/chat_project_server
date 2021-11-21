using Chat_App.BackgammonGame.Logic.Models;
using Chat_App.Models;
using System;
using System.Collections.Generic;

namespace Chat_App.Services.GameServices
{
    public interface IGameService
    {
        GameBoard GameBoard { get; set; }
        Move Move { get; set; }

        void InitAllGamePieces(User sender, User receiver);
        void StartGame();
        void InitBoardState(int[] player1Position, int[] player2Position);
        int GetNumOfCheckersInGoalFieldPlayer1();
        int GetNumOfCheckersInGoalFieldPlayer2();
        int GetNumberOfEliminatedCheckers();
        IEnumerable<Checker> GetNumberOfEliminatedCheckersForColor(bool isWhite);
        string GetEliminatedCheckerColor(int index);
        void ResetCanReceive();
        string GetColor(int index);
        string GetGoalColorPlayer1();
        string GetGoalColorPlayer2();
        void RollDices();
        List<int> GetDices();
        string GetActivePlayerName();
        string GetNonActivePlayerName();
        String GetPlayer1Name();
        String GetPlayer2Name();
        String Player1Color();
        String Player2Color();
        GameBoard GetGameBoard();
        DiceCup GetDiceCup();
        Player GetPlayer1();
        Player GetPlayer2();
        Player GetActivePlayer();
        void MakeMove(Move move);
        void CheckPlayerTurn();
        bool CheckForWinner();
        string ReturnWinner();
        bool AnyMoreMoves();
        List<int> GetPossibleMoveFromPosition(int index);
        void UpdatePossibleMoves();

    }

}
