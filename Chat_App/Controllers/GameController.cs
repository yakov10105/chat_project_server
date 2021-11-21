using Chat_App.BackgammonGame.Logic.Models;
using Chat_App.Models;
using Chat_App.Services.GameServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public GameService gameService;
        public GameController()
        {
            // Checkers in start position
            // Boardfield     0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16 17 18 19 20 21 22 23 -  E  G2 G1
            int[] p1Array = { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 3, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] p2Array = { 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0 };

            var reciver = new User() { Id = 222, UserName = "ggg333" };
            var sender = new User() { Id = 333, UserName = "mmm333" };

            gameService = new GameService();
            gameService.InitBoardState(p1Array, p2Array);
            gameService.StartGame();
            gameService.RollDices();
        }
        [HttpGet("board")]
        public IActionResult GetBoard()
        {

            var a = gameService.GameBoard.PossibleMoves[0];
            var move = new Move() { from = a.From.GetPosition(), to = a.To.GetPosition() };
            gameService.MakeMove(move);

            //return Ok(gameService.GameBoard);
            //return Ok(gameService.GameBoard.BoardFields[0]);
            //return Ok(gameService.GameBoard.BoardFields[0].GetCheckers());
            return Ok(gameService.GetGameBoard());
        }
        [HttpGet("get-moves")]
        public IActionResult GetPossibleMoves(int pos)
        {
            var lst = gameService.GetPossibleMoveFromPosition(pos);
            return Ok(lst);
        }
    }
}
