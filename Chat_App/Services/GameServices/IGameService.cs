using Chat_App.BackgammonGame.Logic.Models;
using System;
using System.Collections.Generic;

namespace Chat_App.Services.GameServices
{
    public interface IGameService
    {
        GameBoard GameBoard { get; }
        void StartGame();
        void InitBoardState(int[] player1Position, int[] player2Position);
        int GetNumOfCheckersInGoalFieldPlayer1();
        int GetNumOfCheckersInGoalFieldPlayer2();
        int GetNumberOfEliminatedCheckers();
        string GetEliminatedCheckerColor(int index);
        string GetColor(int index);
        string GetGoalColorPlayer1();
        string GetGoalColorPlayer2();
        void RollDices();
        List<int> GetDices();
        string GetActivePlayerName();
        string GetNonActivePlayerName();
        String GetPlayer1Name();
        String GetPlayer2Name();
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

    }

}
