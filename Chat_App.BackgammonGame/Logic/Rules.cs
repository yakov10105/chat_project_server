using Chat_App.BackgammonGame.Logic.Exeptions;
using Chat_App.BackgammonGame.Logic.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat_App.BackgammonGame.Logic.Models
{
    public class Rules
    {
        EliminatedField eliminatedField;
        BasicField goalFieldPlayer1, goalFieldPlayer2;
        BoardField[] boardFields;
        DiceCup diceCup;
        Player player1, player2;

        public Rules(BoardField[] boardFields, EliminatedField eliminatedField, BasicField goalFieldPlayer1, BasicField goalFieldPlayer2, DiceCup diceCup, Player player1, Player player2)
        {
            this.eliminatedField = eliminatedField;
            this.goalFieldPlayer1 = goalFieldPlayer1;
            this.goalFieldPlayer2 = goalFieldPlayer2;
            this.diceCup = diceCup;
            this.player1 = player1;
            this.player2 = player2;
            this.boardFields = boardFields;
        }

        // Validates move against list of valid moves
        public bool ValidateMove(BasicField fromField, BasicField toField, List<PossibleMoves> possMoves)
        {
            bool retValue = false;

            foreach (var possibleMove in possMoves)
            {
                if(possibleMove.From.Equals(fromField) && possibleMove.To.Equals(toField))
                {
                    retValue = true;
                    break;
                }
            }
            if (!retValue)
            {
                throw new NoValidMoveException("Not a valid Move .");
            }
            return retValue;
        }

        //Checks if a move is valid
        public bool IsValidMove(BasicField fromField,BasicField toField,Player activePlayer)
        {
            // When moving out from eliminated field
            if (fromField.Equals(eliminatedField))
            {
                if (OkToMoveToField(toField, activePlayer))
                {
                    int d;
                    if (activePlayer.Equals(player1))
                        d = -1;
                    else
                        d = 24;
                    if (MoveMatchesDice(diceCup, d, toField.GetPosition()))
                        return true;

                    return false;
                }
                else
                    return false;

            }
            // When Player 1 tries to move to goal field
            if (toField.Equals(goalFieldPlayer1) && activePlayer.Equals(player1))
            {
                if (OkToMoveFromField(fromField, activePlayer))
                    return true;
                else
                    return false;
            }
            // When Player 2 tries to move to goal field
            if (toField.Equals(goalFieldPlayer2) && activePlayer.Equals(player2))
            {
                if (OkToMoveFromField(fromField, activePlayer))
                    return true;
                else
                    return false;
            }
            // If move does not match any dice
            if (!MoveMatchesDice(diceCup, fromField.GetPosition(), toField.GetPosition()))
            {
                return false;
            }
            //If it's impossible to move from fromfield
            if (!OkToMoveFromField(fromField, activePlayer))
            {
                return false;
            }

            // If it's impossible to move to tofield
            if (!OkToMoveToField(toField, activePlayer))
            {
                return false;
            }
            // Moving in right direction player 1
            if (activePlayer.Equals(player1) &&
                !fromField.Equals(eliminatedField) &&
                !toField.Equals(goalFieldPlayer1) &&
                fromField.GetPosition() >= toField.GetPosition())
            {
                return false;
            }
            // Moving in right direction player 2
            if (activePlayer.Equals(player2) &&
                !fromField.Equals(eliminatedField) &&
                !toField.Equals(goalFieldPlayer2) &&
                fromField.GetPosition() <= toField.GetPosition())
            {
                return false;
            }
            // If player has checkers in elimination field but tries to move another checker
            if (eliminatedField.HasCheckerFrom(activePlayer) && !fromField.Equals(eliminatedField))
            {
                return false;
            }

            return true;
        }

        // Checks if target field is valid
        public bool OkToMoveToField(BasicField field, Player activePlayer)
        {
            if(field.GetCheckerCount() > 1)
            {
                if(!field.GetPlayerInField().Equals(activePlayer))
                    return false;
            }
            return true;
        }

        // Checks if source field is valid
        public bool OkToMoveFromField(BasicField field, Player activePlayer)
        {
            if (field.GetCheckerCount() > 0 && field.GetPlayerInField().Equals(activePlayer))
                return true;
            return false;
        }
        // Checks that move matches any rolled dice
        public bool MoveMatchesDice(DiceCup diceCup, int from, int to)
        {
            int moveDistance = to - from;
            if (moveDistance < 0)
                moveDistance = -moveDistance;

            if (!diceCup.GetMoves().Contains<int>(moveDistance))
                return false;
            return true;
        }

        // Checks if all players checkers is on base area
        public bool AllCheckerOnBaseArea(Player player, List<BasicField> fieldsToCheck)
        {
            int numberOfCheckers=0;
            foreach (BasicField field in fieldsToCheck)
            {
                if((field.GetCheckerCount() > 0)&&(field.GetPlayerInField().Equals(player)))
                        numberOfCheckers += field.GetCheckerCount();
            }
            if (numberOfCheckers >= 15)
                return true;
            else 
                return false;
        }

        // Checks all possible moves
        public List<PossibleMoves> CheckPossibleMoves(Player player)
        {
            var activePlayer = player;
            var possibleMoves = new List<PossibleMoves>();

            foreach (var move in diceCup.GetMoves())
            {
                //if player have eliminated checkers
                if (eliminatedField.HasCheckerFrom(activePlayer))
                {
                    //Player 1 :
                    int to = move - 1;
                    //Player 2 : 
                    if (activePlayer.Equals(player2))
                        to = 24 - move;

                    if (IsValidMove(eliminatedField, boardFields[to], activePlayer))
                        possibleMoves.Add(new PossibleMoves(eliminatedField, boardFields[to], move));
                }
                else
                {
                    //depend on player (direction)
                    //Player 1 : 
                    if (activePlayer.Equals(player1))
                    {
                        //Itarating through all the board fields to figure out possible moves
                        for (int i = 0; i < 24; i++)
                        {
                            int to = i + move;
                            if(to <= 23)
                                if(IsValidMove(boardFields[i],boardFields[to], activePlayer))
                                    possibleMoves.Add(new PossibleMoves(boardFields[i], boardFields[to], move));
                        }
                        //checks the possibility to get to the goal field
                        //creating list with fields to check
                        var fieldsToCheck = new List<BasicField>();

                        for (int i = 18; i < 24; i++)
                        {
                            fieldsToCheck.Add(boardFields[i]);
                        }
                        fieldsToCheck.Add(goalFieldPlayer1);

                        if (AllCheckerOnBaseArea(activePlayer, fieldsToCheck))
                        {
                            for (int i = 18; i < 24; i++)
                            {
                                // If field corrensponding with move has checker owned by active player
                                if ((boardFields[24 - move].GetCheckerCount() > 0) && (boardFields[24 - move].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[24 - move], goalFieldPlayer1, move));
                                    break;
                                }
                                // If there is any checker in field higher than move value. Not possible to move to goal
                                if (((24 - boardFields[i].GetPosition()) > move) && (boardFields[i].GetCheckerCount() > 0) && (boardFields[i].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Not possible to use this move to place checker in goal
                                    break;
                                }
                                // If checker on field equals to or lower than move value. Possible to move to goal
                                if (((24 - boardFields[i].GetPosition()) <= move) && (boardFields[i].GetCheckerCount() > 0) && (boardFields[i].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[i], goalFieldPlayer1, move));
                                    break;
                                }
                            }
                        }
                    }
                    //Player 2 :
                    else
                    {
                        for (int i = 23; i >= 0; i--)
                        {
                            int to = i - move;
                            if (to >= 0)
                                if (IsValidMove(boardFields[i], boardFields[to], activePlayer))
                                        possibleMoves.Add(new PossibleMoves(boardFields[i], boardFields[to], move));
                        }
                        //checks the possibility to get to the goal field
                        //creating list with fields to check
                        var fieldsToCheck = new List<BasicField>();
                        for (int i = 5; i >=0; i--)
                        {
                            fieldsToCheck.Add(boardFields[i]);
                        }
                        fieldsToCheck.Add(goalFieldPlayer2);
                        if (AllCheckerOnBaseArea(activePlayer, fieldsToCheck))
                        {
                            for (int i = 5; i >= 0; i--)
                            {
                                // If field corrensponding with move has checker owned by active player
                                if ((boardFields[move-1].GetCheckerCount() > 0) && (boardFields[move - 1].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[move - 1], goalFieldPlayer2, move));
                                    break;
                                }
                                // If there is any checker in field higher than move value. Not possible to move to goal
                                if (((boardFields[i].GetPosition()) > (move - 1)) && (boardFields[i].GetCheckerCount() > 0) && (boardFields[i].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Not possible to use this move to place checker in goal
                                    break;
                                }
                                // If checker on field equals to or lower than move value. Possible to move to goal
                                if (((boardFields[i].GetPosition()) <= (move - 1)) && (boardFields[i].GetCheckerCount() > 0) && (boardFields[i].GetPlayerInField().Equals(activePlayer)))
                                {
                                    // Possible to move checker to goal
                                    possibleMoves.Add(new PossibleMoves(boardFields[i], goalFieldPlayer2, move));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return possibleMoves;
        }
    }
}